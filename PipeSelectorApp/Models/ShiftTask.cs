using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeSelectorApp.Models
{
    public class ShiftTask
    {
        public int id { get; set; }
        public int? productivity_id { get; set; }
        public int Section_id { get; set; }
        public Productivity Productivity { get; set; }
        public Section Section { get; set; }
    }
}
