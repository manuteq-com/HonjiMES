import {NgModule, Component, OnInit, ViewChild } from '@angular/core';
import { APIResponse } from '../../app.module';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import CustomStore from 'devextreme/data/custom_store';
import DataSource from 'devextreme/data/data_source';
import ArrayStore from 'devextreme/data/array_store';
import { DxDataGridComponent, DxFormComponent, DxPopupComponent, DxFileUploaderComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { SendService } from '../../shared/mylib';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-order-list',
    templateUrl: './order-list.component.html',
    styleUrls: ['./order-list.component.css']
})
export class OrderListComponent {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild(DxFormComponent, { static: false }) form: DxFormComponent;
    @ViewChild(DxFileUploaderComponent) uploader: DxFileUploaderComponent;
    autoNavigateToFocusedRow = true;
    dataSourceDB: any;
    Customerlist: any;
    ProductList: any;
    DetailsDataSourceStorage: any;
    creatpopupVisible = false;
    formData: any;
    dataSourceDBDetails: any[];
    itemkey: number;
    mod: string;
    uploadUrl: string;
    exceldata: any;
    Controller = '/OrderHeads';
    ProductBasicList: any;
    constructor(private http: HttpClient) {

        this.uploadUrl = location.origin + '/api/OrderHeads/PostOrdeByExcel';
        this.cloneIconClick = this.cloneIconClick.bind(this);
        this.DetailsDataSourceStorage = [];
        // this.dataSourceDB = new CustomStore({
        //     key: 'Id',
        //     load: () => SendService.sendRequest( http, '/OrderHeads/GetOrderHeads'),
        //     byKey: () => SendService.sendRequest( http, '/OrderHeads/GetOrderHeads'),
        //     insert: (values) => SendService.sendRequest( http, '/OrderHeads/PostOrderHead', 'POST', { values }),
        //     update: (key, values) => SendService.sendRequest( http, '/OrderHeads/PutOrderHead', 'PUT', { key, values }),
        //     remove: (key) => SendService.sendRequest( http, '/OrderHeads/DeleteOrderHead', 'DELETE')
        // });
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: () => SendService.sendRequest(http, this.Controller + '/GetOrderHeads'),
            byKey: (key) => SendService.sendRequest(http, this.Controller + '/GetOrderHead', 'GET', { key }),
            insert: (values) => SendService.sendRequest(http, this.Controller + '/PostOrderHead', 'POST', { values }),
            update: (key, values) => SendService.sendRequest(http, this.Controller + '/PutOrderHead', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(http, this.Controller + '/DeleteOrderHead/' + key, 'DELETE')
        });

        this.GetData('/Customers/GetCustomers').subscribe(
            (s) => {
                if (s.success) {
                    this.Customerlist = s.data;
                }
            }
        );
        // this.GetData(this.url + '/OrderDetails/GetOrderDetails').subscribe(
        //     (s) => {
        //         console.log(s);
        //         this.dataSourceDBDetails = s.data;
        //         if (s.success) {

        //         }
        //     }
        // );
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
    getDetails(key) {
        let item = this.DetailsDataSourceStorage.find((i) => i.key === key);
        if (!item) {
            item = {
                key,
                dataSourceInstance: new DataSource({
                    store: new ArrayStore({
                        data: this.dataSourceDBDetails,
                        key: 'Id'
                      }),
                      filter: ['OrderId', '=', key]
                  })
              };
            this.DetailsDataSourceStorage.push(item);
          }
        return item.dataSourceInstance;
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
        if (e.parentType === 'dataRow' && (e.dataField === 'OrderNo' || e.dataField === 'CustomerNo')) {
            if (!isNaN(e.row.key)) {
                e.editorOptions.disabled = true;
            }
        }
    }
    to_saleClick(e) {
        this.GetData('/Customers/GetCustomers').subscribe(
            // (s) => {
            //   console.log(s);
            //   this.Customerlist = s.data;
            //   if (s.success) {

            //   }
            // }
        );
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
    //   onEditorPreparing(e) {
    //     if (e.parentType === 'dataRow' && (e.dataField === 'Id')) {
    //       e.editorOptions.disabled = true;
    //     }
    //   }
    //   sendRequest(url: string, method: string = 'GET', data: any = {}): any {

    //     const body = JSON.stringify(data.values);
    //     const keyurl = '/' + data.key;
    //     const httpOptions = { withCredentials: true, body, headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    //     let result;

    //     switch (method) {
    //       case 'GET':
    //         result = this.http.get(url, httpOptions);
    //         break;
    //       case 'PUT':
    //         result = this.http.put(url + keyurl, body, httpOptions);
    //         break;
    //       case 'POST':
    //         result = this.http.post(url, body, httpOptions);
    //         break;
    //       case 'DELETE':
    //         result = this.http.delete(url + keyurl, httpOptions);
    //         break;
    //     }
    //     return result
    //       .toPromise()
    //       // tslint:disable-next-line: no-shadowed-variable
    //       .then((data: any) => {
    //         return method === 'GET' ? data.data : data;
    //       })
    //       .catch(e => {
    //         throw e && e.error && e.error.Message;
    //       });
    //   }

}
