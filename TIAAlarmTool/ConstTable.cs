using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TIAAlarmTool
{
   
    class ConstTable
    {
         public class Constant
         {
             public string Name { get; protected set; }
             public object Value { get; protected set; }
             public string Comment { get; protected set; } 

             public Constant(string name, Object value, string comment)
             {
                 Name = name;
                 Value = value;
                 Comment = comment;
             }
         }

      
        public static string buildFile(string table_name, IEnumerable<Constant> constants)
         {
            
            XmlWriterSettings settings = new XmlWriterSettings {
                ConformanceLevel = ConformanceLevel.Document, 
                Encoding = Encoding.UTF8, 
                Indent = true };
            string filename;
            using (Stream stream = TempFile.Open("AlarmConst","xml", out filename))
            {
                int id = 0;
                XmlWriter w = XmlWriter.Create(stream, settings);
                w.WriteStartDocument();
                w.WriteStartElement("Document");

                w.WriteStartElement("DocumentInfo");
                w.WriteEndElement(); // DocumentInfo
                w.WriteStartElement("SW.ControllerTagTable");
                w.WriteAttributeString("ID", (id++).ToString());
                w.WriteStartElement("AttributeList");
                XMLUtil.SimpleValue(w, "Name", table_name);
                w.WriteEndElement(); // AttributeList



                w.WriteStartElement("ObjectList");
                foreach (Constant c in constants)
                {

                    w.WriteStartElement("SW.ControllerConstant");
                    w.WriteAttributeString("ID", (id++).ToString());
                    w.WriteAttributeString("AggregationName", "Constants");
                    w.WriteStartElement("AttributeList");
                    XMLUtil.SimpleValue(w, "Name", c.Name);
                    XMLUtil.SimpleValue(w, "Value", c.Value.ToString());
                    w.WriteEndElement(); // AttributeList

                    w.WriteStartElement("LinkList");
                    w.WriteStartElement("DataType");
                    w.WriteAttributeString("TargetID", "@OpenLink");
                    XMLUtil.SimpleValue(w, "Name", "Int");
                    w.WriteEndElement(); // DataType
                    w.WriteEndElement(); // LinkList

                    w.WriteStartElement("ObjectList");
                    w.WriteStartElement("MultilingualText");
                    w.WriteAttributeString("ID", (id++).ToString());
                    w.WriteAttributeString("AggregationName", "Comment");
                    w.WriteStartElement("AttributeList");
                    w.WriteStartElement("TextItems");
                    w.WriteStartElement("Value");
                    w.WriteAttributeString("lang", "sv-SE");
                    w.WriteString(c.Comment);
                    w.WriteEndElement(); // Value
                    w.WriteEndElement(); // TextItems
                    w.WriteEndElement(); // AttributeList
                    w.WriteEndElement(); // MultilingualText
                    w.WriteEndElement(); // ObjectList

                    w.WriteEndElement(); // SW.ControllerConstant
                }

               
                w.WriteEndElement(); // ObjectList
                w.WriteEndElement(); // SW.ControllerTagTable
                w.WriteEndElement(); // Document
                w.Close();
            }
            return filename;
         }
        static private void readObjectList(XmlReader r, out string comment)
        {
            comment = null;
            if (!r.ReadToDescendant("MultilingualText")) return;
            if (!r.ReadToDescendant("Value")) return;
            comment = r.ReadElementContentAsString();
        }
        static private void readAttributeList(XmlReader r, out string name, out string value_str)
        {
            name = null;
            value_str = null;
            while (r.Read())
            {
                if (r.IsStartElement())
                {
                    if (r.LocalName == "Name")
                    {
                        name = r.ReadElementContentAsString();
                    }
                    else if (r.LocalName == "Value")
                    {
                        value_str = r.ReadElementContentAsString();
                    }
                }
            }
        
        }
        static private void readLinkList(XmlReader r, out string type)
        {
            type = null;
            if (!r.ReadToDescendant("DataType")) return;
            if (!r.ReadToDescendant("Name")) return;
            type = r.ReadElementContentAsString();
        }
        static private void readConstant(XmlReader r, List<Constant> constants)
        {

          
            string comment = null;
            string name = null;
            string value_str = null;
            string type = null;
            while (r.Read())
            {
                if (r.IsStartElement())
                {
                    if (r.LocalName == "ObjectList")
                    {
                        readObjectList(r.ReadSubtree(), out comment);
                    }
                    else if (r.LocalName == "AttributeList")
                    {
                        readAttributeList(r.ReadSubtree(), out name, out value_str);
                    }
                    else if (r.LocalName == "LinkList")
                    {
                        readLinkList(r.ReadSubtree(), out type);
                    }
                }
            }
            if (name != null && value_str != null && type != null)
            {
                if (type == "Int")
                {
                    object value = int.Parse(value_str);
                    constants.Add(new Constant(name, value, comment));
                }
            }
        }
        static public List<Constant> getConstants(string filename)
        {
            List<Constant> constants = new List<Constant>();

            XmlReaderSettings settings = new XmlReaderSettings
            {
                ConformanceLevel = ConformanceLevel.Document,
            };
            using (Stream stream = File.Open(filename, FileMode.Open))
            {
                XmlReader r = XmlReader.Create(stream, settings);
                if (!r.ReadToFollowing("Document")) throw new XmlException("Top node Document not found");
                if (!r.ReadToDescendant("SW.ControllerTagTable")) throw new XmlException("SW.ControllerTagTable not found");
                if (!r.ReadToDescendant("SW.ControllerConstant")) throw new XmlException("SW.ControllerConstant not found");
                do
                {
                    if (r.GetAttribute("AggregationName") == "Constants")
                    {
                     readConstant(r.ReadSubtree(), constants);
                    }
                } while (r.ReadToNextSibling("SW.ControllerConstant"));
            }
            return constants;
        }
    }
}
