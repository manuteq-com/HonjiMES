﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    [Table("mbom_model_detail")]
    public partial class MbomModelDetail
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("mbom_model_head_id", TypeName = "int(11)")]
        public int MbomModelHeadId { get; set; }
        /// <summary>
        /// &#24037;&#24207;&#38918;&#24207;
        /// </summary>
        [Column("serial_number", TypeName = "int(11)")]
        public int SerialNumber { get; set; }
        [Column("process_id", TypeName = "int(11)")]
        public int ProcessId { get; set; }
        /// <summary>
        /// &#24037;&#24207;&#20195;&#34399;
        /// </summary>
        [Required]
        [Column("process_no", TypeName = "varchar(50)")]
        public string ProcessNo { get; set; }
        /// <summary>
        /// &#24037;&#24207;&#21517;&#31281;
        /// </summary>
        [Required]
        [Column("process_name", TypeName = "varchar(100)")]
        public string ProcessName { get; set; }
        /// <summary>
        /// &#21069;&#32622;&#26178;&#38291;
        /// </summary>
        [Column("process_lead_time", TypeName = "decimal(10,2)")]
        public decimal ProcessLeadTime { get; set; }
        /// <summary>
        /// &#27161;&#28310;&#24037;&#26178;
        /// </summary>
        [Column("process_time", TypeName = "decimal(10,2)")]
        public decimal ProcessTime { get; set; }
        /// <summary>
        /// &#25104;&#26412;
        /// </summary>
        [Column("process_cost", TypeName = "decimal(10,2)")]
        public decimal ProcessCost { get; set; }
        /// <summary>
        /// &#22294;&#34399;
        /// </summary>
        [Column("draw_no", TypeName = "varchar(50)")]
        public string DrawNo { get; set; }
        /// <summary>
        /// &#25152;&#38656;&#20154;&#21147;
        /// </summary>
        [Column("manpower", TypeName = "int(11)")]
        public int? Manpower { get; set; }
        /// <summary>
        /// &#27231;&#21488;
        /// </summary>
        [Column("producing_machine", TypeName = "varchar(50)")]
        public string ProducingMachine { get; set; }
        /// <summary>
        /// &#29376;&#24907;
        /// </summary>
        [Column("status", TypeName = "int(11)")]
        public int Status { get; set; }
        /// <summary>
        /// &#31278;&#39006;
        /// </summary>
        [Required]
        [Column("type", TypeName = "varchar(50)")]
        public string Type { get; set; }
        /// <summary>
        /// &#20633;&#35387;
        /// </summary>
        [Column("remarks", TypeName = "varchar(100)")]
        public string Remarks { get; set; }
        [Column("delete_flag", TypeName = "int(11)")]
        public int DeleteFlag { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }

        [ForeignKey(nameof(MbomModelHeadId))]
        [InverseProperty("MbomModelDetails")]
        public virtual MbomModelHead MbomModelHead { get; set; }
    }
}
