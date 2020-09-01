import { Component, OnInit, OnChanges, ViewChild, Input, EventEmitter, Output } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AppComponent } from 'src/app/app.component';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from 'src/app/shared/mylib';
import notify from 'devextreme/ui/notify';
import { DxDataGridComponent, DxFormComponent } from 'devextreme-angular';
import { Myservice } from 'src/app/service/myservice';
import Swal from 'sweetalert2';


@Component({
    selector: 'app-create-sale',
    templateUrl: './create-sale.component.html',
    styleUrls: ['./create-sale.component.css']
})
export class CreateSaleComponent implements OnInit {
    @Output() childOuter = new EventEmitter();
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @Input() modval: any;
    @Input() itemkeyval: any;
    @Input() ProductBasicList: any;
    autoNavigateToFocusedRow = true;
    detailfilter = [];
    idlist: any;
    dataSourceDB: any;
    visible: boolean;
    topurchasekey: any;
    listStatus: any;
    popupVisibleSale: boolean;
    popupVisiblePurchase: boolean;
    editorOptions: any;
    tosalekey: any;
    mod: string;
    allowEditing: boolean;
    formData: any;
    selectedFilterOperation: string;
    filterValue: any;
    remoteOperations: boolean;

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent) {
        this.remoteOperations = true;
        this.PrintQrCode = this.PrintQrCode.bind(this);
        this.listStatus = myservice.getWorkOrderStatus();
        this.dataSourceDB = [];
        this.popupVisibleSale = false;
        this.allowEditing = false;
    }
    ngOnInit() {
    }
    // tslint:disable-next-line: use-lifecycle-interface
    ngOnChanges() {
        // const oldDay = new Date(new Date(new Date().setHours(0, 0, 0, 0)).toISOString());
        // const toDay = new Date(new Date(new Date().setHours(23, 59, 59, 999)).toISOString());
        // this.selectedFilterOperation = 'between';
        // this.filterValue = [new Date(oldDay.setDate(oldDay.getDate() - 30)), new Date(toDay.setDate(toDay.getDate() + 1))];
        // this.dataSourceDB = new CustomStore({
        //     key: 'Id',
        //     // load: () => SendService.sendRequest(this.http, '/Processes/GetWorkOrderList/0'),
        //     load: (loadOptions) => {
        //         loadOptions.sort = [{ selector: 'ReplyDate', desc: true }];
        //         // if (loadOptions.searchValue) {
        //         loadOptions.filter = [
        //             ['ReplyDate', '>=', oldDay],
        //             'and',
        //             ['ReplyDate', '<=', toDay],
        //         ];
        //         // }
        //         return SendService.sendRequest(
        //             this.http,
        //             '/Sales/GetOrderListByDate',
        //             'GET', { loadOptions, remote: this.remoteOperations, detailfilter: this.detailfilter });
        //     },
        // });
        this.editorOptions = { showSpinButtons: true, mode: 'number', min: 1 };


        this.app.GetData('/Sales/GetOrderList').subscribe(
            (s) => {
                if (s.success) {
                    this.dataSourceDB = s.data;
                }
            });

        // this.dataSourceDB = new CustomStore({
        //     key: 'Id',
        //     load: () => SendService.sendRequest(this.http, '/Sales/GetOrderList'),
        // });
    }
    calculateFilterExpression(filterValue, selectedFilterOperation) {
        const column = this as any;
        // Implementation for the "between" comparison operator
        if (selectedFilterOperation === 'between' && Array.isArray(filterValue)) {
            const filterExpression = [
                [column.dataField, '>=', filterValue[0]],
                'and',
                [column.dataField, '<=', filterValue[1]]
            ];
            return filterExpression;
        }
        // Invokes the default filtering behavior
        return column.defaultCalculateFilterExpression.apply(column, arguments);
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
    onCellPrepared(e) {
        if (e.rowType === 'data') {
            if (e.data.QuantityLimit > e.data.Quantity) {
                e.cellElement.style.backgroundColor = '#d9534f';
                e.cellElement.style.color = '#fff';
            }
        }
    }
    onEditorPreparing(e) {
        if (e.row && e.row.isSelected === false && (e.dataField === 'SaleDate' || e.dataField === 'SaleQuantity')) {
            e.editorOptions.readOnly = true;
        }
    }
    onOptionChanged(e) {
    }
    selectionChanged(e) {
    }
    PrintQrCode(e) {
    }
    async onFormSubmit(e) {
        debugger;
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
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#71c016',
                cancelButtonColor: '#CE312C',
                cancelButtonText: '取消',
                confirmButtonText: '確認'
            }).then(async (result) => {
                debugger;
                if (result.value) {
                    const tosalekeyTemp = [];
                    if (this.tosalekey !== undefined) {
                        this.tosalekey.forEach(element => {
                            tosalekeyTemp.push({
                                CreateTime: new Date(),
                                CreateUser: element.CreateUser,
                                CustomerNo: '330-200430041-0001',
                                Delivered: null,
                                Discount: null,
                                DiscountPrice: null,
                                Drawing: null,
                                DueDate: element.DueDate,
                                Id: element.Id,
                                Ink: null,
                                Label: null,
                                MachineNo: element.MachineNo,
                                Order: null,
                                OrderId: 153,
                                OriginPrice: element.OriginPrice,
                                Package: null,
                                Price: element.Price,
                                Product: null,
                                ProductBasic: null,
                                ProductBasicId: element.ProductBasicId,
                                ProductId: null,
                                Quantity: element.Quantity,
                                Remark: null,
                                Reply: null,
                                ReplyDate: element.ReplyDate,
                                ReplyRemark: null,
                                SaleCount: element.SaleCount,
                                Serial: null,
                                Unit: element.Unit,
                            });
                            // element.Remark = null,
                            // element.ReplyRemark = null,
                            // element.CreateTime = new Date(),
                            // element.CreateUser = 1,
                            // element.Delivered = null,
                            // element.Discount = null,
                            // element.DiscountPrice = null,
                            // element.Drawing = null,
                            // element.Ink = null,
                            // element.Label = null,
                            // element.Order = null,
                            // element.ProductBasic = null;
                            // element.Serial = null,
                            // element.UpdateTime = new Date(),
                            // element.UpdateUser = null;
                        });
                    }
                    const tempData = {
                        CreateTime: new Date(),
                        Orderlist: tosalekeyTemp,
                        Remarks: null,
                        SaleDate: new Date(),
                        SaleID: null,
                        SaleNo: null,
                        SaleQuantity: 0,
                    };
                    try {
                        // tslint:disable-next-line: max-line-length
                        const sendRequest = await SendService.sendRequest(this.http, '/ToSale/OrderToSaleNew', 'POST', { values: tempData });
                        if (sendRequest) {
                            // this.creatpopupVisible = false;
                            this.dataGrid.instance.refresh();
                            this.childOuter.emit(true);
                            notify({
                                message: sendRequest.message,
                                position: {
                                    my: 'center top',
                                    at: 'center top'
                                }
                            }, 'success', 3000);
                        }
                    } catch (error) {

                    }
                }
            });
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
}
