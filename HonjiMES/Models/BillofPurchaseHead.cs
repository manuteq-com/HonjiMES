﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#36914;&#36008;&#21934;
    /// </summary>
    [Table("billof_purchase_head")]
    public partial class BillofPurchaseHead
    {
        public BillofPurchaseHead()
        {
            BillofPurchaseDetails = new HashSet<BillofPurchaseDetail>();
        }

        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#36914;&#36008;&#21934;&#34399;
        /// </summary>
        [Required]
        [Column("billof_purchase_no", TypeName = "varchar(100)")]
        public string BillofPurchaseNo { get; set; }
        [Column("type", TypeName = "int(11)")]
        public int? Type { get; set; }
        /// <summary>
        /// &#36914;&#36008;&#29376;&#24907;
        /// </summary>
        [Column("status", TypeName = "int(11)")]
        public int Status { get; set; }
        /// <summary>
        /// &#20633;&#35387;
        /// </summary>
        [Column("remarks", TypeName = "varchar(100)")]
        public string Remarks { get; set; }
        /// <summary>
        /// &#36914;&#36008;&#26085;&#26399;
        /// </summary>
        [Column("billof_purchase_date", TypeName = "timestamp")]
        public DateTime? BillofPurchaseDate { get; set; }
        [Column("check_time", TypeName = "timestamp")]
        public DateTime? CheckTime { get; set; }
        /// <summary>
        /// &#32317;&#37329;&#38989;
        /// </summary>
        [Column("price_all", TypeName = "decimal(10,2)")]
        public decimal PriceAll { get; set; }
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

        [InverseProperty(nameof(BillofPurchaseDetail.BillofPurchase))]
        public virtual ICollection<BillofPurchaseDetail> BillofPurchaseDetails { get; set; }
    }
}
