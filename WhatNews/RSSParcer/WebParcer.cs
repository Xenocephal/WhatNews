using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Xml.Serialization;
using System.Xml;

namespace WhatNews {
    public class WebParcer { // Класс занимается подключением к веб-ресурсу и получением содержимого        
        protected internal bool DataRecieved { get => Content != null; } // Свойство показывающее есть ли контент
        public string Message { get; set; } // Сообщение
        protected internal Uri URI { get; set; } // свойство содержит ссылку на веб-ресурс 
        protected internal string Content { get; set; } // содержимое считанное с веб-ресурса        
        protected internal RSS NEWS { get; private set; } // считанные новости       

        protected internal bool HasNewData = true; // Флаг показывающий нужно ли обновлять коллекцию новостей.

        private WebClient Client; // веб клиент
        XmlSerializer Serializer; // Сериализатор 

        // ---------------------------- [ Конструкторы ]----------------------------

        protected internal WebParcer(Uri uri, Encoding encoding) { // Конструктор со ссылкой и кодировкой
            Serializer = new XmlSerializer(typeof(RSS));
            Client = new WebClient();
            Client.Encoding = encoding;
            Client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            URI = uri;
            FirstStart();
        }

        // ---------------------------- [ Методы ] ----------------------------

        private void FirstStart() { // Метод выполняется при старте программы
            try {
                Get(); // получаем новости.                
                if (DataRecieved) {
                    GetRSS();
                    if (!File.Exists("file.xml")) SaveXML();
                }
                else NEWS = GetXMLRss();
            }
            catch (Exception ex) { Message += $"FirstStart: [{ex.Source}] {ex.Message}\n"; }
        }

        protected internal void GetNews() { // Главный метод получения новостей
            Get(); // Получаем данные с сайта.
            GetRSS(); // Преобразуем данные в новости.
            RSS Olds = GetXMLRss(); // Получаем новости из XML.
            if (Olds != null && NEWS != null) { // Если все удачно получили,
                Item first = NEWS.Channel.Items.FirstOrDefault(); // Берем первую новость из набора новых.
                Item second = Olds.Channel.Items.FirstOrDefault(); // Берем первую новость из набора старых.
                Message += "\n NEWS: " + first.Title + "\n OLDS: " + second.Title + "\n";
                if (!first.Equals(second)) { // Если новый набор отличается от старого,                   
                    SaveXML(); // сохраняем его.
                    HasNewData = true; // Ставим флаг, что есть новые данные.
                    Message += "GetNews: News has been refreshed\n";
                }
                else { // Иначе говорим, что новых данных нет
                    HasNewData = false;
                    Message += "GetNews: There is no fresh News\n";
                } 
            }
            else if (Olds != null) { // Если новый набор пуст, а старый нет, то берем старый.
                NEWS = Olds;
                Message += "Loaded from XML";
            }
        }

        protected internal void Get() { // Метод получает данные новостей с веб-ресурса
            try {               
                Stream data = Client.OpenRead(URI);
                using (StreamReader reader = new StreamReader(data)) Content = reader.ReadToEnd();
                if (DataRecieved) Message = "Get: Data has been recieved\n";
                else Message = "Get: Data was not recieved\n";
            }
            catch (Exception ex) { Message += $"Get: [{ex.Source}] {ex.Message}\n"; }
        }

        protected internal async void GetAsync() { // Метод ассинхронно получает данные новостей с веб-ресурса
            try {
                Stream data = await Client.OpenReadTaskAsync(URI);
                using (StreamReader reader = new StreamReader(data)) Content = reader.ReadToEnd();
                if (DataRecieved) Message = "Data has recieved\n";
                else Message = "Data was not recieved\n";
            }
            catch (Exception ex) { Message += $"GetAsync: [{ex.Source}] {ex.Message}\n"; }
        }        

        protected internal void GetRSS() { // Метод преобразует строку в XML, который впоследствие дессериализует
            try {
                if (DataRecieved) { // если получили что-то,
                    byte[] content = Encoding.UTF8.GetBytes(Content); // преобразуем в поток
                    Stream stream = new MemoryStream(content);
                    NEWS = (RSS)Serializer.Deserialize(stream); // и десериализуем поток
                    Message += "GetRSS: Loaded from Lenta.ru\n";
                }
                else NEWS = GetXMLRss(); // пробуем получить данные из ранее сохраненного XML
            }
            catch (Exception ex) { // если подключиться не удалось,
                Message += $"GetRSS: [{ex.Source}] {ex.Message}\n";               
            }
        }       

        protected internal RSS GetXMLRss() { // Метод возвращает данные новостей из ранее скачанного xml файла
            try {
                RSS Result = null;
                if (File.Exists("file.xml")) { // Если файл XML существует,                    
                    using (XmlReader reader = XmlReader.Create("file.xml")) { // считываем файл XML 
                        Result = (RSS)Serializer.Deserialize(reader); // десериализуем его.    
                    }
                }
                return Result;
            }
            catch (Exception ex) {
                Message += $"GetXMLRss: [{ex.Source}] {ex.Message}\n";
                return null;
            }            
        }

        protected internal void SaveXML() { // Метод Сохраняет XML-файл данных в папке с программой
            if (Content != null) {
                using (StreamWriter sw = new StreamWriter("file.xml", false, Encoding.UTF8)) {
                    sw.WriteLine(Content);
                }
            }
        }
    }
}
