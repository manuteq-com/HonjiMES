using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    [Table("mbom_model_head")]
    public partial class MbomModelHead
    {
        public MbomModelHead()
        {
            MbomModelDetails = new HashSet<MbomModelDetail>();
        }

        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#20195;&#34399;
        /// </summary>
        [Required]
        [Column("model_code", TypeName = "varchar(50)")]
        public string ModelCode { get; set; }
        /// <summary>
        /// &#21517;&#31281;
        /// </summary>
        [Required]
        [Column("model_name", TypeName = "varchar(50)")]
        public string ModelName { get; set; }
        /// <summary>
        /// &#20633;&#35387;
        /// </summary>
        [Column("model_remarks", TypeName = "varchar(50)")]
        public string ModelRemarks { get; set; }
        [Column("delete_flag", TypeName = "int(11)")]
        public int DeleteFlag { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }

        [InverseProperty(nameof(MbomModelDetail.MbomModelHead))]
        public virtual ICollection<MbomModelDetail> MbomModelDetails { get; set; }
    }
}
