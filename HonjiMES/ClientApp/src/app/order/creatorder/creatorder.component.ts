import { Component, OnInit, ViewChild, Output, EventEmitter, Input, OnChanges } from '@angular/core';
import { APIResponse } from '../../app.module';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import notify from 'devextreme/ui/notify';
import { DxFormComponent, DxDataGridComponent, DxPopupComponent } from 'devextreme-angular';
import { SendService } from '../../shared/mylib';
import { OrderHead, OrderDetail, PostOrderMaster_Detail, CreateNumberInfo } from '../../model/viewmodels';
import CustomStore from 'devextreme/data/custom_store';
import { NgbPaginationNumber } from '@ng-bootstrap/ng-bootstrap';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-creatorder',
    templateUrl: './creatorder.component.html',
    styleUrls: ['./creatorder.component.css']
})
export class CreatorderComponent implements OnInit, OnChanges {

    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() exceldata: any;
    @Input() modval: any;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    buttondisabled = false;
    formData: any;
    SerialNo = 0;
    dataSourceDB: any;
    postval: any;
    labelLocation: string;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    colCount: number;
    width: any;
    ProductList: any;
    url: string;
    editOnkeyPress: boolean;
    enterKeyAction: string;
    enterKeyDirection: string;
    Customerlist: any;
    buttonOptions: any = {
        text: '存檔',
        type: 'success',
        useSubmitBehavior: true,
        icon: 'save'
    };
    selectBoxOptions: any;
    controller: string;
    CustomerVal: string;
    ProductBasicList: any;
    CreateNumberInfoVal: any;
    CreateTime: any;
    CreateOrderNo: any;
    CreateTimeDateBoxOptions: any;
    ProductPricrErrList: any;

