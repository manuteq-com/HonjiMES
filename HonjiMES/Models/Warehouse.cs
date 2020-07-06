﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
[Table("warehouse")]
    public partial class Warehouse
    {
        public Warehouse()
        {
            BillofPurchaseReturns = new HashSet<BillofPurchaseReturn>();
            Materials = new HashSet<Material>();
            Products = new HashSet<Product>();
            ReturnSales = new HashSet<ReturnSale>();
            Wiproducts = new HashSet<Wiproduct>();
        }

        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("code", TypeName = "varchar(50)")]
        public string Code { get; set; }
        [Column("name", TypeName = "varchar(50)")]
        public string Name { get; set; }
        [Column("contact", TypeName = "varchar(50)")]
        public string Contact { get; set; }
        [Column("phone", TypeName = "varchar(50)")]
        public string Phone { get; set; }
        [Column("fax", TypeName = "varchar(50)")]
        public string Fax { get; set; }
        [Column("email", TypeName = "varchar(50)")]
        public string Email { get; set; }
        [Column("address", TypeName = "varchar(50)")]
        public string Address { get; set; }
        [Column("remarks", TypeName = "varchar(50)")]
        public string Remarks { get; set; }
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }
        [Column("recheck", TypeName = "tinyint(4)")]
        public sbyte? Recheck { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime? UpdateTime { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }

        [InverseProperty(nameof(BillofPurchaseReturn.Warehouse))]
        public virtual ICollection<BillofPurchaseReturn> BillofPurchaseReturns { get; set; }
        [InverseProperty(nameof(Material.Warehouse))]
        public virtual ICollection<Material> Materials { get; set; }
        [InverseProperty(nameof(Product.Warehouse))]
        public virtual ICollection<Product> Products { get; set; }
        [InverseProperty(nameof(ReturnSale.Warehouse))]
        public virtual ICollection<ReturnSale> ReturnSales { get; set; }
        [InverseProperty(nameof(Wiproduct.Warehouse))]
        public virtual ICollection<Wiproduct> Wiproducts { get; set; }
    }
}