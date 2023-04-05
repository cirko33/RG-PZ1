using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
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
        private TextBlock tb = null;
        public string Text { get; set; }
        public SolidColorBrush TextColor { get; set; }
        public double TextSize { get; set; }
        public AddText()
        {
            InitializeComponent();
            textColor.SelectedColor = Colors.Black;
        }

        public AddText(TextBlock text)
        {
            InitializeComponent();
            tb = text;
            textColor.SelectedColor = ((SolidColorBrush)text.Foreground).Color;
            textBox.Text = text.Text;
            textSize.Text = text.FontSize.ToString();
        }

        private void addText_Click(object sender, RoutedEventArgs e)
        {
            if (tb != null)
            {
                tb.Text = textBox.Text;
                tb.Foreground = new SolidColorBrush(textColor.SelectedColor ?? Colors.Black);
                tb.FontSize = double.Parse(textSize.Text);
                Close();
                return;
            }
            if (string.IsNullOrEmpty(textBox.Text) || textColor.SelectedColor == null || textSize.Text == null)
                return;

            try
            {
                Text = textBox.Text;
                TextColor = new SolidColorBrush(textColor.SelectedColor ?? Colors.Black);
                TextSize = double.Parse(textSize.Text);
                if (TextSize < 0)
                    throw new Exception();
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
