using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using System.Text.RegularExpressions;

namespace TIAAlarmTool
{
    class AlarmDefinitionList : List<AlarmDefinition>
    {
        public class MissingColumnException : Exception
        {
            public MissingColumnException(string msg)
                : base(msg)
            {
            }
        }
        public class UnknownOptionException : Exception
        {
            public UnknownOptionException(string msg)
                : base(msg)
            {
            }
        }

        public class InvalidIDException : Exception
        {
            public InvalidIDException(string msg)
                : base(msg)
            {
            }
        }
        // Column names for the columns we're interested in
        string ColID;
        string ColName;
        string ColText;
        string ColOptions;


        public AlarmDefinitionList()
        {

        }

        class SharedStringLookup
        {
            Dictionary<int, string> dict;



            public SharedStringLookup(SpreadsheetDocument doc)
            {
                dict = new Dictionary<int, string>();
                SharedStringTablePart sharedStringPart = doc.WorkbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                if (sharedStringPart != null)
                {
                    SharedStringTable table = sharedStringPart.SharedStringTable;
                    int index = 0;
                    foreach (SharedStringItem si in table.Elements<SharedStringItem>())
                    {
                        dict.Add(index, si.InnerText);
                        index++;
                    };
                }

            }
            public string getString(int index)
            {
                string str;
                if (dict.TryGetValue(index, out str))
                {
                    return str;
                }
                else
                {
                    return index.ToString();
                }
            }


            public string getString(Cell cell)
            {
                if (cell.DataType != null && cell.DataType == CellValues.SharedString)
                {
                    return getString(int.Parse(cell.CellValue.Text));
                }
                else
                {
                    return cell.CellValue.Text;
                }

            }
        }

        class SharedStringStore
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            List<string> list = new List<string>();
            int next_string_id = 0;
            public SharedStringStore()
            {

            }

            public void buildTable(SpreadsheetDocument doc)
            {
                SharedStringTablePart sharedStringPart = doc.WorkbookPart.AddNewPart<SharedStringTablePart>();
                SharedStringTable table = new SharedStringTable();
                table.Count = new UInt32Value((uint)list.Count());
                table.UniqueCount = new UInt32Value((uint)list.Count());
                sharedStringPart.SharedStringTable = table;
                foreach (string str in list)
                {
                    SharedStringItem si = new SharedStringItem();
                    si.AppendChild<Text>(new Text(str));
                    table.AppendChild<SharedStringItem>(si);
                }
            }
            public int putString(string str)
            {
                int index;
                if (str == null) str = "";
                if (dict.TryGetValue(str, out index))
                {
                    return index;
                }
                else
                {
                    dict.Add(str, next_string_id);
                    list.Add(str);
                    return next_string_id++;

                }
            }

        }
        private static Regex colPattern = new Regex("[A-Za-z]+");

        private static string GetColumnName(Cell cell)
        {

            Match match = colPattern.Match(cell.CellReference);

            return match.Value;
        }

        private static string GetOptionString(AlarmDefinition.Option opt)
        {
            string opt_str = "";
            bool first = true;
            if (opt.HasFlag(AlarmDefinition.Option.AutoAck))
            {
                opt_str += "autoack";
                first = false;
            }
            if (opt.HasFlag(AlarmDefinition.Option.Silent))
            {
                if (!first) opt_str += ',';
                opt_str += "silent";
            }
            return opt_str;
        }

        static Char[] optionSep = { ',', ' ' };

