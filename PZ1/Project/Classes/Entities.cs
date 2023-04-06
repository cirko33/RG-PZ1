using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Classes
{
    public class Entities
    {
        public static List<PowerEntity> PowerEntities {  get; set; } = new List<PowerEntity>();
        //public static List<SubstationEntity> Substations { get; set; } = new List<SubstationEntity>();
        //public static List<NodeEntity> Nodes { get; set; } = new List<NodeEntity>();
        //public static List<SwitchEntity> Switches { get; set; } = new List<SwitchEntity>();
        public static List<LineEntity> Lines { get; set; } = new List<LineEntity>();
    }
}
