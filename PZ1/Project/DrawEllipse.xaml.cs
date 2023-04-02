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
using System.Windows.Shapes;

namespace Project
{
    /// <summary>
    /// Interaction logic for DrawEllipse.xaml
    /// </summary>
    public partial class DrawEllipse : Window
    {
        public SolidColorBrush Fill { get; set; }
        public SolidColorBrush Stroke { get; set; }
        public double RadiusX { get; set; }
        public double RadiusY { get; set; }
        public double StrokeThickness { get; set; }
        public string Text { get; set; }
        public SolidColorBrush TextColor { get; set; }
        public DrawEllipse()
        {
            InitializeComponent();
            fillColor.SelectedColor = Colors.Black;
            strokeColor.SelectedColor = Colors.Black;
            textColor.SelectedColor = Colors.Black;
        }

        private void drawEllipseButton_Click(object sender, RoutedEventArgs e)
        {
            if (fillColor.SelectedColor == null || strokeColor.SelectedColor == null || radiusX == null || radiusY == null
                || strokeThickness == null)
                return;

            try
            {
                Fill = new SolidColorBrush(fillColor.SelectedColor ?? Colors.Black);
                Stroke = new SolidColorBrush(strokeColor.SelectedColor ?? Colors.Black);
                RadiusX = double.Parse(radiusX.Text);
                RadiusY = double.Parse(radiusY.Text);
                StrokeThickness = double.Parse(strokeThickness.Text);
                Text = textBox.Text;    
                TextColor = new SolidColorBrush(textColor.SelectedColor ?? Colors.Black);
            }
            catch (Exception)
            {
                return;
            }
            Close();
        }
    }
}