    constructor(private http: HttpClient) {
        this.CustomerVal = null;
        this.formData = null;
        this.editOnkeyPress = true;
        this.enterKeyAction = 'moveFocus';
        this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 300;
        this.colCount = 5;
        this.url = location.origin + '/api';
        this.dataSourceDB = [];
        this.controller = '/OrderDetails';
        this.CreateTimeDateBoxOptions = {
            onValueChanged: this.CreateTimeValueChange.bind(this)
        };

        // this.Customerlist = SendRequest.sendRequest(this.http, this.url + '/Customers/GetCustomers' );
        // this.GetData('/Products/GetProducts').subscribe(
        //     (s) => {
        //         console.log(s);
        //         debugger;
        //         if (s.success) {
        //             this.ProductList = s.data;
        //         }
        //     }
        // );
        this.GetData('/Customers/GetCustomers').subscribe(
            (s) => {
                console.log(s);
                if (s.success) {
                    this.Customerlist = s.data;
                    this.selectBoxOptions = {
                        items: this.Customerlist,
                        displayExpr: 'Name',
                        valueExpr: 'Id',
                        onValueChanged: this.onCustomerSelectionChanged.bind(this)
                    };
                }
            }
        );
    }
    ngOnInit() {
    }
    async ngOnChanges() {
        this.GetData('/OrderHeads/GetOrderNumber').subscribe(
            (s) => {
                if (s.success) {
                    this.formData = s.data;
                    this.CreateNumberInfoVal = new CreateNumberInfo();
                    this.CreateNumberInfoVal.CreateNumber = s.data.OrderNo;
                    this.CreateNumberInfoVal.CreateTime = s.data.CreateTime;
                }
            }
        );
        this.ProductList = await SendService.sendRequest(this.http, '/Products/GetProducts');
        this.ProductBasicList = await SendService.sendRequest(this.http, '/Products/GetProductBasics');
        if (this.modval === 'clone') {
            this.GetData('/OrderHeads/GetOrderHead/' + this.itemkeyval).subscribe(
                (s) => {
                    console.log(s);
                    if (s.success) {
                        this.formData = s.data;
                    }
                }
            );
            this.dataSourceDB = new CustomStore({
                key: 'Id',
                load: () => SendService.sendRequest(this.http, this.controller + '/GetOrderDetailsByOrderId?OrderId=' + this.itemkeyval),
                byKey: () => SendService.sendRequest(this.http, this.controller + '/GetOrderDetail'),
                insert: (values) => SendService.sendRequest(this.http, this.controller + '/PostOrderDetail', 'POST', { values }),
                update: (key, values) => SendService.sendRequest(this.http, this.controller + '/PutOrderDetail', 'PUT', { key, values }),
                remove: (key) => SendService.sendRequest(this.http, this.controller + '/DeleteOrderDetail', 'DELETE')
            });
        } else if (this.modval === 'excel') {
            debugger;
            let ProductPricrErr = '';
            if (this.exceldata.Customer === 0) {
                this.exceldata.Customer = null;
            }

            this.ProductPricrErrList = [];
            this.exceldata.OrderDetails.forEach(item => {
                this.SerialNo++;
                if (item.ProductBasicId === 0) {
                    item.ProductBasicId = null;
                } else {
                    const Product = this.ProductBasicList.filter(x => x.Id === item.ProductBasicId)[0];
                    item.DBOriginPrice = Product.Price;
                    item.DBPrice = Product.Price * item.Quantity;
                    if (Product.Price !== item.OriginPrice && !this.ProductPricrErrList.some(x => x === Product.ProductNo)) {
                        this.ProductPricrErrList.push(Product.ProductNo);
                        ProductPricrErr += Product.ProductNo + '：' + Product.Price + '=>' + item.OriginPrice + '<br/>';
                    }
                }
            });

            this.formData = this.exceldata;
            this.formData.OrderNo = this.CreateNumberInfoVal.CreateNumber;
            this.formData.CreateTime = this.CreateNumberInfoVal.CreateTime;
            this.dataSourceDB = this.exceldata.OrderDetails;
            if (ProductPricrErr) {
                Swal.fire({
                    allowEnterKey: false,
                    allowOutsideClick: false,
                    title: '金額不同',
                    icon: 'info',
                    html: '品項=>訂單<br/>' + ProductPricrErr,
                    confirmButtonText: '確認'
                });
            }
        }
    }
    public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>(location.origin + '/api' + apiUrl);
    }
    CreateTimeValueChange = async function(e) {
        // this.formData = this.myform.instance.option('formData');
        if (this.formData.CreateTime != null) {
            this.CreateNumberInfoVal = new CreateNumberInfo();
            this.CreateNumberInfoVal.CreateNumber = this.formData.OrderNo;
            this.CreateNumberInfoVal.CreateTime = this.formData.CreateTime;
            // tslint:disable-next-line: max-line-length
            const sendRequest = await SendService.sendRequest(this.http, '/OrderHeads/GetOrderNumberByInfo', 'POST', { values: this.CreateNumberInfoVal });
            if (sendRequest) {
                this.formData.OrderNo = sendRequest.CreateNumber;
                this.formData.CreateTime = sendRequest.CreateTime;
            }
        }
    };
    onInitNewRow(e) {
        // debugger;
        this.SerialNo++;
        e.data.Serial = this.SerialNo;
    }
    onFocusedCellChanging(e) {
    }
    onCustomerSelectionChanged(e) {
        if (e.value !== null && e.value !== 0) {
            const Customerobj = this.Customerlist.filter(x => x.Id === e.value)[0];
            this.CustomerVal = '代號：\u00A0\u00A0\u00A0' + Customerobj.Code
                + '\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0   電話：\u00A0' + Customerobj.Phone
                + '\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0   傳真：\u00A0' + Customerobj.Fax
                + '\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0   E-mail：\u00A0' + Customerobj.Email
                + '\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0   地址：\u00A0' + Customerobj.Address;
        }
    }
    validate_before(): boolean {
        // 表單驗證
        if (this.myform.instance.validate().isValid === false) {
            notify({
                message: '請注意訂單內容必填的欄位',
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'error', 3000);
            return false;
        }
        return true;
    }
    async onFormSubmit(e) {
        debugger;
        try {
            // this.buttondisabled = true;
            if (this.validate_before() === false) {
                this.buttondisabled = false;
                return;
            }
            this.dataGrid.instance.saveEditData();
            this.formData = this.myform.instance.option('formData');
            const hnull = this.dataSourceDB.find(item => item.ProductBasicId == null);
            if (hnull || (this.SerialNo > 0 && this.dataSourceDB.length < 1)) {
                notify({
                    message: '請注意訂單內容必填的欄位',
                    position: {
                        my: 'center top',
                        at: 'center top'
                    }
                }, 'error', 3000);
                return false;
            } else {
                this.postval = new PostOrderMaster_Detail();
                this.postval.OrderHead = this.formData as OrderHead;
                this.postval.orderDetail = this.dataSourceDB as OrderDetail[];
                // tslint:disable-next-line: max-line-length
                const sendRequest = await SendService.sendRequest(this.http, '/OrderHeads/PostOrderMaster_Detail', 'POST', { values: this.postval });
                // let data = this.client.POST( this.url + '/OrderHeads/PostOrderMaster_Detail').toPromise();
                if (sendRequest) {
                    this.SerialNo = 0;
                    this.dataSourceDB = [];
                    this.dataGrid.instance.refresh();
                    // this.myform.instance.resetValues();
                    this.formData.CreateTime = new Date();
                    this.formData.Customer = 0;
                    this.formData.OrderDate = null;
                    this.formData.ReplyDate = null;
                    this.CustomerVal = null;
                    e.preventDefault();
                    this.childOuter.emit(sendRequest);
                }

            }
        } catch (error) {
        }
        this.buttondisabled = false;
    }
}
