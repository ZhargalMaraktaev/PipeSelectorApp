using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeSelectorApp.Models
{
    public class PipesPerHourMap
    {
        public int Id { get; set; }
        public int PipeNom_id { get; set; }
        public string StrengthClasses { get; set; }
        public string ThreadTypes { get; set; }
        public int PipesPerHour { get; set; }
        public int Section_id { get; set; } // Новый столбец
        public PipeNomenclature PipeNomenclature { get; set; }
        public Section Section { get; set; } // Навигационное свойство
    }
}
