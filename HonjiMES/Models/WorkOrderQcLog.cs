using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    [Table("work_order_qc_log")]
    public partial class WorkOrderQcLog
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#24037;&#21934;&#20027;&#27284;Id
        /// </summary>
        [Column("work_order_head_id", TypeName = "int(11)")]
        public int? WorkOrderHeadId { get; set; }
        /// <summary>
        /// &#24037;&#21934;&#26126;&#32048;ID
        /// </summary>
        [Column("work_order_detail_id", TypeName = "int(11)")]
        public int? WorkOrderDetailId { get; set; }
        /// <summary>
        /// &#22238;&#22577;&#31278;&#39006;
        /// </summary>
        [Column("report_type", TypeName = "int(11)")]
        public int ReportType { get; set; }
        /// <summary>
        /// &#25505;&#36092;&#21934;ID
        /// </summary>
        [Column("purchase_head_id", TypeName = "int(11)")]
        public int? PurchaseHeadId { get; set; }
        /// <summary>
        /// &#25505;&#36092;&#21934;ID
        /// </summary>
        [Column("sale_head_id", TypeName = "int(11)")]
        public int? SaleHeadId { get; set; }
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
        /// &#21487;&#35069;&#36896;&#25976;&#37327;
        /// </summary>
        [Column("m_count", TypeName = "int(11)")]
        public int MCount { get; set; }
        /// <summary>
        /// &#22238;&#22577;&#25976;&#37327;
        /// </summary>
        [Column("re_count", TypeName = "int(11)")]
        public int ReCount { get; set; }
        /// <summary>
        /// &#25277;&#39511;&#25976;&#37327;
        /// </summary>
        [Column("ck_count", TypeName = "int(11)")]
        public int CkCount { get; set; }
        /// <summary>
        /// OK&#25976;&#37327;
        /// </summary>
        [Column("ok_count", TypeName = "int(11)")]
        public int OkCount { get; set; }
        /// <summary>
        /// NG&#25976;&#37327;
        /// </summary>
        [Column("ng_count", TypeName = "int(11)")]
        public int NgCount { get; set; }
        /// <summary>
        /// NC&#26410;&#23436;&#24037;&#37327;
        /// </summary>
        [Column("nc_count", TypeName = "int(11)")]
        public int NcCount { get; set; }
        /// <summary>
        /// &#27298;&#39511;&#32080;&#26524;(0&#21512;&#26684; 1&#19981;&#21512;&#26684;)
        /// </summary>
        [Column("check_result", TypeName = "int(11)")]
        public int? CheckResult { get; set; }
        /// <summary>
        /// &#22238;&#22577;&#35498;&#26126;
        /// </summary>
        [Column("message", TypeName = "varchar(50)")]
        public string Message { get; set; }
        /// <summary>
        /// &#20633;&#35387;
        /// </summary>
        [Column("remark", TypeName = "varchar(50)")]
        public string Remark { get; set; }
        [Column("delete_flag", TypeName = "int(11)")]
        public int DeleteFlag { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }

        [ForeignKey(nameof(PurchaseHeadId))]
        [InverseProperty("WorkOrderQcLogs")]
        public virtual PurchaseHead PurchaseHead { get; set; }
        [ForeignKey(nameof(SaleHeadId))]
        [InverseProperty("WorkOrderQcLogs")]
        public virtual SaleHead SaleHead { get; set; }
        [ForeignKey(nameof(SupplierId))]
        [InverseProperty("WorkOrderQcLogs")]
        public virtual Supplier Supplier { get; set; }
        [ForeignKey(nameof(WorkOrderDetailId))]
        [InverseProperty("WorkOrderQcLogs")]
        public virtual WorkOrderDetail WorkOrderDetail { get; set; }
        [ForeignKey(nameof(WorkOrderHeadId))]
        [InverseProperty("WorkOrderQcLogs")]
        public virtual WorkOrderHead WorkOrderHead { get; set; }
    }
}
