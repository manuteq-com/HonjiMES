﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#35330;&#21934;
    /// </summary>
    [Table("order_head")]
    public partial class OrderHead
    {
        public OrderHead()
        {
            OrderDetails = new HashSet<OrderDetail>();
            SaleDetailNews = new HashSet<SaleDetailNew>();
        }

        /// <summary>
        /// 訂單id
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#35330;&#21934;&#21934;&#34399;
        /// </summary>
        [Required]
        [Column("order_no", TypeName = "varchar(50)")]
        public string OrderNo { get; set; }
        [Column("order_type", TypeName = "varchar(50)")]
        public string OrderType { get; set; }
        /// <summary>
        /// &#23458;&#25142;&#21934;&#34399;
        /// </summary>
        [Required]
        [Column("customer_no", TypeName = "varchar(50)")]
        public string CustomerNo { get; set; }
        /// <summary>
        /// &#35330;&#21934;&#26085;&#26399;
        /// </summary>
        [Column("order_date", TypeName = "timestamp")]
        public DateTime OrderDate { get; set; }
        /// <summary>
        /// &#22238;&#35206;&#20132;&#26399;
        /// </summary>
        [Column("reply_date", TypeName = "timestamp")]
        public DateTime? ReplyDate { get; set; }
        /// <summary>
        /// &#38283;&#22987;&#26085;&#26399;
        /// </summary>
        [Column("start_date", TypeName = "timestamp")]
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// &#23436;&#25104;&#26085;&#26399;
        /// </summary>
        [Column("finish_date", TypeName = "timestamp")]
        public DateTime? FinishDate { get; set; }
        /// <summary>
        /// &#23458;&#25142;id
        /// </summary>
        [Column("customer", TypeName = "int(11)")]
        public int Customer { get; set; }
        [Column("status", TypeName = "tinyint(4)")]
        public sbyte Status { get; set; }
        /// <summary>
        /// &#20633;&#35387;
        /// </summary>
        [Column("remarks", TypeName = "varchar(100)")]
        public string Remarks { get; set; }
        /// <summary>
        /// &#30906;&#35469;&#27396;
        /// </summary>
        [Column("check_flag", TypeName = "int(11)")]
        public int CheckFlag { get; set; }
        [Column("delete_flag", TypeName = "int(11)")]
        public int DeleteFlag { get; set; }
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
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }

        [InverseProperty(nameof(OrderDetail.Order))]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [InverseProperty(nameof(SaleDetailNew.Order))]
        public virtual ICollection<SaleDetailNew> SaleDetailNews { get; set; }
    }
}
