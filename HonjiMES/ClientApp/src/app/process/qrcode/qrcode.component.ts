import { Component, OnInit, OnChanges, ViewChild, Input } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AppComponent } from 'src/app/app.component';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from 'src/app/shared/mylib';
import notify from 'devextreme/ui/notify';
import { DxDataGridComponent } from 'devextreme-angular';
import { Myservice } from 'src/app/service/myservice';

@Component({
    selector: 'app-qrcode',
    templateUrl: './qrcode.component.html',
    styleUrls: ['./qrcode.component.css']
})
export class QrcodeComponent implements OnInit, OnChanges {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @Input() itemkeyval: any;
    autoNavigateToFocusedRow = true;
    qrhref = location.origin + '/api/Codes/GetQrCode';
    remoteOperations: boolean;
    detailfilter = [];
    idlist: any;
    dataSourceDB: any;
    visible: boolean;
    listStatus: any;
    selectedFilterOperation: any;
    filterValue: any;

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent) {
        this.remoteOperations = true;
        this.PrintQrCode = this.PrintQrCode.bind(this);
        this.listStatus = myservice.getWorkOrderType();
        this.dataSourceDB = [];
    }
    ngOnInit() {
    }
    ngOnChanges() {
        const oldDay = new Date(new Date(new Date().setHours(0, 0, 0, 0)).toISOString());
        const toDay = new Date(new Date(new Date().setHours(23, 59, 59, 999)).toISOString());
        this.selectedFilterOperation = 'between';
        this.filterValue = [new Date(oldDay.setDate(oldDay.getDate() - 30)), new Date(toDay.setDate(toDay.getDate() + 1))];
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            // load: () => SendService.sendRequest(this.http, '/Processes/GetWorkOrderList/0'),
            load: (loadOptions) => SendService.sendRequest(
                this.http,
                '/Processes/GetWorkOrderList',
                'GET', { loadOptions, remote: this.remoteOperations, detailfilter: this.detailfilter }),
        });
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

