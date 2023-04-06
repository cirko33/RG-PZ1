using Project.Classes;
using Project.Draw;
using Project.Import;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
        private List<Command> ClearHistory = new List<Command>();
        private List<Command> RedoHistory = new List<Command>();
        private List<Command> UndoHistory = new List<Command>();
        private bool isCleared = false;
        public MainWindow()
        {
            InitializeComponent();
            Undo.IsEnabled = false;
            Redo.IsEnabled = false;
            Clear.IsEnabled = false;
        }

        #region Ellipse,Polygon,Text
        private void DrawEllipseMenu_Click(object sender, RoutedEventArgs e)
        {
            MainCanvas.MouseRightButtonUp -= DrawPolygonMouseUp;
            MainCanvas.MouseRightButtonUp -= AddTextMouseUp;
            MainCanvas.MouseLeftButtonUp -= AddPolygonPoints;
            MainCanvas.MouseRightButtonUp += DrawEllipseMouseUp;
        }
        private void DrawEllipseMouseUp(object sender, MouseButtonEventArgs e)
        {
            MainCanvas.MouseRightButtonUp -= DrawEllipseMouseUp;
            var position = e.GetPosition(MainCanvas);
            var drawEllipse = new DrawEllipse();
            drawEllipse.ShowDialog();
            if (drawEllipse.Stroke == null)
                return;
            var grid = new Grid { Background = Brushes.Transparent };
            var ellipse = new Ellipse
            {
                Stroke = drawEllipse.Stroke,
                Fill = drawEllipse.Fill,
                StrokeThickness = drawEllipse.StrokeThickness,
                Width = drawEllipse.RadiusX,
                Height = drawEllipse.RadiusY,
            };
            var text = new TextBlock
            {
                Text = drawEllipse.Text,
                Foreground = drawEllipse.TextColor,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            ellipse.MouseRightButtonUp += (s, ee) =>
            {
                var de = new DrawEllipse(ellipse, text);
                de.ShowDialog();
            };
            text.MouseRightButtonUp += (s, ee) =>
            {
                var de = new DrawEllipse(ellipse, text);
                de.ShowDialog();
            };

            grid.Children.Add(ellipse);
            grid.Children.Add(text);
            Canvas.SetLeft(grid, position.X);
            Canvas.SetTop(grid, position.Y);
            MainCanvas.Children.Add(grid);
            UndoHistory.Add(new Command 
            { 
                Undo = () => { MainCanvas.Children.Remove(grid); },
                Redo = () => { MainCanvas.Children.Add(grid); }
            });
            UpdateEnables();
        }
        private void DrawPolygonMenu_Click(object sender, RoutedEventArgs e)
        {
            MainCanvas.MouseRightButtonUp -= AddTextMouseUp;
            MainCanvas.MouseRightButtonUp -= DrawEllipseMouseUp;
            currentPolygonLines.Clear();
            MainCanvas.MouseRightButtonUp += AddPolygonPoints;
            MainCanvas.MouseLeftButtonUp += DrawPolygonMouseUp;
        }
        private void AddPolygonPoints(object sender, MouseButtonEventArgs e)
        {
            currentPolygonLines.Add(Mouse.GetPosition(MainCanvas));
        }
        private void DrawPolygonMouseUp(object sender, MouseButtonEventArgs e)
        {
            MainCanvas.MouseRightButtonUp -= AddPolygonPoints;
            MainCanvas.MouseLeftButtonUp -= DrawPolygonMouseUp;

            if (currentPolygonLines.Count < 3)
            {
                currentPolygonLines.Clear();
                MessageBox.Show("Mora biti najmanje 3 tacke za poligon.");
                return;
            }

            var drawPolygon = new DrawPolygon();
            drawPolygon.ShowDialog();

            var canvas = new Canvas { Background = Brushes.Transparent };
            var polygon = new Polygon
            {
                Stroke = drawPolygon.Stroke,
                Fill = drawPolygon.Fill,
                StrokeThickness = drawPolygon.StrokeThickness,
                Points = new PointCollection(currentPolygonLines)
            };
            double minX = currentPolygonLines.Min(p => p.X), maxX = currentPolygonLines.Max(p => p.X),
                minY = currentPolygonLines.Min(p => p.Y), maxY = currentPolygonLines.Max(p => p.Y);
            currentPolygonLines.Clear();
            var text = new TextBlock
            {
                Text = drawPolygon.Text,
                Foreground = drawPolygon.TextColor,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            text.MouseRightButtonUp += (s, ee) =>
            {
                var dp = new DrawPolygon(polygon, text);
                dp.ShowDialog();
            };

            polygon.MouseRightButtonUp += (s, ee) =>
            {
                var dp = new DrawPolygon(polygon, text);
                dp.ShowDialog();
            };

            Canvas.SetZIndex(polygon, 1);
            Canvas.SetZIndex(text, 2);
            Canvas.SetLeft(text, minX + (maxX - minX)/2);
            Canvas.SetTop(text, minY + (maxY - minY)/2);
            canvas.Children.Add(polygon);
            canvas.Children.Add(text);
            MainCanvas.Children.Add(canvas);
            
            UndoHistory.Add(new Command
            {
                Undo = () => { MainCanvas.Children.Remove(canvas);},
                Redo = () => { MainCanvas.Children.Add(canvas); }
            });
            UpdateEnables();
        }
        private void AddTextMenu_Click(object sender, RoutedEventArgs e)
        {
            MainCanvas.MouseRightButtonUp -= DrawPolygonMouseUp;
            MainCanvas.MouseRightButtonUp -= DrawEllipseMouseUp;
            MainCanvas.MouseLeftButtonUp -= AddPolygonPoints;
            MainCanvas.MouseRightButtonUp += AddTextMouseUp;
        }
        private void AddTextMouseUp(object sender, MouseButtonEventArgs e)
        {
            MainCanvas.MouseRightButtonUp -= AddTextMouseUp;

            var addText = new AddText();
            var position = e.GetPosition(MainCanvas);
            addText.ShowDialog();

            var text = new TextBlock
            {
                Text = addText.Text,
                Foreground = addText.TextColor,
                FontSize = addText.TextSize
            };
            text.MouseRightButtonUp += (s, ee) => 
            {
                var at = new AddText(text);
                at.ShowDialog();
            };
            Canvas.SetLeft(text, position.X);
            Canvas.SetTop(text, position.Y);
            MainCanvas.Children.Add(text);
            UndoHistory.Add(new Command
            {
                Undo = () => { MainCanvas.Children.Remove(text); },
                Redo = () => { MainCanvas.Children.Add(text); }
            });
            UpdateEnables();
        }
        #endregion

        #region Undo,Redo,Clear
        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (isCleared)
            {
                ClearHistory.ForEach(t => t.Redo());
                UndoHistory.AddRange(ClearHistory);
                ClearHistory.Clear();

                isCleared = false;
            }
            else if (UndoHistory.Count > 0)
            {
                var command = UndoHistory.Last();
                UndoHistory.Remove(command);
                command.Undo();
                RedoHistory.Add(command);
            }
            UpdateEnables();
        }
        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (RedoHistory.Count > 0)
            {
                var command = RedoHistory.Last();
                RedoHistory.Remove(command);
                command.Redo();
                UndoHistory.Add(command);
            }
            UpdateEnables();
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearHistory.AddRange(UndoHistory);
            UndoHistory.ForEach(t => t.Undo());
            UndoHistory.Clear();

            isCleared = true;
            UpdateEnables();
        }
        private void UpdateEnables()
        {
            Undo.IsEnabled = UndoHistory.Count > 0 || ClearHistory.Count > 0;
            Redo.IsEnabled = RedoHistory.Count > 0;
            Clear.IsEnabled = UndoHistory.Count > 0;
        }
        #endregion
        private void LoadModel_Click(object sender, RoutedEventArgs e)
        {
            PM.Size = 300;
            PM.MoveX = 3;
            PM.MoveY = 3;
            DrawingHelper.Window = this;
            Importer.Load();
            Importer.MinMaxFinder();
            DrawingHelper.DrawEntities();
            DrawingHelper.DrawLines();

            Entities.Lines.Clear();
            Entities.Nodes.Clear();
            Entities.Switches.Clear();
            Entities.Substations.Clear();
        }
    }
}
