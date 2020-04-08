using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonjiMES.Models
{
    /// <summary>
    /// 庫存俢改
    /// </summary>
    public class inventorychange
    {
        /// <summary>
        /// 主Key
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 要修改的倉庫類型
        /// </summary>
        public string mod { get; set; }
        /// <summary>
        /// 數量
        /// </summary>
        public int quantity { get; set; }
        /// <summary>
        /// 修改原因
        /// </summary>
        public string msg { get; set; }
    }
}
