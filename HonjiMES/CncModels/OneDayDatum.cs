﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.CncModels
{
    /// <summary>
    /// &#21152;&#24037;&#31243;&#24335;&#32000;&#37636;
    /// </summary>
    [Table("one_day_data")]
    public partial class OneDayDatum
    {
        /// <summary>
        /// 唯一碼
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(5)")]
        public int Id { get; set; }
        /// <summary>
        /// &#27231;&#21488;ID
        /// </summary>
        [Column("machine_id", TypeName = "int(5)")]
        public int MachineId { get; set; }
        /// <summary>
        /// &#26085;&#26399;
        /// </summary>
        [Column("date", TypeName = "timestamp")]
        public DateTime Date { get; set; }
        /// <summary>
        /// &#36939;&#36681;&#26178;&#38291;
        /// </summary>
        [Column("run_time", TypeName = "int(100)")]
        public int? RunTime { get; set; }
        /// <summary>
        /// &#24453;&#27231;&#26178;&#38291;
        /// </summary>
        [Column("idle_time", TypeName = "int(100)")]
        public int? IdleTime { get; set; }
        /// <summary>
        /// &#38364;&#27231;&#26178;&#38291;
        /// </summary>
        [Column("offline_time", TypeName = "int(100)")]
        public int? OfflineTime { get; set; }
        /// <summary>
        /// &#35686;&#22577;&#26178;&#38291;
        /// </summary>
        [Column("alarm_time", TypeName = "int(100)")]
        public int? AlarmTime { get; set; }
        /// <summary>
        /// &#21152;&#24037;&#37096;&#21697;&#25976;
        /// </summary>
        [Column("parts_count", TypeName = "int(5)")]
        public int? PartsCount { get; set; }
        /// <summary>
        /// &#35686;&#22577;&#27425;&#25976;
        /// </summary>
        [Column("alarm_count", TypeName = "int(5)")]
        public int? AlarmCount { get; set; }
        /// <summary>
        /// &#26032;&#22686;&#26178;&#38291;
        /// </summary>
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }

        [ForeignKey(nameof(MachineId))]
        [InverseProperty(nameof(MachineInformation.OneDayData))]
        public virtual MachineInformation Machine { get; set; }
    }
}