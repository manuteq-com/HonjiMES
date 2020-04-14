﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
[Table("message")]
    public partial class Message
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("data", TypeName = "varchar(100)")]
        public string Data { get; set; }
        [Column("type")]
        public sbyte? Type { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user")]
        public int CreateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("update_user")]
        public int? UpdateUser { get; set; }
    }
}