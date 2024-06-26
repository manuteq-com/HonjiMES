﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#27231;&#21488;&#20445;&#39178;&#32000;&#37636;
    /// </summary>
    [Table("maintenance_log")]
    public partial class MaintenanceLog
    {
        /// <summary>
        /// 唯一碼
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#25805;&#20316;&#20154;&#21729;
        /// </summary>
        [Column("user_id", TypeName = "int(11)")]
        public int UserId { get; set; }
        /// <summary>
        /// &#27231;&#21488;ID
        /// </summary>
        [Column("machine_id", TypeName = "int(11)")]
        public int MachineId { get; set; }
        /// <summary>
        /// &#27231;&#21488;&#21517;&#31281;
        /// </summary>
        [Required]
        [Column("machine_name", TypeName = "varchar(50)")]
        public string MachineName { get; set; }
        /// <summary>
        /// &#20445;&#39178;&#38917;&#30446;
        /// </summary>
        [Required]
        [Column("item", TypeName = "varchar(50)")]
        public string Item { get; set; }
        /// <summary>
        /// &#32173;&#35703;&#26178;&#38291;
        /// </summary>
        [Column("recent_time", TypeName = "timestamp")]
        public DateTime RecentTime { get; set; }
        /// <summary>
        /// &#38283;&#22987;&#26178;&#38291;
        /// </summary>
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        /// <summary>
        /// &#26356;&#26032;&#26178;&#38291;
        /// </summary>
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }
        /// <summary>
        /// &#21034;&#38500;&#35387;&#35352;
        /// </summary>
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }
    }
}
