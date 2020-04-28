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
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("billof_purchase_id")]
        public int BillofPurchaseId { get; set; }
        [Column("purchase_detail_id")]
        public int? PurchaseDetailId { get; set; }
        [Column("purchase_id")]
        public int? PurchaseId { get; set; }
        [Column("billof_purchase_type")]
        public int BillofPurchaseType { get; set; }
        [Column("supplier_id")]
        public int SupplierId { get; set; }
        [Column("order_id")]
        public int? OrderId { get; set; }
        [Column("data_id")]
        public int DataId { get; set; }
        [Required]
        [Column("data_no", TypeName = "varchar(50)")]
        public string DataNo { get; set; }
        [Required]
        [Column("data_name", TypeName = "varchar(50)")]
        public string DataName { get; set; }
        [Column("specification", TypeName = "varchar(50)")]
        public string Specification { get; set; }
        [Column("quantity")]
        public int Quantity { get; set; }
        [Column("originPrice")]
        public int OriginPrice { get; set; }
        [Column("price")]
        public int Price { get; set; }
        [Column("check_status")]
        public int CheckStatus { get; set; }
        [Column("check_count_in")]
        public int CheckCountIn { get; set; }
        [Column("check_count_out")]
        public int CheckCountOut { get; set; }
        [Column("check_price_in")]
        public int CheckPriceIn { get; set; }
        [Column("check_price_out")]
        public int CheckPriceOut { get; set; }
        [Column("remarks", TypeName = "varchar(50)")]
        public string Remarks { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user")]
        public int CreateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("update_user")]
        public int UpdateUser { get; set; }
        [Column("delivered")]
        public int? Delivered { get; set; }
        [Column("purchase_count")]
        public int PurchaseCount { get; set; }

        [ForeignKey("BillofPurchaseId")]
        [InverseProperty("BillofPurchaseDetails")]
        public virtual BillofPurchaseHead BillofPurchase { get; set; }
        [ForeignKey("PurchaseId")]
        [InverseProperty("BillofPurchaseDetails")]
        public virtual PurchaseHead Purchase { get; set; }
        [ForeignKey("PurchaseDetailId")]
        [InverseProperty("BillofPurchaseDetails")]
        public virtual PurchaseDetail PurchaseDetail { get; set; }
        [ForeignKey("SupplierId")]
        [InverseProperty("BillofPurchaseDetails")]
        public virtual Supplier Supplier { get; set; }
    }
}