﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    [Table("work_order_report_log")]
    public partial class WorkOrderReportLog
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#24037;&#21934;&#26126;&#32048;ID
        /// </summary>
        [Column("work_order_detail_id", TypeName = "int(11)")]
        public int WorkOrderDetailId { get; set; }
        /// <summary>
        /// &#22238;&#22577;&#31278;&#39006;
        /// </summary>
        [Column("report_type", TypeName = "int(11)")]
        public int ReportType { get; set; }
        /// <summary>
        /// &#25505;&#36092;&#21934;ID
        /// </summary>
        [Column("purchase_id", TypeName = "int(11)")]
        public int? PurchaseId { get; set; }
        /// <summary>
        /// &#25505;&#36092;&#21934;&#34399;
        /// </summary>
        [Column("purchase_no", TypeName = "varchar(50)")]
        public string PurchaseNo { get; set; }
        /// <summary>
        /// &#20379;&#25033;&#21830;id
        /// </summary>
        [Column("supplier_id", TypeName = "int(11)")]
        public int? SupplierId { get; set; }
        /// <summary>
        /// &#22294;&#34399;
        /// </summary>
        [Column("draw_no", TypeName = "varchar(50)")]
        public string DrawNo { get; set; }
        /// <summary>
        /// &#21152;&#24037;&#31243;&#24335;
        /// </summary>
        [Column("code_no", TypeName = "varchar(50)")]
        public string CodeNo { get; set; }
        /// <summary>
        /// &#38656;&#27714;&#20154;&#21147;
        /// </summary>
        [Column("manpower", TypeName = "int(11)")]
        public int? Manpower { get; set; }
        /// <summary>
        /// &#21152;&#24037;&#27231;&#21488;ID
        /// </summary>
        [Column("producing_machine_id", TypeName = "int(11)")]
        public int? ProducingMachineId { get; set; }
        /// <summary>
        /// &#21152;&#24037;&#27231;&#21488;
        /// </summary>
        [Column("producing_machine", TypeName = "varchar(50)")]
        public string ProducingMachine { get; set; }
        /// <summary>
        /// &#21487;&#35069;&#36896;&#25976;&#37327;
        /// </summary>
        [Column("m_count", TypeName = "int(11)")]
        public int? MCount { get; set; }
        /// <summary>
        /// &#22238;&#22577;&#25976;&#37327;
        /// </summary>
        [Column("re_count", TypeName = "int(11)")]
        public int? ReCount { get; set; }
        /// <summary>
        /// &#23526;&#38555;&#22238;&#22577;&#37329;&#38989;
        /// </summary>
        [Column("re_price", TypeName = "decimal(10,2)")]
        public decimal? RePrice { get; set; }
        /// <summary>
        /// NG&#25976;&#37327;
        /// </summary>
        [Column("ng_count", TypeName = "int(11)")]
        public int NgCount { get; set; }
        /// <summary>
        /// NC&#26410;&#21152;&#24037;
        /// </summary>
        [Column("nc_count", TypeName = "int(11)")]
        public int NcCount { get; set; }
        /// <summary>
        /// &#22238;&#22577;&#35498;&#26126;
        /// </summary>
        [Column("message", TypeName = "varchar(50)")]
        public string Message { get; set; }
        /// <summary>
        /// &#19978;&#19968;&#20491;&#29376;&#24907;
        /// </summary>
        [Column("status_o", TypeName = "int(11)")]
        public int StatusO { get; set; }
        /// <summary>
        /// &#26356;&#26032;&#29376;&#24907;
        /// </summary>
        [Column("status_n", TypeName = "int(11)")]
        public int StatusN { get; set; }
        /// <summary>
        /// &#38928;&#35336;&#38283;&#24037;&#26085;
        /// </summary>
        [Column("due_start_time", TypeName = "timestamp")]
        public DateTime? DueStartTime { get; set; }
        /// <summary>
        /// &#38928;&#35336;&#23436;&#24037;&#26085;
        /// </summary>
        [Column("due_end_time", TypeName = "timestamp")]
        public DateTime? DueEndTime { get; set; }
        /// <summary>
        /// &#23526;&#38555;&#38283;&#24037;&#26085;
        /// </summary>
        [Column("actual_start_time", TypeName = "timestamp")]
        public DateTime? ActualStartTime { get; set; }
        /// <summary>
        /// &#23526;&#38555;&#23436;&#24037;&#26085;
        /// </summary>
        [Column("actual_end_time", TypeName = "timestamp")]
        public DateTime? ActualEndTime { get; set; }
        /// <summary>
        /// &#27231;&#21488;&#38283;&#24037;&#26085;
        /// </summary>
        [Column("machine_start_time", TypeName = "timestamp")]
        public DateTime? MachineStartTime { get; set; }
        /// <summary>
        /// &#27231;&#21488;&#23436;&#24037;&#26085;
        /// </summary>
        [Column("machinel_end_time", TypeName = "timestamp")]
        public DateTime? MachinelEndTime { get; set; }
        [Column("delete_flag", TypeName = "int(11)")]
        public int DeleteFlag { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }

        [ForeignKey(nameof(PurchaseId))]
        [InverseProperty(nameof(PurchaseHead.WorkOrderReportLogs))]
        public virtual PurchaseHead Purchase { get; set; }
        [ForeignKey(nameof(SupplierId))]
        [InverseProperty("WorkOrderReportLogs")]
        public virtual Supplier Supplier { get; set; }
        [ForeignKey(nameof(WorkOrderDetailId))]
        [InverseProperty("WorkOrderReportLogs")]
        public virtual WorkOrderDetail WorkOrderDetail { get; set; }
    }
}
