using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeSelectorApp.Models
{
    public class Productivity
    {
        public int Id { get; set; }
        public int Product_id { get; set; }
        public int Section_id { get; set; }
        public int? PipesPerHour { get; set; } // Изменено на int? для поддержки NULL

        public Product Product { get; set; }
        public Section Section { get; set; }
    }
}
