using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Text.RegularExpressions;

namespace TIAAlarmTool
{
    class AlarmDB
    {
   
        static protected void StartValue(XmlWriter w, string path, string value)
        {
            w.WriteStartElement("StartValue");
            w.WriteAttributeString("Path", path);
            w.WriteString(value);
            w.WriteEndElement();
        }
 

        static public string buildFile(AlarmDefinitionList alarms)
        {
         
            XmlWriterSettings settings = new XmlWriterSettings {
                ConformanceLevel = ConformanceLevel.Document, 
                Encoding = Encoding.UTF8, 
                Indent = true };
            string filename;
            using (Stream stream = TempFile.Open("sDB_Larm","xml", out filename))
            {
                // Round up to multiples of 16, since HMI uses words to transfer alarm state
                int maxAlarmCount = ((alarms.Count()+15)/16) * 16;
                XmlWriter w = XmlWriter.Create(stream, settings);
                w.WriteStartDocument();
                w.WriteStartElement("Document");

                w.WriteStartElement("DocumentInfo");
                w.WriteEndElement(); // DocumentInfo
                w.WriteStartElement("SW.DataBlock");
                w.WriteAttributeString("ID", "0");
                w.WriteStartElement("AttributeList");

                XMLUtil.SimpleValue(w, "AutoNumber", "true");
                XMLUtil.SimpleValue(w, "DatablockType", "SharedDB");
                XMLUtil.SimpleValue(w, "HeaderVersion", "0.1");
               
                w.WriteStartElement("Interface");
                w.WriteStartElement("Sections","http://www.siemens.com/automation/Openness/SW/Interface/v1");
                w.WriteStartElement("Section");
                w.WriteAttributeString("Name", "Static");

                w.WriteStartElement("Member");
                w.WriteAttributeString("Name", "Props");
                w.WriteAttributeString("Remanence", "Classic");
                w.WriteAttributeString("Datatype", "Array[1.."+maxAlarmCount+"] of \"AlarmProperties\"");
                w.WriteStartElement("AttributeList");
                XMLUtil.BooleanAttribute(w, "HmiAccessible", true, XMLUtil.SystemDefined);
                XMLUtil.BooleanAttribute(w, "HmiVisible", true, XMLUtil.SystemDefined);
                w.WriteEndElement(); // AttributeList

                w.WriteStartElement("Sections");
                w.WriteStartElement("Section");
                w.WriteAttributeString("Name", "None");

                // Silent member
                w.WriteStartElement("Member");
                w.WriteAttributeString("Name", "Silent");
                w.WriteAttributeString("Datatype", "Bool");

                w.WriteStartElement("AttributeList");
                w.WriteEndElement(); // AttributeList

                int index = 1;
                foreach(AlarmDefinition a in alarms) {
                    StartValue(w, index.ToString(), (a.Options & AlarmDefinition.Option.Silent) != 0 ? "true" : "false");
                    index++;
                }
                w.WriteEndElement(); // Member

                // AutoAck member
                w.WriteStartElement("Member");
                w.WriteAttributeString("Name", "AutoAck");
                w.WriteAttributeString("Datatype", "Bool");

                w.WriteStartElement("AttributeList");
                w.WriteEndElement(); // AttributeList

                index = 1;
                foreach (AlarmDefinition a in alarms)
                {
                    StartValue(w, index.ToString(), (a.Options & AlarmDefinition.Option.AutoAck) != 0 ? "true" : "false");
                    index++;
                }
                w.WriteEndElement(); // Member

                // Text member
                w.WriteStartElement("Member");
                w.WriteAttributeString("Name", "Text");
                w.WriteAttributeString("Datatype", "String[64]");

                w.WriteStartElement("AttributeList");
                w.WriteEndElement(); // AttributeList

                index = 1;
                foreach (AlarmDefinition a in alarms)
                {
                    StartValue(w, index.ToString(), "'"+a.Text+"'");
                    index++;
                }
                w.WriteEndElement(); // Member
             
                w.WriteEndElement(); // Section
                w.WriteEndElement(); // Sections
                w.WriteEndElement(); // Member

                w.WriteEndElement(); // Section
                w.WriteEndElement(); // Sections
                w.WriteEndElement(); // Interface

                string now = DateTime.UtcNow.ToString("o");
                XMLUtil.SimpleValue(w, "InterfaceModifiedDate", now, true);
                XMLUtil.SimpleValue(w, "IsKnowHowProtected", now, true);
                XMLUtil.SimpleValue(w, "IsOnlyStoredInLoadMemory", "false");
                XMLUtil.SimpleValue(w, "IsPLCDB", "false", true);
                XMLUtil.SimpleValue(w, "IsWriteProtectedInAS", "false");
                XMLUtil.SimpleValue(w, "MemoryLayout", "Standard");
                XMLUtil.SimpleValue(w, "ModifiedDate", now, true);
                XMLUtil.SimpleValue(w, "Name", "sDB_LarmDefs");
                XMLUtil.SimpleValue(w, "Number", "532");
                XMLUtil.SimpleValue(w, "ParameterModified", now, true);
                XMLUtil.SimpleValue(w, "ProgrammingLanguage", "DB");
                XMLUtil.SimpleValue(w, "StructureModified", now, true);
                XMLUtil.SimpleValue(w, "Type", "DB");
                w.WriteEndElement(); // AttributeList
                w.WriteEndElement(); // SW.Datablock
                w.WriteEndElement(); // Document
                w.Close();
            }
            return filename;
        }
        const string InterfaceNS = "http://www.siemens.com/automation/Openness/SW/Interface/v1";
        static void readSilentMember(XmlReader r, AlarmDefinition[] array)
        {
            if (!r.ReadToDescendant("StartValue")) return;
            while (r.IsStartElement())
            {
                int index = int.Parse(r.GetAttribute("Path"));
                if (index >= 1 && index <= array.Length)
                {
                    string value = r.ReadElementContentAsString("StartValue", InterfaceNS);
                    
                    array[index - 1].Options |= (value.Contains("TRUE") || value.Contains("true")) ? AlarmDefinition.Option.Silent : 0;
                }
                else
                {
                    r.ReadToNextSibling("StartValue");
                }
            }
        }

