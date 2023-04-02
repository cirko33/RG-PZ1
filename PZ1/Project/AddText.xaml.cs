using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace Project
{
    /// <summary>
    /// Interaction logic for AddText.xaml
    /// </summary>
    public partial class AddText : Window
    {
        public string Text { get; set; }
        public SolidColorBrush TextColor { get; set; }

        public int TextSize { get; set; }
        public AddText()
        {
            InitializeComponent();
            var fonts = new List<int>();
            for (int i = 2; i <= 50; i+=2)
                fonts.Add(i);

            textSize.ItemsSource = fonts;
            textSize.SelectedIndex = 5;
            textColor.SelectedColor = Colors.Black;
        }

        private void addText_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox.Text) || textColor.SelectedColor == null || textSize.SelectedItem == null)
                return;

            try
            {
                Text = textBox.Text;
                TextColor = new SolidColorBrush(textColor.SelectedColor ?? Colors.Black);
                TextSize = double.Parse(textSize.SelectedItem.ToString());
            }
            catch (Exception)
            {
                return;
            }
            Close();
        }
    }
}
