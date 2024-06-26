﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#25104;&#21697;&#22522;&#26412;&#27284;
    /// </summary>
    [Table("product_basic")]
    public partial class ProductBasic
    {
        public ProductBasic()
        {
            Products = new HashSet<Product>();
        }

        /// <summary>
        /// 唯一碼
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#20027;&#20214;&#21697;&#34399;
        /// </summary>
        [Required]
        [Column("product_no", TypeName = "varchar(50)")]
        public string ProductNo { get; set; }
        /// <summary>
        /// &#24288;&#20839;&#25104;&#21697;&#34399;
        /// </summary>
        [Required]
        [Column("product_number", TypeName = "varchar(50)")]
        public string ProductNumber { get; set; }
        /// <summary>
        /// &#20027;&#20214;&#21697;&#21517;
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
        /// &#23384;&#25918;&#24235;&#21029;
        /// </summary>
        [Column("sub_inventory", TypeName = "varchar(50)")]
        public string SubInventory { get; set; }
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }
        /// <summary>
        /// &#20633;&#35387;
        /// </summary>
        [Column("remarks", TypeName = "varchar(50)")]
        public string Remarks { get; set; }
        /// <summary>
        /// &#26032;&#22686;&#26178;&#38291;
        /// </summary>
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }

        [InverseProperty(nameof(Product.ProductBasic))]
        public virtual ICollection<Product> Products { get; set; }
    }
}
