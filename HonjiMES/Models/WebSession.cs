﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
[Table("web_sessions")]
    public partial class WebSession
    {
        [Key]
        [Column("id", TypeName = "varchar(128)")]
        public string Id { get; set; }
        [Required]
        [Column("ip_address", TypeName = "varchar(45)")]
        public string IpAddress { get; set; }
        [Column("timestamp", TypeName = "int(10) unsigned")]
        public uint Timestamp { get; set; }
        [Required]
        [Column("data", TypeName = "blob")]
        public byte[] Data { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int? CreateUser { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }
    }
}