﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
[Table("wiproduct_log")]
    public partial class WiproductLog
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("adjust_no", TypeName = "varchar(50)")]
        public string AdjustNo { get; set; }
        [Column("link_order", TypeName = "varchar(50)")]
        public string LinkOrder { get; set; }
        [Column("wiproduct_id", TypeName = "int(11)")]
        public int WiproductId { get; set; }
        [Column("original", TypeName = "int(11)")]
        public int Original { get; set; }
        [Column("quantity", TypeName = "int(11)")]
        public int Quantity { get; set; }
        [Column("price", TypeName = "decimal(10,2)")]
        public decimal? Price { get; set; }
        [Column("price_all", TypeName = "decimal(10,2)")]
        public decimal? PriceAll { get; set; }
        [Column("unit", TypeName = "varchar(50)")]
        public string Unit { get; set; }
        [Column("unit_count", TypeName = "decimal(10,2)")]
        public decimal? UnitCount { get; set; }
        [Column("unit_price", TypeName = "decimal(10,2)")]
        public decimal? UnitPrice { get; set; }
        [Column("unit_price_all", TypeName = "decimal(10,2)")]
        public decimal? UnitPriceAll { get; set; }
        [Column("work_price", TypeName = "decimal(10,2)")]
        public decimal? WorkPrice { get; set; }
        [Column("reason", TypeName = "varchar(50)")]
        public string Reason { get; set; }
        [Column("message", TypeName = "varchar(500)")]
        public string Message { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }

        [ForeignKey(nameof(WiproductId))]
        [InverseProperty("WiproductLogs")]
        public virtual Wiproduct Wiproduct { get; set; }
    }
}