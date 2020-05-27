import { Component, OnInit, ViewChild } from '@angular/core';
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
@Component({
  selector: 'app-sale-list',
  templateUrl: './sale-list.component.html',
  styleUrls: ['./sale-list.component.css']
})
export class SaleListComponent implements OnInit {

    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxFileUploaderComponent) uploader: DxFileUploaderComponent;
    autoNavigateToFocusedRow = true;
    dataSourceDB: any;
    Customerlist: any;
    ProductList: any;
    DetailsDataSourceStorage: any;
    creatpopupVisible = false;
    resalepopupVisible = false;
    tosalepopupVisible = false;
    formData: any;
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
    ProductBasicList: any;
    remoteOperations: boolean;
    detailfilter = [];
    editorOptions: any;
    constructor(private http: HttpClient, myservice: Myservice) {
        this.remoteOperations = true;
        this.listSaleOrderStatus = myservice.getSaleOrderStatus();
        this.cloneIconClick = this.cloneIconClick.bind(this);
        this.to_hsaleClick = this.to_hsaleClick.bind(this);
        this.to_dsaleClick = this.to_dsaleClick.bind(this);
        this.onValueChanged = this.onValueChanged.bind(this);
        this.DetailsDataSourceStorage = [];
        this.getdata();
        this.editorOptions = { onValueChanged: this.onValueChanged };

        this.GetData('/Customers/GetCustomers').subscribe(
            (s) => {
                if (s.success) {
                    this.Customerlist = s.data;
                }
            }
        );
        this.GetData('/Products/GetProducts').subscribe(
            (s) => {
                this.ProductList = s.data;
                if (s.success) {
                }
            }
        );
        this.GetData('/Products/GetProductBasics').subscribe(
            (s) => {
                this.ProductBasicList = s.data;
                if (s.success) {
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
            remove: (key) => SendService.sendRequest(this.http, this.Controller + '/DeleteSale', 'DELETE')
        });

    }
    // tslint:disable-next-line: use-lifecycle-interface
    ngOnChanges() {

    }
    getDetails(SaleId) {
        const DController = '/SaleDetailNews';
        let item = this.DetailsDataSourceStorage.find((i) => i.key === SaleId);
        if (!item) {
            item = {
                key: SaleId,
                DetaildataSource:  new CustomStore({
                    key: 'Id',
                    load: () => SendService.sendRequest(this.http, DController + '/GetSaleDetailsBySaleId?SaleId=' + SaleId),
                    byKey: (key) => SendService.sendRequest(this.http, DController + '/GetSaleDetailNew', 'GET', { key }),
                    insert: (values) => SendService.sendRequest(this.http, DController + '/PostSaleDetailNew', 'POST', { values }),
                    // tslint:disable-next-line: max-line-length
                    update: (key, values) => SendService.sendRequest(this.http, DController + '/PutSaleDetailNewQty', 'PUT', { key, values }),
                    remove: (key) => SendService.sendRequest(this.http, DController + '/DeleteSaleDetailNew', 'DELETE')
                })
              };
            this.DetailsDataSourceStorage.push(item);
          }
        return item.DetaildataSource;
    }
    public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>(location.origin + '/api' + apiUrl);
    }
    completedValue(rowData) {
        // tslint:disable-next-line: triple-equals
        return rowData.Status == 'Completed';
    }

    onCellPrepared(e) {
        // tslint:disable-next-line: no-debugger
        //     debugger;
        //     if (e.rowType === 'header' && e.column.command === 'select') {
        //         alert('testheader');
        //  }
    }
    allowEdit(e) {
        if (e.row.data.Status === 1) {
            return false;
        } else {
            return true;
        }
    }
    addDetail(e) {
        this.dataGrid.instance.addRow();
    }
    cloneIconClick(e) {
        // debugger;
        this.itemkey = e.row.key;

        this.mod = 'clone';
        this.creatpopupVisible = true;
        // e.event.preventDefault();
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
        // debugger;
        this.tosalepopupVisible = true;
        this.tosaleitemkey = new ToorderSale();
        this.tosaleitemkey.key = item.key;
        this.tosaleitemkey.qty = item.data.Quantity;
        this.tosaleitemkey.ProductBasicId = item.data.ProductBasicId;
        this.tosaleitemkey.ProductNo = item.data.ProductNo;

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
        // debugger;
        this.resalepopupVisible = true;
        this.resaleitemkey = new ReorderSale();
        this.resaleitemkey.key = item.key;
        this.resaleitemkey.qty = item.data.Quantity;
        this.resaleitemkey.ProductId = item.data.ProductId;
        this.resaleitemkey.ProductNo = item.data.ProductNo;
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
    onUploaded(e) {
        const response = JSON.parse(e.request.response) as APIResponse;
        if (response.success) {
            if (response.message) {
                const shtml = '品號 / 品名 不存在，請先新增成品資訊!<br/>';
                Swal.fire({
                    allowEnterKey: false,
                    allowOutsideClick: false,
                    width: 600,
                    title: '是否新增品號 ?',
                    html: shtml + response.message,
                    icon: 'warning',
                    showCancelButton: true,
                    // confirmButtonColor: '#3085d6',
                    // cancelButtonColor: '#d33',
                    cancelButtonText: '取消匯入',
                    confirmButtonText: '確認新增'
                }).then(async (result) => {
                    if (result.value) {
                        // tslint:disable-next-line: new-parens
                        const postval =  {OrderNo : response.data.OrderNo, Products : response.message};
                        // tslint:disable-next-line: max-line-length
                        const sendRequest = await SendService.sendRequest(this.http, this.Controller + '/PostCreatProductByExcel', 'POST', { values: postval });
                        // let data = this.client.POST( this.url + '/OrderHeads/PostOrderMaster_Detail').toPromise();
                        if (sendRequest) {
                            this.mod = 'excel';
                            this.creatpopupVisible = true;
                            this.exceldata = sendRequest;
                        }
                    } else {
                        this.uploader.instance.reset();
                    }
                });
            } else {
                this.mod = 'excel';
                this.creatpopupVisible = true;
                this.exceldata = response.data;
            }

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
    onDataErrorOccurred(e) {
        debugger;
        notify({
            message: e.error.message,
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'error', 3000);
    }
    onClickQuery(e) {
        debugger;
        this.detailfilter = this.myform.instance.option('formData');
        // this.getdata();
        this.dataGrid.instance.refresh();
    }
    onValueChanged(e) {
        this.onClickQuery(e);
    }
    onDetailsDataErrorOccurred(e) {
        // debugger;
        // notify({
        //     message: e.error.message,
        //     position: {
        //         my: 'center top',
        //         at: 'center top'
        //     }
        // }, 'error', 3000);
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
        if (e.rowType === 'data') {
        }
    }
    onEditingStart(e) {

    }
    onFocusedRowChanged(e) {
    }

  ngOnInit() {
  }

}
