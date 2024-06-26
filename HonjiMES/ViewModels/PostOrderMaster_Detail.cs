﻿using System;
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

    public class PurchaseDetailData : PurchaseDetail
    {        
        public int WarehouseIdA { get; set; }
        public int WarehouseIdB { get; set; }
    }
    

    public class PostPurchaseMaster_Detail
    {
        public PurchaseHead PurchaseHead { get; set; }
        public List<PurchaseDetailData> PurchaseDetails { get; set; }
    }
    
    public class PostBillofPurchaseHead_Detail
    {
        public BillofPurchaseHead BillofPurchaseHead { get; set; }
        public List<BillofPurchaseDetail> BillofPurchaseDetail { get; set; }
    }
    public class PostBillofPurchaseBySupplier
    {
        public int? PurchaseHeadType { get; set; }
        public List<int> PurchaseHeadIdArray { get; set; }
    }
}
