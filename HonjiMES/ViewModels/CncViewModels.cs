using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HonjiMES.CncModels;

namespace HonjiMES.ViewModels
{
    /// <summary>
    /// 加工記錄
    /// </summary>
    public class CncLogVM
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///  加工程式註解
        /// </summary>
        public NcFileInformation FileInf { get; set; }
        /// <summary>
        /// 開始時間
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 結束時間
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 加工數量
        /// </summary>
        public int? CompletedNumber { get; set; }
        /// <summary>
        /// 加工程式碼
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 加工備註
        /// </summary>
        public string Comment { get; set; }
    }
}
