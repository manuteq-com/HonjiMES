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
        text: '廠商管理',
        path: '/customerlist'
      }
    ]
  },{
    text: '組成管理',
    icon: 'folder',
    items: [
      {
        text: '標準製程管理',
        path: '/mbillofmateriallist'
      },
      {
        text: '製程基本資料',
        path: '/processlist'
      }
    ]
  },
  {
    text: '生產管理',
    icon: 'folder',
    items: [
      {
        text: '工單管理',
        path: '/processcontrol'
      },
      {
        text: '生產看板',
        path: '/workorderlist'
      },
      {
        text: '機台看板',
        path: '/machineorder'
      },
      {
        text: '機台保養',
        path: '/maintenance'
      },
      {
        text: '報工記錄查詢',
        path: '/workorderlog'
      }
    ]
  },
  {
    text: '系統管理',
    icon: 'folder',
    items: [
      {
        text: '假日表',
        path: '/holiday'
      },
      {
        text: '成員管理',
        path: '/userlist'
      },
      {
        text: '更改密碼',
        path: '/userpassword'
      },
      {
        text: '機台管理',
        path: '/machineinfo'
      }
    ]
  }
];
