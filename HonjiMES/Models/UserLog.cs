﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
[Table("user_logs")]
    public partial class UserLog
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("login_time", TypeName = "timestamp")]
        public DateTime LoginTime { get; set; }
        [Column("logout_time", TypeName = "timestamp")]
        public DateTime? LogoutTime { get; set; }
        [Column("create_user")]
        public int CreateUser { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("update_user")]
        public int? UpdateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime? UpdateTime { get; set; }
    }
}