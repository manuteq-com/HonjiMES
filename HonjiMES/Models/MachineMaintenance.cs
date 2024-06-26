﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#27231;&#21488;&#20445;&#39178;&#33287;&#32173;&#35703;
    /// </summary>
    [Table("machine_maintenance")]
    public partial class MachineMaintenance
    {
        /// <summary>
        /// 唯一碼
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#27231;&#21488;ID
        /// </summary>
        [Column("machine_id", TypeName = "int(11)")]
        public int MachineId { get; set; }
        /// <summary>
        /// &#20445;&#39178;&#38917;&#30446;
        /// </summary>
        [Required]
        [Column("item", TypeName = "varchar(50)")]
        public string Item { get; set; }
        /// <summary>
        /// &#36913;&#26399;(&#26376;)
        /// </summary>
        [Column("cycle_time", TypeName = "int(11)")]
        public int CycleTime { get; set; }
        /// <summary>
        /// &#36817;&#26399;&#20445;&#39178;&#26178;&#38291;
        /// </summary>
        [Column("recent_time", TypeName = "timestamp")]
        public DateTime RecentTime { get; set; }
        /// <summary>
        /// &#19979;&#27425;&#20445;&#39178;&#26178;&#38291;
        /// </summary>
        [Column("next_time", TypeName = "timestamp")]
        public DateTime NextTime { get; set; }
        /// <summary>
        /// &#20633;&#35387;
        /// </summary>
        [Column("remarks", TypeName = "varchar(50)")]
        public string Remarks { get; set; }
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

        [ForeignKey(nameof(MachineId))]
        [InverseProperty(nameof(MachineInformation.MachineMaintenances))]
        public virtual MachineInformation Machine { get; set; }
    }
}
