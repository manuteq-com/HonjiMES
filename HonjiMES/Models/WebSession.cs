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
        /// <summary>
        /// &#26032;&#22686;&#32773;id
        /// </summary>
        [Column("create_user", TypeName = "int(11)")]
        public int? CreateUser { get; set; }
        /// <summary>
        /// &#26032;&#22686;&#26178;&#38291;
        /// </summary>
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// &#26356;&#26032;&#32773;id
        /// </summary>
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }
        /// <summary>
        /// &#26356;&#26032;&#26178;&#38291;
        /// </summary>
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// &#21034;&#38500;&#35387;&#35352;
        /// </summary>
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }
    }
}
