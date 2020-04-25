using System.Xml.Serialization;

namespace WhatNews {

    [XmlRoot("rss")]
    public class RSS { // Основной класс новостей, содержащий в себе:

        [XmlAttribute("version")]
        public string Version { get; set; } // Аттрибут - версия

        [XmlElement("channel")]
        public Channel Channel { get; set; } // Элемент - канал
    }
}
