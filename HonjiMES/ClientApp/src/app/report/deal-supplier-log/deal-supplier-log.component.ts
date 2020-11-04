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
  selector: 'app-deal-supplier-log',
  templateUrl: './deal-supplier-log.component.html',
  styleUrls: ['./deal-supplier-log.component.css']
})
export class DealSupplierLogComponent implements OnInit {
    @Input() itemkeyval: any;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild(DxFormComponent, { static: false }) form: DxFormComponent;
    autoNavigateToFocusedRow = true;
    dataSourceDB: any;
    formData: any;
    Controller = '/PurchaseHeads';
    itemkey: number;
    mod: string;
    uploadUrl: string;
    remoteOperations: boolean;
    ProductBasicList: any;
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

        this.app.GetData('/ProductBasics/GetProductBasics').subscribe(
            (s) => {
                if (s.success) {
                    this.ProductBasicList = s.data;
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
    ngOnChanges() {
        debugger;
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: () => SendService.sendRequest(this.http, this.Controller + '/GetBillOfPurchaseByDetailId?id=' + this.itemkeyval)
        });
    }

    ngOnInit(){}

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
    onEditorPreparing(e) {
    }
    selectionChanged(e) {
    }
    download(){}

}
