﻿using Project.Classes;
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
using System.Windows.Documents;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Project.Draw
{
    public static class DrawingHelper
    {
        private static Dictionary<long, (int, int)> positionIds  = new Dictionary<long, (int, int)>();
        private static List<LineEntity> linesForSecond = new List<LineEntity>();
        private static (Ellipse, Ellipse) entityClicked = (null, null);
        private static (Brush, Brush) entityClickedBrush = (null, null);
        public static MainWindow Window { get; set; }

        private static List<(long, long)> linesDrawn  = new List<(long, long)>();
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
                    ToolTip = $"ID: {entity.Id}\nName: {entity.Name}\nType: ",
                    Tag = entity.Id.ToString()
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
                Canvas.SetTop(ellipse, x * PM.MoveX);
                Canvas.SetLeft(ellipse, y * PM.MoveY);
                Window.MainCanvas.Children.Add(ellipse);
            });
        }
        public static void DrawEntities()
        {
            foreach (var node in Entities.PowerEntities)
                DrawEntity(node);
        }
        private static (int, int) MapXY(double x, double y)
        {
            return ((int)Math.Floor((PM.MaxX - x) * PM.OffsetX), (int)Math.Floor((y - PM.MinY) * PM.OffsetY));
        }
        private static void AddToStoryboard(Storyboard storyboard, Ellipse ellipse)
        {
            var time = new Duration(TimeSpan.FromSeconds(1));
            var scaleX = new DoubleAnimation { To = 2.0, Duration = time };
            ellipse.RenderTransform = new ScaleTransform { ScaleX = 1 };
            Storyboard.SetTarget(scaleX, ellipse);
            Storyboard.SetTargetProperty(scaleX, new PropertyPath("(Ellipse.RenderTransform).(ScaleTransform.ScaleX)"));
            storyboard.Children.Add(scaleX);

            var scaleY = new DoubleAnimation { To = 2.0, Duration = time };
            ellipse.RenderTransform = new ScaleTransform { ScaleY = 1 };
            Storyboard.SetTarget(scaleY, ellipse);
            Storyboard.SetTargetProperty(scaleY, new PropertyPath("(Ellipse.RenderTransform).(ScaleTransform.ScaleY)"));
            storyboard.Children.Add(scaleY);

            var color = new ColorAnimation { To = Colors.DarkOrange, Duration = time };
            Storyboard.SetTarget(color, ellipse);
            Storyboard.SetTargetProperty(color, new PropertyPath("(Shape.Fill).(SolidColorBrush.Color)"));
            storyboard.Children.Add(color);
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
                path.MouseRightButtonUp += (s, e) =>
                {
                    Ellipse first = null, second = null;
                    foreach (var item in Window.MainCanvas.Children)
                    {
                        if(item is Ellipse)
                        {
                            var el = (Ellipse)item;
                            if (el.Tag.ToString() == line.FirstEnd.ToString())
                                first = el;
                            else if (el.Tag.ToString() == line.SecondEnd.ToString())
                                second = el;
                        }
                        if (first != null && second != null)
                            break;
                    }
                    if(first != null && second != null)
                    {
                        if(entityClicked.Item1 != null)
                        {
                            entityClicked.Item1.Fill = entityClickedBrush.Item1;
                            entityClicked.Item2.Fill = entityClickedBrush.Item2;

                            entityClicked.Item1.RenderTransform = new ScaleTransform { ScaleY = 1, ScaleX = 1 };
                            entityClicked.Item2.RenderTransform = new ScaleTransform { ScaleY = 1, ScaleX = 1 };
                        }

                        entityClicked = (first, second);
                        entityClickedBrush = (first.Fill.CloneCurrentValue(), second.Fill.CloneCurrentValue());
                        
                        var storyboard = new Storyboard();
                        AddToStoryboard(storyboard, first);
                        AddToStoryboard(storyboard, second);
                        storyboard.Begin();
                    }
                };
                var geometry = new PathGeometry();
                var figure = new PathFigure
                {
                    StartPoint = new Win.Point(points.First().Item2 * PM.MoveY + PM.MoveY / 2,
                        points.First().Item1 * PM.MoveX + PM.MoveX / 2)
                };

                for (int i = 1; i < points.Count; i++)
                    figure.Segments.Add(new LineSegment(new Win.Point(points[i].Item2 * PM.MoveY + PM.MoveY / 2, 
                        points[i].Item1 * PM.MoveX + PM.MoveX / 2), true));

                geometry.Figures.Add(figure);
                path.Data = geometry;
                Panel.SetZIndex(path, -1);
                Window.MainCanvas.Children.Add(path);
            });
        }
        private static void CalculateAndDrawLine1(LineEntity line)
        {
            if (linesDrawn.Any(t => (t.Item1 == line.FirstEnd && t.Item2 == line.SecondEnd) 
                || (t.Item2 == line.FirstEnd && t.Item1 == line.SecondEnd)))
                return;
            linesDrawn.Add((line.FirstEnd, line.SecondEnd));

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

            linesDrawn.Add((line.FirstEnd, line.SecondEnd));

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
                Canvas.SetTop(ellipse, mark.Item1 * PM.MoveX + .5);
                Canvas.SetLeft(ellipse, mark.Item2 * PM.MoveY + .5);
                Window.MainCanvas.Children.Add(ellipse);
            }
        }
        public static void DrawLines()
        {
            var lines = Entities.Lines.FindAll(t => positionIds.ContainsKey(t.FirstEnd) && positionIds.ContainsKey(t.SecondEnd))
                .OrderBy(t => {
                    var start = positionIds[t.FirstEnd];
                    var end = positionIds[t.SecondEnd];
                
                    //sqrt((x2-x1)^2 + (y2-y1)^2)
                    return Math.Sqrt(Math.Pow(start.Item1 - end.Item1, 2) +  Math.Pow(start.Item2 - end.Item2, 2));
                });

            foreach (var line in lines)
            {
                CalculateAndDrawLine1(line);
            }

            foreach (var line in linesForSecond)
            {
                CalculateAndDrawLine2(line);
            }

            linesForSecond.Clear();
            positionIds.Clear();
            linesDrawn.Clear();
        }
    }
}
