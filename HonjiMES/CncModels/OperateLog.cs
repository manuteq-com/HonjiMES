﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.CncModels
{
    [Table("operate_logs")]
    public partial class OperateLog
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#25805;&#20316;&#30340;&#36039;&#26009;&#34920;
        /// </summary>
        [Required]
        [Column("op_table_name", TypeName = "varchar(30)")]
        public string OpTableName { get; set; }
        /// <summary>
        /// &#36039;&#26009;&#21015;id
        /// </summary>
        [Column("relative_id", TypeName = "int(11)")]
        public int RelativeId { get; set; }
        /// <summary>
        /// 1:insert 2:update 3:delete 4:disable/enable
        /// </summary>
        [Column("op_action_type", TypeName = "int(11)")]
        public int OpActionType { get; set; }
        /// <summary>
        /// &#25805;&#20316;&#20154;&#21729;
        /// </summary>
        [Column("operator", TypeName = "int(11)")]
        public int Operator { get; set; }
        /// <summary>
        /// &#25805;&#20316;&#26178;&#38291;
        /// </summary>
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
    }
}