﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
[Table("product_log")]
    public partial class ProductLog
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("product_id", TypeName = "int(11)")]
        public int ProductId { get; set; }
        [Column("original", TypeName = "int(11)")]
        public int Original { get; set; }
        [Column("quantity", TypeName = "int(11)")]
        public int Quantity { get; set; }
        [Column("reason", TypeName = "varchar(50)")]
        public string Reason { get; set; }
        [Column("message", TypeName = "varchar(500)")]
        public string Message { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }

        [ForeignKey(nameof(ProductId))]
        [InverseProperty("ProductLogs")]
        public virtual Product Product { get; set; }
    }
}