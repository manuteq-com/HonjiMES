﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#36864;&#36008;&#35352;&#37636;
    /// </summary>
    [Table("return_sale")]
    public partial class ReturnSale
    {
        /// <summary>
        /// 唯一碼
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#37559;&#36864;&#21934;&#34399;
        /// </summary>
        [Column("return_no", TypeName = "varchar(50)")]
        public string ReturnNo { get; set; }
        [Column("sale_detail_new_id", TypeName = "int(11)")]
        public int SaleDetailNewId { get; set; }
        /// <summary>
        /// &#36864;&#36008;&#20489;&#24235;ID
        /// </summary>
        [Column("warehouse_id", TypeName = "int(11)")]
        public int WarehouseId { get; set; }
        /// <summary>
        /// &#36864;&#36008;&#25976;&#37327;
        /// </summary>
        [Column("quantity", TypeName = "int(11)")]
        public int Quantity { get; set; }
        /// <summary>
        /// &#21407;&#22240;
        /// </summary>
        [Column("reason", TypeName = "varchar(50)")]
        public string Reason { get; set; }
        /// <summary>
        /// &#20633;&#35387;
        /// </summary>
        [Column("remarks", TypeName = "varchar(500)")]
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

        [ForeignKey(nameof(SaleDetailNewId))]
        [InverseProperty("ReturnSales")]
        public virtual SaleDetailNew SaleDetailNew { get; set; }
        [ForeignKey(nameof(WarehouseId))]
        [InverseProperty("ReturnSales")]
        public virtual Warehouse Warehouse { get; set; }
    }
}
