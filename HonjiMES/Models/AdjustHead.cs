using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    [Table("adjust_head")]
    public partial class AdjustHead
    {
        public AdjustHead()
        {
            AdjustDetails = new HashSet<AdjustDetail>();
        }

        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#35519;&#25972;&#21934;&#34399;
        /// </summary>
        [Required]
        [Column("adjust_no", TypeName = "varchar(50)")]
        public string AdjustNo { get; set; }
        /// <summary>
        /// &#38364;&#32879;&#21934;&#34399;
        /// </summary>
        [Column("link_order", TypeName = "varchar(50)")]
        public string LinkOrder { get; set; }
        [Column("temp", TypeName = "int(11)")]
        public int? Temp { get; set; }
        /// <summary>
        /// &#29376;&#24907;
        /// </summary>
        [Column("status", TypeName = "int(11)")]
        public int? Status { get; set; }
        /// <summary>
        /// &#20633;&#35387;
        /// </summary>
        [Column("remarks", TypeName = "varchar(50)")]
        public string Remarks { get; set; }
        [Column("delete_flag", TypeName = "int(11)")]
        public int DeleteFlag { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime? UpdateTime { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }

        [InverseProperty(nameof(AdjustDetail.AdjustHead))]
        public virtual ICollection<AdjustDetail> AdjustDetails { get; set; }
    }
}
