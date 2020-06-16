﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
[Table("material")]
    public partial class Material
    {
        public Material()
        {
            MaterialLogs = new HashSet<MaterialLog>();
        }

        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("material_basic_id", TypeName = "int(11)")]
        public int MaterialBasicId { get; set; }
        [Required]
        [Column("material_no", TypeName = "varchar(50)")]
        public string MaterialNo { get; set; }
        [Required]
        [Column("name", TypeName = "varchar(50)")]
        public string Name { get; set; }
        [Column("quantity", TypeName = "int(11)")]
        public int Quantity { get; set; }
        [Column("specification", TypeName = "varchar(50)")]
        public string Specification { get; set; }
        [Required]
        [Column("property", TypeName = "varchar(50)")]
        public string Property { get; set; }
        [Column("price")]
        public float Price { get; set; }
        [Column("unit", TypeName = "varchar(50)")]
        public string Unit { get; set; }
        [Column("composition", TypeName = "int(11)")]
        public int Composition { get; set; }
        [Column("base_quantity", TypeName = "int(11)")]
        public int BaseQuantity { get; set; }
        [Column("supplier", TypeName = "int(11)")]
        public int? Supplier { get; set; }
        [Column("sub_inventory", TypeName = "varchar(50)")]
        public string SubInventory { get; set; }
        [Column("warehouse_id", TypeName = "int(11)")]
        public int WarehouseId { get; set; }
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

        [ForeignKey(nameof(MaterialBasicId))]
        [InverseProperty("Materials")]
        public virtual MaterialBasic MaterialBasic { get; set; }
        [ForeignKey(nameof(WarehouseId))]
        [InverseProperty("Materials")]
        public virtual Warehouse Warehouse { get; set; }
        [InverseProperty(nameof(MaterialLog.Material))]
        public virtual ICollection<MaterialLog> MaterialLogs { get; set; }
    }
}