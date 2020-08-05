import { Component, OnInit, OnChanges, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AppComponent } from 'src/app/app.component';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from 'src/app/shared/mylib';
import notify from 'devextreme/ui/notify';
import { DxDataGridComponent } from 'devextreme-angular';

@Component({
    selector: 'app-qrcode',
    templateUrl: './qrcode.component.html',
    styleUrls: ['./qrcode.component.css']
})
export class QrcodeComponent implements OnInit, OnChanges {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    autoNavigateToFocusedRow = true;
    qrhref = location.origin + '/api/Codes/GetQrCode';
    idlist: any;
    dataSourceDB: any;
    visible: boolean;
    constructor(private http: HttpClient, public app: AppComponent) {
        this.PrintQrCode = this.PrintQrCode.bind(this);
        this.dataSourceDB = new CustomStore({
            key: 'Key',
            load: () => SendService.sendRequest(http, '/Processes/GetWorkOrderList/0'),
        });
    }
    ngOnInit() {
    }
    ngOnChanges() {

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
}

