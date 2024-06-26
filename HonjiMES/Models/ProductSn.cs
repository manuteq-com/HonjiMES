﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#21697;&#34399;
    /// </summary>
    [Table("product_sn")]
    public partial class ProductSn
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#21697;&#34399;(&#21508;&#24288;&#21830;)
        /// </summary>
        [Required]
        [Column("product_number", TypeName = "varchar(100)")]
        public string ProductNumber { get; set; }
        /// <summary>
        /// &#21697;&#34399;(&#20839;&#37096;)
        /// </summary>
        [Column("product_id", TypeName = "int(11)")]
        public int? ProductId { get; set; }
        /// <summary>
        /// &#24288;&#21830;
        /// </summary>
        [Column("customer_id", TypeName = "int(11)")]
        public int? CustomerId { get; set; }
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
