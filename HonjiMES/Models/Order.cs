﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
[Table("order")]
    public partial class Order
    {
        [Required]
        [Column("project_no", TypeName = "varchar(50)")]
        public string ProjectNo { get; set; }
        [Column("product_no", TypeName = "int(11)")]
        public int ProductNo { get; set; }
        [Column("quantity", TypeName = "int(11)")]
        public int Quantity { get; set; }
        [Column("price", TypeName = "int(11)")]
        public int Price { get; set; }
        [Required]
        [Column("machine_id", TypeName = "varchar(50)")]
        public string MachineId { get; set; }
        [Column("order_delivery_date", TypeName = "date")]
        public DateTime? OrderDeliveryDate { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        [Column("status", TypeName = "varchar(50)")]
        public string Status { get; set; }
        [Column("customer_order_no", TypeName = "varchar(50)")]
        public string CustomerOrderNo { get; set; }
        [Column("finish", TypeName = "varchar(50)")]
        public string Finish { get; set; }
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }
    }
}