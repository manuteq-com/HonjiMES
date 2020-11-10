import { Injectable } from '@angular/core';
import { Selectitem, WorkSchedulerStatu } from '../model/viewmodels';
@Injectable({
    providedIn: 'root'
})
export class Myservice {
    getpurchasetypes(): Selectitem[] {
        return purchasetypes;
    }
    getPermission(): Selectitem[] {
        return permissiontypes;
    }
    getDepartment(): Selectitem[] {
        return departmenttypes;
    }
    getOrderStatus(): Selectitem[] {
        return OrderStatus;
    }
    getOrderType(): Selectitem[] {
        return OrderType;
    }
    getOrderTypeShow(): Selectitem[] {
        return OrderTypeShow;
    }
    getPurchaseOrderStatus(): Selectitem[] {
        return purchaseOrderSatatus;
    }
    getSaleOrderHeadStatus(): Selectitem[] {
        return saleOrderHeadStatus;
    }
    getSaleOrderDetailStatus(): Selectitem[] {
        return saleOrderDetailtSatus;
    }
    getBillofPurchaseOrderStatus(): Selectitem[] {
        return billofpurchaseOrderStatus;
    }
    getComponent(): Selectitem[] {
        return componenttypes;
    }
    getlistAdjustStatus(): Selectitem[] {
        return adjusttypes;
    }
    getlistMaterialType(): Selectitem[] {
        return materialtypes;
    }
    getWorkOrderStatus(): Selectitem[] {
        return workorderstatus;
    }
    getOrderToWorkOrderStatus(): Selectitem[] {
        return orderToWorkOrderStatus;
    }
    getWorkOrderTypes(): Selectitem[] {
        return workordertype;
    }
    getReportType(): Selectitem[] {
        return reporttype;
    }
    getWorkSchedulerStatus(): WorkSchedulerStatu[] {
        return WorkSchedulerStatus;
    }
    getTimeType(): Selectitem[] {
        return timetype;
    }
    getResourceWorkOrderStatus(): Selectitem[] {
        return resourceworkorderstatus;
    }
    getMasterType(): Selectitem[] {
        return mastertype;
    }
    getStockType(): Selectitem[] {
        return stocktype;
    }
    getQcType(): Selectitem[] {
        return qctype;
    }
    getQcResult(): Selectitem[] {
        return qcresult;
    }
    getProcessType(): Selectitem[] {
        return processtype;
    }
    getResponsibilityType(): Selectitem[] {
        return responsibilitytype;
    }
    getInventoryLogType(): Selectitem[] {
        return inventorylogtype;
    }
}
const purchasetypes: Selectitem[] = [
    { Id: 10, Name: '採購' },
    { Id: 20, Name: '外包' },
    { Id: 30, Name: '表處' },
    { Id: 40, Name: '傳統銑床' }
];
const permissiontypes: Selectitem[] = [
    { Id: 1, Name: '系統管理員' },
    { Id: 2, Name: '生產共用帳戶' },
    { Id: 3, Name: '品管共用帳戶' },
    { Id: 20, Name: '主管' },
    { Id: 30, Name: '一般人員' },
    { Id: 40, Name: '採購人員' },
    { Id: 50, Name: '設計人員' },
    { Id: 60, Name: '生管人員' },
    { Id: 70, Name: '品管人員' },
    { Id: 80, Name: '生產人員' }
];
const departmenttypes: Selectitem[] = [
    { Id: 1, Name: '預設' }
];
const OrderStatus: Selectitem[] = [
    { Id: 0, Name: '未完成' },
    { Id: 1, Name: '完成銷貨' },
    { Id: 2, Name: '完成採購' },
    { Id: 10, Name: '結案' }
];
const OrderType: Selectitem[] = [
    { Id: 0, Name: '一般訂單' },
    { Id: 10, Name: '重製訂單' }
];
const OrderTypeShow: Selectitem[] = [
    { Id: 0, Name: '一般' },
    { Id: 10, Name: '重製' }
];
const purchaseOrderSatatus: Selectitem[] = [
    { Id: 0, Name: '新建' },
    { Id: 1, Name: '完成採購' },
    { Id: 2, Name: '未完成' },
];
const saleOrderHeadStatus: Selectitem[] = [
    { Id: 0, Name: '新建' },
    { Id: 1, Name: '未完成' },
    { Id: 2, Name: '結案' }
];
const saleOrderDetailtSatus: Selectitem[] = [
    { Id: 0, Name: '' },
    { Id: 1, Name: '已銷貨' },
];
const billofpurchaseOrderStatus: Selectitem[] = [
    { Id: 0, Name: '新建' },
    { Id: 1, Name: '結案' },
    { Id: 2, Name: '未完成' },
];
const componenttypes: Selectitem[] = [
    { Id: 1, Name: '原料' },
    { Id: 2, Name: '成品' },
];
const adjusttypes: Selectitem[] = [
    { Id: 1, Name: '原料' },
    { Id: 2, Name: '成品' },
    { Id: 3, Name: '半成品' },
];
const materialtypes: Selectitem[] = [
    { Id: null, Name: '' },
    { Id: 1, Name: '原料' },
    { Id: 2, Name: '成品' }
];
const workorderstatus: Selectitem[] = [ // 訂單[主檔]&[明細]共用狀態
    { Id: 0, Name: '新建' },
    { Id: 1, Name: '已派工' },
    { Id: 2, Name: '已開工' },
    { Id: 3, Name: '完工' },
    { Id: 4, Name: '轉單' },
    { Id: 5, Name: '結案' },
    { Id: 6, Name: '超時完工' }, // 超時完工
    { Id: 7, Name: '工序暫停' },
];
const reporttype: Selectitem[] = [
    { Id: 1, Name: '開工回報' },
    { Id: 2, Name: '完工回報' },
    { Id: 3, Name: '再開工回報' },
    { Id: 4, Name: '工序暫停' },
    { Id: 5, Name: '回復加工' },
];
const resourceworkorderstatus: Selectitem[] = [
    { Id: 0, Name: '全部資料' },
    { Id: 1, Name: '派工' },
    { Id: 5, Name: '結案' },
];
const orderToWorkOrderStatus: Selectitem[] = [
    { Id: 0, Name: '' },
    { Id: 1, Name: '工單已建立' },
    { Id: 2, Name: '無MBOM資訊' }
];
const workordertype: Selectitem[] = [
    { Id: 0, Name: '' },
    { Id: 1, Name: '委外' },
    // { Id: 2, Name: '委外(工)' }
];
const mastertype: Selectitem[] = [
    { Id: 0, Name: '' },
    { Id: 1, Name: '主件' },
];
const WorkSchedulerStatus: WorkSchedulerStatu[] = [
    { text: '新建', id: 0, color: '#929292' },
    { text: '派工', id: 1, color: '#FFC300' },
    { text: '開工', id: 2, color: '#FFC300' },
    { text: '完工', id: 3, color: '#0B9500' },
    { text: '轉單', id: 4, color: '#929292' }
];
const timetype: Selectitem[] = [
    { Id: 1, Name: '前置時間' },
    { Id: 2, Name: '標準工時' },
    { Id: 3, Name: '總時間' },
];
const stocktype: Selectitem[] = [
    { Id: 0, Name: '半成品入庫' },
    { Id: 1, Name: '成品入庫' },
];
const qctype: Selectitem[] = [
    { Id: 1, Name: '生產品質檢驗' },
    { Id: 2, Name: '表面處理品質檢驗' }
];
const qcresult: Selectitem[] = [
    { Id: 0, Name: '合格' },
    { Id: 1, Name: '不合格' },
];
const processtype: Selectitem[] = [
    { Id: null, Name: '' },
    { Id: 10, Name: 'NC加工' },
    { Id: 20, Name: 'QC檢驗' },
    { Id: 100, Name: '其他' },
];
const responsibilitytype: Selectitem[] = [
    { Id: 0, Name: '其他' },
    { Id: 1, Name: '廠商' },
];
const inventorylogtype: Selectitem[] = [ // id數值沒有意義，資料庫是使用Name過濾
    { Id: 9, Name: '[進貨驗收]入庫' },
    { Id: 10, Name: '[進貨驗退]出庫' },
    { Id: 11, Name: '[銷貨]出庫' },
    { Id: 12, Name: '[銷退]入庫' },
    { Id: 8, Name: '[轉倉]表處單' },
    { Id: 4, Name: '[工單領料]出庫' },
    { Id: 3, Name: '[工單退料]入庫' },
    { Id: 1, Name: '[入庫單]半成品入庫' },
    { Id: 2, Name: '[入庫單]成品入庫' },
    { Id: 7, Name: '[新增品號]' },
    { Id: 5, Name: '[庫存調整]' },
    { Id: 6, Name: '[庫存調整單]' },
];
