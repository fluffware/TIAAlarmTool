using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TIAAlarmTool
{
    class HMITag
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public string HMIDataType { get; set; }
        public string ControllerTag { get; set; }
        public string AcquisitionCycle { get; set; }
        public string Connection { get; set; }
        public int ArrayLength { get; set; }

        public HMITag(string name, string data_type, string hmi_data_type, string controller_tag = null, string connection = null)
        {
            Name = name;
            DataType = data_type;
            HMIDataType = hmi_data_type;
            ControllerTag = controller_tag;
            AcquisitionCycle = "1 s";
            Connection = Connection;
            ArrayLength = 0;
        }

        public void write(XmlWriter w)
        {
            w.WriteStartElement("Hmi.Tag.Tag");
            w.WriteAttributeString("AggregationName", "Tags");
            w.WriteStartElement("AttributeList");
            XMLUtil.SimpleValue(w, "AddressAccessMode", "Absolute");
            XMLUtil.SimpleValue(w, "Name", Name);

            w.WriteEndElement(); // AttributeList

            w.WriteStartElement("LinkList");
            XMLUtil.Link(w,"AcquisitionCycle", "10 s");
            if (ControllerTag != null)
            {
                XMLUtil.Link(w, "ControllerTag", ControllerTag);
            }
            if (Connection != null)
            {
                XMLUtil.Link(w, "Connection", ControllerTag);
            }
            XMLUtil.Link(w, "DataType", DataType);
            XMLUtil.Link(w, "HmiDataType", HMIDataType);
            w.WriteEndElement(); // LinkList

            w.WriteStartElement("ObjectList");

            if (ArrayLength > 0)
            {
                for (int i = 0; i < ArrayLength; i++)
                {
                    w.WriteStartElement("Hmi.Tag.TagArrayMember");
                    w.WriteAttributeString("AggregationName", "Elements");
                    w.WriteStartElement("AttributeList");
                    XMLUtil.SimpleValue(w, "Name", "["+i.ToString()+"]");
                    w.WriteEndElement(); // AttributeList

                    w.WriteEndElement(); // Hmi.Tag.TagArrayMember

                    w.WriteEndElement(); // ObjectList
                }
            }
            w.WriteEndElement(); // HMI.Tag.Tag
        }
    }
}
