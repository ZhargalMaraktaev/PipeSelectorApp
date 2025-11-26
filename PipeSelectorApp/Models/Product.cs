using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeSelectorApp.Models
{
    public class Product
    {
        public int id { get; set; }
        public int ProductNomencl_id { get; set; }
        public Product_Nomenclature Product_Nomenclature { get; set; } = null!;
        public int Thread_id { get; set; }
        public PipeThread PipeThread { get; set; } = null!;
    }
}
