﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Line l = new Line();
            l.X1 = 10;
            l.Y1 = 10;
            l.X2 = 200;
            l.Y2 = 500;
            l.Stroke = new SolidColorBrush(Colors.Black);
            l.StrokeThickness = 5;
            c.Children.Add(l);
        }
    }
}
