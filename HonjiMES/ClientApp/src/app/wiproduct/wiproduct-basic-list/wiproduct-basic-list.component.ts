import { Component, OnInit, ViewChild } from '@angular/core';
import { DxFormComponent, DxDataGridComponent } from 'devextreme-angular';
import { HttpClient } from '@angular/common/http';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from 'src/app/shared/mylib';
import { Observable } from 'rxjs';
import { APIResponse } from 'src/app/app.module';
import notify from 'devextreme/ui/notify';
import { AppComponent } from 'src/app/app.component';
import { Title } from '@angular/platform-browser';


@Component({
    selector: 'app-wiproduct-basic-list',
    templateUrl: './wiproduct-basic-list.component.html',
    styleUrls: ['./wiproduct-basic-list.component.css']
})
export class WiproductBasicListComponent implements OnInit {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild(DxFormComponent, { static: false }) form: DxFormComponent;
    autoNavigateToFocusedRow = true;
    dataSourceDB: any;
    formData: any;
    Controller = '/WiproductBasics';
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

    constructor(private http: HttpClient, public app: AppComponent, private titleService: Title) {
        this.Inventory_Change_Click = this.Inventory_Change_Click.bind(this);
        this.cancelClickHandler = this.cancelClickHandler.bind(this);
        this.saveClickHandler = this.saveClickHandler.bind(this);
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: () => SendService.sendRequest(http, this.Controller + '/GetWiproductBasics'),
            byKey: (key) => SendService.sendRequest(http, this.Controller + '/GetWiproductBasic', 'GET', { key }),
            insert: (values) => SendService.sendRequest(http, this.Controller + '/PostWiproductBasic', 'POST', { values }),
            update: (key, values) => SendService.sendRequest(http, this.Controller + '/PutWiproductBasic', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(http, this.Controller + '/DeleteWiproductBasic/' + key, 'DELETE')
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
                if (s.success) {
                    s.data.forEach(e => {
                        e.Name = e.Code + e.Name;
                    });
                    this.WarehouseList = s.data;
                }
            }
        );
    }
    ngOnInit() {
        this.titleService.setTitle('半成品庫存');
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
            message: '半成品資料新增完成',
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
        this.mod = 'wiproduct';
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
    onRowChanged() {
        this.dataGrid.instance.refresh();
    }
    onRowPrepared(e) {
        this.hint = false;
        if (e.data !== undefined) {
            e.data.Wiproducts.forEach(element => {
                if (element.QuantityLimit > element.Quantity) {
                    this.hint = true;
                }
            });
            if (this.hint) {
                e.rowElement.style.color = '#d9534f';
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
    onEditingStart(e) {

    }
    onFocusedRowChanged(e) {
    }
    onCellPrepared(e) {
        if (e.rowType === 'data') {
            if (e.data.QuantityLimit > e.data.Quantity) {

                e.cellElement.style.backgroundColor = '#d9534f';
                e.cellElement.style.color = '#fff';
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
