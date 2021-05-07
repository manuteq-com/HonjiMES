using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    [Table("menu")]
    public partial class Menu
    {
        public Menu()
        {
            UserRoles = new HashSet<UserRole>();
        }

        /// <summary>
        /// 唯一碼
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#29238;ID
        /// </summary>
        [Column("pid", TypeName = "int(11)")]
        public int? Pid { get; set; }
        /// <summary>
        /// &#21517;&#31281;
        /// </summary>
        [Required]
        [Column("name", TypeName = "varchar(50)")]
        public string Name { get; set; }
        [Column("icon", TypeName = "varchar(50)")]
        public string Icon { get; set; }
        [Column("routerLink", TypeName = "varchar(200)")]
        public string RouterLink { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }
        /// <summary>
        /// &#21034;&#38500;&#35387;&#35352;
        /// </summary>
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }
        /// <summary>
        /// &#25490;&#24207;
        /// </summary>
        [Column("order", TypeName = "int(11)")]
        public int? Order { get; set; }
        /// <summary>
        /// &#35498;&#26126;
        /// </summary>
        [Column("memo", TypeName = "text")]
        public string Memo { get; set; }

        [InverseProperty(nameof(UserRole.Menu))]
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
