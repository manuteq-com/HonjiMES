using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#35069;&#31243;&#20992;&#20855;&#34920;
    /// </summary>
    [Table("toolset")]
    public partial class Toolset
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#35069;&#31243;ID
        /// </summary>
        [Column("process_id", TypeName = "int(11)")]
        public int ProcessId { get; set; }
        /// <summary>
        /// &#20992;&#20855;ID
        /// </summary>
        [Column("tool_id", TypeName = "int(11)")]
        public int? ToolId { get; set; }
        /// <summary>
        /// &#20992;&#26751;ID
        /// </summary>
        [Column("holder_id", TypeName = "int(11)")]
        public int? HolderId { get; set; }
        /// <summary>
        /// &#20633;&#35387;
        /// </summary>
        [Column("remarks", TypeName = "varchar(100)")]
        public string Remarks { get; set; }
        /// <summary>
        /// &#21034;&#38500;&#35387;&#35352;
        /// </summary>
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }

        [ForeignKey(nameof(HolderId))]
        [InverseProperty(nameof(ToolManagement.ToolsetHolders))]
        public virtual ToolManagement Holder { get; set; }
        [ForeignKey(nameof(ProcessId))]
        [InverseProperty("Toolsets")]
        public virtual Process Process { get; set; }
        [ForeignKey(nameof(ToolId))]
        [InverseProperty(nameof(ToolManagement.ToolsetTools))]
        public virtual ToolManagement Tool { get; set; }
    }
}
