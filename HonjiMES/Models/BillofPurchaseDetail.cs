﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
[Table("billof_purchase_detail")]
    public partial class BillofPurchaseDetail
    {
        public BillofPurchaseDetail()
        {
            BillofPurchaseCheckins = new HashSet<BillofPurchaseCheckin>();
        }

        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("billof_purchase_id", TypeName = "int(11)")]
        public int BillofPurchaseId { get; set; }
        [Column("purchase_detail_id", TypeName = "int(11)")]
        public int? PurchaseDetailId { get; set; }
        [Column("purchase_id", TypeName = "int(11)")]
        public int? PurchaseId { get; set; }
        [Column("billof_purchase_type", TypeName = "int(11)")]
        public int BillofPurchaseType { get; set; }
        [Column("supplier_id", TypeName = "int(11)")]
        public int SupplierId { get; set; }
        [Column("order_id", TypeName = "int(11)")]
        public int? OrderId { get; set; }
        [Column("data_id", TypeName = "int(11)")]
        public int DataId { get; set; }
        [Required]
        [Column("data_no", TypeName = "varchar(50)")]
        public string DataNo { get; set; }
        [Required]
        [Column("data_name", TypeName = "varchar(50)")]
        public string DataName { get; set; }
        [Column("specification", TypeName = "varchar(50)")]
        public string Specification { get; set; }
        [Column("quantity", TypeName = "int(11)")]
        public int Quantity { get; set; }
        [Column("originPrice", TypeName = "int(11)")]
        public int OriginPrice { get; set; }
        [Column("price", TypeName = "int(11)")]
        public int Price { get; set; }
        [Column("check_status", TypeName = "int(11)")]
        public int CheckStatus { get; set; }
        [Column("check_count_in", TypeName = "int(11)")]
        public int CheckCountIn { get; set; }
        [Column("check_count_out", TypeName = "int(11)")]
        public int CheckCountOut { get; set; }
        [Column("check_price_in", TypeName = "int(11)")]
        public int CheckPriceIn { get; set; }
        [Column("check_price_out", TypeName = "int(11)")]
        public int CheckPriceOut { get; set; }
        [Column("remarks", TypeName = "varchar(50)")]
        public string Remarks { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int UpdateUser { get; set; }
        [Column("delivered", TypeName = "int(11)")]
        public int? Delivered { get; set; }
        [Column("purchase_count", TypeName = "int(11)")]
        public int PurchaseCount { get; set; }
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }

        [ForeignKey(nameof(BillofPurchaseId))]
        [InverseProperty(nameof(BillofPurchaseHead.BillofPurchaseDetails))]
        public virtual BillofPurchaseHead BillofPurchase { get; set; }
        [ForeignKey(nameof(PurchaseId))]
        [InverseProperty(nameof(PurchaseHead.BillofPurchaseDetails))]
        public virtual PurchaseHead Purchase { get; set; }
        [ForeignKey(nameof(PurchaseDetailId))]
        [InverseProperty("BillofPurchaseDetails")]
        public virtual PurchaseDetail PurchaseDetail { get; set; }
        [ForeignKey(nameof(SupplierId))]
        [InverseProperty("BillofPurchaseDetails")]
        public virtual Supplier Supplier { get; set; }
        [InverseProperty(nameof(BillofPurchaseCheckin.BillofPurchaseDetail))]
        public virtual ICollection<BillofPurchaseCheckin> BillofPurchaseCheckins { get; set; }
    }
}