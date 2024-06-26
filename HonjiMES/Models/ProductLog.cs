﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#25104;&#21697;LOG
    /// </summary>
    [Table("product_log")]
    public partial class ProductLog
    {
        /// <summary>
        /// 唯一碼
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#35519;&#25972;&#21934;&#34399;
        /// </summary>
        [Column("adjust_no", TypeName = "varchar(50)")]
        public string AdjustNo { get; set; }
        /// <summary>
        /// &#38364;&#36899;&#35330;&#21934;
        /// </summary>
        [Column("link_order", TypeName = "varchar(50)")]
        public string LinkOrder { get; set; }
        /// <summary>
        /// &#25104;&#21697;ID
        /// </summary>
        [Column("product_id", TypeName = "int(11)")]
        public int ProductId { get; set; }
        /// <summary>
        /// &#21407;&#22987;&#25976;&#37327;
        /// </summary>
        [Column("original", TypeName = "decimal(10,1)")]
        public decimal Original { get; set; }
        /// <summary>
        /// &#22686;&#28187;&#25976;&#37327;
        /// </summary>
        [Column("quantity", TypeName = "decimal(10,1)")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// &#21934;&#20729;
        /// </summary>
        [Column("price", TypeName = "decimal(10,2)")]
        public decimal? Price { get; set; }
        /// <summary>
        /// &#32317;&#37329;&#38989;
        /// </summary>
        [Column("price_all", TypeName = "decimal(10,2)")]
        public decimal? PriceAll { get; set; }
        /// <summary>
        /// &#21934;&#20301;
        /// </summary>
        [Column("unit", TypeName = "varchar(50)")]
        public string Unit { get; set; }
        /// <summary>
        /// &#21934;&#20301;&#25976;&#37327;
        /// </summary>
        [Column("unit_count", TypeName = "decimal(10,2)")]
        public decimal? UnitCount { get; set; }
        /// <summary>
        /// &#21934;&#20301;&#37329;&#38989;
        /// </summary>
        [Column("unit_price", TypeName = "decimal(10,2)")]
        public decimal? UnitPrice { get; set; }
        /// <summary>
        /// &#21934;&#20301;&#32317;&#38989;
        /// </summary>
        [Column("unit_price_all", TypeName = "decimal(10,2)")]
        public decimal? UnitPriceAll { get; set; }
        /// <summary>
        /// &#21152;&#24037;&#36027;&#29992;
        /// </summary>
        [Column("work_price", TypeName = "decimal(10,2)")]
        public decimal? WorkPrice { get; set; }
        /// <summary>
        /// &#20462;&#25913;&#21407;&#22240;
        /// </summary>
        [Column("reason", TypeName = "varchar(50)")]
        public string Reason { get; set; }
        /// <summary>
        /// &#35036;&#20805;&#35498;&#26126;
        /// </summary>
        [Column("message", TypeName = "varchar(500)")]
        public string Message { get; set; }
        /// <summary>
        /// &#24314;&#31435;&#26085;&#26399;
        /// </summary>
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// &#20351;&#29992;&#32773;id
        /// </summary>
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        /// <summary>
        /// &#26356;&#26032;&#26178;&#38291;
        /// </summary>
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// &#26356;&#26032;&#32773;id
        /// </summary>
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }
        /// <summary>
        /// &#21034;&#38500;&#35387;&#35352;
        /// </summary>
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }

        [ForeignKey(nameof(ProductId))]
        [InverseProperty("ProductLogs")]
        public virtual Product Product { get; set; }
    }
}
