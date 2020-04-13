﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
[Table("users")]
    public partial class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("username", TypeName = "varchar(50)")]
        public string Username { get; set; }
        [Required]
        [Column("realname", TypeName = "varchar(50)")]
        public string Realname { get; set; }
        [Required]
        [Column("password", TypeName = "tinytext")]
        public string Password { get; set; }
        [Required]
        [Column("permission", TypeName = "varchar(50)")]
        public string Permission { get; set; }
        [Column("update_user")]
        public int? UpdateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime? UpdateTime { get; set; }
        [Required]
        [Column("department", TypeName = "varchar(50)")]
        public string Department { get; set; }
        [Column("remarks", TypeName = "varchar(50)")]
        public string Remarks { get; set; }
        [Column("create_user")]
        public int CreateUser { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
    }
}