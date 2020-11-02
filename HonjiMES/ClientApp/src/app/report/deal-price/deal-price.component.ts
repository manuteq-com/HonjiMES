import { NgModule, Component, OnInit, ViewChild, Input, OnChanges } from '@angular/core';
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
  selector: 'app-deal-price',
  templateUrl: './deal-price.component.html',
  styleUrls: ['./deal-price.component.css']
})
export class DealPriceComponent implements OnInit {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild(DxFormComponent, { static: false }) form: DxFormComponent;
    autoNavigateToFocusedRow = true;
    dataSourceDB: any;
    formData: any;
    Controller = '/OrderHeads';
    itemkey: number;
    mod: string;
    uploadUrl: string;
    remoteOperations: boolean;
    editorOptions: any;
    detailfilter: any;
    MaterialList: any;
    listSaleOrderStatus: any;
    CustomerList: any;
    OrderTypeList: any;
    ProductList: any;
    UserList: any;
    logpopupVisible: boolean;
    constructor(private http: HttpClient, myservice: Myservice, private app: AppComponent, private titleService: Title) {
        this.listSaleOrderStatus = myservice.getSaleOrderHeadStatus();
        this.remoteOperations = true;
        this.editorOptions = { onValueChanged: this.onValueChanged.bind(this) };
        this.getdata();
        this.app.GetData('/Users/GetUsers').subscribe(
            (s) => {
                if (s.success) {
                    this.UserList = s.data;
                }
            }
        );
        this.app.GetData('/Customers/GetCustomers').subscribe(
            (s) => {
                if (s.success) {
                    this.CustomerList = s.data;
                }
            }
        );
    }
    getdata() {
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) => SendService.sendRequest(
                this.http,
                this.Controller + '/GetOrderData',
                'GET', { loadOptions, remote: this.remoteOperations, detailfilter: this.detailfilter }),
            byKey: (key) => SendService.sendRequest(this.http, this.Controller + '/GetSaleReturnRecords', 'GET', { key }),
            insert: (values) => SendService.sendRequest(this.http, this.Controller + '/PostAdjustLog', 'POST', { values }),
            update: (key, values) => SendService.sendRequest(this.http, this.Controller + '/PutAdjustLog', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(this.http, this.Controller + '/DeleteAdjustLog/' + key, 'DELETE')
        });
    }
    onUploaded(e) {

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
    onValueChanged(e) {
        debugger;
        if (e.value === '全部資料') {
            this.dataGrid.instance.clearFilter();
        } else {
            this.dataGrid.instance.filter(['Message', '=', e.value]);
        }
    }
    ngOnInit() {
        this.titleService.setTitle('銷貨退回紀錄查詢');
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

    readLog(e, data) {
        debugger;
        this.itemkey = data.key;
        this.logpopupVisible = true;
    }
    onFocusedRowChanged(e) {
    }
    onEditorPreparing(e) {
    }
    selectionChanged(e) {
    }
    download(){}
}
