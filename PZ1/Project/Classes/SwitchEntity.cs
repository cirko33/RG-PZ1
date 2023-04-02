using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Classes
{
    public class SwitchEntity : PowerEntity
    {
        private string status;

        public SwitchEntity() { }
        public string Status { get => status; set => status = value; }
    }
}
