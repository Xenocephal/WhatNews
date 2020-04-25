using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WhatNews {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public MainModel model;
        public MainWindow() {
            InitializeComponent();
            model = new MainModel();
            DataContext = model;
            NewsList.ItemsSource = model.ItemsToShow;
            NewsList.MouseRightButtonUp += model.AddNews;
            NewsList.MouseDoubleClick += model.OpenLink;
        }
    }
}
