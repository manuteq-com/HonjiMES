﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#20351;&#29992;&#32773;
    /// </summary>
    [Table("users")]
    public partial class User
    {
        public User()
        {
            StaffManagements = new HashSet<StaffManagement>();
            UserRoles = new HashSet<UserRole>();
        }

        /// <summary>
        /// 唯一碼
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#24115;&#34399;
        /// </summary>
        [Required]
        [Column("username", TypeName = "varchar(50)")]
        public string Username { get; set; }
        /// <summary>
        /// &#22995;&#21517;
        /// </summary>
        [Required]
        [Column("realname", TypeName = "varchar(50)")]
        public string Realname { get; set; }
        /// <summary>
        /// &#23494;&#30908;
        /// </summary>
        [Required]
        [Column("password", TypeName = "tinytext")]
        public string Password { get; set; }
        /// <summary>
        /// &#36523;&#20998;&#21029;
        /// </summary>
        [Column("permission", TypeName = "int(11)")]
        public int Permission { get; set; }
        /// <summary>
        /// &#37096;&#38272;
        /// </summary>
        [Column("department", TypeName = "int(11)")]
        public int Department { get; set; }
        /// <summary>
        /// &#20633;&#35387;
        /// </summary>
        [Column("remarks", TypeName = "varchar(50)")]
        public string Remarks { get; set; }
        /// <summary>
        /// &#26032;&#22686;&#26178;&#38291;
        /// </summary>
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

        [InverseProperty(nameof(StaffManagement.User))]
        public virtual ICollection<StaffManagement> StaffManagements { get; set; }
        [InverseProperty(nameof(UserRole.Users))]
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
