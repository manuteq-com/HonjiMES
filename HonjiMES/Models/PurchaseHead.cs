﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
[Table("purchase_head")]
    public partial class PurchaseHead
    {
        public PurchaseHead()
        {
            BillofPurchaseDetails = new HashSet<BillofPurchaseDetail>();
            PurchaseDetails = new HashSet<PurchaseDetail>();
            WorkOrderDetails = new HashSet<WorkOrderDetail>();
        }

        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Required]
        [Column("purchase_no", TypeName = "varchar(100)")]
        public string PurchaseNo { get; set; }
        [Column("supplier_id", TypeName = "int(11)")]
        public int SupplierId { get; set; }
        [Column("type", TypeName = "int(11)")]
        public int? Type { get; set; }
        [Column("status", TypeName = "int(11)")]
        public int Status { get; set; }
        [Column("remarks", TypeName = "varchar(100)")]
        public string Remarks { get; set; }
        [Column("purchase_date", TypeName = "timestamp")]
        public DateTime? PurchaseDate { get; set; }
        [Column("price_all", TypeName = "int(11)")]
        public int PriceAll { get; set; }
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

        [InverseProperty(nameof(BillofPurchaseDetail.Purchase))]
        public virtual ICollection<BillofPurchaseDetail> BillofPurchaseDetails { get; set; }
        [InverseProperty(nameof(PurchaseDetail.Purchase))]
        public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; }
        [InverseProperty(nameof(WorkOrderDetail.Purchase))]
        public virtual ICollection<WorkOrderDetail> WorkOrderDetails { get; set; }
    }
}