﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
[Table("purchase")]
    public partial class Purchase
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("purchase_no", TypeName = "varchar(50)")]
        public string PurchaseNo { get; set; }
        [Column("material_no")]
        public int MaterialNo { get; set; }
        [Required]
        [Column("name", TypeName = "varchar(50)")]
        public string Name { get; set; }
        [Column("quantity")]
        public int Quantity { get; set; }
        [Column("price")]
        public int Price { get; set; }
        [Required]
        [Column("specification", TypeName = "varchar(50)")]
        public string Specification { get; set; }
        [Column("supplier")]
        public int Supplier { get; set; }
        [Column("purchase_date", TypeName = "date")]
        public DateTime PurchaseDate { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user")]
        public int CreateUser { get; set; }
        [Column("remarks", TypeName = "varchar(50)")]
        public string Remarks { get; set; }
    }
}