        static void readAutoAckMember(XmlReader r, AlarmDefinition[] array)
        {

            if (!r.ReadToDescendant("StartValue")) return;
            while (r.IsStartElement())
            {
                int index = int.Parse(r.GetAttribute("Path"));
                if (index >= 1 && index <= array.Length)
                {
                     string value = r.ReadElementContentAsString("StartValue", InterfaceNS);
                    
                    array[index - 1].Options |= (value.Contains("TRUE") || value.Contains("true")) ? AlarmDefinition.Option.AutoAck : 0;
                }
                else
                {
                    r.ReadToNextSibling("StartValue");
                }
            }
        }
        static void readTextMember(XmlReader r, AlarmDefinition[] array)
        {

            if (!r.ReadToDescendant("StartValue")) return;
            while (r.IsStartElement())
            {
                int index = int.Parse(r.GetAttribute("Path"));
                if (index >= 1 && index <= array.Length)
                {
                    string text = r.ReadElementContentAsString("StartValue", InterfaceNS);
                    array[index - 1].Text = text.Substring(1, text.Length - 2); // Remove quotes
                }
                else
                {
                    r.ReadToNextSibling("StartValue");
                }
            }
        }
        static private void readStaticSection(XmlReader r, AlarmDefinitionList alarms)
        {
            if (!r.ReadToDescendant("Member")) return;
         
            string name = r.GetAttribute("Name");
            if (name != "Props") throw new XmlException("No Props member found");
            string type = r.GetAttribute("Datatype");
            Match match = Regex.Match(type,"^Array\\[\\d+..(\\d+)\\] of");
            if (match.Success)
            {
                int last = int.Parse(match.Groups[1].Value);
                Console.WriteLine("Last index {0}", last);
                AlarmDefinition[] array = new AlarmDefinition[last];
                for (int i = 0; i < last; i++)
                {
                    array[i] = new AlarmDefinition(null, null);
                    array[i].ID = i + 1;
                }
                    if (!r.ReadToDescendant("Member")) throw new XmlException("Member not found");
                do
                {
                    string member_name = r.GetAttribute("Name");
                    string member_type = r.GetAttribute("Datatype");
                    if (member_name == "Silent" && member_type == "Bool")
                    {
                        readSilentMember(r.ReadSubtree(), array);
                    }
                    else if (member_name == "AutoAck" && member_type == "Bool")
                    {
                        readAutoAckMember(r.ReadSubtree(), array);
                    }
                    else if (member_name == "Text" && member_type.StartsWith("String["))
                    {
                        readTextMember(r.ReadSubtree(), array);
                    }
                } while (r.ReadToNextSibling("Member"));
                alarms.Clear();
                alarms.AddRange(array);
            }
        }
        static public void parseFile(AlarmDefinitionList alarms, string filename)
        {

            XmlReaderSettings settings = new XmlReaderSettings
            {
                ConformanceLevel = ConformanceLevel.Document,
            };
            using (Stream stream = File.Open(filename, FileMode.Open))
            {
                XmlReader r = XmlReader.Create(stream, settings);
                if (!r.ReadToFollowing("Document")) throw new XmlException("Top node Document not found");
                if (!r.ReadToDescendant("SW.DataBlock")) throw new XmlException("SW.Datablock not found");
                if (!r.ReadToDescendant("Interface")) throw new XmlException("Interface not found");
                if (!r.ReadToDescendant("Section")) throw new XmlException("Section not found");
                do
                {
                    if (r.GetAttribute("Name") == "Static")
                    {
                        readStaticSection(r.ReadSubtree(), alarms);
                    }
                } while (r.ReadToNextSibling("Section"));           
            }
        }
    }
}
