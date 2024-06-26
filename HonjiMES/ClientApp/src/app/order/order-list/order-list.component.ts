import { NgModule, Component, OnInit, ViewChild } from '@angular/core';
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
import { Myservice } from '../../service/myservice';
import { AppComponent } from 'src/app/app.component';
import { AuthService } from 'src/app/service/auth.service';
import { Title } from '@angular/platform-browser';
import { CreatorderComponent } from '../creatorder/creatorder.component';
import { OrderOverviewComponent } from '../order-overview/order-overview.component';
import { OrderSaleunfinishedComponent } from '../order-saleunfinished/order-saleunfinished.component';

@Component({
    selector: 'app-order-list',
    templateUrl: './order-list.component.html',
    styleUrls: ['./order-list.component.css']
})
export class OrderListComponent {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxFileUploaderComponent) uploader: DxFileUploaderComponent;
    @ViewChild(CreatorderComponent) creatorder: CreatorderComponent;
    @ViewChild(OrderOverviewComponent) orderOverview: OrderOverviewComponent;
    @ViewChild(OrderSaleunfinishedComponent) orderSaleunfinished: OrderSaleunfinishedComponent;
    autoNavigateToFocusedRow = true;
    dataSourceDB: any;
    Customerlist: any;
    MaterialList: any;
    DetailsDataSourceStorage: any;
    creatpopupVisible = false;
    formData: any;
    dataSourceDBDetails: any[];
    itemkey: number;
    mod: string;
    uploadUrl: string;
    exceldata: any;
    Controller = '/OrderHeads';
    MaterialBasicList: any;
    detailfilter = [];
    listOrderStatus: any;
    remoteOperations: boolean;
    editorOptions: any;
    uploadHeaders: any;
    UserList: any;
    randomkey: number;
    OrderTypeList: any;
    overviewpopupVisible = false;
    overRandomkey: number;
    selectedOperation: string = "between";
    saleunfinshedVisible = false;
    clearOverviewSelection: boolean;
    clearCondition: { cancel: any; toggle: boolean; };
    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent, private titleService: Title) {
        const authenticationService = new AuthService(http);
        const currentUser = authenticationService.currentUserValue;
        this.uploadHeaders = {
            Authorization: 'Bearer ' + currentUser.Token,
            routerLink: location.pathname,
            apitype: 'POST'
        };
        this.remoteOperations = true;
        this.listOrderStatus = myservice.getOrderStatus();
        this.OrderTypeList = myservice.getOrderTypeShow();
        this.uploadUrl = location.origin + '/api/OrderHeads/PostOrdeByExcel';
        this.cloneIconClick = this.cloneIconClick.bind(this);
        this.onValueChanged = this.onValueChanged.bind(this);
        this.DetailsDataSourceStorage = [];
        this.getdata();
        this.getMaterialsData();
        this.editorOptions = { onValueChanged: this.onValueChanged };
        this.checkData = this.checkData.bind(this);
        this.CheckFlagIconClick = this.CheckFlagIconClick.bind(this);
        this.app.GetData('/Customers/GetCustomers').subscribe(
            (s) => {
                if (s.success) {
                    this.Customerlist = s.data;
                }
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
    // tslint:disable-next-line: use-lifecycle-interface
    ngOnInit() {
        this.titleService.setTitle('客戶訂單');
    }
    getdata() {
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) => {
                // loadOptions.sort = [{ selector: 'WorkOrderNo', desc: true }];
                // if (loadOptions.searchValue) {
                // loadOptions.filter = [
                //     ['CreateTime', '>=', oldDay],
                //     'and',
                //     ['CreateTime', '<=', toDay],
                // ];
                // }
                return SendService.sendRequest(
                    this.http,
                    this.Controller + '/GetOrderHeads',
                    'GET', { loadOptions, remote: this.remoteOperations, detailfilter: this.detailfilter });
            },
            byKey: (key) => SendService.sendRequest(this.http, this.Controller + '/GetOrderHead', 'GET', { key }),
            insert: (values) => SendService.sendRequest(this.http, this.Controller + '/PostOrderHead', 'POST', { values }),
            update: (key, values) => SendService.sendRequest(this.http, this.Controller + '/PutOrderHead', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(this.http, this.Controller + '/DeleteOrderHead/' + key, 'DELETE')
        });
    }
    getMaterialsData() {
        this.app.GetData('/Materials/GetMaterials').subscribe(
            (s) => {
                if (s.success) {
                    this.MaterialList = s.data;
                }
            }
        );
        this.app.GetData('/Materials/GetMaterialBasics').subscribe(
            (s) => {
                if (s.success) {
                    this.MaterialBasicList = s.data;
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
        this.randomkey = new Date().getTime();
        this.creatpopupVisible = true;
    }
    overviewpopup(e) {
        this.overRandomkey = new Date().getTime();
        this.overviewpopupVisible = true;
    }
    updatepopup_result(e) {
        this.dataGrid.instance.refresh();
    }
    creatpopup_result(e) {
        this.creatpopupVisible = false;
        this.dataGrid.instance.refresh();
        if (e.message !== undefined) {
            notify({
                message: '注意!! 客戶單號已存在!! ' + e.message,
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'warning', 6000);
        } else {
            notify({
                message: '存檔完成!',
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'success', 3000);
        }
    }
    overpopup_result(e) {
        this.overviewpopupVisible = false;
        this.dataGrid.instance.refresh();
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
        this.app.GetData('/Customers/GetCustomers').subscribe(
            // (s) => {
            //   console.log(s);
            //   this.Customerlist = s.data;
            //   if (s.success) {

            //   }
            // }
        );
    }
    onUploaded(e) {
        debugger;
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
                        // tslint:disable-next-line: max-line-length
                        const postval = { OrderNo: response.data.OrderNo, Materials: response.message, CustomerNo: response.data.CustomerNo };
                        // tslint:disable-next-line: max-line-length
                        const sendRequest = await SendService.sendRequest(this.http, this.Controller + '/PostCreatMaterialByExcel', 'POST', { values: postval });
                        // let data = this.client.POST( this.url + '/OrderHeads/PostOrderMaster_Detail').toPromise();
                        if (sendRequest) {
                            this.uploader.instance.reset();
                            this.mod = 'excel';
                            this.creatpopupVisible = true;
                            this.exceldata = sendRequest;
                        }
                    } else {
                    }
                    this.getMaterialsData();
                });
            } else {
                this.uploader.instance.reset();
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

    //detail只能展開一個下拉式視窗
    onFocusedRowChanging(e) {
        // debugger;
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
        if (e.data !== undefined) {
            if (e.data.Status === 1) {
                e.rowElement.style.color = '#008800';
            } else {
                if (e.data !== undefined) {
                    e.data.OrderDetails.forEach(element => {
                        const DeliverydateBefore = new Date(element.DueDate);
                        const DeliverydateAfter = new Date(new Date().setDate(new Date().getDate() - 1));
                        if (DeliverydateBefore <= DeliverydateAfter) {
                            e.rowElement.style.color = '#d9534f';
                        }
                    });
                }
            }
        }
    }
    checkData(e, data) {
        const sendRequest = SendService.sendRequest(this.http, this.Controller + '/CheckData', 'PUT', { key: data.key });
        if (sendRequest) {
            this.dataGrid.instance.refresh();
        }
    }
    onEditingStart(e) {
    }
    onFocusedRowChanged(e) {
    }
    onClickQuery(e) {
        debugger;
        this.detailfilter = this.myform.instance.option('formData');
        this.dataGrid.instance.refresh();
    }
    CheckFlagVisible(e) {
        if (e.row.data.CheckFlag === 1) {
            return true;
        } else {
            return false;
        }
    }
    unCheckFlagVisible(e) {
        if (e.row.data.CheckFlag !== 1) {
            return true;
        } else {
            return false;
        }
    }
    async CheckFlagIconClick(e) {
        try {
            const sendRequest = await SendService.sendRequest(this.http, this.Controller + '/CheckData', 'PUT', { key: e.row.key });
            if (sendRequest) {
                this.dataGrid.instance.refresh();
            }
        } catch (error) {

        }
    }
    onValueChanged(e) {
        this.onClickQuery(e);
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
    saleunfinishedpopup(e) {
        this.overRandomkey = new Date().getTime();
        this.saleunfinshedVisible = true;
    }

    unsalepopup_result(e) {
        this.saleunfinshedVisible = false;
        this.dataGrid.instance.refresh();
    }

    clearOverview(e){
        this.orderOverview.dataGrid.instance.clearSelection();
    }

    clearCreatorder(e){
        this.creatorder.dataGrid.instance.cancelEditData();
    }

    clearSaleunfinished(e){
        this.orderSaleunfinished.dataGrid.instance.clearSelection();
    }

}
