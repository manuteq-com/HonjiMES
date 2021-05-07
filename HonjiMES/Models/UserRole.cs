using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    [Table("user_roles")]
    public partial class UserRole
    {
        /// <summary>
        /// 唯一碼
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#27402;&#38480;
        /// </summary>
        [Column("roles", TypeName = "varchar(10)")]
        public string Roles { get; set; }
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
        /// &#30446;&#37636;ID
        /// </summary>
        [Column("menu_id", TypeName = "int(11)")]
        public int MenuId { get; set; }
        /// <summary>
        /// &#20351;&#29992;&#37117;ID
        /// </summary>
        [Column("users_id", TypeName = "int(11)")]
        public int UsersId { get; set; }
        /// <summary>
        /// &#35498;&#26126;
        /// </summary>
        [Column("memo", TypeName = "text")]
        public string Memo { get; set; }

        [ForeignKey(nameof(MenuId))]
        [InverseProperty("UserRoles")]
        public virtual Menu Menu { get; set; }
        [ForeignKey(nameof(UsersId))]
        [InverseProperty(nameof(User.UserRoles))]
        public virtual User Users { get; set; }
    }
}
