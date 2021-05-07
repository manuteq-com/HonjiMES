using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#36914;&#36008;&#27298;&#39511;
    /// </summary>
    [Table("billof_purchase_checkin")]
    public partial class BillofPurchaseCheckin
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("billof_purchase_detail_id", TypeName = "int(11)")]
        public int BillofPurchaseDetailId { get; set; }
        /// <summary>
        /// &#39511;&#25910;&#39006;&#22411;
        /// </summary>
        [Column("checkin_type", TypeName = "int(11)")]
        public int? CheckinType { get; set; }
        /// <summary>
        /// &#39511;&#25910;&#25976;&#37327;
        /// </summary>
        [Column("quantity", TypeName = "int(11)")]
        public int Quantity { get; set; }
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
        [InverseProperty("BillofPurchaseCheckins")]
        public virtual BillofPurchaseDetail BillofPurchaseDetail { get; set; }
    }
}
