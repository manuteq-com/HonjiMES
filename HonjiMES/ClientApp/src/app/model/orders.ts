export class OrderHead {
    id: number;
    orderNo: string;
    customerNo: string;
    orderDate: Date;
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
