using System.Collections.Generic;
using System.Xml.Serialization;

namespace WhatNews {    
    
    public class Channel { // Класс описывает элемент - Канал, содержащий в себе:

        [XmlElement("language")]
        public string Language { get; set; } // элемент - Язык

        [XmlElement("link")]
        public string Link { get; set; } // элемент - Ссылку

        [XmlElement("title")]
        public string Title { get; set; } // элемент - Заголовок канала

        [XmlElement("description")]
        public string Description { get; set; } // элемент - Описание

        [XmlElement("item", typeof(Item))]
        public List<Item> Items{ get; set; } // коллекцию элементов - Новостей
    }
}
