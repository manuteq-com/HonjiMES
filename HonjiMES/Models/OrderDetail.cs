﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#35330;&#21934;&#26126;&#32048;
    /// </summary>
    [Table("order_detail")]
    public partial class OrderDetail
    {
        public OrderDetail()
        {
            OrderDetailAndWorkOrderHeads = new HashSet<OrderDetailAndWorkOrderHead>();
            PurchaseDetails = new HashSet<PurchaseDetail>();
            SaleDetailNews = new HashSet<SaleDetailNew>();
            WorkOrderHeads = new HashSet<WorkOrderHead>();
        }

        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#35330;&#21934;id
        /// </summary>
        [Column("order_id", TypeName = "int(11)")]
        public int OrderId { get; set; }
        /// <summary>
        /// &#23458;&#25142;&#21934;&#34399;
        /// </summary>
        [Required]
        [Column("customer_no", TypeName = "varchar(50)")]
        public string CustomerNo { get; set; }
        /// <summary>
        /// &#24207;&#34399;
        /// </summary>
        [Column("serial", TypeName = "int(11)")]
        public int Serial { get; set; }
        /// <summary>
        /// &#21697;&#34399;&#22522;&#26412;&#36039;&#35338;id
        /// </summary>
        [Column("material_basic_id", TypeName = "int(11)")]
        public int MaterialBasicId { get; set; }
        /// <summary>
        /// &#21697;&#34399;id
        /// </summary>
        [Column("material_id", TypeName = "int(11)")]
        public int? MaterialId { get; set; }
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
        /// &#25240;&#25187;&#29575;
        /// </summary>
        [Column("discount", TypeName = "decimal(10,2)")]
        public decimal? Discount { get; set; }
        /// <summary>
        /// &#25240;&#24460;&#21934;&#20729;
        /// </summary>
        [Column("discount_price", TypeName = "decimal(10,2)")]
        public decimal? DiscountPrice { get; set; }
        /// <summary>
        /// &#25240;&#24460;&#20729;&#26684;
        /// </summary>
        [Column("price", TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        /// <summary>
        /// &#23526;&#38555;&#20132;&#36008;&#25976;
        /// </summary>
        [Column("delivered", TypeName = "int(11)")]
        public int? Delivered { get; set; }
        /// <summary>
        /// &#21934;&#20301;
        /// </summary>
        [Required]
        [Column("unit", TypeName = "varchar(50)")]
        public string Unit { get; set; }
        /// <summary>
        /// &#38928;&#20132;&#26085;
        /// </summary>
        [Column("due_date", TypeName = "timestamp")]
        public DateTime DueDate { get; set; }
        /// <summary>
        /// &#20633;&#35387;
        /// </summary>
        [Column("remark", TypeName = "varchar(50)")]
        public string Remark { get; set; }
        /// <summary>
        /// &#22238;&#35206;&#20132;&#26399;
        /// </summary>
        [Column("reply_date", TypeName = "timestamp")]
        public DateTime ReplyDate { get; set; }
        /// <summary>
        /// &#22238;&#35206;&#21934;&#20729;
        /// </summary>
        [Column("reply_price", TypeName = "decimal(10,2)")]
        public decimal ReplyPrice { get; set; }
        /// <summary>
        /// &#22238;&#35206;&#20633;&#35387;
        /// </summary>
        [Column("replyRemark", TypeName = "varchar(50)")]
        public string ReplyRemark { get; set; }
        /// <summary>
        /// &#27231;&#34399;
        /// </summary>
        [Required]
        [Column("machine_no", TypeName = "varchar(100)")]
        public string MachineNo { get; set; }
        /// <summary>
        /// &#22294;&#27284;
        /// </summary>
        [Column("drawing", TypeName = "varchar(50)")]
        public string Drawing { get; set; }
        /// <summary>
        /// &#22132;&#22696;
        /// </summary>
        [Column("ink", TypeName = "varchar(50)")]
        public string Ink { get; set; }
        /// <summary>
        /// &#27161;&#31844;
        /// </summary>
        [Column("label", TypeName = "varchar(50)")]
        public string Label { get; set; }
        /// <summary>
        /// &#21253;&#35037;&#25976;
        /// </summary>
        [Column("package", TypeName = "int(11)")]
        public int? Package { get; set; }
        /// <summary>
        /// &#22238;&#35206;&#37327;
        /// </summary>
        [Column("reply", TypeName = "int(11)")]
        public int? Reply { get; set; }
        /// <summary>
        /// &#24050;&#37559;&#36008;&#25976;
        /// </summary>
        [Column("sale_count", TypeName = "int(11)")]
        public int SaleCount { get; set; }
        /// <summary>
        /// &#23436;&#25104;&#37559;&#36008;&#25976;&#37327;
        /// </summary>
        [Column("saled_count", TypeName = "int(11)")]
        public int SaledCount { get; set; }
        [Column("delete_flag", TypeName = "int(11)")]
        public int DeleteFlag { get; set; }
        /// <summary>
        /// &#24314;&#31435;&#26085;&#26399;
        /// </summary>
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }

        [ForeignKey(nameof(MaterialId))]
        [InverseProperty("OrderDetails")]
        public virtual Material Material { get; set; }
        [ForeignKey(nameof(MaterialBasicId))]
        [InverseProperty("OrderDetails")]
        public virtual MaterialBasic MaterialBasic { get; set; }
        [ForeignKey(nameof(OrderId))]
        [InverseProperty(nameof(OrderHead.OrderDetails))]
        public virtual OrderHead Order { get; set; }
        [InverseProperty(nameof(OrderDetailAndWorkOrderHead.OrderDetail))]
        public virtual ICollection<OrderDetailAndWorkOrderHead> OrderDetailAndWorkOrderHeads { get; set; }
        [InverseProperty(nameof(PurchaseDetail.OrderDetail))]
        public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; }
        [InverseProperty(nameof(SaleDetailNew.OrderDetail))]
        public virtual ICollection<SaleDetailNew> SaleDetailNews { get; set; }
        [InverseProperty(nameof(WorkOrderHead.OrderDetail))]
        public virtual ICollection<WorkOrderHead> WorkOrderHeads { get; set; }
    }
}
