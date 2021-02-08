import { first } from 'rxjs/operators';
import { NgModule, Component, OnInit, ViewChild, Input, OnChanges, Output, EventEmitter } from '@angular/core';
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
import CheckBox from 'devextreme/ui/check_box';
import { ToSaleInfo } from 'src/app/model/viewmodels';

@Component({
  selector: 'app-sale-overview',
  templateUrl: './sale-overview.component.html',
  styleUrls: ['./sale-overview.component.css']
})
export class SaleOverviewComponent implements OnInit, OnChanges {
    @Output() childOuter = new EventEmitter();
    @Input() randomkeyval: any;
    @ViewChild('dataGrid1') dataGrid1: DxDataGridComponent;
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
    SaleOrderDetailStatusList: any;
    SaleTypeList: any;
    readOnly: boolean;
    tosalekey: any;
    disabledValues: any;
    ToSaleList: ToSaleInfo[];
    selectedOperation: string = "between";

    constructor(private http: HttpClient, myservice: Myservice, private app: AppComponent, private titleService: Title) {
        this.readOnly = true;
        this.remoteOperations = true;
        this.OrderTypeList = myservice.getOrderTypeShow();
        this.SaleOrderDetailStatusList = myservice.getSaleOrderDetailStatus();
        this.editorOptions = { showSpinButtons: true, mode: 'number', min: 1 };
        this.dataSourceDB = [];
    }
    ngOnInit() {

    }
    async ngOnChanges() {
        this.disabledValues = [];
        // this.getdata();
        this.app.GetData('/Warehouses/GetWarehouses').subscribe(
            (s) => {
                if (s.success) {
                    s.data.forEach(element => {
                        element.Name = element.Code + element.Name;
                    });
                    this.WarehouseList = s.data;
                }
            }
        );
        this.app.GetData(this.Controller + '/GetSaleOrderList').subscribe(
            (s) => {
                if (s.success) {
                    this.dataGrid1.instance.clearSelection();
                    this.dataSourceDB = s.data;
                }
            }
        );
    }
    async getdata() {
        // this.dataSourceDB = new CustomStore({
        //     key: 'Id',
        //     load: (loadOptions) => SendService.sendRequest(
        //         this.http,
        //         this.Controller + '/GetSaleData',
        //         'GET', { loadOptions, remote: this.remoteOperations, detailfilter: this.detailfilter }),
        //     byKey: (key) => SendService.sendRequest(this.http, this.Controller + '/GetSaleData', 'GET', { key })
        // });
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
    onCellPrepared(e: any) {
        if (e.rowType === 'data' && e.column.command === 'select') {
            if (e.data.Status !== 0) {
                const instance = CheckBox.getInstance(e.cellElement.querySelector('.dx-select-checkbox'));
                instance.option('disabled', true);
                this.disabledValues.push(e.data.Id);
            }
        }
    }
    onSelectionChanged(e) {// CheckBox disabled還是會勾選，必須清掉，這是官方寫法
        const disabledKeys = e.currentSelectedRowKeys.filter(i => this.disabledValues.indexOf(i) > -1);
        if (disabledKeys.length > 0) {
            e.component.deselectRows(disabledKeys);
        }
        if (e.currentDeselectedRowKeys.length !== 0) {
            e.currentDeselectedRowKeys.forEach(element => {
                const saleData = this.dataSourceDB.find(z => z.Id === element);
                // saleData.SaleQuantity = null;
                saleData.WarehouseId = null;
            });
        }
        if (e.currentSelectedRowKeys.length !== 0) {
            e.currentSelectedRowKeys.forEach(element => {
                const saleData = this.dataSourceDB.find(z => z.Id === element && z.Status === 0);
                if (saleData !== undefined) {
                    // saleData.SaleQuantity = saleData.Quantity;
                    const WarehouseIdVal = this.WarehouseList.find(x => x.Code === '301')?.Id ?? null;
                    saleData.WarehouseId = WarehouseIdVal !== null ? WarehouseIdVal : this.WarehouseList[0].Id;
                }
            });
        }
    }
    onToolbarPreparing(e) {
        e.toolbarOptions.visible = false;
    }
    onEditorPreparing(e) {
        if (e.row && (e.dataField === 'WarehouseId' || e.dataField === 'SaleQuantity')) {
            e.editorOptions.readOnly = true;
            if (e.row.isSelected === true) {
                // e.editorOptions = true;
                e.editorOptions.readOnly = false;
            }
        }
    }
    async to_saleClick(e) {
        this.dataGrid1.instance.saveEditData();
        this.tosalekey = null;
        this.tosalekey = this.dataGrid1.instance.getSelectedRowsData();
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
            this.ToSaleList = new Array<ToSaleInfo>();
            this.tosalekey.forEach(element => {
                this.ToSaleList.push({
                    Id: element.Id,
                    WarehouseId: element.WarehouseId
                });
            });

            Swal.fire({
                showCloseButton: true,
                allowEnterKey: false,
                allowOutsideClick: false,
                title: '確認銷貨?',
                html: '如確認銷貨，請點選[確認]!',
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#CE312C',
                cancelButtonText: '取消',
                confirmButtonText: '確認'
            }).then(async (result) => {
                if (result.value) {
                    try {
                        // tslint:disable-next-line: max-line-length
                        const sendRequest = await SendService.sendRequest(this.http, '/ToSale/SaleOrderToSale', 'POST', { values: this.ToSaleList });
                        if (sendRequest) {
                            this.dataGrid1.instance.refresh();
                            this.dataGrid1.instance.clearSelection();
                            this.childOuter.emit(true);
                            notify({
                                message: '更新完成',
                                position: {
                                    my: 'center top',
                                    at: 'center top'
                                }
                            }, 'success', 3000);
                        }
                    } catch (error) {

                    }
                }
            });
        }
    }

}
