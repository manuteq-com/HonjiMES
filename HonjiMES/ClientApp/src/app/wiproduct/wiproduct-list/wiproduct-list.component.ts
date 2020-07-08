import { NgModule, Component, OnInit, ViewChild, Input, OnChanges } from '@angular/core';
import { APIResponse } from '../../app.module';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import CustomStore from 'devextreme/data/custom_store';
import DataSource from 'devextreme/data/data_source';
import ArrayStore from 'devextreme/data/array_store';
import { DxDataGridComponent, DxFormComponent, DxPopupComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { SendService } from '../../shared/mylib';

@Component({
  selector: 'app-wiproduct-list',
  templateUrl: './wiproduct-list.component.html',
  styleUrls: ['./wiproduct-list.component.css']
})
export class WiproductListComponent implements OnInit, OnChanges {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild(DxFormComponent, { static: false }) form: DxFormComponent;
    @Input() masterkey: number;
    @Input() itemval: any;
    autoNavigateToFocusedRow = true;
    dataSourceDB: any;
    formData: any;
    Controller = '/Wiproducts';
    MaterialBasicList: any;
    WarehouseList: any;
    creatpopupVisible: boolean;
    editpopupVisible: boolean;
    itemkey: string;
    itemdata: any;
    exceldata: any;
    mod: string;
    visible: boolean;
    public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>(location.origin + '/api' + apiUrl);
    }
    constructor(private http: HttpClient) {
        this.Inventory_Change_Click = this.Inventory_Change_Click.bind(this);
        this.cancelClickHandler = this.cancelClickHandler.bind(this);
        this.saveClickHandler = this.saveClickHandler.bind(this);
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: () => SendService.sendRequest(http, this.Controller + '/GetWiproductsById/' + this.masterkey),
            byKey: (key) => SendService.sendRequest(http, this.Controller + '/GetWiproduct', 'GET', { key }),
            insert: (values) => SendService.sendRequest(http, this.Controller + '/PostWiproduct', 'POST', { values }),
            update: (key, values) => SendService.sendRequest(http, this.Controller + '/PutWiproduct', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(http, this.Controller + '/DeleteWiproduct/' + key, 'DELETE')
        });
        this.GetData('/MaterialBasics/GetMaterialBasicsAsc').subscribe(
            (s) => {
                this.MaterialBasicList = s.data;
            }
        );
        this.GetData('/Warehouses/GetWarehouses').subscribe(
            (s) => {
                this.WarehouseList = s.data;
            }
        );
    }
    ngOnInit() {
    }
    ngOnChanges() {
        if (this.masterkey) {
            this.visible = false;
        }
    }
    creatdata() {
        this.creatpopupVisible = true;
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
    onRowPrepared(e) {
        if (e.rowType === 'data') {
            if (e.data.QuantityLimit > e.data.Quantity) {
                e.rowElement.style.backgroundColor = '#d9534f';
                e.rowElement.style.color = '#fff';
            }
        }
    }
    cellClick(e) {
        if (e.rowType === 'header') {
            if (e.column.type === 'buttons') {
                if (e.column.cssClass === 'addmod') {
                    // tslint:disable-next-line: deprecation
                    this.itemdata = this.itemval.data;
                    this.creatdata();
                }
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