using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Classes
{
    public class Command
    {
        public Action Undo { get; set; }
        public Action Redo { get; set; } 
    }
}
