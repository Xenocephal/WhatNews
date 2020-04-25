using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Timers;

namespace WhatNews {
    public class MainModel : INotifyPropertyChanged {

        public string About { get; } = 
            "Данная программа получает новости с адреса https://lenta.ru/rss/news \n" +
            "\nПри нажатии правой кнопки мыши, в набор добавится 4 прошлых новости.\n" +
            "\nВ папку с программой автоматически сохраняется xml-файл со всеми полученными новостями. " +
            "При отсутствие интернета и наличии данного файла, новости будут считаны из него. " +
            "\nИнтервал обновления составляет 2 минуты. Последний заголовок полученных новостей " +
            "сравнивается с заголовком сохраненных новостей, при различие происходит обновление контента.\n" +
            "\nАвтор: Тен Антон\ne-mail: gauss87@mail.ru\nvk: https://vk.com/id318606";

        public Uri Link { get; set; } = new Uri("https://lenta.ru/rss/news");
        private List<Item> Items;
        private Item selectedItem;
        private int index = 0;
        private Timer Timer;        

        public ObservableCollection<Item> ItemsToShow { get; private set; } = new ObservableCollection<Item>();
        public Item SelectedItem { get => selectedItem; set { selectedItem = value; OnPropertyChanged("SelectedItem"); } }

        private string message;
        public string Message { get => message; set { message = value; OnPropertyChanged("Message"); } }

        public Command CloseCommand { get; }
        public Command RefreshCommand { get; }
        public Command OpenMessageCommand { get; }

        private WebParcer NewsReader;

        public MainModel() {
            CloseCommand = new Command(Close);
            RefreshCommand = new Command(Refresh);
            OpenMessageCommand = new Command(ShowMessages);

            NewsReader = new WebParcer(Link, System.Text.Encoding.Default);
            Items = NewsReader.NEWS.Channel.Items.OrderByDescending(x => x.PubDate).ToList(); // Берем набор и сотрируем по дате.
            AddItems(10); // Добавляем 10 новостей из набора.
            Timer = new Timer(); // Создаем таймер.
            Timer.Interval = 120000; // Интервал обновления новостей 2 минуты
            Timer.Elapsed += OnTick; // задаем действие по таймеру.
            Timer.Start(); // Запускаем.
            
        }

        private void ShowMessages() { // Метод выводит окно для просмотра системных сообщений.
            MessageWindow window = new MessageWindow();
            window.DataContext = this;
            window.Show();
        }

        private void Refresh() { // Метод обновления новостей.
            try {
                Message = "Getting News...";
                NewsReader.GetNews(); // Получаем новости.
                if (NewsReader.DataRecieved) { // Если данные получены и они новые:                    
                    Items = NewsReader.NEWS.Channel.Items.OrderByDescending(x => x.PubDate).ToList(); // Берем набор и сотрируем по дате.    
                    ItemsToShow.Clear(); // Очищаем отображаемый набор.
                    index = 0; // Обнуляем индекс.
                    AddItems(10); // Добавляем 10 новостей из набора.
                    Message = "\n" + NewsReader.Message; // Обновляем сообщения.
                }
            }
            catch (Exception ex) { Message = $"Refresh: [{ex.Source}] {ex.Message}\n"; }
        }

        private void RefreshOnTick() { // Метод обновления новостей.
            try {
                Message = "";
                NewsReader.GetNews(); // Получаем новости.
                if (NewsReader.DataRecieved) { // Если данные получены 
                    if (NewsReader.HasNewData) { // И если они новые:
                        Items = NewsReader.NEWS.Channel.Items.OrderByDescending(x => x.PubDate).ToList(); // Берем набор и сотрируем по дате.
                        int count = ItemsToShow.Count + 1; // Находим количество новостей, которое добавим с новым набором.
                        index = 0;
                        ItemsToShow.Clear();
                        AddItems(count);
                        Message = NewsReader.Message; // Обновляем сообщения.
                    }
                    else Message += "\nData recieved, but there is no new Data.\n" + NewsReader.Message;
                }
                else Message = "Data was not recieved";
            }
            catch (Exception ex) { Message = $"Refresh: [{ex.Source}] {ex.Message}\n"; }
        }

        private void OnTick(object sender, ElapsedEventArgs e) {
            Application.Current.Dispatcher.Invoke(() => RefreshOnTick()); // Обновление производим из главного потока.
        }
        
        internal void OpenLink(object sender, MouseButtonEventArgs e) { // Метод перехода по ссылке
            System.Diagnostics.Process.Start(SelectedItem.Link);
        }
       
        private void AddItems(int n) { // Метод добавления новостей
            if (index + n < Items.Count) { // Если мы берем не больше чем есть,
                Items.GetRange(index, n).ForEach(item => ItemsToShow.Add(item)); // берем n новостей и добавляем их в отображаемые.
                index += n; // Смещаем индекс на количество взятых.
            }            
        }

        internal void AddNews(object sender, EventArgs e) { // Метод для события добавления новостей в список.           
            AddItems(4); 
        }       

        private void Close() { 
            Application.Current.Shutdown();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
