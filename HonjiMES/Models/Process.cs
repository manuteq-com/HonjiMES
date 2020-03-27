﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
[Table("process")]
    public partial class Process
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name", TypeName = "varchar(50)")]
        public string Name { get; set; }
        [Required]
        [Column("code", TypeName = "varchar(50)")]
        public string Code { get; set; }
        [Column("remark", TypeName = "varchar(50)")]
        public string Remark { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user")]
        public int CreateUser { get; set; }
    }
}