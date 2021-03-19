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
import { PostOrderMaster_Detail } from 'src/app/model/viewmodels';

@Component({
  selector: 'app-order-saleunfinished',
  templateUrl: './order-saleunfinished.component.html',
  styleUrls: ['./order-saleunfinished.component.css']
})
export class OrderSaleunfinishedComponent implements OnInit {
    @Output() childOuter = new EventEmitter();
    @Input() randomkeyval: any;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild(DxFormComponent, { static: false }) form: DxFormComponent;
    autoNavigateToFocusedRow = true;
    dataSourceDB: any;
    formData: any;
    Controller = '/OrderHeads';
    itemkey: number;
    Supplierlist: any;
    remoteOperations: boolean;
    MaterialList: any;
    OrderTypeList: any;
    editorOptions: any;
    detailfilter: any;
    MaterialBasicList: any;
    CustomerList: any;
    topurchasekey: any;
    toworkkey: any;
    popupVisibleWork: boolean;
    popupVisiblePurchase: boolean;
    popupVisibleSale: boolean;
    Otoworkkey: any;
    selectedOperation: string = "between";
    tosalekey: any;
    mod: string;

    constructor(private http: HttpClient, myservice: Myservice, private app: AppComponent, private titleService: Title) {
        this.remoteOperations = true;
        this.OrderTypeList = myservice.getOrderTypeShow();
        this.editorOptions = { onValueChanged: this.onValueChanged.bind(this) };
    }

