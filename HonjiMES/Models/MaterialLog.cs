﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
[Table("material_log")]
    public partial class MaterialLog
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("material_id")]
        public int MaterialId { get; set; }
        [Column("original")]
        public int Original { get; set; }
        [Column("quantity")]
        public int Quantity { get; set; }
        [Column("reason", TypeName = "varchar(50)")]
        public string Reason { get; set; }
        [Column("message", TypeName = "varchar(500)")]
        public string Message { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user")]
        public int CreateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("update_user")]
        public int? UpdateUser { get; set; }

        [ForeignKey("MaterialId")]
        [InverseProperty("MaterialLogs")]
        public virtual Material Material { get; set; }
    }
}