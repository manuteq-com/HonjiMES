export const navigation = [
  {
    text: 'Home',
    path: '',
    icon: 'home'
  },
  {
    text: '訂單管理',
    icon: 'folder',
    items: [
      {
        text: '客戶訂單',
        path: '/orderlist'
      },
      {
        text: '採購單',
        path: '/purchaseorder'
      },
      {
        text: '進貨單',
        path: '/billpurchase'
      },
      {
        text: '銷貨單',
        path: '/salelist'
      },
      {
        text: '表面處理',
        path: '/surfacetreat'
      }
    ]
  },
  {
    text: '庫存管理',
    icon: 'folder',
    items: [
      {
        text: '品號資料',
        path: '/materialbasiclist'
      },
      {
        text: '調整單管理',
        path: '/adjustlist'
      },
      {
        text: '調整單紀錄',
        path: '/adjustlog'
      },
      {
        text: '庫存變動紀錄',
        path: '/inventorylog'
      }
    ]
  },{
    text: '組成管理',
    icon: 'folder',
    items: [
      {
        text: '物料清單管理',
        path: '/billofmateriallist'
      },
      {
        text: '標準製程管理',
        path: '/mbillofmateriallist'
      },
      {
        text: '快速建BOM',
        path: '/createbom'
      }
    ]
  },
  {
    text: '生產管理',
    icon: 'folder',
    items: [
      {
        text: '機台看板',
        path: '/machineorder'
      },
      {
        text: '人員管理',
        path: '/staffmanagement'
      },
      {
        text: '機台保養',
        path: '/maintenance'
      },
      {
        text: '總工時統計',
        path: '/worktimesummary'
      },
      {
        text: '製程基本資料',
        path: '/processlist'
      },
      {
        text: '工單管理',
        path: '/processcontrol'
      },
      {
        text: '領料資料',
        path: '/receiveList'
      },
      {
        text: '退料資料',
        path: '/rebackList'
      },
      {
        text: '生產看板',
        path: '/workorderlist'
      },
      {
        text: '工單入庫確認',
        path: '/workorderstock'
      },
      {
        text: '報工紀錄查詢',
        path: '/workorderlog'
      },
      {
        text: '生產行事曆',
        path: '/workscheduler'
      },
      {
        text: '工單交期預估',
        path: '/workestimate'
      },
      {
        text: '機台資源分配',
        path: '/resourceallocation'
      }
    ]
  },
  {
    text: '基本資訊管理',
    icon: 'folder',
    items: [
      {
        text: '客戶資料',
        path: '/customerlist'
      },
      {
        text: '供應商資料',
        path: '/supplierlist'
      },
      {
        text: '倉庫資訊管理',
        path: '/warehouselist'
      },
      {
        text: '使用者帳戶管理',
        path: '/userlist'
      },
      {
        text: '更改密碼',
        path: '/userpassword'
      }
    ]
  },
  {
    text: '報表管理',
    icon: 'folder',
    items: [
      {
        text: '銷貨退回紀錄查詢',
        path: '/salereturn'
      },
      {
        text: '進貨驗退紀錄查詢',
        path: '/billpurchasereturn'
      },
      {
        text: '品質記錄查詢',
        path: '/qualityrecord'
      },
      {
        text: '交易單價紀錄',
        path: '/dealprice'
      },
      {
        text: '採購單統計查詢',
        path: '/purchasetotal'
      },
      {
        text: '廠商交易紀錄',
        path: '/dealsupplier'
      }
    ]
  }
];