  ngOnInit() {
  }
    ngOnChanges() {
        this.getdata();
        // this.app.GetData('/Materials/GetMaterials').subscribe(
        //     (s) => {
        //         if (s.success) {
        //             this.MaterialList = s.data;
        //         }
        //     }
        // );
        this.app.GetData('/MaterialBasics/GetMaterialBasicsAsc').subscribe(
            (s) => {
                if (s.success) {
                    // this.MaterialBasicList = s.data;
                    this.MaterialBasicList = { paginate: true, store: { type: 'array', data: s.data, key: 'Id' } };
                }
            }
        );
        this.app.GetData('/Customers/GetCustomers').subscribe(
            (s) => {
                if (s.success) {
                    this.CustomerList = s.data;
                }
            }
        );
    }
    getdata() {
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) => SendService.sendRequest(
                this.http,
                this.Controller + '/GetOrderData',
                'GET', { loadOptions, remote: this.remoteOperations, detailfilter: this.detailfilter }),
            byKey: (key) => SendService.sendRequest(this.http, this.Controller + '/GetOrderData', 'GET', { key }),
            insert: (values) => SendService.sendRequest(this.http, this.Controller + '/PostAdjustLog', 'POST', { values }),
            update: (key, values) => SendService.sendRequest(this.http, this.Controller + '/PutAdjustLog', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(this.http, this.Controller + '/DeleteAdjustLog/' + key, 'DELETE')
        });
    }
    to_outsideClick(e) {
        // this.topurchasekey = null;
        // this.topurchasekey = this.dataGrid.instance.getSelectedRowsData();
        // this.app.GetData('/Inventory/GetBasicsData').subscribe(
        //     (s) => {
        //         if (s.success) {
        //             this.GetData(s.data, this.topurchasekey);
        //         }
        //     }
        // );

        // if (this.topurchasekey.length === 0) {
        //     Swal.fire({
        //         allowEnterKey: false,
        //         allowOutsideClick: false,
        //         title: '沒有勾選任何訂單項目',
        //         html: '請勾選要轉托工的訂單項目',
        //         icon: 'warning',
        //         timer: 3000
        //     });
        // } else {
        //     Swal.fire({
        //         showCloseButton: true,
        //         allowEnterKey: false,
        //         allowOutsideClick: false,
        //         title: '轉托工',
        //         html: '如需合併托工單，請點選[輸入托工單]!',
        //         icon: 'question',
        //         showCancelButton: true,
        //         confirmButtonColor: '#3085d6',
        //         cancelButtonColor: '#71c016',
        //         cancelButtonText: '輸入托工單',
        //         confirmButtonText: '新建托工單'
        //     }).then(async (result) => {
        //         if (result.value) {
        //             this.mod = 'add-outside';
        //             this.popupVisiblePurchase = true;
        //         } else if (result.dismiss === Swal.DismissReason.cancel) {
        //             this.mod = 'merge-outside';
        //             this.popupVisiblePurchase = true;
        //         } else if (result.dismiss === Swal.DismissReason.close) {
        //             this.popupVisiblePurchase = false;
        //         }
        //     });
        // }
    }
    // async to_workClick(e) {
    //     this.topurchasekey = null;
    //     this.topurchasekey = this.dataGrid.instance.getSelectedRowsData();
    //     if (this.topurchasekey.length === 0) {
    //         Swal.fire({
    //             allowEnterKey: false,
    //             allowOutsideClick: false,
    //             title: '沒有勾選任何訂單項目',
    //             html: '請勾選要轉工單的訂單項目',
    //             icon: 'warning',
    //             timer: 3000
    //         });
    //     } else {
    //         try {
    //             const OrderData = new PostOrderMaster_Detail();
    //             OrderData.orderDetail = this.topurchasekey;

    //             // tslint:disable-next-line: max-line-length
    //             const sendRequest = await SendService.sendRequest(this.http, '/WorkOrders/OrderToWorkOrderCheck', 'POST', { values: OrderData });
    //             if (sendRequest.length !== 0) {
    //                 this.toworkkey = sendRequest;
    //                 this.Otoworkkey = this.topurchasekey;
    //                 this.popupVisibleSale = true;
    //             }
    //         } catch (error) {

    //         }
    //     }
    // }
    to_purchaseClick(e) {
        // this.topurchasekey = null;
        // this.topurchasekey = this.dataGrid.instance.getSelectedRowsData();

        // this.app.GetData('/Inventory/GetBasicsData').subscribe(
        //     (s) => {
        //         if (s.success) {
        //             this.GetBomData(s.data);
        //         }
        //     }
        // );

        // if (this.topurchasekey.length === 0) {
        //     Swal.fire({
        //         allowEnterKey: false,
        //         allowOutsideClick: false,
        //         title: '沒有勾選任何訂單項目',
        //         html: '請勾選要轉採購的訂單項目',
        //         icon: 'warning',
        //         timer: 3000
        //     });
        // } else {
        //     Swal.fire({
        //         showCloseButton: true,
        //         allowEnterKey: false,
        //         allowOutsideClick: false,
        //         title: '轉採購',
        //         html: '如需合併採購單，請點選[輸入採購單]!',
        //         icon: 'question',
        //         showCancelButton: true,
        //         confirmButtonColor: '#3085d6',
        //         cancelButtonColor: '#71c016',
        //         cancelButtonText: '輸入採購單',
        //         confirmButtonText: '新建採購單'
        //     }).then(async (result) => {
        //         if (result.value) {
        //             this.mod = 'add';
        //             this.popupVisiblePurchase = true;
        //         } else if (result.dismiss === Swal.DismissReason.cancel) {
        //             this.mod = 'merge';
        //             this.popupVisiblePurchase = true;
        //         } else if (result.dismiss === Swal.DismissReason.close) {
        //             this.popupVisiblePurchase = false;
        //         }
        //     });
        // }
    }
    to_saleClick(e) {
        this.tosalekey = null;
        this.tosalekey = this.dataGrid.instance.getSelectedRowsData();
        if (this.tosalekey.length === 0) {
            Swal.fire({
                allowEnterKey: false,
                allowOutsideClick: false,
                title: '沒有勾選任何訂單項目',
                html: '請勾選要轉銷貨的訂單項目',
                icon: 'warning',
                timer: 3000
            });
        } else {
            Swal.fire({
                showCloseButton: true,
                allowEnterKey: false,
                allowOutsideClick: false,
                title: '轉銷貨',
                html: '如需合併銷貨單，請點選[輸入銷貨單]!',
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#71c016',
                cancelButtonText: '輸入銷貨單',
                confirmButtonText: '新建銷貨單'
            }).then(async (result) => {
                if (result.value) {
                    this.mod = 'add';
                    this.popupVisibleSale = true;
                } else if (result.dismiss === Swal.DismissReason.cancel) {
                    this.mod = 'merge';
                    this.popupVisibleSale = true;
                } else if (result.dismiss === Swal.DismissReason.close) {
                    this.popupVisibleSale = false;
                }
            });
        }
    }
    popup_result(e) {
        this.popupVisiblePurchase = false;
        this.popupVisibleSale = false;
        this.popupVisibleWork = false;
        this.childOuter.emit(true);
        this.dataGrid.instance.refresh();
        this.dataGrid.instance.clearSelection();
        notify({
            message: '存檔完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
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
        debugger;
        if (e.value === '全部資料') {
            this.dataGrid.instance.clearFilter();
        } else {
            this.dataGrid.instance.filter(['Message', '=', e.value]);
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
    onFocusedRowChanged(e) {
    }
    onCellPrepared(e) {

    }
    onEditorPreparing(e) {
    }
    selectionChanged(e) {
    }

}
