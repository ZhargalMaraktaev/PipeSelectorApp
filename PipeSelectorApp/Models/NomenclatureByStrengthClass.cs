using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeSelectorApp.Models
{
    public class NomenclatureByStrengthClass
    {
        public int id { get; set; }
        public int PipeNom_id { get; set; }
        public PipeNomenclature PipeNomenclature { get; set; } = null!;
        public int StrengthClass_id { get; set; }
        public PipeStrengthClass StrengthClass { get; set; } = null!;
    }
}
