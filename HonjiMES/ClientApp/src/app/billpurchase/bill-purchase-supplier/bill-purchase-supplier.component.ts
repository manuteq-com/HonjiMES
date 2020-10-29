import { Component, OnInit, OnChanges, ViewChild, Input, EventEmitter, Output } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AppComponent } from 'src/app/app.component';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from 'src/app/shared/mylib';
import notify from 'devextreme/ui/notify';
import { DxDataGridComponent } from 'devextreme-angular';
import { Myservice } from 'src/app/service/myservice';
import Swal from 'sweetalert2';


@Component({
  selector: 'app-bill-purchase-supplier',
  templateUrl: './bill-purchase-supplier.component.html',
  styleUrls: ['./bill-purchase-supplier.component.css']
})
export class BillPurchaseSupplierComponent implements OnInit, OnChanges {
    @Output() childOuter = new EventEmitter();
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @Input() itemkeyval: any;
    @Input() modval: any;
    @Input() MaterialBasicList: any;
    autoNavigateToFocusedRow = true;
    detailfilter = [];
    Controller = '/PurchaseHeads';
    idlist: any;
    dataSourceDB: any;
    visible: boolean;
    topurchasekey: any;
    listStatus: any;
    popupVisibleSale: boolean;
    popupVisiblePurchase: boolean;
    mod: string;
    itemkey: number;
    formData: any;
    SelectSupplier: any;
    SupplierList: any;
    editorOptions: { onValueChanged: any; };
    remoteOperations: boolean;
    PurchaseTypeList: any;
    creatpopupVisible: boolean;
    WarehouseList: any;
    DataTypeList: any;
    randomkey: number;

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent) {
        this.PrintQrCode = this.PrintQrCode.bind(this);
        this.listStatus = myservice.getWorkOrderStatus();
        this.popupVisibleSale = false;
        this.editorOptions = { onValueChanged: this.onValueChanged.bind(this) };
        this.remoteOperations = true;
        this.PurchaseTypeList = myservice.getpurchasetypes();
        this.DataTypeList = myservice.getlistAdjustStatus();
    }
    ngOnInit() {

    }
    ngOnChanges() {
        this.dataSourceDB = [];
        this.app.GetData('/Warehouses/GetWarehouses').subscribe(
            (s) => {
                s.data.forEach(e => {
                    e.Name = e.Code + e.Name;
                });
                this.WarehouseList = s.data;
            }
        );
        this.app.GetData('/Suppliers/GetSuppliers').subscribe(
            (s) => {
                if (s.success) {
                    s.data.forEach(element => {
                        element.Name = element.Code + '_' + element.Name;
                    });
                    this.SupplierList = s.data;
                    // this.AdjustTypeList.forEach(x => x.Message);
                    this.SelectSupplier = {
                        items: this.SupplierList,
                        displayExpr: 'Name',
                        valueExpr: 'Id',
                        searchEnabled: true,
                        onValueChanged: this.onValueChanged.bind(this)
                    };
                }
            }
        );
    }
    onValueChanged(e) {
        // debugger;
        // this.dataSourceDB = new CustomStore({
        //     key: 'Id',
        //     load: () => SendService.sendRequest(this.http, this.Controller + '/GetPurchaseList/' + e.value, 'GET', ),
        // });
        this.app.GetData('/PurchaseHeads/GetPurchaseList/' + e.value).subscribe(
            (s) => {
                if (s.success) {
                    s.data.forEach(element => {
                        element.UnPurchasedCount = element.Quantity - element.PurchasedCount;
                    });
                    this.dataSourceDB = s.data;
                }
            }
        );
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
    creatdata() {
        this.creatpopupVisible = true;
    }
    creatpopup_result(e) {
        debugger;
        this.creatpopupVisible = false;
        this.childOuter.emit(true);
        this.dataSourceDB = [];
        this.dataGrid.instance.refresh();
        this.dataGrid.instance.clearSelection();
        this.SelectSupplier = {
            items: this.SupplierList,
            displayExpr: 'Name',
            valueExpr: 'Id',
            searchEnabled: true,
            value: null,
            onValueChanged: this.onValueChanged.bind(this)
        };
        notify({
            message: '存檔完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
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
    }
    selectionChanged(e) {
    }
    PrintQrCode(e) {
    }
    async onFormSubmit(e) {
        this.topurchasekey = null;
        this.topurchasekey = this.dataGrid.instance.getSelectedRowsData();
        if (this.topurchasekey.length === 0) {
            Swal.fire({
                allowEnterKey: false,
                allowOutsideClick: false,
                title: '沒有勾選任何需進貨項目',
                html: '請勾選要轉進貨的訂單項目',
                icon: 'warning',
                timer: 3000
            });
        } else {
            Swal.fire({
                showCloseButton: true,
                allowEnterKey: false,
                allowOutsideClick: false,
                title: '編輯進貨單?',
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#71c016',
                confirmButtonText: '確認',
                cancelButtonText: '取消'
            }).then(async (result) => {
                if (result.value) {
                    this.mod = 'add';
                    this.randomkey = new Date().getTime();
                    this.creatpopupVisible = true;
                } else if (result.dismiss === Swal.DismissReason.close) {
                    this.creatpopupVisible = false;
                }
            });
        }
    }
}
