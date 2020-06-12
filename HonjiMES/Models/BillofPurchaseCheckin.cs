﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
[Table("billof_purchase_checkin")]
    public partial class BillofPurchaseCheckin
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("billof_purchase_detail_id", TypeName = "int(11)")]
        public int BillofPurchaseDetailId { get; set; }
        [Column("checkin_type", TypeName = "int(11)")]
        public int? CheckinType { get; set; }
        [Column("quantity", TypeName = "int(11)")]
        public int Quantity { get; set; }
        [Column("price")]
        public float? Price { get; set; }
        [Column("price_all")]
        public float PriceAll { get; set; }
        [Column("unit", TypeName = "varchar(50)")]
        public string Unit { get; set; }
        [Column("unit_count")]
        public float? UnitCount { get; set; }
        [Column("unit_count_all")]
        public float? UnitCountAll { get; set; }
        [Column("remarks", TypeName = "varchar(100)")]
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

        [ForeignKey(nameof(BillofPurchaseDetailId))]
        [InverseProperty("BillofPurchaseCheckins")]
        public virtual BillofPurchaseDetail BillofPurchaseDetail { get; set; }
    }
}