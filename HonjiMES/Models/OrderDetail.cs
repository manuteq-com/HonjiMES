﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
[Table("order_detail")]
    public partial class OrderDetail
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("order_id")]
        public int OrderId { get; set; }
        [Column("serial")]
        public int Serial { get; set; }
        [Column("product_id")]
        public int ProductId { get; set; }
        [Column("quantity")]
        public int Quantity { get; set; }
        [Column("originPrice")]
        public int OriginPrice { get; set; }
        [Column("discount")]
        public int? Discount { get; set; }
        [Column("discount_price")]
        public int? DiscountPrice { get; set; }
        [Column("price")]
        public int Price { get; set; }
        [Column("delivered")]
        public int? Delivered { get; set; }
        [Required]
        [Column("unit", TypeName = "varchar(50)")]
        public string Unit { get; set; }
        [Column("due_date", TypeName = "timestamp")]
        public DateTime DueDate { get; set; }
        [Column("remark", TypeName = "varchar(50)")]
        public string Remark { get; set; }
        [Column("reply")]
        public int? Reply { get; set; }
        [Column("reply_date", TypeName = "timestamp")]
        public DateTime ReplyDate { get; set; }
        [Column("replyRemark", TypeName = "varchar(50)")]
        public string ReplyRemark { get; set; }
        [Column("machine_no")]
        public int MachineNo { get; set; }
        [Column("drawing", TypeName = "varchar(50)")]
        public string Drawing { get; set; }
        [Column("ink", TypeName = "varchar(50)")]
        public string Ink { get; set; }
        [Column("label", TypeName = "varchar(50)")]
        public string Label { get; set; }
        [Column("package")]
        public int? Package { get; set; }
        [Column("create_user")]
        public int CreateUser { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("update_user")]
        public int? UpdateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime? UpdateTime { get; set; }

        [ForeignKey("OrderId")]
        [InverseProperty("OrderDetails")]
        public virtual OrderHead Order { get; set; }
    }
}