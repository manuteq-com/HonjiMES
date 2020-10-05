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
import Swal from 'sweetalert2';

@Component({
  selector: 'app-sale-overview',
  templateUrl: './sale-overview.component.html',
  styleUrls: ['./sale-overview.component.css']
})
export class SaleOverviewComponent implements OnInit {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild(DxFormComponent, { static: false }) form: DxFormComponent;
    autoNavigateToFocusedRow = true;
    dataSourceDB: any;
    formData: any;
    Controller = '/Sales';
    itemkey: number;
    Supplierlist: any;
    remoteOperations: boolean;
    WarehouseList: any;
    OrderTypeList: any;
    editorOptions: any;
    detailfilter: any;
    selectSaleType: any;
    SaleTypeList: any;
    readOnly: boolean;
    tosalekey: any;
    constructor(private http: HttpClient, myservice: Myservice, private app: AppComponent, private titleService: Title) {
        this.readOnly = true;
        this.remoteOperations = true;
        this.OrderTypeList = myservice.getOrderTypeShow();
        this.selectSaleType = myservice.getSaleOrderStatus();
        this.editorOptions = { showSpinButtons: true, mode: 'number', min: 1 };
        this.getdata();
        this.app.GetData('/Warehouses/GetWarehouses').subscribe(
            (s) => {
                if (s.success) {
                    s.data.forEach(element => {
                        element.Name = element.Code + '_' + element.Name;
                    });
                    this.WarehouseList = s.data;
                }
            }
        );

        this.selectSaleType.unshift({Id: null, Name: '全部資料'});
        this.SaleTypeList = {
            dataSource: this.selectSaleType,
            displayExpr: 'Name',
            valueExpr: 'Id',
            searchEnabled: true
        };
    }
    getdata() {
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) => SendService.sendRequest(
                this.http,
                this.Controller + '/GetSaleData',
                'GET', { loadOptions, remote: this.remoteOperations, detailfilter: this.detailfilter }),
            byKey: (key) => SendService.sendRequest(this.http, this.Controller + '/GetSaleData', 'GET', { key })
        });
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
    onValueChanged(e) {
    }
    ngOnInit() {}
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
        if (e.row && (e.dataField === 'Warehouse' || e.dataField === 'SaleQuantity')) {
            e.editorOptions.readOnly = true;
            if (e.row.isSelected === true) {
                // e.editorOptions = true;
                e.editorOptions.readOnly = false;
            }
        }
    }

    async onFormSubmit(e) {
        debugger;
        this.dataGrid.instance.saveEditData();
        this.tosalekey = null;
        this.tosalekey = this.dataGrid.instance.getSelectedRowsData();
        if (this.tosalekey.length === 0) {
            Swal.fire({
                allowEnterKey: false,
                allowOutsideClick: false,
                title: '沒有勾選任何銷貨項目',
                html: '請勾選需銷貨的項目',
                icon: 'warning',
                timer: 3000
            });
        } else {
            let dataCheck = true;
            this.dataSourceDB.forEach(element => {
                if (element.SaleDate === undefined || element.SaleQuantity === undefined) {
                    dataCheck = false;
                }
            });
        }
    }

}
