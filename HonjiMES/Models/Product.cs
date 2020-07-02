﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
[Table("product")]
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
            ProductLogs = new HashSet<ProductLog>();
            SaleDetailNews = new HashSet<SaleDetailNew>();
        }

        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("product_basic_id", TypeName = "int(11)")]
        public int ProductBasicId { get; set; }
        [Required]
        [Column("product_no", TypeName = "varchar(50)")]
        public string ProductNo { get; set; }
        [Required]
        [Column("product_number", TypeName = "varchar(50)")]
        public string ProductNumber { get; set; }
        [Required]
        [Column("name", TypeName = "varchar(50)")]
        public string Name { get; set; }
        [Column("quantity", TypeName = "int(11)")]
        public int Quantity { get; set; }
        [Column("quantity_limit", TypeName = "int(11)")]
        public int? QuantityLimit { get; set; }
        [Column("quantity_adv", TypeName = "int(11)")]
        public int QuantityAdv { get; set; }
        [Column("specification", TypeName = "varchar(50)")]
        public string Specification { get; set; }
        [Required]
        [Column("property", TypeName = "varchar(50)")]
        public string Property { get; set; }
        [Column("price", TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        [Column("material_id", TypeName = "int(11)")]
        public int? MaterialId { get; set; }
        [Column("material_require", TypeName = "int(11)")]
        public int? MaterialRequire { get; set; }
        [Column("sub_inventory", TypeName = "varchar(50)")]
        public string SubInventory { get; set; }
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }
        [Column("remarks", TypeName = "varchar(50)")]
        public string Remarks { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }
        [Column("warehouse_id", TypeName = "int(11)")]
        public int WarehouseId { get; set; }

        [ForeignKey(nameof(ProductBasicId))]
        [InverseProperty("Products")]
        public virtual ProductBasic ProductBasic { get; set; }
        [ForeignKey(nameof(WarehouseId))]
        [InverseProperty("Products")]
        public virtual Warehouse Warehouse { get; set; }
        [InverseProperty(nameof(OrderDetail.Product))]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [InverseProperty(nameof(ProductLog.Product))]
        public virtual ICollection<ProductLog> ProductLogs { get; set; }
        [InverseProperty(nameof(SaleDetailNew.Product))]
        public virtual ICollection<SaleDetailNew> SaleDetailNews { get; set; }
    }
}