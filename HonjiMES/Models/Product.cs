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
        [Key]
        [Column("id")]
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
        [Column("quantity")]
        public int Quantity { get; set; }
        [Column("quantity_limit")]
        public int? QuantityLimit { get; set; }
        [Column("specification", TypeName = "varchar(50)")]
        public string Specification { get; set; }
        [Required]
        [Column("property", TypeName = "varchar(50)")]
        public string Property { get; set; }
        [Column("price")]
        public float Price { get; set; }
        [Column("material_id")]
        public int MaterialId { get; set; }
        [Column("material_require")]
        public int MaterialRequire { get; set; }
        [Required]
        [Column("sub_inventory", TypeName = "varchar(50)")]
        public string SubInventory { get; set; }
        [Column("delete_flag")]
        public sbyte DeleteFlag { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user")]
        public int CreateUser { get; set; }
        [Column("update_user")]
        public int? UpdateUser { get; set; }
        [Column("remarks", TypeName = "varchar(50)")]
        public string Remarks { get; set; }
    }
}