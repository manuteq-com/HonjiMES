﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
[Table("purchase_detail")]
    public partial class PurchaseDetail
    {
        public PurchaseDetail()
        {
            BillofPurchaseDetails = new HashSet<BillofPurchaseDetail>();
        }

        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("purchase_id", TypeName = "int(11)")]
        public int PurchaseId { get; set; }
        [Column("purchase_type", TypeName = "int(11)")]
        public int? PurchaseType { get; set; }
        [Column("supplier_id", TypeName = "int(11)")]
        public int SupplierId { get; set; }
        [Column("order_id", TypeName = "int(11)")]
        public int? OrderId { get; set; }
        [Column("delivery_time", TypeName = "timestamp")]
        public DateTime DeliveryTime { get; set; }
        [Column("data_type", TypeName = "int(11)")]
        public int DataType { get; set; }
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
        [Column("warehouse_id", TypeName = "int(11)")]
        public int? WarehouseId { get; set; }
        [Column("purchase_count", TypeName = "int(11)")]
        public int PurchaseCount { get; set; }
        [Column("remarks", TypeName = "varchar(50)")]
        public string Remarks { get; set; }
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

        [ForeignKey(nameof(PurchaseId))]
        [InverseProperty(nameof(PurchaseHead.PurchaseDetails))]
        public virtual PurchaseHead Purchase { get; set; }
        [InverseProperty(nameof(BillofPurchaseDetail.PurchaseDetail))]
        public virtual ICollection<BillofPurchaseDetail> BillofPurchaseDetails { get; set; }
    }
}