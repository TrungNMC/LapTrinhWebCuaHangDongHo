using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class RatingModel
    {
        public long ID { get; set; }

        public long ProductId { get; set; }

        public long CustomerId { get; set; }
        public string CustomerName { get; set; }

        public string Comment { get; set; }

        public int Rate { get; set; }

        public DateTime? CommentedOn { get; set; }
    }
}
