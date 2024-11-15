using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Gabana.ShareSource.ClassStructure
{
    // ตัวอย่าง การ deserialize xml to object เผื่อก็ณีจำเป็นต้องใช้
    //using (StreamReader sr = new StreamReader(Forms.Context.Assets.Open("Slip01.xml")))
    //{
    //    string testData = sr.ReadToEnd();
    //    XmlSerializer serializer = new XmlSerializer(typeof(SlipLayout));
    //    using (TextReader reader = new StringReader(testData))
    //    {
    //        SlipLayout result = (SlipLayout)serializer.Deserialize(reader);
    //    }

    //}
    [XmlRoot("SlipLayout")]
    public class SlipLayout
    {
        [XmlElement("Header")]
        public Header header { get; set; }
        [XmlElement("Details")]
        public Details details { get; set; }

        [XmlElement("Footer")]
        public Footer footer { get; set; }
    }
    public class Line
    {
        [XmlElement("Text")]
        public List<string> Text { get; set; }
    }
    public class Header
    {
        [XmlElement("Line")]
        public List<Line> line { get; set; }

    }
    public class Details
    {
        [XmlElement("Line")]
        public List<Line> line { get; set; }
    }
    public class Footer
    {
        [XmlElement("Line")]
        public List<Line> line { get; set; }
    }
}
