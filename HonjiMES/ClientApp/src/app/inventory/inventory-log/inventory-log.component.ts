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
  selector: 'app-inventory-log',
  templateUrl: './inventory-log.component.html',
  styleUrls: ['./inventory-log.component.css']
})
export class InventoryLogComponent implements OnInit {

    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild(DxFormComponent, { static: false }) form: DxFormComponent;
    autoNavigateToFocusedRow = true;
    dataSourceDB: any;
    formData: any;
    Controller = '/Adjusts';
    creatpopupVisible: boolean;
    editpopupVisible: boolean;
    itemkey: number;
    mod: string;
    uploadUrl: string;
    exceldata: any;
    Supplierlist: any;
    remoteOperations: boolean;
    MaterialList: any;
    listAdjustStatus: any;
    editorOptions: any;
    detailfilter: any;
    selectAdjustType: any;
    listAdjustType: any;
    AdjustTypeList: any;
    WarehouseList: any;
    UserList: any;
    selectedOperation: string = "between";

    constructor(private http: HttpClient, myservice: Myservice, private app: AppComponent, private titleService: Title) {
        this.listAdjustStatus = myservice.getlistAdjustStatus();
        this.remoteOperations = true;
        this.Inventory_Change_Click = this.Inventory_Change_Click.bind(this);
        this.cancelClickHandler = this.cancelClickHandler.bind(this);
        this.saveClickHandler = this.saveClickHandler.bind(this);
        this.editorOptions = { onValueChanged: this.onValueChanged.bind(this) };
        this.getdata();
        this.app.GetData('/Materials/GetMaterials').subscribe(
            (s) => {
                if (s.success) {
                    this.MaterialList = s.data;
                }
            }
        );
        // this.app.GetData(this.Controller + '/GetAdjustType').subscribe(
        //     (s) => {
        //         if (s.success) {
        //             this.AdjustTypeList = s.data;
        //             // this.AdjustTypeList.forEach(x => x.Message);
        //             this.selectAdjustType = {
        //                 items: this.AdjustTypeList,
        //                 displayExpr: 'Message',
        //                 valueExpr: 'Message',
        //                 searchEnabled: true,
        //                 onValueChanged: this.onValueChanged.bind(this)
        //             };
        //         }
        //     }
        // );
        this.selectAdjustType = {
            items: myservice.getInventoryLogType(),
            displayExpr: 'Name',
            valueExpr: 'Name',
            searchEnabled: true,
            onValueChanged: this.onValueChanged.bind(this)
        };
        this.app.GetData('/Warehouses/GetWarehouses').subscribe(
            (s) => {
                s.data.forEach(e => {
                    e.Name = e.Code + e.Name;
                });
                this.WarehouseList = s.data;
            }
        );
        this.app.GetData('/Users/GetUsers').subscribe(
            (s2) => {
                if (s2.success) {
                    this.UserList = s2.data;
                }
            }
        );
    }
    getdata() {
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) => SendService.sendRequest(
                this.http,
                this.Controller + '/GetAdjustLog',
                'GET', { loadOptions, remote: this.remoteOperations, detailfilter: this.detailfilter }),
            byKey: (key) => SendService.sendRequest(this.http, this.Controller + '/GetAdjustLog', 'GET', { key }),
            insert: (values) => SendService.sendRequest(this.http, this.Controller + '/PostAdjustLog', 'POST', { values }),
            update: (key, values) => SendService.sendRequest(this.http, this.Controller + '/PutAdjustLog', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(this.http, this.Controller + '/DeleteAdjustLog/' + key, 'DELETE')
        });
    }
    creatdata() {
        this.creatpopupVisible = true;
    }
    handleCancel() {
        this.creatpopupVisible = false;
    }
    creatpopup_result(e) {
        // this.creatpopupVisible = false;
        // this.dataGrid.instance.refresh();
        // notify({
        //     message: '調整單新增完成',
        //     position: {
        //         my: 'center top',
        //         at: 'center top'
        //     }
        // }, 'success', 3000);
    }
    onUploaded(e) {
        //    debugger;
        // const response = JSON.parse(e.request.response) as APIResponse;
        // if (response.success) {
        //     this.mod = 'excel';
        //     this.creatpopupVisible = true;
        //     this.exceldata = response.data;
        // } else {
        //     notify({
        //         message: 'Excel 檔案讀取失敗:' + response.message,
        //         position: {
        //             my: 'center top',
        //             at: 'center top'
        //         }
        //     }, 'error', 3000);
        // }
    }
    Inventory_Change_Click(e) {
        this.itemkey = e.row.key;
        this.mod = 'customer';
        this.editpopupVisible = true;
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
        // debugger;
        if (e.value === '全部資料') {
            this.dataGrid.instance.clearFilter();
        } else {
            this.dataGrid.instance.filter(['Message', '=', e.value]);
        }
    }
    ngOnInit() {
        this.titleService.setTitle('庫存變動紀錄');
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
    onEditorPreparing(e) {
    }
    selectionChanged(e) {
    }


}
