﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#34920;&#34389;&#24037;&#21934;&#38364;&#32879;&#32000;&#37636;
    /// </summary>
    [Table("surface_workorder")]
    public partial class SurfaceWorkorder
    {
        /// <summary>
        /// 唯一碼
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#25505;&#36092;&#21934;ID
        /// </summary>
        [Column("purchase_id", TypeName = "int(11)")]
        public int PurchaseId { get; set; }
        /// <summary>
        /// &#24037;&#21934;ID
        /// </summary>
        [Column("work_order_id", TypeName = "int(11)")]
        public int WorkOrderId { get; set; }
        /// <summary>
        /// &#24037;&#21934;&#34399;
        /// </summary>
        [Required]
        [Column("work_order_no", TypeName = "varchar(50)")]
        public string WorkOrderNo { get; set; }
        /// <summary>
        /// &#24037;&#24207;ID
        /// </summary>
        [Column("process_id", TypeName = "int(11)")]
        public int ProcessId { get; set; }
        /// <summary>
        /// &#26032;&#22686;&#26178;&#38291;
        /// </summary>
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// &#26032;&#22686;&#20154;&#21729;
        /// </summary>
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        /// <summary>
        /// &#26356;&#26032;&#26178;&#38291;
        /// </summary>
        [Column("update_time", TypeName = "timestamp")]
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// &#26356;&#26032;&#20154;&#21729;
        /// </summary>
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }
        /// <summary>
        /// &#21034;&#38500;&#35387;&#35352;
        /// </summary>
        [Column("delete_flag", TypeName = "int(11)")]
        public int DeleteFlag { get; set; }
    }
}
