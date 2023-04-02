using Project.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Instrumentation;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Project.Draw
{
    public static class BFS
    {
        static bool[,] map = new bool[PM.Size, PM.Size];

        private static List<(int,int)> FilterAndFillMap(List<(int,int)> path)
        {
            var ret = new List<(int,int)>();
            ret.Add(path.First());
            for(int i = 1; i < path.Count - 1; i++)
            {
                map[path[i].Item1, path[i].Item2] = true;
                if (Cases(path[i - 1], path[i], path[i + 1]))
                    ret.Add(path[i]);
            }
            ret.Add(path.Last());

            return ret;
        }

        private static bool Cases((int,int) prev, (int,int) curr, (int,int) next)
        {
            if(prev.Item1 != curr.Item1 && curr.Item2 != next.Item2)
                return true;

            if (prev.Item2 != curr.Item2 && curr.Item1 != next.Item1)
                return true;

            return false;
        }

        public static List<(int, int)> GetLinePointsFirst((int, int) start, (int, int) end)
        {
            bool[,] visited = new bool[PM.Size, PM.Size];
            Queue<List<(int, int)>> queue = new Queue<List<(int, int)>>();
            queue.Enqueue(new List<(int, int)>() { start });

            while (queue.Count > 0)
            {
                var path = queue.Dequeue();
                var last = path.Last();

                if (visited[last.Item1, last.Item2])
                    continue;

                visited[last.Item1, last.Item2] = true;

                foreach (var neighbor in GetNeighbors(last))
                {
                    if (neighbor == end)
                    {
                        path.Add(neighbor);
                        return FilterAndFillMap(path);
                    }

                    if (!visited[neighbor.Item1, neighbor.Item2] && !map[neighbor.Item1, neighbor.Item2])
                    {
                        List<(int, int)> newPath = new List<(int, int)>(path);
                        newPath.Add(neighbor);
                        queue.Enqueue(newPath);
                    }
                }
            }
            return null;
        }

        public static (List<(int, int)>, List<(int,int)>) GetLinePointsSecond((int, int) start, (int, int) end)
        {
            bool[,] visited = new bool[PM.Size, PM.Size];
            Queue<List<(int, int)>> queue = new Queue<List<(int, int)>>();
            queue.Enqueue(new List<(int, int)>() { start });

            while (queue.Count > 0)
            {
                var path = queue.Dequeue();
                var last = path.Last();

                if (visited[last.Item1, last.Item2])
                    continue;

                visited[last.Item1, last.Item2] = true;

                foreach (var neighbor in GetNeighbors(last))
                {
                    if (neighbor == end)
                    {
                        path.Add(neighbor);
                        var marks = AddMarks(path);
                        return (marks, FilterAndFillMap(path));
                    }

                    if (!visited[neighbor.Item1, neighbor.Item2] /*&& !map[neighbor.Item1, neighbor.Item2]*/)
                    {
                        List<(int, int)> newPath = new List<(int, int)>(path);
                        newPath.Add(neighbor);
                        queue.Enqueue(newPath);
                    }
                }

                //if(queue.Count == 0)
                //{
                //    foreach (var neighbor in GetNeighbors(last))
                //    {
                //        if (neighbor == end)
                //        {
                //            path.Add(neighbor);
                //            var marks = AddMarks(path);
                //            return (marks, FilterAndFillMap(path));
                //        }

                //        if (!visited[neighbor.Item1, neighbor.Item2])
                //        {
                //            List<(int, int)> newPath = new List<(int, int)>(path);
                //            newPath.Add(neighbor);
                //            queue.Enqueue(newPath);
                //        }
                //    }
                //}
            }
            return (null, null);
        }

        private static List<(int, int)> AddMarks(List<(int, int)> path)
        {
            var ret = new List<(int, int)>();
            for (int i = 1; i < path.Count - 1; i++)
            {
                if (map[path[i].Item1, path[i].Item2])
                    ret.Add(path[i]);
            }
            return ret;
        }

        private static List<(int, int)> GetNeighbors((int, int) pos)
        {
            List<(int, int)> neighbors = new List<(int, int)>();

            if (pos.Item1 > 0)
                neighbors.Add((pos.Item1 - 1, pos.Item2));

            if (pos.Item2 > 0)
                neighbors.Add((pos.Item1, pos.Item2 - 1));

            if (pos.Item1 < PM.Size - 1)
                neighbors.Add((pos.Item1 + 1, pos.Item2));

            if (pos.Item2 < PM.Size - 1)
                neighbors.Add((pos.Item1, pos.Item2 + 1));

            return neighbors;
        }
    }
}
