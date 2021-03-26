import { Component, OnInit, OnChanges, ViewChild, Input, EventEmitter, Output } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AppComponent } from 'src/app/app.component';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from 'src/app/shared/mylib';
import notify from 'devextreme/ui/notify';
import { DxDataGridComponent } from 'devextreme-angular';
import { Myservice } from 'src/app/service/myservice';
import Swal from 'sweetalert2';
import { PostOrderMaster_Detail } from 'src/app/model/viewmodels';
import CheckBox from 'devextreme/ui/check_box';

@Component({
  selector: 'app-sufacetowork',
  templateUrl: './sufacetowork.component.html',
  styleUrls: ['./sufacetowork.component.css']
})
export class SufacetoworkComponent implements OnInit, OnChanges {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() Oitemkeyval: any;
    autoNavigateToFocusedRow = true;
    remoteOperations: boolean;
    dataSourceDB: any;
    visible: boolean;
    listStatus: any;
    DataTypeList: any;
    modName: string;
    disabledValues: any;
    DueEndTime: any;
    EditorOptions: { showSpinButtons: boolean; mode: string; format: string; value: number; min: number; };

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent) {
        this.remoteOperations = true;
        this.listStatus = myservice.getOrderToWorkOrderStatus();
        this.DataTypeList = myservice.getlistAdjustStatus();
        this.dataSourceDB = [];
        this.disabledValues = [];
        this.DueEndTime = new Date();
        this.EditorOptions = {
            showSpinButtons: true,
            mode: 'number',
            format: '#0.0',
            value: 0,
            min: 1
        };
    }
    ngOnInit() {
        this.dataSourceDB = [{
			"StockCount": "無庫存",
			"OrderCount": 0.0,
			"Total": null,
			"Id": 0,
			"WorkOrderNo": "HJ210326001",
			"OrderDetailId": 166,
			"MachineNo": "616516",
			"DataType": 1,
			"DataId": 62,
			"DataNo": "6061T651 25.4*25.4*300",
			"DataName": "6061T651 25.4*25.4*300",
			"Count": 10,
			"ReCount": null,
			"Status": 0,
			"TotalTime": null,
			"DispatchTime": null,
			"DueStartTime": null,
			"DueEndTime": null,
			"ActualStartTime": null,
			"ActualEndTime": null,
			"DeleteFlag": 0,
			"CreateTime": "0001-01-01T00:00:00",
			"CreateUser": 1,
			"UpdateTime": "0001-01-01T00:00:00",
			"UpdateUser": null,
			"OrderDetail": null,
			"OrderDetailAndWorkOrderHeads": [],
			"Requisitions": [],
			"StaffManagements": [],
			"WorkOrderDetails": [],
			"WorkOrderQcLogs": []
		},
        {
			"StockCount": "無庫存",
			"OrderCount": 0.0,
			"Total": null,
			"Id": 1,
			"WorkOrderNo": "HJ210326001",
			"OrderDetailId": 166,
			"MachineNo": "616516",
			"DataType": 1,
			"DataId": 62,
			"DataNo": "6061T651 25.4*25.4*300",
			"DataName": "6061T651 25.4*25.4*300",
			"Count": 10,
			"ReCount": null,
			"Status": 0,
			"TotalTime": null,
			"DispatchTime": null,
			"DueStartTime": null,
			"DueEndTime": null,
			"ActualStartTime": null,
			"ActualEndTime": null,
			"DeleteFlag": 0,
			"CreateTime": "0001-01-01T00:00:00",
			"CreateUser": 1,
			"UpdateTime": "0001-01-01T00:00:00",
			"UpdateUser": null,
			"OrderDetail": null,
			"OrderDetailAndWorkOrderHeads": [],
			"Requisitions": [],
			"StaffManagements": [],
			"WorkOrderDetails": [],
			"WorkOrderQcLogs": []
		}
        ]
    }
    ngOnChanges() {
        this.disabledValues = [];
        if (this.itemkeyval !== null && this.itemkeyval !== undefined) {
            this.itemkeyval.forEach((element, index) => {
                element.Id = index + 1;
                element.OrderCount = element.Count;
                element.DueEndTime = new Date();
            });
        }
        //this.dataSourceDB = this.itemkeyval;

    }
    onFocusedRowChanging(e) {
        const rowsCount = e.component.getVisibleRows().length;
        const pageCount = e.component.pageCount();
        const pageIndex = e.component.pageIndex();
        const key = e.event && e.event.key;

        if (key && e.prevRowIndex === e.newRowIndex) {
            if (e.newRowIndex === rowsCount - 1 && pageIndex < pageCount - 1) {
                // tslint:disable-next-line: only-arrow-functions
                e.component.pageIndex(pageIndex + 1).done(function () {
                    e.component.option('focusedRowIndex', 0);
                });
            } else if (e.newRowIndex === 0 && pageIndex > 0) {
                // tslint:disable-next-line: only-arrow-functions
                e.component.pageIndex(pageIndex - 1).done(function () {
                    e.component.option('focusedRowIndex', rowsCount - 1);
                });
            }
        }
    }
    onRowPrepared(e) {
        if (e.rowType === 'data') {
        }
    }
    cellClick(e) {
        if (e.rowType === 'header') {
            if (e.column.type === 'buttons') {
                if (e.column.cssClass === 'addmod') {
                }
            }
        }
    }
    onDataErrorOccurred(e) {
        notify({
            message: e.error.message,
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'error', 3000);
    }
    onEditingStart(e) {
    }
    onFocusedRowChanged(e) {
    }
    onRowClick(e) {
        // const arr =  this.dataGrid.instance.getSelectedRowsData();
        // if (e.isSelected) {
        //     const index = arr.indexOf(e.data, 0);
        //     if (index > -1) {
        //         arr.splice(index, 1);
        //     }
        // } else {
        //     arr.push(e.data);
        // }
        // e.component.selectRows(arr);
    }
    onCellPrepared(e: any) {
        if (e.rowType === 'data' && e.column.command === 'select') {
            if (e.data.Status !== 0) {
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
    onEditorPreparing(e) {
    }
    SaveOnClick(e) {
        this.modName = 'save';
    }
    CancerOnClick(e) {
        this.modName = 'cancer';
    }
    onFormSubmit = async function (e) {
        this.dataGrid.instance.saveEditData();
        this.topurchasekey = this.dataGrid.instance.getSelectedRowsData();
        if (this.topurchasekey.length === 0) {
            Swal.fire({
                allowEnterKey: false,
                allowOutsideClick: false,
                title: '沒有勾選任何項目',
                html: '請勾選要轉工單的項目',
                icon: 'warning',
                timer: 3000
            });
        } else {
            const OrderData = new PostOrderMaster_Detail();
            OrderData.orderDetail = this.Oitemkeyval;
            OrderData.workOrderHead = this.topurchasekey;

            // tslint:disable-next-line: max-line-length
            const sendRequest = await SendService.sendRequest(this.http, '/WorkOrders/OrderToWorkOrder', 'POST', { values: OrderData });
            if (sendRequest) {
                this.childOuter.emit(true);
                this.dataGrid.instance.clearSelection();
                if (sendRequest.message === '' || sendRequest.message === undefined) {
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
        }
    };
}

