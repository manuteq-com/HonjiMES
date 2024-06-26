﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#25505;&#36092;&#21934;
    /// </summary>
    [Table("purchase_head")]
    public partial class PurchaseHead
    {
        public PurchaseHead()
        {
            BillofPurchaseDetails = new HashSet<BillofPurchaseDetail>();
            PurchaseDetails = new HashSet<PurchaseDetail>();
            WorkOrderDetails = new HashSet<WorkOrderDetail>();
            WorkOrderQcLogs = new HashSet<WorkOrderQcLog>();
            WorkOrderReportLogs = new HashSet<WorkOrderReportLog>();
        }

        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#25505;&#36092;&#21934;&#34399;
        /// </summary>
        [Required]
        [Column("purchase_no", TypeName = "varchar(100)")]
        public string PurchaseNo { get; set; }
        /// <summary>
        /// &#20379;&#25033;&#21830;id
        /// </summary>
        [Column("supplier_id", TypeName = "int(11)")]
        public int SupplierId { get; set; }
        [Column("type", TypeName = "int(11)")]
        public int? Type { get; set; }
        /// <summary>
        /// &#25505;&#36092;&#29376;&#24907;
        /// </summary>
        [Column("status", TypeName = "int(11)")]
        public int Status { get; set; }
        /// <summary>
        /// &#20633;&#35387;
        /// </summary>
        [Column("remarks", TypeName = "varchar(100)")]
        public string Remarks { get; set; }
        /// <summary>
        /// &#25505;&#36092;&#26085;&#26399;
        /// </summary>
        [Column("purchase_date", TypeName = "timestamp")]
        public DateTime? PurchaseDate { get; set; }
        /// <summary>
        /// &#32317;&#37329;&#38989;
        /// </summary>
        [Column("price_all", TypeName = "decimal(10,2)")]
        public decimal PriceAll { get; set; }
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }

        [ForeignKey(nameof(SupplierId))]
        [InverseProperty("PurchaseHeads")]
        public virtual Supplier Supplier { get; set; }
        [InverseProperty(nameof(BillofPurchaseDetail.Purchase))]
        public virtual ICollection<BillofPurchaseDetail> BillofPurchaseDetails { get; set; }
        [InverseProperty(nameof(PurchaseDetail.Purchase))]
        public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; }
        [InverseProperty(nameof(WorkOrderDetail.Purchase))]
        public virtual ICollection<WorkOrderDetail> WorkOrderDetails { get; set; }
        [InverseProperty(nameof(WorkOrderQcLog.PurchaseHead))]
        public virtual ICollection<WorkOrderQcLog> WorkOrderQcLogs { get; set; }
        [InverseProperty(nameof(WorkOrderReportLog.Purchase))]
        public virtual ICollection<WorkOrderReportLog> WorkOrderReportLogs { get; set; }
    }
}
