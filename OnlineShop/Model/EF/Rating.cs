namespace Model.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Rating")]
    public partial class Rating
    {
        public long ID { get; set; }

        public long ProductId { get; set; }

        public long CustomerId { get; set; }

        [StringLength(500)]
        public string Comment { get; set; }
        public int Rate { get; set; }

        public DateTime? CommentedOn { get; set; }
    }
}
