import { NgModule, Component, OnInit, ViewChild } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import CustomStore from 'devextreme/data/custom_store';
import DataSource from 'devextreme/data/data_source';
import ArrayStore from 'devextreme/data/array_store';
import { DxDataGridComponent, DxFormComponent, DxPopupComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { APIResponse } from 'src/app/app.module';
import { SendService } from 'src/app/shared/mylib';

@Component({
    selector: 'app-material-list',
    templateUrl: './material-list.component.html',
    styleUrls: ['./material-list.component.css']
})
export class MaterialListComponent implements OnInit {

    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild(DxFormComponent, { static: false }) form: DxFormComponent;
    autoNavigateToFocusedRow = true;
    dataSourceDB: any;
    formData: any;
    Controller = '/Materials';
    creatpopupVisible: boolean;
    editpopupVisible: boolean;
    itemkey: number;
    mod: string;
    uploadUrl: string;
    exceldata: any;
    Supplierlist: any;
    public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>(location.origin + '/api' + apiUrl);
    }
    constructor(private http: HttpClient) {
        this.uploadUrl = location.origin + '/api/OrderHeads/PostOrdeByExcel';
        this.Inventory_Change_Click = this.Inventory_Change_Click.bind(this);
        this.cancelClickHandler = this.cancelClickHandler.bind(this);
        this.saveClickHandler = this.saveClickHandler.bind(this);
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: () => SendService.sendRequest(http, this.Controller + '/GetMaterials'),
            byKey: (key) => SendService.sendRequest(http, this.Controller + '/GetMaterial', 'GET', { key }),
            insert: (values) => SendService.sendRequest(http, this.Controller + '/PostMaterial', 'POST', { values }),
            update: (key, values) => SendService.sendRequest(http, this.Controller + '/PutMaterial', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(http, this.Controller + '/DeleteMaterial', 'DELETE')
        });
        this.GetData('/Suppliers/GetSuppliers').subscribe(
            (s) => {
                console.log(s);
                this.Supplierlist = s.data;
                if (s.success) {

                }
            }
        );
    }
    creatdata() {
        this.creatpopupVisible = true;
    }
    creatpopup_result(e) {
        this.creatpopupVisible = false;
        this.dataGrid.instance.refresh();
        notify({
            message: '原料新增完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
    }
    editpopup_result(e) {
        this.editpopupVisible = false;
        this.dataGrid.instance.refresh();
        notify({
            message: '庫存數量修改完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
    }
    onUploaded(e) {
        //    debugger;
        const response = JSON.parse(e.request.response) as APIResponse;
        if (response.success) {
            this.mod = 'excel';
            this.creatpopupVisible = true;
            this.exceldata = response.data;
        } else {
            notify({
                message: 'Excel 檔案讀取失敗:' + response.message,
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'error', 3000);
        }
    }
    Inventory_Change_Click(e) {
        this.itemkey = e.row.key;
        this.mod = 'material';
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
    ngOnInit() {
    }

}