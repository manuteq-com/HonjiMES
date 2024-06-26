﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    [Table("supplier_of_material")]
    public partial class SupplierOfMaterial
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("supplier_id", TypeName = "int(11)")]
        public int SupplierId { get; set; }
        [Column("material_basic_id", TypeName = "int(11)")]
        public int MaterialBasicId { get; set; }
        [Column("remarks", TypeName = "varchar(50)")]
        public string Remarks { get; set; }
        [Column("delete_flag", TypeName = "int(11)")]
        public int DeleteFlag { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }

        [ForeignKey(nameof(MaterialBasicId))]
        [InverseProperty("SupplierOfMaterials")]
        public virtual MaterialBasic MaterialBasic { get; set; }
        [ForeignKey(nameof(SupplierId))]
        [InverseProperty("SupplierOfMaterials")]
        public virtual Supplier Supplier { get; set; }
    }
}
