export class CreateNumberInfo {
    CreateNumber: string;
    CreateTime: Date;
}
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
export class WorkOrderHead {
    Id: number;
    WorkOrderNo: string;
    OrderDetailId: number;
    MachineNo: string;
    DataType: number;
    DataId: number;
    DataNo: string;
    DataName: string;
    Count: number;
    ReCount: number;
    Status: number;
    TotalTime: number;
    DispatchTime: Date;
    DueStartTime: Date;
    DueEndTime: Date;
    ActualStartTime: Date;
    ActualEndTime: Date;
}
// tslint:disable-next-line: class-name
export class PostOrderMaster_Detail {
    orderHead: OrderHead;
    orderDetail: OrderDetail[];
    workOrderHead: WorkOrderHead[];
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
export class User {
    id: number;
    username: string;
    realname: string;
    permission: number;
    department: number;
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
export class ToorderSale {
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

// tslint:disable-next-line: class-name
export class workOrderReportData {
    WorkOrderID: number;
    WorkOrderSerial: number;
    ReCount: number;
    RePrice: number;
    Remarks: string;
    ProducingMachine: string;
    PurchaseId: number;
    PurchaseNo: string;
    SupplierId: number;
    WarehouseId: number;
    CreateUser: number;
    CodeNo: string;
}

// tslint:disable-next-line: class-name
export class requisitionsDetailInfo {
    RequisitionId: number;
    WarehouseId: number;
}

// tslint:disable-next-line: class-name
export class basicData {
    TempId: number;
    DataType: number;
    DataId: number;
    DataNo: string;
    Name: string;
    Specification: string;
    Property: string;
    Price: number;
    WarehouseId: number;
}

export class MbomModelData {
    MbomModelHeadId: number;
    ModelCode: string;
    ModelName: string;
}

// tslint:disable-next-line: class-name
export class mBillOfMaterial {
    SerialNumber: number;
    ProcessId: number;
    ProcessNo: string;
    ProcessName: string;
    ProcessLeadTime: number;
    ProcessTime: number;
    ProcessCost: number;
    DrawNo: string;
    Manpower: string;
    ProducingMachine: string;
    Status: number;
    Type: string;
    Remarks: string;
}
export class WorkSchedulerStatu {
    text: string;
    id: number;
    color: string;
}
