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
    selector: 'app-adjust-list',
    templateUrl: './adjust-list.component.html',
    styleUrls: ['./adjust-list.component.css']
})
export class AdjustListComponent implements OnInit {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    tabs;
    selectTabKey: number =1;
    creatpopupVisible: boolean;
    autoNavigateToFocusedRow = true;
    dataSourceDB: any;
    SupplierList: any;
    MaterialBasicList: any;
    itemkey: number;
    mod: string;
    Controller = '/Adjusts';
    topurchase: any[] & Promise<any> & JQueryPromise<any>;
    listBillofPurchaseOrderStatus: any;
    exceldata: any;

    remoteOperations: boolean;
    formData: any;
    editorOptions: any;
    detailfilter = [];
    DetailsDataSourceStorage: any;
    adjustpopupVisible: boolean;
    dataSourceDBDetail: CustomStore;
    ItemTypeList: any;
    WarehouseList: any;
    selectedOperation: string = "between";
    selectedRowKeys = [];

    constructor(private http: HttpClient, myservice: Myservice, private app: AppComponent, private titleService: Title) {
        this.tabs = [{"key": 1 ,"text": "以調整單顯示"},{"key": 2 ,"text": "以總覽列表顯示"}]
        this.listBillofPurchaseOrderStatus = myservice.getBillofPurchaseOrderStatus();
        this.remoteOperations = true;
        this.DetailsDataSourceStorage = [];
        this.getdata();
        this.editorOptions = { onValueChanged: this.onValueChanged.bind(this) };
        this.ItemTypeList = myservice.getlistAdjustStatus();

        this.app.GetData('/Suppliers/GetSuppliers').subscribe(
            (s) => {
                if (s.success) {
                    this.SupplierList = s.data;
                    this.SupplierList.forEach(x => {
                        x.Name = x.Name.substring(0, 4);
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
        this.app.GetData('/Warehouses/GetWarehouses').subscribe(
            (s) => {
                s.data.forEach(e => {
                    e.Name = e.Code + e.Name;
                });
                this.WarehouseList = s.data;
            }
        );
    }
    getdata() {
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) => SendService.sendRequest(
                this.http,
                this.Controller + '/GetAdjustLists',
                'GET', { loadOptions, remote: this.remoteOperations, detailfilter: this.detailfilter }),
            byKey: (key) => SendService.sendRequest(this.http, this.Controller + '/GetAdjustList', 'GET', { key }),
            insert: (values) => SendService.sendRequest(this.http, this.Controller + '/PostAdjustList', 'POST', { values }),
            update: (key, values) => SendService.sendRequest(this.http, this.Controller + '/PutAdjustList', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(this.http, this.Controller + '/DeleteAdjustList/' + key, 'DELETE')
        });
    }
    ngOnInit() {
        this.titleService.setTitle('調整單管理');
    }
    creatdata() {
        this.creatpopupVisible = true;
    }
    creatAdjust() {
        this.adjustpopupVisible = true;
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
    adjustpopup_result(e) {
        this.adjustpopupVisible = false;
        this.dataGrid.instance.refresh();
        notify({
            message: '庫存調整單建立完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
    }

    async deleteAdjust(e, data) {
        // this.requisitionId = data.data.Id;
        // Swal.fire({
        //     allowEnterKey: false,
        //     allowOutsideClick: false,
        //     width: 400,
        //     title: '確認刪除 ?',
        //     html: '刪除後將無法復原!',
        //     icon: 'warning',
        //     showCancelButton: true,
        //     // confirmButtonColor: '#3085d6',
        //     // cancelButtonColor: '#d33',
        //     cancelButtonText: '取消',
        //     confirmButtonText: '確認'
        // }).then(async (result) => {
        //     if (result.value) {
        //         // tslint:disable-next-line: max-line-length
        //         const sendRequest = await SendService.sendRequest(this.http, this.Controller + '/DeleteRequisition/' + this.requisitionId, 'DELETE');
        //         if (sendRequest) {
        //             this.dataGrid.instance.refresh();
        //             notify({
        //                 message: '刪除成功 !',
        //                 position: {
        //                     my: 'center top',
        //                     at: 'center top'
        //                 }
        //             }, 'success', 3000);
        //         }
        //     } else {
        //         this.dataGrid.instance.refresh();
        //     }
        // });
    }
    readAdjustDetail(e, data) {
        this.dataSourceDBDetail = new CustomStore({
            key: 'TempId',
            load: () => SendService.sendRequest(this.http, this.Controller + '/GetAdjustDetailByPId?PId=' + data.data.Id),
            byKey: (key) => SendService.sendRequest(this.http, this.Controller + '/GetAdjustDetail', 'GET', { key }),
            // tslint:disable-next-line: max-line-length
            insert: (values) => SendService.sendRequest(this.http, this.Controller + '/PostAdjustDetail?PId=' + data.data.Id, 'POST', { values }),
            // tslint:disable-next-line: max-line-length
            update: (key, values) => SendService.sendRequest(this.http, this.Controller + '/PutAdjustDetail', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(this.http, this.Controller + '/DeleteAdjustDetail', 'DELETE')
        });
    }

    onRowClick(e){
        let selectRowId = e.key;
        this.dataSourceDBDetail = new CustomStore({
            key: 'TempId',
            load: () => SendService.sendRequest(this.http, this.Controller + '/GetAdjustDetailByPId?PId=' + selectRowId),
            byKey: (key) => SendService.sendRequest(this.http, this.Controller + '/GetAdjustDetail', 'GET', { key }),
            // tslint:disable-next-line: max-line-length
            insert: (values) => SendService.sendRequest(this.http, this.Controller + '/PostAdjustDetail?PId=' + selectRowId, 'POST', { values }),
            // tslint:disable-next-line: max-line-length
            update: (key, values) => SendService.sendRequest(this.http, this.Controller + '/PutAdjustDetail', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(this.http, this.Controller + '/DeleteAdjustDetail', 'DELETE')
        });
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
        if (e.rowType === 'data') {
        }
    }
    onEditingStart(e) {

    }
    onFocusedRowChanged(e) {
    }
    onCellPrepared(e) {

    }

    async savedata() {
        // this.dataGrid2.instance.saveEditData();
        // this.postval = {
        //     ProductBasicId: this.productbasicId,
        //     BomId: this.bomId,
        //     MBillOfMaterialList: this.dataSourceDB_Process
        // };
        // // tslint:disable-next-line: max-line-length
        // const sendRequest = await SendService.sendRequest(this.http, '/BillOfMaterials/PostMbomlist', 'POST', { values: this.postval });
        // // let data = this.client.POST( this.url + '/OrderHeads/PostOrderMaster_Detail').toPromise();
        // if (sendRequest) {
        //     notify({
        //         message: '更新成功',
        //         position: {
        //             my: 'center top',
        //             at: 'center top'
        //         }
        //     }, 'success', 2000);
        // }
    }

    selectTab(e){
        //console.log("select",e);
        this.selectTabKey = e.component.option("selectedItemKeys")[0]
    }

}
