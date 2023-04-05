using System;
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
        private Ellipse el = null;
        private TextBlock tb = null;
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

        public DrawEllipse(Ellipse e, TextBlock t)
        {
            InitializeComponent();
            el = e;
            tb = t;
            radiusX.Text = e.Width.ToString();
            radiusY.Text = e.Height.ToString();
            strokeThickness.Text = e.StrokeThickness.ToString();
            strokeColor.SelectedColor = ((SolidColorBrush)e.Stroke).Color;
            fillColor.SelectedColor = ((SolidColorBrush)e.Fill).Color;
            textColor.SelectedColor = ((SolidColorBrush)t.Foreground).Color;
            textBox.Text = t.Text;
        }
        private void drawEllipseButton_Click(object sender, RoutedEventArgs e)
        {
            if(el != null)
            {
                el.Fill = new SolidColorBrush(fillColor.SelectedColor ?? Colors.Black);
                el.Stroke = new SolidColorBrush(strokeColor.SelectedColor ?? Colors.Black);
                el.Width = double.Parse(radiusX.Text);
                el.Height = double.Parse(radiusY.Text);
                el.StrokeThickness = double.Parse(strokeThickness.Text);
                tb.Text = textBox.Text;
                tb.Foreground = new SolidColorBrush(textColor.SelectedColor ?? Colors.Black);
                Close();
                return;
            }

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
                MessageBox.Show("Polja nisu lepo popunjena");
                return;
            }
            Close();
        }
    }
}
