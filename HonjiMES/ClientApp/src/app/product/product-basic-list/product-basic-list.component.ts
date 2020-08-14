import { Product } from './../../model/viewmodels';
import { Component, OnInit, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { DxFormComponent, DxDataGridComponent } from 'devextreme-angular';
import { HttpClient } from '@angular/common/http';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from 'src/app/shared/mylib';
import { Observable } from 'rxjs';
import { APIResponse } from 'src/app/app.module';
import notify from 'devextreme/ui/notify';
import { AppComponent } from 'src/app/app.component';

@Component({
    selector: 'app-product-basic-list',
    templateUrl: './product-basic-list.component.html',
    styleUrls: ['./product-basic-list.component.css']
})
export class ProductBasicListComponent implements OnInit {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild(DxFormComponent, { static: false }) form: DxFormComponent;
    autoNavigateToFocusedRow = true;
    dataSourceDB: any;
    formData: any;
    Controller = '/ProductBasics';
    MaterialList: any;
    WarehouseList: any;
    creatpopupVisible: boolean;
    editpopupVisible: boolean;
    adjustpopupVisible: boolean;
    itemkey: string;
    exceldata: any;
    mod: string;
    uploadUrl: string;
    hint: boolean;
    remoteOperations: boolean;
    detailfilter: any;
    constructor(private http: HttpClient, public app: AppComponent) {
        this.Inventory_Change_Click = this.Inventory_Change_Click.bind(this);
        this.cancelClickHandler = this.cancelClickHandler.bind(this);
        this.saveClickHandler = this.saveClickHandler.bind(this);
        this.remoteOperations = true;
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) => SendService.sendRequest(
                this.http,
                this.Controller + '/GetProductBasics',
                'GET', { loadOptions, remote: this.remoteOperations , detailfilter: this.detailfilter}),
            byKey: (key) => SendService.sendRequest(http, this.Controller + '/GetProductBasic', 'GET', { key }),
            insert: (values) => SendService.sendRequest(http, this.Controller + '/PostProductBasic', 'POST', { values }),
            update: (key, values) => SendService.sendRequest(http, this.Controller + '/PutProductBasic', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(http, this.Controller + '/DeleteProductBasic/' + key, 'DELETE')
        });
        this.app.GetData('/Materials/GetMaterials').subscribe(
            (s) => {
                console.log(s);
                this.MaterialList = s.data;
                if (s.success) {

                }
            }
        );
        this.app.GetData('/Warehouses/GetWarehouses').subscribe(
            (s) => {
                console.log(s);
                this.WarehouseList = s.data;
                if (s.success) {

                }
            }
        );
    }
    ngOnInit() {
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
            message: '成品資料新增完成',
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
        this.mod = 'product';
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
    onRowPrepared(e) {
        // debugger;
        this.hint = false;
        if (e.data !== undefined) {
            e.data.Products.forEach(element => {
                if (element.QuantityLimit > element.Quantity) {
                    this.hint = true;
                }
            });
            if (this.hint) {
                e.rowElement.style.color = '#d9534f';
            }
        }
    }
    onReflash() {
        this.dataGrid.instance.refresh();
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
    onEditingStart(e) {

    }
    onFocusedRowChanged(e) {
    }
    onCellPrepared(e) {
        this.hint = false;
        if (e.date !== undefined) {
            e.data.Products.forEach(element => {
                if (element.QuantityLimit > element.Quantity) {
                    this.hint = true;
                }
            });
            if (this.hint) {
                e.rowElement.style.backgroundColor = '#d9534f';
                e.rowElement.style.color = '#fff';
            }
        }
    }
    onEditorPreparing(e) {
    }
    selectionChanged(e) {
    }
    QuantityAdvValue(rowData) {
        return rowData.Quantity - rowData.QuantityAdv;
    }
}

