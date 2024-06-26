﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#36914;&#36008;&#21934;&#26126;&#32048;
    /// </summary>
    [Table("billof_purchase_detail")]
    public partial class BillofPurchaseDetail
    {
        public BillofPurchaseDetail()
        {
            BillofPurchaseCheckins = new HashSet<BillofPurchaseCheckin>();
            BillofPurchaseReturns = new HashSet<BillofPurchaseReturn>();
        }

        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#36914;&#36008;&#21934;&#34399;
        /// </summary>
        [Column("billof_purchase_id", TypeName = "int(11)")]
        public int BillofPurchaseId { get; set; }
        [Column("purchase_detail_id", TypeName = "int(11)")]
        public int? PurchaseDetailId { get; set; }
        /// <summary>
        /// &#25505;&#36092;&#21934;id
        /// </summary>
        [Column("purchase_id", TypeName = "int(11)")]
        public int? PurchaseId { get; set; }
        /// <summary>
        /// &#36914;&#36008;&#31278;&#39006;
        /// </summary>
        [Column("billof_purchase_type", TypeName = "int(11)")]
        public int BillofPurchaseType { get; set; }
        /// <summary>
        /// &#20379;&#25033;&#21830;id
        /// </summary>
        [Column("supplier_id", TypeName = "int(11)")]
        public int SupplierId { get; set; }
        /// <summary>
        /// &#35330;&#21934;&#21934;&#34399;id
        /// </summary>
        [Column("order_id", TypeName = "int(11)")]
        public int? OrderId { get; set; }
        /// <summary>
        /// &#26009;&#34399;&#31278;&#39006;(1&#21407;&#26009; 2&#25104;&#21697; 3 &#21322;&#25104;&#21697;)
        /// </summary>
        [Column("data_type", TypeName = "int(11)")]
        public int? DataType { get; set; }
        /// <summary>
        /// &#36914;&#36008;&#20839;&#23481;ID
        /// </summary>
        [Column("data_id", TypeName = "int(11)")]
        public int DataId { get; set; }
        /// <summary>
        /// &#36914;&#36008;&#20839;&#23481;&#32232;&#34399;
        /// </summary>
        [Required]
        [Column("data_no", TypeName = "varchar(50)")]
        public string DataNo { get; set; }
        /// <summary>
        /// &#36914;&#36008;&#20839;&#23481;&#21517;&#31281;
        /// </summary>
        [Required]
        [Column("data_name", TypeName = "varchar(50)")]
        public string DataName { get; set; }
        /// <summary>
        /// &#35215;&#26684;
        /// </summary>
        [Column("specification", TypeName = "varchar(50)")]
        public string Specification { get; set; }
        /// <summary>
        /// &#25976;&#37327;
        /// </summary>
        [Column("quantity", TypeName = "int(11)")]
        public int Quantity { get; set; }
        /// <summary>
        /// &#21407;&#21934;&#20729;	
        /// </summary>
        [Column("originPrice", TypeName = "decimal(10,2)")]
        public decimal OriginPrice { get; set; }
        /// <summary>
        /// &#20729;&#26684;
        /// </summary>
        [Column("price", TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
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
        /// &#21934;&#20301;&#21934;&#20729;
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
        /// &#20489;&#21029;id
        /// </summary>
        [Column("warehouse_id", TypeName = "int(11)")]
        public int? WarehouseId { get; set; }
        /// <summary>
        /// &#39511;&#25910;&#29376;&#24907;
        /// </summary>
        [Column("check_status", TypeName = "int(11)")]
        public int CheckStatus { get; set; }
        /// <summary>
        /// &#39511;&#25910;&#25976;&#37327;
        /// </summary>
        [Column("check_count_in", TypeName = "int(11)")]
        public int CheckCountIn { get; set; }
        /// <summary>
        /// &#39511;&#36864;&#25976;&#37327;
        /// </summary>
        [Column("check_count_out", TypeName = "int(11)")]
        public int CheckCountOut { get; set; }
        /// <summary>
        /// &#39511;&#25910;&#37329;&#38989;
        /// </summary>
        [Column("check_price_in", TypeName = "decimal(10,2)")]
        public decimal CheckPriceIn { get; set; }
        /// <summary>
        /// &#39511;&#36864;&#37329;&#38989;
        /// </summary>
        [Column("check_price_out", TypeName = "decimal(10,2)")]
        public decimal CheckPriceOut { get; set; }
        /// <summary>
        /// &#20633;&#35387;
        /// </summary>
        [Column("remarks", TypeName = "varchar(50)")]
        public string Remarks { get; set; }
        /// <summary>
        /// &#23526;&#38555;&#20132;&#36008;&#25976;
        /// </summary>
        [Column("delivered", TypeName = "int(11)")]
        public int? Delivered { get; set; }
        /// <summary>
        /// &#24050;&#38283;&#25505;&#36092;&#25976;&#37327;
        /// </summary>
        [Column("purchase_count", TypeName = "int(11)")]
        public int PurchaseCount { get; set; }
        /// <summary>
        /// &#21034;&#38500;&#35387;&#35352;
        /// </summary>
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int UpdateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }

        [ForeignKey(nameof(BillofPurchaseId))]
        [InverseProperty(nameof(BillofPurchaseHead.BillofPurchaseDetails))]
        public virtual BillofPurchaseHead BillofPurchase { get; set; }
        [ForeignKey(nameof(PurchaseId))]
        [InverseProperty(nameof(PurchaseHead.BillofPurchaseDetails))]
        public virtual PurchaseHead Purchase { get; set; }
        [ForeignKey(nameof(PurchaseDetailId))]
        [InverseProperty("BillofPurchaseDetails")]
        public virtual PurchaseDetail PurchaseDetail { get; set; }
        [ForeignKey(nameof(SupplierId))]
        [InverseProperty("BillofPurchaseDetails")]
        public virtual Supplier Supplier { get; set; }
        [InverseProperty(nameof(BillofPurchaseCheckin.BillofPurchaseDetail))]
        public virtual ICollection<BillofPurchaseCheckin> BillofPurchaseCheckins { get; set; }
        [InverseProperty(nameof(BillofPurchaseReturn.BillofPurchaseDetail))]
        public virtual ICollection<BillofPurchaseReturn> BillofPurchaseReturns { get; set; }
    }
}
