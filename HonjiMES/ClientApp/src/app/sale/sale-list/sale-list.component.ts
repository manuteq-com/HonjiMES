import { Component, OnInit, ViewChild, OnChanges } from '@angular/core';
import { DxDataGridComponent, DxFormComponent, DxFileUploaderComponent } from 'devextreme-angular';
import { HttpClient } from '@angular/common/http';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from '../../shared/mylib';
import DataSource from 'devextreme/data/data_source';
import ArrayStore from 'devextreme/data/array_store';
import { Observable } from 'rxjs';
import { APIResponse } from '../../app.module';
import notify from 'devextreme/ui/notify';
import Swal from 'sweetalert2';
import { POrderSale, ReorderSale, ToorderSale } from '../../model/viewmodels';
import Select from 'devextreme/ui/check_box';
import { Myservice } from '../../service/myservice';
import { AppComponent } from 'src/app/app.component';
import { Title } from '@angular/platform-browser';
import { CreateSaleComponent } from '../create-sale/create-sale.component';
@Component({
    selector: 'app-sale-list',
    templateUrl: './sale-list.component.html',
    styleUrls: ['./sale-list.component.css']
})
export class SaleListComponent implements OnInit, OnChanges {

    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxFileUploaderComponent) uploader: DxFileUploaderComponent;
    @ViewChild(CreateSaleComponent) createSale: CreateSaleComponent;

    autoNavigateToFocusedRow = true;
    dataSourceDB: any;
    Customerlist: any;
    MaterialList: any;
    creatpopupVisible: any;
    resalepopupVisible = false;
    tosalepopupVisible = false;
    dataSourceDBDetails: any[];
    itemkey: number;
    mod: string;
    resaleitemkey: any;
    tosaleitemkey: any;
    resalemod: string;
    uploadUrl: string;
    exceldata: any;
    Controller = '/Sales';
    listSaleOrderStatus: any;
    postval: POrderSale;
    MaterialBasicList: any;

    remoteOperations: boolean;
    formData: any;
    editorOptions: any;
    detailfilter = [];
    DetailsDataSourceStorage: any;
    randomkey: number;
    Quantityval: any;
    Priceval: number;
    OriginPriceval: any;
    saleHeadId: any;
    Url = '';
    UserList: any;
    overviewpopupVisible: boolean;
    overRandomkey: number;
    selectedOperation: string = "between";

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent, private titleService: Title) {
        this.listSaleOrderStatus = myservice.getSaleOrderHeadStatus();
        this.cloneIconClick = this.cloneIconClick.bind(this);
        this.to_hsaleClick = this.to_hsaleClick.bind(this);
        this.to_dsaleClick = this.to_dsaleClick.bind(this);
        this.creatpopupVisible = false;
        this.remoteOperations = true;
        this.DetailsDataSourceStorage = [];
        this.editorOptions = { onValueChanged: this.onValueChanged.bind(this) };

        this.app.GetData('/Customers/GetCustomers').subscribe(
            (s) => {
                if (s.success) {
                    this.Customerlist = s.data;
                }
            }
        );
        this.app.GetData('/Materials/GetMaterials').subscribe(
            (s) => {
                this.MaterialList = s.data;
                if (s.success) {
                }
            }
        );
        this.app.GetData('/Materials/GetMaterialBasics').subscribe(
            (s) => {
                this.MaterialBasicList = s.data;
                if (s.success) {
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
    getdata() {
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            // load: () => SendService.sendRequest(this.http, this.Controller + '/GetSalesByStatus'),
            load: (loadOptions) => SendService.sendRequest(
                this.http,
                this.Controller + '/GetSales',
                'GET', { loadOptions, remote: this.remoteOperations, detailfilter: this.detailfilter }),
            byKey: (key) => SendService.sendRequest(this.http, this.Controller + '/GetSale', 'GET', { key }),
            insert: (values) => SendService.sendRequest(this.http, this.Controller + '/PostSale', 'POST', { values }),
            update: (key, values) => SendService.sendRequest(this.http, this.Controller + '/PutSale', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(this.http, this.Controller + '/DeleteSale/' + key, 'DELETE')
        });

    }
    ngOnInit() {
        this.titleService.setTitle('銷貨單');
    }
    ngOnChanges() {

    }
    getDetails(SaleId) {
        const DController = '/SaleDetailNews';
        let item = this.DetailsDataSourceStorage.find((i) => i.key === SaleId);
        if (!item) {
            item = {
                key: SaleId,
                DetaildataSource: new CustomStore({
                    key: 'Id',
                    load: () => SendService.sendRequest(this.http, DController + '/GetSaleDetailsBySaleId?SaleId=' + SaleId),
                    byKey: (key) => SendService.sendRequest(this.http, DController + '/GetSaleDetailNew', 'GET', { key }),
                    insert: (values) => SendService.sendRequest(this.http, DController + '/PostSaleDetailNew', 'POST', { values }),
                    // tslint:disable-next-line: max-line-length
                    update: (key, values) => SendService.sendRequest(this.http, DController + '/PutSaleDetailNewQty', 'PUT', { key, values }),
                    remove: (key) => SendService.sendRequest(this.http, DController + '/DeleteSaleDetailNew/' + key, 'DELETE')
                })
            };
            this.DetailsDataSourceStorage.push(item);
        }
        return item.DetaildataSource;
    }
    completedValue(rowData) {
        // tslint:disable-next-line: triple-equals
        return rowData.Status == 'Completed';
    }

    onCellPrepared(e) {
        //     if (e.rowType === 'header' && e.column.command === 'select') {
        //         alert('testheader');
        //  }
    }
    allowEdit(e) {
        if (e.row.data.Status !== 0) {
            return false;
        } else {
            return true;
        }
    }
    addDetail(e) {
        this.dataGrid.instance.addRow();
    }
    cloneIconClick(e) {
        this.itemkey = e.row.key;

        this.mod = 'clone';
        this.creatpopupVisible = true;
        // e.event.preventDefault();
    }
    creatdata() {
        this.randomkey = new Date().getTime();
        this.creatpopupVisible = true;
    }
    overviewpopup(e) {
        this.overRandomkey = new Date().getTime();
        this.overviewpopupVisible = true;
    }
    creatpopup_result(e) {
        this.overviewpopupVisible = false;
        this.creatpopupVisible = false;
        this.dataGrid.instance.refresh();
    }
    resalepopup_result(e) {
        this.resalepopupVisible = false;
        this.dataGrid.instance.refresh();
        notify({
            message: '存檔完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
    }
    tosalepopup_result(e) {
        this.tosalepopupVisible = false;
        this.dataGrid.instance.refresh();
        notify({
            message: '存檔完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
    }
    selectionChanged(e) {
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
    QuantityValueChanged(e, data) {
        data.setValue(e.value);
        this.Quantityval = e.value;
        this.Priceval = this.Quantityval * this.OriginPriceval;
    }
    OriginPriceValueChanged(e, data) {
        data.setValue(e.value);
        this.OriginPriceval = e.value;
        this.Priceval = this.Quantityval * this.OriginPriceval;
    }

    // onEditingStart(e) {
    //     e.component.columnOption('OrderNo', 'allowEditing', false);
    //     e.component.columnOption('CustomerNo', 'allowEditing', false);
    //     e.component.columnOption('OrderDate', 'allowEditing', false);
    // }

    // onInitNewRow(e) {
    //     e.component.columnOption('OrderNo', 'allowEditing', true);
    //     e.component.columnOption('CustomerNo', 'allowEditing', true);
    //     e.component.columnOption('OrderDate', 'allowEditing', true);
    // }
    onEditorPreparing(e) {
        if (e.parentType === 'dataRow' && (e.dataField === 'OrderNo' || e.dataField === 'CustomerNo' || e.dataField === 'OrderDate')) {
            if (!isNaN(e.row.key)) {
                e.editorOptions.disabled = true;
            }
        }
    }
    async to_hsaleClick(e) {
        this.postval = new POrderSale();
        this.postval.SaleID = e.row.key;
        const sendRequest = await SendService.sendRequest(this.http, '/ToSale/OrderSale', 'POST', { values: this.postval });
        if (sendRequest) {
            notify({
                message: '銷貨完成',
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'success', 3000);
            this.dataGrid.instance.refresh();
        }
    }
    async to_dsaleClick(e, item) {
        this.tosalepopupVisible = true;
        this.tosaleitemkey = new ToorderSale();
        this.tosaleitemkey.key = item.key;
        this.tosaleitemkey.qty = item.data.Quantity;
        this.tosaleitemkey.MaterialBasicId = item.data.MaterialBasicId;
        this.tosaleitemkey.MaterialNo = item.data.MaterialNo;

        // vvv以下舊的方法停用
        // this.postval = new POrderSale();
        // this.postval.SaleDID = item.key;
        // const sendRequest = await SendService.sendRequest(this.http, '/ToSale/OrderSale', 'POST', { values: this.postval });
        // if (sendRequest) {
        //     notify({
        //         message: '銷貨完成',
        //         position: {
        //             my: 'center top',
        //             at: 'center top'
        //         }
        //     }, 'success', 3000);
        //     this.dataGrid.instance.refresh();
        // }
    }
    async to_redsaleClick(e, item) {
        this.resalepopupVisible = true;
        this.resaleitemkey = new ReorderSale();
        this.resaleitemkey.key = item.key;
        this.resaleitemkey.qty = item.data.Quantity;
        this.resaleitemkey.MaterialBasicId = item.data.MaterialBasicId;
        this.resaleitemkey.MaterialId = item.data.MaterialId;
        this.resaleitemkey.MaterialNo = item.data.MaterialNo;
        // Swal.fire({
        //     allowEnterKey: false,
        //     allowOutsideClick: false,
        //     width: 600,
        //     title: '銷貨退回',
        //     html: shtml + response.message,
        //     icon: 'warning',
        //     showCancelButton: true,
        //     // confirmButtonColor: '#3085d6',
        //     // cancelButtonColor: '#d33',
        //     cancelButtonText: '取消退回',
        //     confirmButtonText: '確認退貨'
        // }).then(async (result) => {
        //     if (result.value) {

        //     } else {

        //     }
        // });

        // this.postval = new POrderSale();
        // this.postval.SaleDID = e.row.key;
        // const sendRequest = await SendService.sendRequest(this.http, '/ToSale/ReOrderSale', 'POST', { values: this.postval });
        // if (sendRequest) {
        //     notify({
        //         message: '銷退完成',
        //         position: {
        //             my: 'center top',
        //             at: 'center top'
        //         }
        //     }, 'success', 3000);
        //     this.dataGrid.instance.refresh();
        // }
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
    onClickQuery(e) {
        this.detailfilter = this.myform.instance.option('formData');
        // this.getdata();
        this.dataGrid.instance.refresh();
    }
    onValueChanged(e) {
        this.onClickQuery(e);
    }
    onDetailsDataErrorOccurred(e) {
        // notify({
        //     message: e.error.message,
        //     position: {
        //         my: 'center top',
        //         at: 'center top'
        //     }
        // }, 'error', 3000);
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
    onRowPrepared(e) {
        if (e.data !== undefined) {
            let hint = false;
            if (e.data.Status === 2) {
                e.rowElement.style.color = '#008800';
            } else {
                if (e.data !== undefined) {
                    const DeliverydateBefore = new Date(e.data.SaleDate);
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
    onEditingStart2(e) {
        this.Quantityval = e.data.Quantity;
        this.OriginPriceval = e.data.OriginPrice;
        this.Priceval = e.data.Price;
    }
    onFocusedRowChanged(e) {
    }

    customizeText(e) {
        return '總數：' + e.value + '筆';
    }
    downloadSaleOrder(e, data) {
        this.saleHeadId = data.key;
        this.Url = '/Api/Report/GetSaleOrderPDF/' + this.saleHeadId;
    }

    clearCreateSale(e){
           this.createSale.dataGrid2.instance.clearSelection();
           this.createSale.cleanDB1();
    }
}
