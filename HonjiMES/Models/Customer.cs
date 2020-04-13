﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
[Table("customer")]
    public partial class Customer
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
        [Required]
        [Column("phone", TypeName = "varchar(50)")]
        public string Phone { get; set; }
        [Required]
        [Column("fax", TypeName = "varchar(50)")]
        public string Fax { get; set; }
        [Required]
        [Column("email", TypeName = "varchar(50)")]
        public string Email { get; set; }
        [Required]
        [Column("address", TypeName = "varchar(50)")]
        public string Address { get; set; }
        [Column("bank", TypeName = "varchar(50)")]
        public string Bank { get; set; }
        [Column("branch", TypeName = "varchar(50)")]
        public string Branch { get; set; }
        [Column("uniform_no", TypeName = "varchar(50)")]
        public string UniformNo { get; set; }
        [Column("account", TypeName = "varchar(50)")]
        public string Account { get; set; }
        [Column("remarks", TypeName = "varchar(50)")]
        public string Remarks { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user")]
        public int? CreateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("update_user")]
        public int? UpdateUser { get; set; }
    }
}