﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#25505;&#36092;&#21934;&#26126;&#32048;
    /// </summary>
    [Table("purchase_detail")]
    public partial class PurchaseDetail
    {
        public PurchaseDetail()
        {
            BillofPurchaseDetails = new HashSet<BillofPurchaseDetail>();
        }

        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#25505;&#36092;&#21934;&#34399;
        /// </summary>
        [Column("purchase_id", TypeName = "int(11)")]
        public int PurchaseId { get; set; }
        /// <summary>
        /// &#25505;&#36092;&#31278;&#39006;
        /// </summary>
        [Column("purchase_type", TypeName = "int(11)")]
        public int? PurchaseType { get; set; }
        /// <summary>
        /// &#20379;&#25033;&#21830;id
        /// </summary>
        [Column("supplier_id", TypeName = "int(11)")]
        public int SupplierId { get; set; }
        /// <summary>
        /// &#35330;&#21934;&#26126;&#32048;id
        /// </summary>
        [Column("order_detail_id", TypeName = "int(11)")]
        public int? OrderDetailId { get; set; }
        /// <summary>
        /// &#38928;&#35336;&#20132;&#26399;
        /// </summary>
        [Column("delivery_time", TypeName = "timestamp")]
        public DateTime DeliveryTime { get; set; }
        /// <summary>
        /// &#26009;&#34399;&#31278;&#39006;(1&#21407;&#26009; 2&#25104;&#21697; 3 &#21322;&#25104;&#21697;)
        /// </summary>
        [Column("data_type", TypeName = "int(11)")]
        public int? DataType { get; set; }
        /// <summary>
        /// &#25505;&#36092;&#20839;&#23481;ID
        /// </summary>
        [Column("data_id", TypeName = "int(11)")]
        public int DataId { get; set; }
        /// <summary>
        /// &#25505;&#36092;&#20839;&#23481;&#32232;&#34399;
        /// </summary>
        [Required]
        [Column("data_no", TypeName = "varchar(50)")]
        public string DataNo { get; set; }
        /// <summary>
        /// &#25505;&#36092;&#20839;&#23481;&#21517;&#31281;
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
        /// &#24050;&#20132;&#25976;&#37327;
        /// </summary>
        [Column("delivered", TypeName = "int(11)")]
        public int? Delivered { get; set; }
        /// <summary>
        /// &#26410;&#20132;&#25976;&#37327;
        /// </summary>
        [Column("undelivered", TypeName = "int(11)")]
        public int? Undelivered { get; set; }
        /// <summary>
        /// &#21512;&#26684;&#25976;&#37327;
        /// </summary>
        [Column("ok", TypeName = "int(11)")]
        public int? Ok { get; set; }
        /// <summary>
        /// &#19981;&#21512;&#26684;&#25976;&#37327;
        /// </summary>
        [Column("not_ok", TypeName = "int(11)")]
        public int? NotOk { get; set; }
        /// <summary>
        /// &#21487;&#20462;&#25976;&#37327;
        /// </summary>
        [Column("repair", TypeName = "int(11)")]
        public int? Repair { get; set; }
        /// <summary>
        /// &#19981;&#21487;&#20462;&#25976;&#37327;
        /// </summary>
        [Column("unrepair", TypeName = "int(11)")]
        public int? Unrepair { get; set; }
        /// <summary>
        /// &#24288;&#20839;NG&#25976;&#37327;
        /// </summary>
        [Column("in_NG", TypeName = "int(11)")]
        public int? InNg { get; set; }
        /// <summary>
        /// &#24288;&#22806;NG&#25976;&#37327;
        /// </summary>
        [Column("out_NG", TypeName = "int(11)")]
        public int? OutNg { get; set; }
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
        /// &#20489;&#21029;id
        /// </summary>
        [Column("warehouse_id", TypeName = "int(11)")]
        public int? WarehouseId { get; set; }
        /// <summary>
        /// &#36914;&#36008;&#21934;&#25976;&#37327;
        /// </summary>
        [Column("purchase_count", TypeName = "int(11)")]
        public int PurchaseCount { get; set; }
        /// <summary>
        /// &#23436;&#25104;&#36914;&#36008;&#37327;
        /// </summary>
        [Column("purchased_count", TypeName = "int(11)")]
        public int PurchasedCount { get; set; }
        /// <summary>
        /// &#24037;&#21934;&#32000;&#37636;
        /// </summary>
        [Column("work_order_log", TypeName = "text")]
        public string WorkOrderLog { get; set; }
        /// <summary>
        /// &#20633;&#35387;
        /// </summary>
        [Column("remarks", TypeName = "varchar(50)")]
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

        [ForeignKey(nameof(OrderDetailId))]
        [InverseProperty("PurchaseDetails")]
        public virtual OrderDetail OrderDetail { get; set; }
        [ForeignKey(nameof(PurchaseId))]
        [InverseProperty(nameof(PurchaseHead.PurchaseDetails))]
        public virtual PurchaseHead Purchase { get; set; }
        [ForeignKey(nameof(SupplierId))]
        [InverseProperty("PurchaseDetails")]
        public virtual Supplier Supplier { get; set; }
        [ForeignKey(nameof(WarehouseId))]
        [InverseProperty("PurchaseDetails")]
        public virtual Warehouse Warehouse { get; set; }
        [InverseProperty(nameof(BillofPurchaseDetail.PurchaseDetail))]
        public virtual ICollection<BillofPurchaseDetail> BillofPurchaseDetails { get; set; }
    }
}
