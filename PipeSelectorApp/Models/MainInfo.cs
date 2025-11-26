using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeSelectorApp.Models
{
    public class MainInfo
    {
        public int id { get; set; }
        public int PipeNom_id { get; set; }
        public PipeNomenclature PipeNomenclature { get; set; } = null!;
        public int NomByStrClass_id { get; set; }
        public NomenclatureByStrengthClass NomenclatureByStrengthClass { get; set; } = null!;
        public int Prod_id { get; set; }
        public Productivity Productivity { get; set; } = null!;
        public int Type_id { get; set; }
        public Product_type ProductType { get; set; } = null!;
        public int Steel_id { get; set; }
        public SteelMark SteelMark { get; set; } = null!;
    }
}
