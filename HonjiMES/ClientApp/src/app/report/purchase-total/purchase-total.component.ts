import { NgModule, Component, OnInit, ViewChild, Input, OnChanges, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import CustomStore from 'devextreme/data/custom_store';
import DataSource from 'devextreme/data/data_source';
import ArrayStore from 'devextreme/data/array_store';
import { DxDataGridComponent, DxFormComponent, DxPopupComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { APIResponse } from 'src/app/app.module';
import { SendService } from 'src/app/shared/mylib';
import { Myservice } from 'src/app/service/myservice';
import { AppComponent } from 'src/app/app.component';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-purchase-total',
  templateUrl: './purchase-total.component.html',
  styleUrls: ['./purchase-total.component.css']
})
export class PurchaseTotalComponent implements OnInit {
    @Output() status = false;
    @Output() childOuter = new EventEmitter();
    @ViewChild('dataGrid') dataGrid: DxDataGridComponent;
    @ViewChild('dataGrid2') dataGrid2: DxDataGridComponent;
    @ViewChild(DxFormComponent, { static: false }) form: DxFormComponent;
    autoNavigateToFocusedRow = true;
    dataSourceDB: any;
    dataSourceDB2: any;
    formData: any;
    Controller = '/PurchaseHeads';
    itemkey: number;
    remoteOperations: boolean;
    editorOptions: any;
    detailfilter: any;
    TypeList: any;
    listPurchaseOrderStatus: any;
    SupplierList: any;
    UserList: any;
    MaterialBasicList: any;
    key: any;
    selectedOperation: string = "between";
    constructor(private http: HttpClient, myservice: Myservice, private app: AppComponent, private titleService: Title) {
        this.TypeList = myservice.getpurchasetypes();
        this.listPurchaseOrderStatus = myservice.getPurchaseOrderStatus();
        this.remoteOperations = true;
        this.editorOptions = { onValueChanged: this.onValueChanged.bind(this) };
        // this.getdata();
        this.app.GetData('/Suppliers/GetSuppliers').subscribe(
            (s) => {
                if (s.success) {
                    this.SupplierList = s.data;
                }
            }
        );
        this.app.GetData('/Users/GetUsers').subscribe(
            (s2) => {
                if (s2.success) {
                    this.UserList = s2.data;
                    this.getdata();
                }
            }
        );
        this.app.GetData('/MaterialBasics/GetMaterialBasics').subscribe(
            (s) => {
                if (s.success) {
                    this.MaterialBasicList = s.data.data;
                }
            }
        );
    }
    getdata() {
        // debugger;
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: () => SendService.sendRequest(this.http, this.Controller + '/GetPurchaseQuantityByStatus', 'GET'),
            byKey: (key) => SendService.sendRequest(this.http, this.Controller + '/GetMachineReport', 'GET', { key }),
            insert: (values) => SendService.sendRequest(this.http, this.Controller + '/PostAdjustLog', 'POST', { values }),
            update: (key, values) => SendService.sendRequest(this.http, this.Controller + '/PutAdjustLog', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(this.http, this.Controller + '/DeleteAdjustLog/' + key, 'DELETE')
        });
    }
    onRowChanged() {
        // this.dataGrid2.instance.refresh();
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
    onValueChanged(e) {
        if (e.value === '全部資料') {
            this.dataGrid.instance.clearFilter();
        } else {
            this.dataGrid.instance.filter(['Message', '=', e.value]);
        }
    }
    ngOnInit() {
        this.titleService.setTitle('採購單統計查詢');
    }
    cancelClickHandler(e) {
        this.dataGrid.instance.cancelEditData();
    }
    saveClickHandler(e) {
        this.dataGrid.instance.saveEditData();
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
    onFocusedRowChanged(e) {
    }
    onCellPrepared(e) {

    }
    selectionChanged(e) {
        debugger;
        this.key = e.selectedRowsData[0].Type,
        this.dataSourceDB2 = new CustomStore({
            key: 'Id',
            load: (loadOptions) => SendService.sendRequest(
                this.http,
                this.Controller + '/GetPurchaseHeadsByType?type=' + this.key,
                'GET', { loadOptions, remote: this.remoteOperations, detailfilter: this.detailfilter }),
            byKey: (key) => SendService.sendRequest(this.http, this.Controller + '/GetAdjustList', 'GET', { key }),
            insert: (values) => SendService.sendRequest(this.http, this.Controller + '/PostAdjustList', 'POST', { values }),
            update: (key, values) => SendService.sendRequest(this.http, this.Controller + '/PutAdjustList', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(this.http, this.Controller + '/DeleteAdjustList/' + key, 'DELETE')
        });
    }

}
