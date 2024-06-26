﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#38936;&#26009;&#36039;&#26009;&#21103;&#27284;
    /// </summary>
    [Table("requisition_detail")]
    public partial class RequisitionDetail
    {
        public RequisitionDetail()
        {
            Receives = new HashSet<Receive>();
        }

        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("requisition_id", TypeName = "int(11)")]
        public int RequisitionId { get; set; }
        /// <summary>
        /// &#38542;&#23652;
        /// </summary>
        [Column(TypeName = "int(11)")]
        public int? Lv { get; set; }
        /// <summary>
        /// &#20803;&#20214;&#21697;&#21517;
        /// </summary>
        [Column("name", TypeName = "varchar(50)")]
        public string Name { get; set; }
        [Column("material_basic_id", TypeName = "int(11)")]
        public int? MaterialBasicId { get; set; }
        /// <summary>
        /// &#20803;&#20214;&#21697;&#21517;
        /// </summary>
        [Column("material_name", TypeName = "varchar(50)")]
        public string MaterialName { get; set; }
        /// <summary>
        /// &#20803;&#20214;&#21697;&#34399;
        /// </summary>
        [Column("material_no", TypeName = "varchar(50)")]
        public string MaterialNo { get; set; }
        /// <summary>
        /// &#20803;&#20214;&#35215;&#26684;
        /// </summary>
        [Column("material_specification", TypeName = "varchar(50)")]
        public string MaterialSpecification { get; set; }
        [Column("product_basic_id", TypeName = "int(11)")]
        public int? ProductBasicId { get; set; }
        /// <summary>
        /// &#20027;&#20214;&#21697;&#34399;
        /// </summary>
        [Column("product_no", TypeName = "varchar(50)")]
        public string ProductNo { get; set; }
        /// <summary>
        /// &#24288;&#20839;&#25104;&#21697;&#34399;
        /// </summary>
        [Column("product_number", TypeName = "varchar(50)")]
        public string ProductNumber { get; set; }
        /// <summary>
        /// &#20027;&#20214;&#21697;&#21517;
        /// </summary>
        [Column("product_name", TypeName = "varchar(50)")]
        public string ProductName { get; set; }
        /// <summary>
        /// &#35215;&#26684;
        /// </summary>
        [Column("product_specification", TypeName = "varchar(50)")]
        public string ProductSpecification { get; set; }
        /// <summary>
        /// &#39511;&#25910;&#25976;&#37327;
        /// </summary>
        [Column("quantity", TypeName = "decimal(10,3)")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// &#20633;&#35387;
        /// </summary>
        [Column("remarks", TypeName = "varchar(100)")]
        public string Remarks { get; set; }
        /// <summary>
        /// &#21034;&#38500;&#35387;&#35352;
        /// </summary>
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
        /// <summary>
        /// &#26159;&#21542;&#28858;&#21407;&#26009;
        /// </summary>
        [Column("ismaterial")]
        public bool? Ismaterial { get; set; }

        [ForeignKey(nameof(MaterialBasicId))]
        [InverseProperty("RequisitionDetailMaterialBasics")]
        public virtual MaterialBasic MaterialBasic { get; set; }
        [ForeignKey(nameof(ProductBasicId))]
        [InverseProperty("RequisitionDetailProductBasics")]
        public virtual MaterialBasic ProductBasic { get; set; }
        [ForeignKey(nameof(RequisitionId))]
        [InverseProperty("RequisitionDetails")]
        public virtual Requisition Requisition { get; set; }
        [InverseProperty(nameof(Receive.RequisitionDetail))]
        public virtual ICollection<Receive> Receives { get; set; }
    }
}
