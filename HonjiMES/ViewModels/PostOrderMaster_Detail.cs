using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonjiMES.Models
{
    public class PostOrderMaster_Detail
    {
        public OrderHead OrderHead { get; set; }
        public List<OrderDetail> OrderDetail { get; set; }
    }

    public class PostPurchaseMaster_Detail
    {
        public PurchaseHead PurchaseHead { get; set; }
        public List<PurchaseDetail> PurchaseDetails { get; set; }
    }
}
