﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
[Table("bill_of_material_ver")]
    public partial class BillOfMaterialVer
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("product_basic_id", TypeName = "int(11)")]
        public int ProductBasicId { get; set; }
        [Column("version", TypeName = "decimal(10,2)")]
        public decimal Version { get; set; }
        [Column("bomid", TypeName = "int(11)")]
        public int Bomid { get; set; }
        [Column("bompid", TypeName = "int(11)")]
        public int? Bompid { get; set; }
        [Column("product_no", TypeName = "varchar(50)")]
        public string ProductNo { get; set; }
        [Column("product_name", TypeName = "varchar(50)")]
        public string ProductName { get; set; }
        [Column("material_no", TypeName = "varchar(50)")]
        public string MaterialNo { get; set; }
        [Column("material_name", TypeName = "varchar(50)")]
        public string MaterialName { get; set; }
        [Column("name", TypeName = "varchar(50)")]
        public string Name { get; set; }
        [Column("quantity", TypeName = "int(11)")]
        public int Quantity { get; set; }
        [Column("unit", TypeName = "varchar(10)")]
        public string Unit { get; set; }
        [Column("lv", TypeName = "int(11)")]
        public int Lv { get; set; }
        [Column("outsource", TypeName = "tinyint(4)")]
        public sbyte? Outsource { get; set; }
        [Column("group", TypeName = "int(11)")]
        public int Group { get; set; }
        [Column("type", TypeName = "tinyint(4)")]
        public sbyte? Type { get; set; }
        [Column("remarks", TypeName = "varchar(50)")]
        public string Remarks { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }
    }
}