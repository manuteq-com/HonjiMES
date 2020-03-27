export class Orders {
    id: number;
    project_no: string;
    customer_order_no: string | null;
    product_no: number;
    name: string;
    specification: string;
    quantity: number;
    price: number;
    machine_id: string;
    order_delivery_date: Date | null;
    status: string | null;
    customer: number;
    create_user: number;
    create_time: Date;
    update_time: Date;
}

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
