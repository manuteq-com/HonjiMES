﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.CncModels
{
    [Table("process")]
    public partial class Process
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#20195;&#34399;
        /// </summary>
        [Required]
        [Column("process_alias", TypeName = "varchar(30)")]
        public string ProcessAlias { get; set; }
        /// <summary>
        /// &#21517;&#31281;
        /// </summary>
        [Required]
        [Column("process_name", TypeName = "varchar(30)")]
        public string ProcessName { get; set; }
        /// <summary>
        /// &#25944;&#36848;
        /// </summary>
        [Column("description", TypeName = "text")]
        public string Description { get; set; }
        /// <summary>
        /// &#27161;&#28310;&#24037;&#26178;
        /// </summary>
        [Required]
        [Column("standard_time", TypeName = "varchar(10)")]
        public string StandardTime { get; set; }
        /// <summary>
        /// &#24615;&#36074; 1:&#24288;&#20839; 2:&#22996;&#22806;
        /// </summary>
        [Column("property", TypeName = "int(11)")]
        public int Property { get; set; }
        /// <summary>
        /// &#37096;&#38272;
        /// </summary>
        [Column("department", TypeName = "int(11)")]
        public int? Department { get; set; }
        /// <summary>
        /// &#21934;&#20729;
        /// </summary>
        [Column("price")]
        public float Price { get; set; }
        /// <summary>
        /// &#21855;&#29992;&#29376;&#24907;0:&#20572;&#29992;1:&#21855;&#29992;
        /// </summary>
        [Column("status", TypeName = "int(1)")]
        public int Status { get; set; }
        /// <summary>
        /// &#35731;&#20351;&#29992;&#32773;&#19981;&#20877;&#30475;&#21040;
        /// </summary>
        [Required]
        [Column("visible")]
        public bool? Visible { get; set; }
        /// <summary>
        /// &#26356;&#26032;&#26178;&#38291;
        /// </summary>
        [Column("last_update_time", TypeName = "timestamp")]
        public DateTime LastUpdateTime { get; set; }
        /// <summary>
        /// &#24314;&#31435;&#26178;&#38291;
        /// </summary>
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// &#24314;&#31435;&#20154;&#21729;
        /// </summary>
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
    }
}