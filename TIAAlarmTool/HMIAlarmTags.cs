using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TIAAlarmTool
{
    class HMIAlarmTags
    {

        public static string buildFile(AlarmDefinitionList defs, string data_block, string connection)
        {
          

                XmlWriterSettings settings = new XmlWriterSettings
                {
                    ConformanceLevel = ConformanceLevel.Document,
                    Encoding = Encoding.UTF8,
                    Indent = true
                };
                string filename;
                using (Stream stream = TempFile.Open("AlarmConst", "xml", out filename))
                {

                    XmlWriter w = XmlWriter.Create(stream, settings);
                    HMITagTable table = new HMITagTable("Alarms");
                    foreach (AlarmDefinition a in defs) {
                        HMITag t = new HMITag(a.Name, "String", "WString", data_block+".Props["+a.ID+"].Text",connection);
                        table.Add(t);
                    }
                    table.write(w);
                }
                return filename;
            }
        }
}
