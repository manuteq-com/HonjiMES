﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#23458;&#25142;
    /// </summary>
    [Table("customer")]
    public partial class Customer
    {
        /// <summary>
        /// 唯一碼
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#23458;&#25142;
        /// </summary>
        [Required]
        [Column("name", TypeName = "varchar(50)")]
        public string Name { get; set; }
        /// <summary>
        /// &#20195;&#34399;
        /// </summary>
        [Required]
        [Column("code", TypeName = "varchar(50)")]
        public string Code { get; set; }
        /// <summary>
        /// &#38651;&#35441;
        /// </summary>
        [Column("phone", TypeName = "varchar(50)")]
        public string Phone { get; set; }
        /// <summary>
        /// &#20659;&#30495;
        /// </summary>
        [Column("fax", TypeName = "varchar(50)")]
        public string Fax { get; set; }
        /// <summary>
        /// &#38651;&#23376;&#37109;&#20214;
        /// </summary>
        [Column("email", TypeName = "varchar(50)")]
        public string Email { get; set; }
        /// <summary>
        /// &#22320;&#22336;
        /// </summary>
        [Column("address", TypeName = "varchar(50)")]
        public string Address { get; set; }
        /// <summary>
        /// &#25910;&#27454;&#37504;&#34892;
        /// </summary>
        [Column("bank", TypeName = "varchar(50)")]
        public string Bank { get; set; }
        /// <summary>
        /// &#20998;&#34892;
        /// </summary>
        [Column("branch", TypeName = "varchar(50)")]
        public string Branch { get; set; }
        /// <summary>
        /// &#32113;&#19968;&#32232;&#34399;
        /// </summary>
        [Column("uniform_no", TypeName = "varchar(50)")]
        public string UniformNo { get; set; }
        /// <summary>
        /// &#37504;&#34892;&#24115;&#34399;
        /// </summary>
        [Column("account", TypeName = "varchar(50)")]
        public string Account { get; set; }
        /// <summary>
        /// &#20633;&#35387;
        /// </summary>
        [Column("remarks", TypeName = "varchar(50)")]
        public string Remarks { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }
        /// <summary>
        /// &#21034;&#38500;&#35387;&#35352;
        /// </summary>
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }
    }
}
