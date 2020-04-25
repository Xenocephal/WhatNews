using System.Xml.Serialization;

namespace WhatNews {

    public class Item { // Класс описывает элемент Новость, содержащий в себе:
        
        private string description; 

        [XmlElement("link")]
        public string Link { get; set; } // Ссылку на новость

        [XmlElement("title")]
        public string Title { get; set; } // Заголовок новости

        [XmlElement("description")]
        public string Description { get => description; // Описание новости
            set {
                description = value;
                if (description.Contains("<br />")) description = description.Replace("<br />", string.Empty);
                if (description.StartsWith("\n")) description = description.Replace("\n",string.Empty).Trim();                
            } 
        } 

        [XmlElement("pubDate")]
        public string PubDate { get; set; } // Дату публикации.

        [XmlElement("enclosure")]
        public Enclosure Enclosure { get; set; } // Вложение

        public override bool Equals(object obj) { // Переопределяем метод сравнения для Новости.
            Item itemObj = obj as Item; // Приводим объект к нужному типу.
            if (itemObj == null) return false; // Если приведение не удалось, возвращаем ложь
            else return Title.Equals(itemObj.Title); // Иначе возвращаем результат сравнения Заголовков.
        }
        public override int GetHashCode() {
            return Title.GetHashCode();
        }
    }
}
