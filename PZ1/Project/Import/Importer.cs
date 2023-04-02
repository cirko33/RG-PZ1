using Project.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Project.Import
{
    public class Importer
    {
        public static void MinMaxFinder()
        {
            var values = new List<PowerEntity>().Concat(Entities.Switches).Concat(Entities.Nodes).Concat(Entities.Substations);
            var doublesX = values.Select(t => t.X).ToList();
            var doublesY = values.Select(t => t.Y).ToList();

            PM.MinX = doublesX.Min();
            PM.MinY = doublesY.Min();
            PM.MaxX = doublesX.Max();
            PM.MaxY = doublesY.Max();

            PM.OffsetX = PM.Size / (PM.MaxX - PM.MinX);
            PM.OffsetY = PM.Size / (PM.MaxY - PM.MinY);
        }
        public static void Load() 
        {
            XmlDocument xml = new XmlDocument();
            xml.Load("../../Import/geographic.xml");

            XmlNodeList nodeList = xml.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity");
            double tempX, tempY;
            foreach (XmlNode item in nodeList)
            {
                var substationEntity = new SubstationEntity();

                substationEntity.Id = long.Parse(item.SelectSingleNode("Id").InnerText);
                substationEntity.Name = item.SelectSingleNode("Name").InnerText;
                substationEntity.X = double.Parse(item.SelectSingleNode("X").InnerText);
                substationEntity.Y = double.Parse(item.SelectSingleNode("Y").InnerText);

                ToLatLon(substationEntity.X, substationEntity.Y, 34, out tempX, out tempY);
                substationEntity.X = tempX;
                substationEntity.Y = tempY;

                Entities.Substations.Add(substationEntity);
            }

            nodeList = xml.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");
            foreach (XmlNode item in nodeList)
            {
                var nodeEntity = new NodeEntity();

                nodeEntity.Id = long.Parse(item.SelectSingleNode("Id").InnerText);
                nodeEntity.Name = item.SelectSingleNode("Name").InnerText;
                nodeEntity.X = double.Parse(item.SelectSingleNode("X").InnerText);
                nodeEntity.Y = double.Parse(item.SelectSingleNode("Y").InnerText);

                ToLatLon(nodeEntity.X, nodeEntity.Y, 34, out tempX, out tempY);
                nodeEntity.X = tempX;
                nodeEntity.Y = tempY;

                Entities.Nodes.Add(nodeEntity);
            }

            nodeList = xml.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");
            foreach (XmlNode item in nodeList)
            {
                var switchEntity = new SwitchEntity();

                switchEntity.Id = long.Parse(item.SelectSingleNode("Id").InnerText);
                switchEntity.Name = item.SelectSingleNode("Name").InnerText;
                switchEntity.Status = item.SelectSingleNode("Status").InnerText;
                switchEntity.X = double.Parse(item.SelectSingleNode("X").InnerText);
                switchEntity.Y = double.Parse(item.SelectSingleNode("Y").InnerText);

                ToLatLon(switchEntity.X, switchEntity.Y, 34, out tempX, out tempY);
                switchEntity.X = tempX;
                switchEntity.Y = tempY;

                Entities.Switches.Add(switchEntity);
            }

            nodeList = xml.DocumentElement.SelectNodes("/NetworkModel/Lines/LineEntity");
            foreach (XmlNode item in nodeList)
            {
                var lineEntity = new LineEntity();

                lineEntity.Id = long.Parse(item.SelectSingleNode("Id").InnerText);
                lineEntity.Name = item.SelectSingleNode("Name").InnerText;
                lineEntity.IsUnderground = item.SelectSingleNode("IsUnderground").InnerText.Equals("true") ? true : false;
                lineEntity.R = float.Parse(item.SelectSingleNode("R").InnerText);
                lineEntity.ConductorMaterial = item.SelectSingleNode("ConductorMaterial").InnerText;
                lineEntity.LineType = item.SelectSingleNode("LineType").InnerText;
                lineEntity.ThermalConstantHeat = long.Parse(item.SelectSingleNode("ThermalConstantHeat").InnerText);
                lineEntity.FirstEnd = long.Parse(item.SelectSingleNode("FirstEnd").InnerText);
                lineEntity.SecondEnd = long.Parse(item.SelectSingleNode("SecondEnd").InnerText);

                Entities.Lines.Add(lineEntity);
            }
        }

        static void ToLatLon(double utmX, double utmY, int zoneUTM, out double latitude, out double longitude)
        {
            bool isNorthHemisphere = true;

            var diflat = -0.00066286966871111111111111111111111111;
            var diflon = -0.0003868060578;

            var zone = zoneUTM;
            var c_sa = 6378137.000000;
            var c_sb = 6356752.314245;
            var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
            var e2cuadrada = Math.Pow(e2, 2);
            var c = Math.Pow(c_sa, 2) / c_sb;
            var x = utmX - 500000;
            var y = isNorthHemisphere ? utmY : utmY - 10000000;

            var s = ((zone * 6.0) - 183.0);
            var lat = y / (c_sa * 0.9996);
            var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
            var a = x / v;
            var a1 = Math.Sin(2 * lat);
            var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
            var j2 = lat + (a1 / 2.0);
            var j4 = ((3 * j2) + a2) / 4.0;
            var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
            var alfa = (3.0 / 4.0) * e2cuadrada;
            var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
            var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
            var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
            var b = (y - bm) / v;
            var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
            var eps = a * (1 - (epsi / 3.0));
            var nab = (b * (1 - epsi)) + lat;
            var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
            var delt = Math.Atan(senoheps / (Math.Cos(nab)));
            var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

            longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
            latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;
        }
    }
}
