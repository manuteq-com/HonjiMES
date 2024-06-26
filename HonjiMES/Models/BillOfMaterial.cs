﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#29986;&#21697;&#32068;&#25104;&#34920;
    /// </summary>
    [Table("bill_of_material")]
    public partial class BillOfMaterial
    {
        public BillOfMaterial()
        {
            InverseP = new HashSet<BillOfMaterial>();
        }

        /// <summary>
        /// 唯一碼
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#29238;ID
        /// </summary>
        [Column("pid", TypeName = "int(11)")]
        public int? Pid { get; set; }
        /// <summary>
        /// &#20027;&#20214;
        /// </summary>
        [Column("master", TypeName = "int(11)")]
        public int Master { get; set; }
        [Column("product_basic_id", TypeName = "int(11)")]
        public int? ProductBasicId { get; set; }
        [Column("material_basic_id", TypeName = "int(11)")]
        public int? MaterialBasicId { get; set; }
        /// <summary>
        /// &#21517;&#31281;
        /// </summary>
        [Column("name", TypeName = "varchar(50)")]
        public string Name { get; set; }
        /// <summary>
        /// &#25976;&#37327;
        /// </summary>
        [Column("quantity", TypeName = "decimal(10,3)")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// &#21934;&#20301;
        /// </summary>
        [Column("unit", TypeName = "varchar(10)")]
        public string Unit { get; set; }
        /// <summary>
        /// &#23652;&#25976;
        /// </summary>
        [Column("lv", TypeName = "int(11)")]
        public int Lv { get; set; }
        /// <summary>
        /// &#22806;&#21253;&#35387;&#35352;
        /// </summary>
        [Column("outsource", TypeName = "tinyint(4)")]
        public sbyte? Outsource { get; set; }
        /// <summary>
        /// &#32068;&#25104;&#34920;&#32676;&#32068;
        /// </summary>
        [Column("group", TypeName = "int(11)")]
        public int Group { get; set; }
        /// <summary>
        /// BOM&#30340;&#39006;&#22411;
        /// </summary>
        [Column("type", TypeName = "tinyint(4)")]
        public sbyte? Type { get; set; }
        /// <summary>
        /// &#20633;&#35387;
        /// </summary>
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
        /// <summary>
        /// &#21034;&#38500;&#35387;&#35352;
        /// </summary>
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }

        [ForeignKey(nameof(MaterialBasicId))]
        [InverseProperty("BillOfMaterialMaterialBasics")]
        public virtual MaterialBasic MaterialBasic { get; set; }
        [ForeignKey(nameof(Pid))]
        [InverseProperty(nameof(BillOfMaterial.InverseP))]
        public virtual BillOfMaterial P { get; set; }
        [ForeignKey(nameof(ProductBasicId))]
        [InverseProperty("BillOfMaterialProductBasics")]
        public virtual MaterialBasic ProductBasic { get; set; }
        [InverseProperty(nameof(BillOfMaterial.P))]
        public virtual ICollection<BillOfMaterial> InverseP { get; set; }
    }
}
