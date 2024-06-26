﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#25104;&#21697;&#24235;&#23384;
    /// </summary>
    [Table("product")]
    public partial class Product
    {
        public Product()
        {
            ProductLogs = new HashSet<ProductLog>();
        }

        /// <summary>
        /// 唯一碼
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("product_basic_id", TypeName = "int(11)")]
        public int ProductBasicId { get; set; }
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
        /// &#24235;&#23384;&#37327;
        /// </summary>
        [Column("quantity", TypeName = "decimal(10,1)")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// &#24235;&#23384;&#26997;&#38480;
        /// </summary>
        [Column("quantity_limit", TypeName = "int(11)")]
        public int? QuantityLimit { get; set; }
        /// <summary>
        /// &#38928;&#20808;&#25187;&#24235;&#25976;&#37327;
        /// </summary>
        [Column("quantity_adv", TypeName = "int(11)")]
        public int QuantityAdv { get; set; }
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
        /// &#20803;&#20214;&#21697;&#34399;
        /// </summary>
        [Column("material_id", TypeName = "int(11)")]
        public int? MaterialId { get; set; }
        /// <summary>
        /// &#21407;&#26009;&#38656;&#27714;&#37327;
        /// </summary>
        [Column("material_require", TypeName = "int(11)")]
        public int? MaterialRequire { get; set; }
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
        [Column("warehouse_id", TypeName = "int(11)")]
        public int WarehouseId { get; set; }

        [ForeignKey(nameof(ProductBasicId))]
        [InverseProperty("Products")]
        public virtual ProductBasic ProductBasic { get; set; }
        [ForeignKey(nameof(WarehouseId))]
        [InverseProperty("Products")]
        public virtual Warehouse Warehouse { get; set; }
        [InverseProperty(nameof(ProductLog.Product))]
        public virtual ICollection<ProductLog> ProductLogs { get; set; }
    }
}
