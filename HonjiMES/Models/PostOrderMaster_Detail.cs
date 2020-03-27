using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonjiMES.Models
{
    public class PostOrderMaster_Detail
    {
        public OrderHead OrderHead { get; set; }
        public  List<OrderDetail> OrderDetail { get; set; }
    }
}
