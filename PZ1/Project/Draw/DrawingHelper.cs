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

namespace Project.Draw
{
    public class DrawingHelper
    {
        public static Dictionary<(int, int), long> PositionsWithIds { get; set; } = new Dictionary<(int, int), long>();
        public static Dictionary<(int, int), bool> Map { get; set; } = new Dictionary<(int, int), bool>();
        private static (int, int) ChangePosition(int x, int y)
        {
            
            for (int it = 1; ; it++)
            {
                for (int i = x - it; i <= x + it; i++)
                    for (int j = y - it; j <= y + it; j++)
                        if (!PositionsWithIds.ContainsKey((i, j)))
                            return (i, j);
            }
        }
        private static void DrawEntity(PowerEntity entity, MainWindow window)
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
            else if(entity.GetType() == typeof(SubstationEntity))
            {
                ellipse.Fill = Brushes.Blue;
                ellipse.ToolTip += "Substation";
            }
            
            int x, y;
            (x, y) = MapXY(entity.X, entity.Y);
            if (PositionsWithIds.ContainsKey((x, y)))
                (x, y) = ChangePosition(x, y);

            PositionsWithIds[(x, y)] = entity.Id;
            Canvas.SetLeft(ellipse, x * PM.Move);
            Canvas.SetTop(ellipse, y * PM.Move);
            window.MainCanvas.Children.Add(ellipse);
        }
        
        public static void DrawEntities(MainWindow window)
        {
            foreach (var node in Entities.Nodes)
                DrawEntity(node, window);

            foreach (var node in Entities.Switches)
                DrawEntity(node, window);

            foreach (var node in Entities.Substations)
                DrawEntity(node, window);
        }

        static (int, int) MapXY(double x, double y)
        {
            return ((int)Math.Round((x - PM.MinX) * PM.OffsetX), (int)Math.Round((y - PM.MinY) * PM.OffsetY));
        }
        static void DrawLine(LineEntity line, MainWindow window)
        {
            (int startX, int startY) = PositionsWithIds.Single(t => t.Value == line.FirstEnd).Key;
            (int endX, int endY) = PositionsWithIds.Single(t => t.Value == line.SecondEnd).Key;

            List<(int, int)> LinePoints = GetLinePoints(startX, endX, startY, endY);    
           
        }

        private static List<(int, int)> GetLinePoints(int startX, int endX, int startY, int endY)
        {
            Dictionary<(int, int), bool> visited = new Dictionary<(int, int), bool>();

        }

        public static void DrawLines(MainWindow window)
        {
            foreach (var key in PositionsWithIds.Keys)
                Map[key] = true;

            foreach (var line in Entities.Lines)
            {
                DrawLine(line, window);
            }
        }
    }
}
