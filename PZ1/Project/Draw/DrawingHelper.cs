using Project.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Win = System.Windows;
using System.Xml.Serialization;
using System.Windows;
using System.Windows.Media.Animation;

namespace Project.Draw
{
    public static class DrawingHelper
    {
        private static Dictionary<long, (int, int)> positionIds  = new Dictionary<long, (int, int)>();
        private static List<LineEntity> linesForSecond = new List<LineEntity>();
        public static MainWindow Window { get; set; }

        private static (int, int) ChangePosition(int x, int y)
        {
            
            for (int it = 1; ; it++)
            {
                for (int i = x - it; i <= x + it; i++)
                {
                    if (i < 0 || i >= PM.Size) 
                        continue;
                    for (int j = y - it; j <= y + it; j++)
                    {
                        if(j < 0 || j >= PM.Size) 
                            continue;
                        if (!positionIds.Any(t => t.Value == (i, j)))
                            return (i, j);
                    }
                }
            }
        }
        private static bool IsOffScope((int,int) tuple)
        {
            if (tuple.Item1 < 0 || tuple.Item2 < 0 || tuple.Item1 >= PM.Size || tuple.Item2 >= PM.Size)
                return true;
            return false;
        }
        private static void DrawEntity(PowerEntity entity)
        {
            Window.Dispatcher.Invoke(() =>
            {
                Ellipse ellipse = new Ellipse()
                {
                    Width = 2,
                    Height = 2,
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.2,
                    ToolTip = $"ID: {entity.Id}\nName: {entity.Name}\nType: "
                };
                if (entity.GetType() == typeof(NodeEntity))
                {
                    ellipse.Fill = Brushes.Red;
                    ellipse.ToolTip += "Node";
                }
                else if (entity.GetType() == typeof(SwitchEntity))
                {
                    ellipse.Fill = Brushes.Green;
                    ellipse.ToolTip += "Switch";
                }
                else if (entity.GetType() == typeof(SubstationEntity))
                {
                    ellipse.Fill = Brushes.Blue;
                    ellipse.ToolTip += "Substation";
                }

                int x, y;
                (x, y) = MapXY(entity.X, entity.Y);
                if (positionIds.Any(t => t.Value == (x, y)) || IsOffScope((x,y)))
                    (x, y) = ChangePosition(x, y);

                positionIds[entity.Id] = (x, y);
                Canvas.SetLeft(ellipse, x * PM.Move);
                Canvas.SetTop(ellipse, y * PM.Move);
                Window.MainCanvas.Children.Add(ellipse);
            });
        }
        public static void DrawEntities()
        {
            foreach (var node in Entities.Nodes)
                DrawEntity(node);

            foreach (var node in Entities.Switches)
                DrawEntity(node);

            foreach (var node in Entities.Substations)
                DrawEntity(node);
        }
        private static (int, int) MapXY(double x, double y)
        {
            return ((int)Math.Floor((x - PM.MinX) * PM.OffsetX), (int)Math.Floor((y - PM.MinY) * PM.OffsetY));
        }
        private static void DrawLineCanvas(LineEntity line, List<(int,int)> points)
        {
            Window.Dispatcher.Invoke(() =>
            {
                var path = new Path
                {
                    Stroke = Brushes.Blue,
                    StrokeThickness = 0.4,
                    ToolTip = $"ID: {line.Id}\nName: {line.Name}"
                };
                var geometry = new PathGeometry();
                var figure = new PathFigure
                {
                    StartPoint = new Win.Point(points.First().Item1 * PM.Move + PM.Move / 2,
                    points.First().Item2 * PM.Move + PM.Move / 2)
                };

                for (int i = 1; i < points.Count; i++)
                    figure.Segments.Add(new LineSegment(new Win.Point(points[i].Item1 * PM.Move + PM.Move / 2, 
                        points[i].Item2 * PM.Move + PM.Move / 2), true));

                geometry.Figures.Add(figure);
                path.Data = geometry;
                Panel.SetZIndex(path, -1);
                Window.MainCanvas.Children.Add(path);
            });
        }
        private static void CalculateAndDrawLine1(LineEntity line)
        {
            if (!positionIds.ContainsKey(line.FirstEnd) || !positionIds.ContainsKey(line.SecondEnd))
                return;

            var start = positionIds[line.FirstEnd];
            var end = positionIds[line.SecondEnd];

            List<(int, int)> linePoints = BFS.GetLinePointsFirst(start, end);
            if(linePoints == null)
            {
                linesForSecond.Add(line);
                return;
            }
            DrawLineCanvas(line, linePoints);
        }
        private static void CalculateAndDrawLine2(LineEntity line)
        {
            if (!positionIds.ContainsKey(line.FirstEnd) || !positionIds.ContainsKey(line.SecondEnd))
                return;

            var start = positionIds[line.FirstEnd];
            var end = positionIds[line.SecondEnd];

            (var marks, var linePoints) = BFS.GetLinePointsSecond(start, end);
            if (linePoints == null)
                return;

            DrawLineCanvas(line, linePoints);
            foreach(var mark in marks)
            {
                Ellipse ellipse = new Ellipse()
                {
                    Width = 1,
                    Height = 1,
                    Stroke = Brushes.Purple,
                    StrokeThickness = 0.3,
                    Fill = Brushes.Purple,
                };
                Canvas.SetLeft(ellipse, mark.Item1 * PM.Move + .5);
                Canvas.SetTop(ellipse, mark.Item2 * PM.Move + .5);
                Window.MainCanvas.Children.Add(ellipse);
            }
        }
        public static void DrawLines()
        {
            foreach (var line in Entities.Lines)
            {
                CalculateAndDrawLine1(line);
            }

            foreach (var line in linesForSecond)
            {
                CalculateAndDrawLine2(line);
            }
        }
    }
}
