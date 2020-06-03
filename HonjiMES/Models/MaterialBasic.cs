﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
[Table("material_basic")]
    public partial class MaterialBasic
    {
        public MaterialBasic()
        {
            BillOfMaterials = new HashSet<BillOfMaterial>();
            Materials = new HashSet<Material>();
            RequisitionDetails = new HashSet<RequisitionDetail>();
        }

        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Required]
        [Column("material_no", TypeName = "varchar(50)")]
        public string MaterialNo { get; set; }
        [Required]
        [Column("name", TypeName = "varchar(50)")]
        public string Name { get; set; }
        [Column("specification", TypeName = "varchar(50)")]
        public string Specification { get; set; }
        [Required]
        [Column("property", TypeName = "varchar(50)")]
        public string Property { get; set; }
        [Column("supplier", TypeName = "int(11)")]
        public int? Supplier { get; set; }
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

        [InverseProperty(nameof(BillOfMaterial.MaterialBasic))]
        public virtual ICollection<BillOfMaterial> BillOfMaterials { get; set; }
        [InverseProperty(nameof(Material.MaterialBasic))]
        public virtual ICollection<Material> Materials { get; set; }
        [InverseProperty(nameof(RequisitionDetail.MaterialBasic))]
        public virtual ICollection<RequisitionDetail> RequisitionDetails { get; set; }
    }
}