        const int AlarmBase = 1;
        public void ValidateID()
        {
            int index = 0;
            while (index < Count)
            {
                int next_id = index + AlarmBase;
                if (this[index].ID < 0)
                {
                    this[index].ID = next_id;
                }
                else
                {
                    int id = this[index].ID;
                    if (next_id > id) throw new InvalidIDException("Invalid sequence of ID");
                    while (next_id < id)
                    {
                        AlarmDefinition a = new AlarmDefinition("Index" + next_id, "Reserved " + next_id);
                        a.ID = next_id;
                        Insert(index, a);
                        index++;
                        next_id++;
                    }
                }
                index++;
            }
        }
        public void Load(string file)
        {
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(file, false)) // Open read only
            {
                Workbook wb = doc.WorkbookPart.Workbook;
                SharedStringLookup lookup = new SharedStringLookup(doc);
                Sheet sheet;
                try
                {
                    foreach (string str in wb.Sheets.Select<OpenXmlElement, string>(s => (s.GetAttribute("name", null).Value)))
                    {
                        Console.WriteLine(str);
                    }

                    OpenXmlElement sheetElem = wb.Sheets.First(s => (s.GetAttribute("name", null).Value == "Alarms"));
                    sheet = (Sheet)sheetElem;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("No sheet named 'Alarms' found", ex);
                }

                string rel = sheet.GetAttribute("id", "http://schemas.openxmlformats.org/officeDocument/2006/relationships").Value;
                Console.WriteLine("Rel: " + rel);
                WorksheetPart wsp = (WorksheetPart)doc.WorkbookPart.GetPartById(rel);
                Row head = wsp.Worksheet.GetFirstChild<SheetData>().GetFirstChild<Row>();


                // Look for columns by name, uing first row
                foreach (Cell cell in head.Elements<Cell>())
                {

                    string v = lookup.getString(cell);
                    string colStr = GetColumnName(cell);
                    if (v == "ID") ColID = colStr;
                    else if (v == "Name") ColName = colStr;
                    else if (v == "Text") ColText = colStr;
                    else if (v == "Options") ColOptions = colStr;
                    Console.WriteLine(colStr + ": " + v);

                }

                if (ColID == null)
                {
                    throw new MissingColumnException("No column named 'ID'");
                }
                else if (ColName == null)
                {
                    throw new MissingColumnException("No column named 'Name'");
                }
                else if (ColText == null)
                {
                    throw new MissingColumnException("No column named 'Text'");
                }
                else if (ColOptions == null)
                {
                    throw new MissingColumnException("No column named 'Options'");
                }

                Clear();
                Row row = head.NextSibling<Row>();
                while (row != null)
                {
                    int id = -1;
                    string name = null;
                    string text = null;
                    int options = 0;

                    foreach (Cell cell in row.Elements<Cell>())
                    {

                        string colStr = GetColumnName(cell);
                        if (colStr == ColID)
                        {
                            id = int.Parse(lookup.getString(cell));
                        }
                        else if (colStr == ColName)
                        {
                            name = lookup.getString(cell);
                        }
                        else if (colStr == ColText)
                        {
                            text = lookup.getString(cell);
                        }
                        else if (colStr == ColOptions)
                        {
                            string v = lookup.getString(cell);
                            string[] opt_str = v.Split(optionSep, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string o in opt_str)
                            {
                                if (o == "silent")
                                {
                                    options |= (int)AlarmDefinition.Option.Silent;
                                }
                                else if (o == "autoack")
                                {
                                    options |= (int)AlarmDefinition.Option.AutoAck;
                                }
                                else
                                {
                                    throw new UnknownOptionException("Unknown option '" + o + "'");
                                }
                            }
                        }
                    }
                    if (name == null || text == null)
                    {
                        throw new MissingColumnException("No value set for column 'Name' or 'Text' at row " + row.RowIndex);
                    }
                    AlarmDefinition ad = new AlarmDefinition(name, text);
                    ad.ID = id;
                    ad.Options = (AlarmDefinition.Option)options;
                    Add(ad);
                    Console.WriteLine(ad.ID + " " + ad.Name + " " + ad.Text + " " + ad.Options);
                    row = row.NextSibling<Row>();
                }
                //foreach (Row r in wsp.Worksheet.GetFirstChild<SheetData>().Elements<Row>()) {
                //   Console.WriteLine("Row: "+r);
                //}
            }
        }

        static void
        add_cell_string(Row row, SharedStringStore strings, string str)
        {
            uint col_index = (uint)row.ChildElements.Count();
            uint row_index = row.RowIndex;
            Cell cell = new Cell()
            {
                DataType = CellValues.SharedString,
                CellValue = new CellValue(strings.putString(str).ToString()),
                CellReference = Char.ConvertFromUtf32((int)'A' + (int)col_index) + row_index
            };
            row.AppendChild<Cell>(cell);
        }

        static void
        add_cell_int(Row row, int v)
        {
            uint col_index = (uint)row.ChildElements.Count();
            uint row_index = row.RowIndex;
            Cell cell = new Cell()
            {
                DataType = CellValues.Number,
                CellValue = new CellValue(v.ToString()),
                CellReference = Char.ConvertFromUtf32((int)'A' + (int)col_index) + row_index
            };
            row.AppendChild<Cell>(cell);
        }


        public void Save(string file)
        {
            using (SpreadsheetDocument doc = SpreadsheetDocument.Create(file, SpreadsheetDocumentType.Workbook)) // Create new workbook
            {
                SharedStringStore strings = new SharedStringStore();


                WorkbookPart wbp = doc.AddWorkbookPart();
                wbp.Workbook = new Workbook();
                WorksheetPart wsp = wbp.AddNewPart<WorksheetPart>();
                SheetData sheet_data = new SheetData();
                wsp.Worksheet = new Worksheet(sheet_data);




                Sheets sheets = wbp.Workbook.AppendChild<Sheets>(new Sheets());
                Sheet sheet = new Sheet() { Id = wbp.GetIdOfPart(wsp), SheetId = 1, Name = "Alarms" };
                sheets.Append(sheet);

                uint row_index = 1;
                // Header row
                Row row = new Row() { RowIndex = new UInt32Value(row_index) };
                // ID
                add_cell_string(row, strings, "ID");
                // Name
                add_cell_string(row, strings, "Name");
                // Text
                add_cell_string(row, strings, "Text");
                // Options
                add_cell_string(row, strings, "Options");

                sheet_data.AppendChild<Row>(row);
                row_index++;

                foreach (AlarmDefinition d in this)
                {
                    row = new Row() { RowIndex = new UInt32Value(row_index) };
                    // ID
                    add_cell_int(row, d.ID);

                    // Name
                    add_cell_string(row, strings, d.Name);

                    // Text
                    add_cell_string(row, strings, d.Text);

                    // Options
                    add_cell_string(row, strings, GetOptionString(d.Options));


                    sheet_data.AppendChild<Row>(row);
                    row_index++;
                }

                strings.buildTable(doc);



                wbp.Workbook.Save();
            }
        }
        public AlarmDefinition FindByID(int ID)
        {
            List<AlarmDefinition> found = this.FindAll(a => a.ID == ID);
            if (found.Count == 0) return null;
            return found.First();
        }
    }
}
