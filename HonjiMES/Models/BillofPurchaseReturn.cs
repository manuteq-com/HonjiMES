using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    [Table("billof_purchase_return")]
    public partial class BillofPurchaseReturn
    {
        /// <summary>
        /// 唯一碼
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#39511;&#36864;&#21934;&#34399;
        /// </summary>
        [Column("return_no", TypeName = "varchar(50)")]
        public string ReturnNo { get; set; }
        /// <summary>
        /// &#36914;&#36008;&#21934;&#26126;&#32048;id
        /// </summary>
        [Column("billof_purchase_detail_id", TypeName = "int(11)")]
        public int BillofPurchaseDetailId { get; set; }
        /// <summary>
        /// &#20489;&#24235;id
        /// </summary>
        [Column("warehouse_id", TypeName = "int(11)")]
        public int WarehouseId { get; set; }
        /// <summary>
        /// &#25976;&#37327;
        /// </summary>
        [Column("quantity", TypeName = "decimal(10,2)")]
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
        public decimal PriceAll { get; set; }
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
        /// &#27512;&#36012;(0&#33258;&#24049;1&#24288;&#21830;)
        /// </summary>
        [Column("responsibility", TypeName = "int(11)")]
        public int? Responsibility { get; set; }
        /// <summary>
        /// &#23492;&#22238;&#26085;
        /// </summary>
        [Column("return_time", TypeName = "timestamp")]
        public DateTime? ReturnTime { get; set; }
        /// <summary>
        /// &#21407;&#22240;
        /// </summary>
        [Required]
        [Column("reason", TypeName = "varchar(100)")]
        public string Reason { get; set; }
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

        [ForeignKey(nameof(BillofPurchaseDetailId))]
        [InverseProperty("BillofPurchaseReturns")]
        public virtual BillofPurchaseDetail BillofPurchaseDetail { get; set; }
        [ForeignKey(nameof(WarehouseId))]
        [InverseProperty("BillofPurchaseReturns")]
        public virtual Warehouse Warehouse { get; set; }
    }
}
