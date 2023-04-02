using Project.Classes;
using Project.Draw;
using Project.Import;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Win = System.Windows;

namespace Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Win.Point> currentPolygonLines = new List<Win.Point>();
        public MainWindow()
        {
            InitializeComponent();

            PM.Size = 300;
            PM.Move = 3;
            DrawingHelper.Window = this;
        }
        private void DrawEllipseMouseUp(object sender, MouseButtonEventArgs e)
        {
            MainCanvas.MouseRightButtonDown -= DrawEllipseMouseUp;
            var position = e.GetPosition(MainCanvas);
            var de = new DrawEllipse();
            de.ShowDialog();
            
            var c = new Grid();
            var ellipse = new Ellipse
            {
                Stroke = de.Stroke,
                Fill = de.Fill,
                StrokeThickness = de.StrokeThickness,
                Width = de.RadiusX,
                Height = de.RadiusY
            };
            c.Background = Brushes.Transparent;
            c.Children.Add(ellipse);
            if(!string.IsNullOrEmpty(de.Text))
            {
                var text = new TextBlock 
                { 
                    Text = de.Text, 
                    Foreground = de.TextColor, 
                    HorizontalAlignment =  HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                c.Children.Add(text);
            }
            MainCanvas.Children.Add(c);
        }
        private void DrawEllipseMenu_Click(object sender, RoutedEventArgs e)
        {
            MainCanvas.MouseRightButtonUp += DrawEllipseMouseUp;
        }

        private void AddPolygonPoints(object sender, MouseButtonEventArgs e)
        {
            currentPolygonLines.Add(Mouse.GetPosition(MainCanvas));
        }
        private void DrawPolygon(object sender, MouseButtonEventArgs e)
        {
            MainCanvas.MouseRightButtonUp -= AddPolygonPoints;
            MainCanvas.MouseLeftButtonUp -= DrawPolygon;

            if(currentPolygonLines.Count < 3)
            {
                currentPolygonLines.Clear();
                MessageBox.Show("Mora biti najmanje 3 tacke za poligon.");
                return;
            }

            var dp = new DrawPolygon();
            dp.ShowDialog();

            var c = new Grid();
            var polygon = new Polygon
            {
                Stroke = dp.Stroke,
                Fill = dp.Fill,
                StrokeThickness = dp.StrokeThickness,
                Points = new PointCollection(currentPolygonLines)
            };
            currentPolygonLines.Clear();
            c.Background = Brushes.Transparent;
            c.Children.Add(polygon);
            if (!string.IsNullOrEmpty(dp.Text))
            {
                var text = new TextBlock
                {
                    Text = dp.Text,
                    Foreground = dp.TextColor,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                c.Children.Add(text);
            }
            MainCanvas.Children.Add(c);
        }
        private void DrawPolygonMenu_Click(object sender, RoutedEventArgs e)
        {
            currentPolygonLines.Clear();
            MainCanvas.MouseRightButtonUp += AddPolygonPoints;
            MainCanvas.MouseLeftButtonUp += DrawPolygon;
        }

        private void LoadModel_Click(object sender, RoutedEventArgs e)
        {
            Importer.Load();
            Importer.MinMaxFinder();
            DrawingHelper.DrawEntities();
            DrawingHelper.DrawLines();
        }

        private void AddTextMenu_Click(object sender, RoutedEventArgs e)
        {
            MainCanvas.MouseRightButtonUp += AddTextButtonDown;
        }

        private void AddTextButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainCanvas.MouseRightButtonUp -= AddTextButtonDown;

            var at = new AddText();
            var position = e.GetPosition(MainCanvas);
            at.ShowDialog();

            var text = new TextBlock
            {
                Text = at.Text,
                Foreground = at.TextColor,
                FontSize = at.TextSize
            };
            
            MainCanvas.Children.Add(text);
        }
    }
}
