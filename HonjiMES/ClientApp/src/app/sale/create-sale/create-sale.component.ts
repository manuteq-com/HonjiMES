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
  selector: 'app-create-sale',
  templateUrl: './create-sale.component.html',
  styleUrls: ['./create-sale.component.css']
})
export class CreateSaleComponent implements OnInit {
    @Output() childOuter = new EventEmitter();
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
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
    tosalekey: any;
    mod: string;
    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent) {
        this.PrintQrCode = this.PrintQrCode.bind(this);
        this.listStatus = myservice.getWorkOrderStatus();
        this.dataSourceDB = [];
        this.popupVisibleSale = false;
    }
    ngOnInit() {
    }
    ngOnChanges() {
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: () => SendService.sendRequest(this.http, '/Sales/GetOrderList'),
        });
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
    }
    selectionChanged(e) {
    }
    PrintQrCode(e) {
    }
    download() {
        debugger;
        const rowkeys = this.dataGrid.instance.getSelectedRowKeys();
        const url = '/Api/Codes/GetQrCode?id=' + rowkeys.toString();
        this.http.get<any>(url, { responseType: 'blob' as 'json' }).subscribe(
            (s) => {
                debugger;
                const blob = new Blob([s], { type: 'application/pdf' }); // application後面接下載的副檔名
                const downloadURL = window.URL.createObjectURL(blob);
                const link = document.createElement('a');
                link.href = downloadURL;
                link.download = 'QrCode.pdf'; //瀏覽器下載時的檔案名稱
                link.click();
            }
        );
        //return location.origin + '/api/Codes/GetQrCode'

    }
    to_saleClick(e) {
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
