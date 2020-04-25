using System.Xml.Serialization;

namespace WhatNews {
    
    public class Enclosure {
        
        [XmlAttribute("url")]
        public string Image { get; set; }
    }
}
