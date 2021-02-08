import { Component, OnInit, ViewChild } from '@angular/core';
import notify from 'devextreme/ui/notify';
import { DxDataGridComponent, DxFormComponent } from 'devextreme-angular';
import { HttpClient } from '@angular/common/http';
import CustomStore from 'devextreme/data/custom_store';
import { Observable } from 'rxjs';
import { SendService } from 'src/app/shared/mylib';
import { APIResponse } from 'src/app/app.module';
import { Myservice } from '../../service/myservice';
import { AppComponent } from 'src/app/app.component';
import { Title } from '@angular/platform-browser';

@Component({
    selector: 'app-bill-purchase',
    templateUrl: './bill-purchase.component.html',
    styleUrls: ['./bill-purchase.component.css']
})
export class BillPurchaseComponent implements OnInit {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;

    creatpopupVisible: boolean;
    autoNavigateToFocusedRow = true;
    dataSourceDB: any;
    SupplierList: any;
    MaterialBasicList: any;
    itemkey: number;
    mod: string;
    Controller = '/BillofPurchaseHeads';
    topurchase: any[] & Promise<any> & JQueryPromise<any>;
    listBillofPurchaseOrderStatus: any;

    remoteOperations: boolean;
    formData: any;
    editorOptions: any;
    detailfilter = [];
    DetailsDataSourceStorage: any;
    newpopupVisible: boolean;
    UserList: any;
    selectedOperation: string = "between";

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent, private titleService: Title) {
        this.listBillofPurchaseOrderStatus = myservice.getBillofPurchaseOrderStatus();
        this.remoteOperations = true;
        this.DetailsDataSourceStorage = [];
        this.editorOptions = { onValueChanged: this.onValueChanged.bind(this) };

        this.app.GetData('/Suppliers/GetSuppliers').subscribe(
            (s) => {
                if (s.success) {
                    this.SupplierList = s.data;
                    this.SupplierList.forEach(x => {
                        x.Name = x.Code + x.Name.substring(0, 4);
                    });
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
        this.titleService.setTitle('進貨單');
    }
    getdata() {
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            // load: () => SendService.sendRequest(this.http, this.Controller + '/GetBillofPurchaseHeads'),
            load: (loadOptions) => SendService.sendRequest(
                this.http,
                this.Controller + '/GetBillofPurchaseHeads',
                'GET', { loadOptions, remote: this.remoteOperations, detailfilter: this.detailfilter }),
            byKey: (key) => SendService.sendRequest(this.http, this.Controller + '/GetBillofPurchaseHead', 'GET', { key }),
            insert: (values) => SendService.sendRequest(this.http, this.Controller + '/PostBillofPurchaseHead', 'POST', { values }),
            update: (key, values) => SendService.sendRequest(this.http, this.Controller + '/PutBillofPurchaseHead', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(this.http, this.Controller + '/DeleteBillofPurchaseHead/' + key, 'DELETE')
        });
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
    allowEdit(e) {
        if (e.row.data.Status === 0) {
            return true;
        } else {
            return false;
        }
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
    updatepopup_result(e) {
        this.dataGrid.instance.refresh();
    }
    onEditorPreparing(e) {
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
    // onClickQuery(e) {
    //     debugger;
    //     alert('!!');
    //     this.detailfilter = this.myform.instance.option('formData');
    //     // this.getdata();
    //     this.dataGrid.instance.refresh();
    // }
    onValueChanged(e) {
        debugger;
        this.detailfilter = this.myform.instance.option('formData');
        this.dataGrid.instance.refresh();
    }
    onRowPrepared(e) {
        if (e.data !== undefined) {
            let hint = false;
            if (e.data.Status === 1) {
                e.rowElement.style.color = '#008800';
            } else {
                if (e.data !== undefined) {
                    const DeliverydateBefore = new Date(e.data.BillofPurchaseDate);
                    const DeliverydateAfter = new Date(new Date().setDate(new Date().getDate() - 1));
                    if (DeliverydateBefore <= DeliverydateAfter) {
                        hint = true;
                    }
                    if (hint) {
                        e.rowElement.style.color = '#d9534f';
                    }
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
}
