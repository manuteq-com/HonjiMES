using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    [Table("stock_detail")]
    public partial class StockDetail
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#35519;&#25972;&#21934;ID
        /// </summary>
        [Column("stock_head_id", TypeName = "int(11)")]
        public int StockHeadId { get; set; }
        /// <summary>
        /// &#26009;&#34399;&#31278;&#39006;(1&#21407;&#26009;2&#25104;&#21697;3&#21322;&#25104;&#21697;)
        /// </summary>
        [Column("item_type", TypeName = "int(11)")]
        public int? ItemType { get; set; }
        /// <summary>
        /// &#26009;&#34399;ID
        /// </summary>
        [Column("item_id", TypeName = "int(11)")]
        public int ItemId { get; set; }
        [Column("data_no", TypeName = "varchar(50)")]
        public string DataNo { get; set; }
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
        [Column("message", TypeName = "varchar(50)")]
        public string Message { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }
        [Column("delete_flag", TypeName = "int(11)")]
        public int DeleteFlag { get; set; }

        [ForeignKey(nameof(StockHeadId))]
        [InverseProperty("StockDetails")]
        public virtual StockHead StockHead { get; set; }
    }
}
