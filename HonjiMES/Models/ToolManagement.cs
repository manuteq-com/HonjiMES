using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#20992;&#20855;&#28165;&#21934;
    /// </summary>
    [Table("tool_management")]
    public partial class ToolManagement
    {
        public ToolManagement()
        {
            ToolsetHolders = new HashSet<Toolset>();
            ToolsetTools = new HashSet<Toolset>();
        }

        /// <summary>
        /// 唯一碼
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#21517;&#31281;
        /// </summary>
        [Column("tool_name", TypeName = "varchar(50)")]
        public string ToolName { get; set; }
        /// <summary>
        /// &#20992;&#20855;&#32232;&#34399;
        /// </summary>
        [Required]
        [Column("tool_serialno", TypeName = "varchar(50)")]
        public string ToolSerialno { get; set; }
        /// <summary>
        /// &#35215;&#26684;
        /// </summary>
        [Required]
        [Column("tool_specification", TypeName = "varchar(50)")]
        public string ToolSpecification { get; set; }
        /// <summary>
        /// &#31278;&#39006;
        /// </summary>
        [Column("type", TypeName = "tinyint(4)")]
        public sbyte Type { get; set; }
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

        [InverseProperty(nameof(Toolset.Holder))]
        public virtual ICollection<Toolset> ToolsetHolders { get; set; }
        [InverseProperty(nameof(Toolset.Tool))]
        public virtual ICollection<Toolset> ToolsetTools { get; set; }
    }
}
