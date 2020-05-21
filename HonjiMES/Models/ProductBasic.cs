﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
[Table("product_basic")]
    public partial class ProductBasic
    {
        public ProductBasic()
        {
            BillOfMaterials = new HashSet<BillOfMaterial>();
            OrderDetails = new HashSet<OrderDetail>();
            Products = new HashSet<Product>();
            SaleDetailNews = new HashSet<SaleDetailNew>();
        }

        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Required]
        [Column("product_no", TypeName = "varchar(50)")]
        public string ProductNo { get; set; }
        [Required]
        [Column("product_number", TypeName = "varchar(50)")]
        public string ProductNumber { get; set; }
        [Required]
        [Column("name", TypeName = "varchar(50)")]
        public string Name { get; set; }
        [Column("specification", TypeName = "varchar(50)")]
        public string Specification { get; set; }
        [Required]
        [Column("property", TypeName = "varchar(50)")]
        public string Property { get; set; }
        [Column("price")]
        public float Price { get; set; }
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

        [InverseProperty("ProductBasic")]
        public virtual ICollection<BillOfMaterial> BillOfMaterials { get; set; }
        [InverseProperty("ProductBasic")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [InverseProperty("ProductBasic")]
        public virtual ICollection<Product> Products { get; set; }
        [InverseProperty("ProductBasic")]
        public virtual ICollection<SaleDetailNew> SaleDetailNews { get; set; }
    }
}