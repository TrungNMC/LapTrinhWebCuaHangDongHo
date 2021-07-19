using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class StarModel
    {
        public long ID { get; set; }

        public long ProductId { get; set; }
        public int Rate { get; set; }
        public float Star { get; set; }
        public float SumStar { get; set; }
        public int amount { get; set; }
        public DateTime? CommentedOn { get; set; }
    }
}
