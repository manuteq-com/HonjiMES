import { style } from '@angular/animations';
//import { PurchaseDetailComponent } from './../purchase-detail/purchase-detail.component';
import { DatePipe } from '@angular/common';
import { Component, OnInit, Output, ViewChild } from '@angular/core';
import { DxDataGridComponent, DxFormComponent } from 'devextreme-angular';
import { HttpClient } from '@angular/common/http';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from '../shared/mylib';
import { APIResponse } from '../app.module';
import { Observable, interval } from 'rxjs';
import notify from 'devextreme/ui/notify';
import { Myservice } from 'src/app/service/myservice';
import { AppComponent } from 'src/app/app.component';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-surface-treatment',
  templateUrl: './surface-treatment.component.html',
  styleUrls: ['./surface-treatment.component.css'],
  providers: [DatePipe]
})
export class SurfaceTreatmentComponent implements OnInit {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @Output() status = true;
    creatpopupVisible: boolean;
    newpopupVisible: boolean;
    autoNavigateToFocusedRow = true;
    dataSourceDB: any;
    SupplierList: any;
    MaterialBasicList: any;
    itemkey: number;
    mod: string;
    Controller = '/PurchaseHeads';
    topurchase: any[] & Promise<any> & JQueryPromise<any>;

    listPurchaseOrderStatus: any;
    remoteOperations: boolean;
    formData: any;
    editorOptions: any;
    detailfilter = [];
    DetailsDataSourceStorage: any;
    hint: boolean;
    date: any;
    TypeList: any;
    Url = '';
    purchaseHeadId: any;
    UserList: any;
    selectedOperation: string = "between";

    // tslint:disable-next-line: max-line-length
    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent, public datepipe: DatePipe, private titleService: Title) {
        this.TypeList = myservice.getpurchasetypes();
        this.listPurchaseOrderStatus = myservice.getPurchaseOrderStatus();
        this.remoteOperations = true;
        this.DetailsDataSourceStorage = [];
        this.editorOptions = { onValueChanged: this.onValueChanged.bind(this) };

        this.app.GetData('/Suppliers/GetSuppliers').subscribe(
            (s) => {
                if (s.success) {
                    this.SupplierList = s.data;
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
        this.app.GetData('/Users/GetUsers').subscribe(
            (s2) => {
                if (s2.success) {
                    this.UserList = s2.data;
                    this.getdata();
                }
            }
        );
    }
    ngOnInit() {
        this.titleService.setTitle('表面處理');
    }
    getdata() {
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            // load: () => SendService.sendRequest(this.http, this.Controller + '/GetPurchaseHeads'),
            load: (loadOptions) => SendService.sendRequest(
                this.http,
                this.Controller + '/GetPurchaseHeads',
                'GET', { loadOptions, remote: this.remoteOperations, detailfilter: this.detailfilter }),
            byKey: (key) => SendService.sendRequest(this.http, this.Controller + '/GetPurchaseHead', 'GET', { key }),
            insert: (values) => SendService.sendRequest(this.http, this.Controller + '/PostPurchaseHead', 'POST', { values }),
            update: (key, values) => {
                if (values.Status === 0) {
                    values.StatusVal = values.Status;
                }
                return SendService.sendRequest(this.http, this.Controller + '/PutPurchaseHead', 'PUT', { key, values });
            },
            remove: (key) => SendService.sendRequest(this.http, this.Controller + '/DeletePurchaseHead/' + key, 'DELETE')
        });
    }
    allowEdit(e) {
        if (e.row.data.Status === 0 || e.row.data.Status === 2) {
            return true;
        } else {
            return false;
        }
    }
    newdata() {
        this.newpopupVisible = true;
    }
    newpopup_result(e) {
        this.newpopupVisible = false;
        this.dataGrid.instance.refresh();
        notify({
            message: '存檔完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
    }
    creatdata() {
        this.creatpopupVisible = true;
    }
    creatpopup_result(e) {
        this.creatpopupVisible = false;
        this.dataGrid.instance.refresh();
        notify({
            message: '存檔完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
    }
    onEditorPreparing(e) {
        if (e.parentType === 'dataRow' && (e.dataField === 'PurchaseNo')) {
            if (!isNaN(e.row.key)) {
                e.editorOptions.disabled = true;
            }
        }
    }
    selectionChanged(e) {
        // debugger;
        // 只開一筆Detail資料
        e.component.collapseAll(-1);
        e.component.expandRow(e.currentSelectedRowKeys[0]);
    }
    contentReady(e) {
        // 預設要打開的子表單
        if (!e.component.getSelectedRowKeys().length) {
            e.component.selectRowsByIndexes(0);
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
    onFocusedRowChanging(e) {
        // debugger;
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
    onRowChanged() {
        this.dataGrid.instance.refresh();
    }
    onValueChanged(e) {
        // debugger;
        this.detailfilter = this.myform.instance.option('formData');
        this.dataGrid.instance.refresh();
    }
    onRowPrepared(e) {
        // debugger;
        if (e.data !== undefined) {
            this.hint = false;
            if (e.data.Status === 1) {
                e.rowElement.style.color = '#008800';
            } else {
                if (e.data !== undefined) {
                    e.data.PurchaseDetails.forEach(element => {
                        const DeliverydateBefore = new Date(element.DeliveryTime);
                        const DeliverydateAfter = new Date(new Date().setDate(new Date().getDate() - 1));
                        if (DeliverydateBefore <= DeliverydateAfter) {
                            e.rowElement.style.color = '#d9534f';
                        }
                    });
                }
            }
        }
    }
    onEditingStart(e) {
    }
    onFocusedRowChanged(e) {
    }
    onCellPrepared(e) {
    }
    downloadPurchaseOrder(e, data) {
        this.purchaseHeadId = data.key;
        this.Url = '/Api/Report/GetPurchaseOrderPDF/' + this.purchaseHeadId;
    }
}
