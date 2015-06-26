using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TIAAlarmTool
{
    class HMITagTable
    {
        protected List<HMITag> tags;
        public string Name { get; set; }
        public HMITagTable(string name)
        {
            Name = name;
            tags = new List<HMITag>();
        }

        public void Add(HMITag tag)
        {
            tags.Add(tag);
        }

        public void write(XmlWriter w)
        {
            w.WriteStartElement("Hmi.Tag.TagTable");
            w.WriteStartElement("AttributeList");
            XMLUtil.SimpleValue(w, "Name", Name);
            w.WriteEndElement(); // AttributeList
            w.WriteStartElement("ObjectList");
            foreach (HMITag t in tags)
            {
                t.write(w);
            }
            w.WriteEndElement(); // ObjectList
            w.WriteEndElement(); // Hmi.Tag.TagTable
        }
    }
}
