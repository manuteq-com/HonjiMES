﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#20351;&#29992;&#32773;LOG
    /// </summary>
    [Table("user_logs")]
    public partial class UserLog
    {
        /// <summary>
        /// 唯一碼
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#20351;&#29992;&#32773;ID
        /// </summary>
        [Column("user_id", TypeName = "int(11)")]
        public int UserId { get; set; }
        /// <summary>
        /// &#30331;&#20837;&#26178;&#38291;
        /// </summary>
        [Column("login_time", TypeName = "timestamp")]
        public DateTime LoginTime { get; set; }
        /// <summary>
        /// &#30331;&#20986;&#26178;&#38291;
        /// </summary>
        [Column("logout_time", TypeName = "timestamp")]
        public DateTime? LogoutTime { get; set; }
        /// <summary>
        /// &#26032;&#22686;&#32773;id
        /// </summary>
        [Column("create_user", TypeName = "int(11)")]
        public int? CreateUser { get; set; }
        /// <summary>
        /// &#26032;&#22686;&#26178;&#38291;
        /// </summary>
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// &#26356;&#26032;&#32773;id
        /// </summary>
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }
        /// <summary>
        /// &#26356;&#26032;&#26178;&#38291;
        /// </summary>
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// &#21034;&#38500;&#35387;&#35352;
        /// </summary>
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }
    }
}
