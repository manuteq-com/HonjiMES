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
  selector: 'app-deal-supplier',
  templateUrl: './deal-supplier.component.html',
  styleUrls: ['./deal-supplier.component.css']
})
export class DealSupplierComponent implements OnInit {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild(DxFormComponent, { static: false }) form: DxFormComponent;
    autoNavigateToFocusedRow = true;
    dataSourceDB: any;
    formData: any;
    Controller = '/PurchaseDetails';
    itemkey: number;
    remoteOperations: boolean;
    editorOptions: any;
    detailfilter: any;
    MaterialList: any;
    listPurchaseStatus: any;
    SupplierList: any;
    OrderTypeList: any;
    ProductList: any;
    UserList: any;
    logpopupVisible: boolean;
    listPurchaseOrderStatus: any;
    listStatus: any;
    warehousesList: any;
    constructor(private http: HttpClient, myservice: Myservice, private app: AppComponent, private titleService: Title) {
        this.listPurchaseStatus = myservice.getpurchasetypes();
        this.listPurchaseOrderStatus = myservice.getPurchaseOrderStatus();
        this.listStatus = myservice.getlistAdjustStatus();
        this.remoteOperations = true;
        this.getdata();
        this.app.GetData('/Users/GetUsers').subscribe(
            (s) => {
                if (s.success) {
                    this.UserList = s.data;
                }
            }
        );
        this.app.GetData('/Suppliers/GetSuppliers').subscribe(
            (s) => {
                if (s.success) {
                    this.SupplierList = s.data;
                }
            }
        );
        this.app.GetData('/Warehouses/GetWarehouses').subscribe(
            (s) => {
                if (s.success) {
                    s.data.forEach(e => {
                        e.Name = e.Code + e.Name;
                    });
                    this.warehousesList = s.data;
                }
            }
        );
    }
    getdata() {
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) => SendService.sendRequest(
                this.http,
                this.Controller + '/GetPurchaseDetails',
                'GET', { loadOptions, remote: this.remoteOperations, detailfilter: this.detailfilter }),
            byKey: (key) => SendService.sendRequest(this.http, this.Controller + '/GetPurchaseData', 'GET', { key }),
            insert: (values) => SendService.sendRequest(this.http, this.Controller + '/PostAdjustLog', 'POST', { values }),
            update: (key, values) => SendService.sendRequest(this.http, this.Controller + '/PutAdjustLog', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(this.http, this.Controller + '/DeleteAdjustLog/' + key, 'DELETE')
        });
    }
    onUploaded(e) {
    }

    ngOnInit() {
        this.titleService.setTitle('廠商交易紀錄');
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
    download(){}
}
