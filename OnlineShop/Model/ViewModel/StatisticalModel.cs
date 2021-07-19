using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class StatisticalModel
    {
        public DateTime? date { get; set; }
        public decimal? revenue { get; set; } 
        public decimal? benefit { get; set; }
        public int status { get; set; }
    }
}
