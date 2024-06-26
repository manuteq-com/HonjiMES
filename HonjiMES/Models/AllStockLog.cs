﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    [Table("all_stock_log")]
    public partial class AllStockLog
    {
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("adjust_no", TypeName = "varchar(50)")]
        public string AdjustNo { get; set; }
        [Column("link_order", TypeName = "varchar(50)")]
        public string LinkOrder { get; set; }
        [Column("data_type", TypeName = "int(11)")]
        public int? DataType { get; set; }
        [Column("data_id", TypeName = "int(11)")]
        public int DataId { get; set; }
        [Column("data_no", TypeName = "varchar(50)")]
        public string DataNo { get; set; }
        [Column("data_name", TypeName = "varchar(50)")]
        public string DataName { get; set; }
        [Column("original", TypeName = "decimal(10,1)")]
        public decimal Original { get; set; }
        [Column("quantity", TypeName = "decimal(10,1)")]
        public decimal Quantity { get; set; }
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
        [Required]
        [Column("name_log", TypeName = "varchar(13)")]
        public string NameLog { get; set; }
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }
        [Column("name_type", TypeName = "int(1)")]
        public int NameType { get; set; }
        [Column("warehouse_id", TypeName = "int(11)")]
        public int? WarehouseId { get; set; }
    }
}
