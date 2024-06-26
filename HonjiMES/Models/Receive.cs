﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#38936;&#26009;&#25976;&#37327;&#35352;&#37636;&#34920;
    /// </summary>
    [Table("receive")]
    public partial class Receive
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("requisition_detail_id", TypeName = "int(11)")]
        public int RequisitionDetailId { get; set; }
        /// <summary>
        /// &#38936;&#21462;&#25976;&#37327;
        /// </summary>
        [Column("quantity", TypeName = "decimal(10,1)")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// &#20351;&#29992;&#20489;&#21029;
        /// </summary>
        [Column("warehouse_id", TypeName = "int(11)")]
        public int? WarehouseId { get; set; }
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

        [ForeignKey(nameof(RequisitionDetailId))]
        [InverseProperty("Receives")]
        public virtual RequisitionDetail RequisitionDetail { get; set; }
        [ForeignKey(nameof(WarehouseId))]
        [InverseProperty("Receives")]
        public virtual Warehouse Warehouse { get; set; }
    }
}
