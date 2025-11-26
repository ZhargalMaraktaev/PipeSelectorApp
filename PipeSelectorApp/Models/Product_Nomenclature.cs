using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeSelectorApp.Models
{
    public class Product_Nomenclature
    {
        public int id { get; set; }
        public int PipeNom_id { get; set; }
        public int StrClass_id { get; set; }

        public PipeNomenclature PipeNomenclature { get; set; }
        public PipeStrengthClass StrengthClass { get; set; }
    }
}
