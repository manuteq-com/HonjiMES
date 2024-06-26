﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#21407;&#26009;&#22522;&#26412;&#27284;
    /// </summary>
    [Table("material_basic")]
    public partial class MaterialBasic
    {
        public MaterialBasic()
        {
            BillOfMaterialMaterialBasics = new HashSet<BillOfMaterial>();
            BillOfMaterialProductBasics = new HashSet<BillOfMaterial>();
            Materials = new HashSet<Material>();
            OrderDetails = new HashSet<OrderDetail>();
            RequisitionDetailMaterialBasics = new HashSet<RequisitionDetail>();
            RequisitionDetailProductBasics = new HashSet<RequisitionDetail>();
            Requisitions = new HashSet<Requisition>();
            SaleDetailNews = new HashSet<SaleDetailNew>();
            SupplierOfMaterials = new HashSet<SupplierOfMaterial>();
        }

        /// <summary>
        /// 唯一碼
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#21697;&#34399;
        /// </summary>
        [Required]
        [Column("material_no", TypeName = "varchar(50)")]
        public string MaterialNo { get; set; }
        /// <summary>
        /// &#24288;&#20839;&#21697;&#34399;	
        /// </summary>
        [Column("material_number", TypeName = "varchar(50)")]
        public string MaterialNumber { get; set; }
        /// <summary>
        /// &#21697;&#34399;&#31278;&#39006;
        /// </summary>
        [Column("material_type", TypeName = "int(11)")]
        public int? MaterialType { get; set; }
        /// <summary>
        /// &#21697;&#21517;
        /// </summary>
        [Required]
        [Column("name", TypeName = "varchar(50)")]
        public string Name { get; set; }
        /// <summary>
        /// &#35215;&#26684;
        /// </summary>
        [Column("specification", TypeName = "varchar(50)")]
        public string Specification { get; set; }
        /// <summary>
        /// &#23526;&#38555;&#35215;&#26684;
        /// </summary>
        [Column("actual_specification", TypeName = "varchar(50)")]
        public string ActualSpecification { get; set; }
        /// <summary>
        /// &#23660;&#24615;
        /// </summary>
        [Required]
        [Column("property", TypeName = "varchar(50)")]
        public string Property { get; set; }
        /// <summary>
        /// &#21407;&#20729;&#26684;
        /// </summary>
        [Column("price", TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        /// <summary>
        /// &#21934;&#20301;
        /// </summary>
        [Column("unit", TypeName = "varchar(50)")]
        public string Unit { get; set; }
        /// <summary>
        /// &#20379;&#25033;&#21830;
        /// </summary>
        [Column("supplier_id", TypeName = "int(11)")]
        public int? SupplierId { get; set; }
        /// <summary>
        /// &#22294;&#34399;
        /// </summary>
        [Column("draw_no", TypeName = "varchar(100)")]
        public string DrawNo { get; set; }
        /// <summary>
        /// &#37325;&#37327;(&#20844;&#26020;)
        /// </summary>
        [Column("weight", TypeName = "decimal(10,2)")]
        public decimal? Weight { get; set; }
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

        [ForeignKey(nameof(SupplierId))]
        [InverseProperty("MaterialBasics")]
        public virtual Supplier Supplier { get; set; }
        [InverseProperty(nameof(BillOfMaterial.MaterialBasic))]
        public virtual ICollection<BillOfMaterial> BillOfMaterialMaterialBasics { get; set; }
        [InverseProperty(nameof(BillOfMaterial.ProductBasic))]
        public virtual ICollection<BillOfMaterial> BillOfMaterialProductBasics { get; set; }
        [InverseProperty(nameof(Material.MaterialBasic))]
        public virtual ICollection<Material> Materials { get; set; }
        [InverseProperty(nameof(OrderDetail.MaterialBasic))]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [InverseProperty(nameof(RequisitionDetail.MaterialBasic))]
        public virtual ICollection<RequisitionDetail> RequisitionDetailMaterialBasics { get; set; }
        [InverseProperty(nameof(RequisitionDetail.ProductBasic))]
        public virtual ICollection<RequisitionDetail> RequisitionDetailProductBasics { get; set; }
        [InverseProperty(nameof(Requisition.MaterialBasic))]
        public virtual ICollection<Requisition> Requisitions { get; set; }
        [InverseProperty(nameof(SaleDetailNew.MaterialBasic))]
        public virtual ICollection<SaleDetailNew> SaleDetailNews { get; set; }
        [InverseProperty(nameof(SupplierOfMaterial.MaterialBasic))]
        public virtual ICollection<SupplierOfMaterial> SupplierOfMaterials { get; set; }
    }
}
