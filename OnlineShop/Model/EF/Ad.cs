namespace Model.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Ad")]
    public partial class Ad
    {
        public int ID { get; set; }
        [StringLength(50)]
        [Display(Name = "Tài khoản")]
        [Required(ErrorMessage ="Bạn phải nhập tên đăng nhập")]
        public string UserName { get; set; }
        [StringLength(50)]
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage ="Bạn phải nhập mật khẩu")]
        public string Password { get; set; }
        [StringLength(250)]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Bạn phải nhập email")]
        public string Email { get; set; }
        [StringLength(50)]
        [Display(Name = "Tên")]
        [Required(ErrorMessage = "Bạn phải nhập tên")]
        public string Name { get; set; }
        [StringLength(50)]
        [Display(Name = "Quyền")]
        public string GroupID { get; set; }
        [Display(Name = "Trạng thái")]
        public bool Status { get; set; }
    }
}
