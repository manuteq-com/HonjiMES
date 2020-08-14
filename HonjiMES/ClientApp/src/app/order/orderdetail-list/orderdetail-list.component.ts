import { Component, OnInit, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import DataSource from 'devextreme/data/data_source';
import ArrayStore from 'devextreme/data/array_store';
import { DxDataGridComponent } from 'devextreme-angular';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from '../../shared/mylib';
import { HttpClient } from '@angular/common/http';
import notify from 'devextreme/ui/notify';
import Swal from 'sweetalert2';
import { APIResponse } from '../../app.module';
import { Observable } from 'rxjs';
import CheckBox from 'devextreme/ui/check_box';
import { PostOrderMaster_Detail } from 'src/app/model/viewmodels';
import { AppComponent } from 'src/app/app.component';
@Component({
    selector: 'app-orderdetail-list',
    templateUrl: './orderdetail-list.component.html',
    styleUrls: ['./orderdetail-list.component.css']
})
export class OrderdetailListComponent implements OnInit {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @Output() childOuter = new EventEmitter();
    @Input() itemkey: number;
    @Input() ProductBasicList: any;
    popupVisiblePurchase: boolean;
    popupVisibleSale: boolean;
    topurchasekey: any;
    tosalekey: any;
    mod: string;
    dataSourceDB: any;
    controller: string;
    allMode: string;
    checkBoxesMode: string;
    disabledValues: number[];
    dataSource: any;
    totalcount: any;

    constructor(private http: HttpClient, public app: AppComponent) {
        this.popupVisiblePurchase = false;
        this.popupVisibleSale = false;
        this.disabledValues = [];
        this.onCellPrepared = this.onCellPrepared.bind(this);
        this.onSelectionChanged = this.onSelectionChanged.bind(this);
        this.allMode = 'allPages';
        this.checkBoxesMode = 'always'; // 'onClick';
        this.controller = '/OrderDetails';
        this.totalcount = 0;
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: () => SendService.sendRequest(http, this.controller + '/GetOrderDetailsByOrderId?OrderId=' + this.itemkey),
            byKey: () => SendService.sendRequest(http, this.controller + '/GetOrderDetail'),
            insert: (values) => SendService.sendRequest(http, this.controller + '/PostOrderDetail?PID=' + this.itemkey, 'POST', { values }),
            update: (key, values) => SendService.sendRequest(http, this.controller + '/PutOrderDetail', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(http, this.controller + '/DeleteOrderDetail/' + key, 'DELETE')
        });
    }
    ngOnInit() {
    }
    async to_workClick(e) {
        this.topurchasekey = null;
        this.topurchasekey = this.dataGrid.instance.getSelectedRowsData();
        if (this.topurchasekey.length === 0) {
            Swal.fire({
                allowEnterKey: false,
                allowOutsideClick: false,
                title: '沒有勾選任何訂單項目',
                html: '請勾選要轉工單的訂單項目',
                icon: 'warning',
                timer: 3000
            });
        } else {
            try {
                let OrderData = new PostOrderMaster_Detail();
                OrderData.orderDetail = this.topurchasekey;
                // tslint:disable-next-line: max-line-length
                const sendRequest = await SendService.sendRequest(this.http, '/WorkOrders/OrderToWorkOrder', 'POST', { values: OrderData });
                if (sendRequest.success) {
                    if (sendRequest.message === '') {
                        notify({
                            message: '工單建立完成',
                            position: {
                                my: 'center top',
                                at: 'center top'
                            }
                        }, 'success', 3000);
                    } else {
                        notify({
                            message: sendRequest.message,
                            position: {
                                my: 'center top',
                                at: 'center top'
                            }
                        }, 'warning', 6000);
                    }
                }
                this.dataGrid.instance.refresh();
            } catch (error) {

            }
        }
    }
    to_purchaseClick(e) {
        this.topurchasekey = null;
        this.topurchasekey = this.dataGrid.instance.getSelectedRowsData();

        this.app.GetData('/Inventory/GetBasicsData').subscribe(
            (s) => {
                if (s.success) {
                    this.GetBomData(s.data);
                }
            }
        );

        if (this.topurchasekey.length === 0) {
            Swal.fire({
                allowEnterKey: false,
                allowOutsideClick: false,
                title: '沒有勾選任何訂單項目',
                html: '請勾選要轉採購的訂單項目',
                icon: 'warning',
                timer: 3000
            });
        } else {
            Swal.fire({
                showCloseButton: true,
                allowEnterKey: false,
                allowOutsideClick: false,
                title: '轉採購',
                html: '如需合併採購單，請點選[輸入採購單]!',
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#71c016',
                cancelButtonText: '輸入採購單',
                confirmButtonText: '新建採購單'
            }).then(async (result) => {
                if (result.value) {
                    this.mod = 'add';
                    this.popupVisiblePurchase = true;
                } else if (result.dismiss === Swal.DismissReason.cancel) {
                    this.mod = 'merge';
                    this.popupVisiblePurchase = true;
                } else if (result.dismiss === Swal.DismissReason.close) {
                    this.popupVisiblePurchase = false;
                }
            });
        }
    }
    to_saleClick(e) {
        this.tosalekey = null;
        this.tosalekey = this.dataGrid.instance.getSelectedRowsData();
        if (this.tosalekey.length === 0) {
            Swal.fire({
                allowEnterKey: false,
                allowOutsideClick: false,
                title: '沒有勾選任何訂單項目',
                html: '請勾選要轉銷貨的訂單項目',
                icon: 'warning',
                timer: 3000
            });
        } else {
            Swal.fire({
                showCloseButton: true,
                allowEnterKey: false,
                allowOutsideClick: false,
                title: '轉銷貨',
                html: '如需合併銷貨單，請點選[輸入銷貨單]!',
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#71c016',
                cancelButtonText: '輸入銷貨單',
                confirmButtonText: '新建銷貨單'
            }).then(async (result) => {
                if (result.value) {
                    this.mod = 'add';
                    this.popupVisibleSale = true;
                } else if (result.dismiss === Swal.DismissReason.cancel) {
                    this.mod = 'merge';
                    this.popupVisibleSale = true;
                } else if (result.dismiss === Swal.DismissReason.close) {
                    this.popupVisibleSale = false;
                }
            });
        }
    }
    GetBomData(BasicData) {
        let indexVal = 0;
        let serial = 1;
        const tempdataSource = [];
        this.topurchasekey.forEach(x => {
            // 取得BOM原料清單
            this.app.GetData('/BillOfMaterials/GetBomlist/' + x.ProductBasicId).subscribe(
                (s) => {
                    if (s.success) {
                        let productId = 1;
                        let productQuantity = 1;
                        s.data.forEach(element => {
                            if (element.MaterialBasicId == null) {
                                if (element.Pid === productId) {
                                    productId = element.Id;
                                    productQuantity = productQuantity * element.Quantity;
                                } else {
                                    productId = element.Id;
                                    productQuantity = element.Quantity;
                                }
                            }
                            let tempQuantity = element.Quantity;
                            if (element.Pid !== 0 && element.Pid === productId) {
                                tempQuantity = element.Quantity * productQuantity;
                            }

                            const index = tempdataSource.findIndex(z => z.DataType === 1 && z.DataId === element.MaterialBasicId);
                            if (~index) {
                                tempdataSource[index].Quantity += x.Quantity * tempQuantity;
                                tempdataSource[index].Price += (x.Quantity * tempQuantity) * element.MaterialPrice;
                            } else if (element.MaterialBasicId != null) { // 只取得料號為[原料]的項目
                                const result = BasicData.find(x => x.DataType === 1 && x.DataId === element.MaterialBasicId);
                                if (result) {
                                    tempdataSource.push({
                                        Serial: serial,
                                        TempId: result.TempId,
                                        DataType: 1,
                                        DataId: element.MaterialBasicId,
                                        WarehouseId: null,
                                        Quantity: x.Quantity * tempQuantity,
                                        OriginPrice: element.MaterialPrice,
                                        Price: (x.Quantity * tempQuantity) * element.MaterialPrice,
                                        DeliveryTime: new Date()
                                    });
                                }
                            }
                        });
                        serial++;
                    }
                    if (indexVal === this.topurchasekey.length - 1) {
                        this.dataSource = [];
                        // 檢查庫存量，過濾需要採購項目
                        this.dataSource = this.CheckStockQuantity(tempdataSource, BasicData);
                    }
                    indexVal++;
                }
            );
        });
    }
    CheckStockQuantity(data, BasicData) {
        const resultArray = [];
        data.forEach(element => {
            const result = BasicData.find(x => x.TempId === element.TempId);
            if (result.Quantity < element.Quantity) {
                element.Quantity = element.Quantity - result.Quantity;
                element.Price = element.Quantity * element.OriginPrice;
                resultArray.push(element);
            }
        });
        return resultArray;
    }
    rowClick(e) {
        debugger;
        this.app.GetData('/OrderDetails/GetStockCountByProductBasicId/' + e.key).subscribe(
            (s) => {
                if (s.success) {
                    this.totalcount = s.data.TotalCount;
                } else {
                    this.totalcount = s.message;
                }
            }
        );
    }
    cellClick(e) {
        if (e.rowType === 'header') {
            if (e.column.type === 'buttons') {
                if (e.column.cssClass === 'addmod') {
                    // tslint:disable-next-line: deprecation
                    this.dataGrid.instance.insertRow();
                }

            }
        }
    }
    onCellPrepared(e: any) {
        if (e.rowType === 'data' && e.column.command === 'select') {
            if (e.data.Quantity === e.data.SaleCount) {
                const instance = CheckBox.getInstance(e.cellElement.querySelector('.dx-select-checkbox'));
                instance.option('disabled', true);
                this.disabledValues.push(e.data.Id);
            }
        }
    }
    onSelectionChanged(e) {// CheckBox disabled還是會勾選，必須清掉，這是官方寫法
        const disabledKeys = e.currentSelectedRowKeys.filter(i => this.disabledValues.indexOf(i) > -1);
        if (disabledKeys.length > 0) {
            e.component.deselectRows(disabledKeys);
        }
    }
    popup_result(e) {
        this.popupVisiblePurchase = false;
        this.popupVisibleSale = false;
        this.childOuter.emit(true);
        this.dataGrid.instance.refresh();
        this.dataGrid.instance.clearSelection();
        notify({
            message: '存檔完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
    }
    distinct(value) {
        const list = value.filter(
            (v, i, arr) => arr.findIndex(t => t.SaleId === v.SaleId) === i
        );
        return list;
    }
    // getid(value) {
    //     debugger;
    //     <span></span>
    //     value.array.forEach(element => {

    //     });
    //     return
    // }

    customizeText(e) {
        return '總數：' + e.value + '筆';
    }
}
