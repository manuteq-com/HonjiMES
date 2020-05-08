export class OrderHead {
    id: number;
    orderNo: string;
    customerNo: string;
    orderDate: Date;
    replyDate: Date;
    startDate: Date;
    finishDate: Date;
    customer: number;
    createDate: Date;
    createUser: number;
}
export class OrderDetail {
    id: number;
    orderId: number;
    productId: number;
    serial: number;
    originPrice: number;
    price: number;
    unit: string;
    quantity: number;
    dueDate: Date;
    replyDate: Date;
    machineNo: number;
    remark: string;
    replyRemark: string;
    createDate: Date;
}
// tslint:disable-next-line: class-name
export class PostOrderMaster_Detail {
    orderHead: OrderHead;
    orderDetail: OrderDetail[];
}

export class InventoryChange {
    id: number;
    mod: string;
    quantity: number;
    reason: string;
    message: string;
}

export class Material {
    id: number;
    materialNo: string;
    name: string;
    quantity: number;
    specification: string;
    property: string;
    supplier: number;
    subInventory: string;
}
export class Product {
    id: number;
    productNo: string;
    productNumber: string;
    name: string;
    quantity: number;
    quantityLimit: number;
    specification: string;
    property: string;
    price: number;
    materialId: number;
    materialRequire: number;
    subInventory: string;
    deleteFlag: number;
    createTime: Date;
    createUser: number;
    updateUser: number;
    remarks: string;
}
export class Customer {
    id: number;
    name: string;
    code: string;
    phone: string;
    fax: string;
    email: string;
    address: string;
    remarks: string;
    deleteFlag: number;
    createTime: Date;
    createUser: number;
    updateTime: Date;
    updateUser: number;
}
export class Supplier {
    id: number;
    name: string;
    code: string;
    phone: string;
    fax: string;
    email: string;
    address: string;
    uniformNo: string;
    remarks: string;
    deleteFlag: number;
    createTime: Date;
    createUser: number;
    updateTime: Date;
    updateUser: number;
}
export class Warehouse {
    id: number;
    name: string;
    code: string;
    phone: string;
    fax: string;
    email: string;
    address: string;
    remarks: string;
    deleteFlag: number;
    createTime: Date;
    createUser: number;
    updateTime: Date;
    updateUser: number;
}
export class POrderSale {
    SaleID: number;
    SaleDID: number;
  }
export class ReorderSale {
    key: number;
    qty: number;
  }
export class Selectitem {
    Id: number;
    Name: string;
}

export class BillofPurchaseDetail {
    Id: number;
    SupplierId: number;
    DataId: number;
    Quantity: number;
    OriginPrice: number;
    Price: number;
}
