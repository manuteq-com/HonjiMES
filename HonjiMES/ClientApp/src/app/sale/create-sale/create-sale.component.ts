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
export class CreateSaleComponent implements OnInit, OnChanges {
    @Output() childOuter = new EventEmitter();
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @Input() itemkeyval: any;
    @Input() randomkeyval: any;
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
    loadOptions: any;

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent) {
        this.remoteOperations = true;
        this.listStatus = myservice.getWorkOrderStatus();
        this.dataSourceDB = [];
        this.popupVisibleSale = false;
        this.allowEditing = false;
        this.loadOptions = {};

    }
    ngOnInit() {
    }
    async ngOnChanges() {
        this.editorOptions = { showSpinButtons: true, mode: 'number', min: 1 };
        this.app.GetData('/Sales/GetOrderList').subscribe(
            (s) => {
                if (s.success) {
                    this.dataGrid.instance.clearSelection();
                    this.dataSourceDB = s.data;
                    // this.loadOptions = this.dataGrid.instance.getDataSource().loadOptions();
                }
            }
        );
        // this.dataSourceDB = new CustomStore({
        //     key: 'Id',
        //     load: () => SendService.sendRequest(this.http, '/Sales/GetOrderList'),
        //     // update: (key, values) => SendService.sendNull()
        // });
        // this.dataSourceDB = {
        //     store: SendService.sendRequest(this.http, '/Sales/GetOrderList'),
        // };
        // let loadOptions = this.dataGrid.instance.getDataSource().loadOptions;

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
                e.component.pageIndex(pageIndex + 1).done(function() {
                    e.component.option('focusedRowIndex', 0);
                });
            } else if (e.newRowIndex === 0 && pageIndex > 0) {
                // tslint:disable-next-line: only-arrow-functions
                e.component.pageIndex(pageIndex - 1).done(function() {
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
    }
    onEditorPreparing(e) {
        if (e.row && e.row.isSelected === false && (e.dataField === 'SaleDate' || e.dataField === 'SaleQuantity')) {
            e.editorOptions.readOnly = true;
        }
    }
    onOptionChanged(e) {
    }
    onSelectionChanged(e) {
        if (e.currentDeselectedRowKeys.length !== 0) {
            e.currentDeselectedRowKeys.forEach(element => {
                const basicData = this.dataSourceDB.find(z => z.Id === element);
                basicData.SaleDate = null;
                basicData.SaleQuantity = null;
            });
        }
        if (e.currentSelectedRowKeys.length !== 0) {
            e.currentSelectedRowKeys.forEach(element => {
                const basicData = this.dataSourceDB.find(z => z.Id === element);
                basicData.SaleDate = new Date();
                basicData.SaleQuantity = basicData.Quantity - basicData.SaleCount;
            });
        }
    }
    onToolbarPreparing(e) {
        const toolbarItems = e.toolbarOptions.items;
        toolbarItems.forEach(item => {
            if (item.name === 'saveButton') {
                // item.options.icon = '';
                // item.options.text = '退料';
                // item.showText = 'always';
                item.visible = false;
            } else if (item.name === 'revertButton') {
                // item.options.icon = '';
                // item.options.text = '取消';
                // item.showText = 'always';
                item.visible = false;
            }
        });
    }
    async onFormSubmit(e) {
        this.dataGrid.instance.saveEditData();
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
            let dataCheck = true;
            this.tosalekey.forEach(element => {
                if (element.SaleDate === undefined || element.SaleQuantity === undefined) {
                    dataCheck = false;
                }
            });

            if (dataCheck) {
                Swal.fire({
                    showCloseButton: true,
                    allowEnterKey: false,
                    allowOutsideClick: false,
                    title: '確認轉銷貨?',
                    icon: 'question',
                    showCancelButton: true,
                    confirmButtonColor: '#3085d6',
                    cancelButtonColor: '#CE312C',
                    cancelButtonText: '取消',
                    confirmButtonText: '確認'
                }).then(async (result) => {
                    if (result.value) {
                        try {
                            // tslint:disable-next-line: max-line-length
                            const sendRequest = await SendService.sendRequest(this.http, '/ToSale/OrderToSaleBySelected', 'POST', { values: this.tosalekey });
                            if (sendRequest) {
                                // this.creatpopupVisible = false;
                                this.dataGrid.instance.refresh();
                                this.dataGrid.instance.clearSelection();
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
            } else {
                notify({
                    message: '請確認勾選項目的資料是否完整!',
                    position: {
                        my: 'center top',
                        at: 'center top'
                    }
                }, 'error', 3000);
            }
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
