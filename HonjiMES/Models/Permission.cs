using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    [Table("permission")]
    public partial class Permission
    {
        /// <summary>
        /// 唯一碼
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#26032;&#22686;
        /// </summary>
        [Column("new", TypeName = "int(11)")]
        public int New { get; set; }
        /// <summary>
        /// &#20462;&#25913;
        /// </summary>
        [Column("edit", TypeName = "int(11)")]
        public int Edit { get; set; }
        /// <summary>
        /// &#21034;&#38500;
        /// </summary>
        [Column("del", TypeName = "int(11)")]
        public int Del { get; set; }
        /// <summary>
        /// &#25628;&#23563;
        /// </summary>
        [Column("search", TypeName = "int(11)")]
        public int Search { get; set; }
        /// <summary>
        /// &#25490;&#24207;
        /// </summary>
        [Column("sort", TypeName = "int(11)")]
        public int Sort { get; set; }
        /// <summary>
        /// &#21295;&#20837;
        /// </summary>
        [Column("import", TypeName = "int(11)")]
        public int Import { get; set; }
        /// <summary>
        /// &#21295;&#20986;
        /// </summary>
        [Column("export", TypeName = "int(11)")]
        public int Export { get; set; }
        /// <summary>
        /// &#35079;&#35069;&#26032;&#22686;
        /// </summary>
        [Column("copy_add", TypeName = "int(11)")]
        public int CopyAdd { get; set; }
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
