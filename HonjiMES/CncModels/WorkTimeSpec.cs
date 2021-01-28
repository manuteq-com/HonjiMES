﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.CncModels
{
    [Table("work_time_spec")]
    public partial class WorkTimeSpec
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("work_time_group_id", TypeName = "int(11)")]
        public int WorkTimeGroupId { get; set; }
        [Required]
        [Column("time_type", TypeName = "varchar(30)")]
        public string TimeType { get; set; }
        [Required]
        [Column("value", TypeName = "varchar(30)")]
        public string Value { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
    }
}