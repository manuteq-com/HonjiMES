﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#35330;&#21934;
    /// </summary>
    [Table("order")]
    public partial class Order
    {
        /// <summary>
        /// 專案號
        /// </summary>
        [Required]
        [Column("project_no", TypeName = "varchar(50)")]
        public string ProjectNo { get; set; }
        /// <summary>
        /// &#20027;&#20214;&#21697;&#34399;
        /// </summary>
        [Column("product_no", TypeName = "int(11)")]
        public int ProductNo { get; set; }
        /// <summary>
        /// &#25976;&#37327;
        /// </summary>
        [Column("quantity", TypeName = "int(11)")]
        public int Quantity { get; set; }
        /// <summary>
        /// &#25240;&#24460;&#21934;&#20729;
        /// </summary>
        [Column("price", TypeName = "int(11)")]
        public int Price { get; set; }
        /// <summary>
        /// &#27231;&#34399;
        /// </summary>
        [Required]
        [Column("machine_id", TypeName = "varchar(50)")]
        public string MachineId { get; set; }
        /// <summary>
        /// &#35330;&#21934;&#20132;&#26399;
        /// </summary>
        [Column("order_delivery_date", TypeName = "date")]
        public DateTime? OrderDeliveryDate { get; set; }
        /// <summary>
        /// &#26356;&#26032;&#26178;&#38291;
        /// </summary>
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// &#26032;&#22686;&#26178;&#38291;
        /// </summary>
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// &#24314;&#31435;&#20154;&#21729;
        /// </summary>
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        /// <summary>
        /// &#29376;&#24907;
        /// </summary>
        [Column("status", TypeName = "varchar(50)")]
        public string Status { get; set; }
        /// <summary>
        /// &#23458;&#25142;&#21934;&#34399;
        /// </summary>
        [Column("customer_order_no", TypeName = "varchar(50)")]
        public string CustomerOrderNo { get; set; }
        /// <summary>
        /// &#32080;&#26696;&#21542;
        /// </summary>
        [Column("finish", TypeName = "varchar(50)")]
        public string Finish { get; set; }
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
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
    }
}
