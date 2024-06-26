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
import { AppComponent } from 'src/app/app.component';
import { Myservice } from 'src/app/service/myservice';

@Component({
    selector: 'app-creatorder',
    templateUrl: './creatorder.component.html',
    styleUrls: ['./creatorder.component.css']
})
export class CreatorderComponent implements OnInit, OnChanges {

    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() randomkeyval: any;
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
    MaterialList: any;
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
    MaterialBasicList: any;
    CreateNumberInfoVal: any;
    CreateTime: any;
    CreateOrderNo: any;
    CreateTimeDateBoxOptions: any;
    MaterialPricrErrList: any;
    btnMod: any;
    Quantity: number;
    DBOriginPrice: number;
    OriginPrice: number;
    DBPrice: number;
    Price: number;
    dataGridRowIndex: number;
    OrderTypeVisible: boolean;
    OrderTypeList: any;
    TypeSelectBoxOptions: any;
    gridsaveCheck: any;
    colCountByScreen: Object;

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent) {
        this.OrderTypeList = myservice.getOrderType();
        this.CustomerVal = null;
        this.formData = null;
        this.editOnkeyPress = true;
        this.enterKeyAction = 'moveFocus';
        this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        //this.minColWidth = 300;
        //this.colCount = 25;
        this.dataSourceDB = [];
        this.controller = '/OrderDetails';
        this.Quantity = 0;
        this.DBOriginPrice = 0;
        this.OriginPrice = 0;
        this.DBPrice = 0;
        this.Price = 0;
        this.dataGridRowIndex = 0;
        this.OrderTypeVisible = true;

        this.CreateTimeDateBoxOptions = {
            onValueChanged: this.CreateTimeValueChange.bind(this)
        };
        this.TypeSelectBoxOptions = {
            items: myservice.getOrderType(),
            displayExpr: 'Name',
            valueExpr: 'Id',
        };

        this.colCountByScreen = {
            md: 3,
            sm: 1
        };

        // this.Customerlist = SendRequest.sendRequest(this.http, this.url + '/Customers/GetCustomers' );
        // this.app.GetData('/Materials/GetMaterials').subscribe(
        //     (s) => {
        //         console.log(s);
        //         debugger;
        //         if (s.success) {
        //             this.MaterialList = s.data;
        //         }
        //     }
        // );
    }
    ngOnInit() {
    }
    async ngOnChanges() {
        this.TypeSelectBoxOptions = {
            items: this.OrderTypeList,
            displayExpr: 'Name',
            valueExpr: 'Id',
            value: 0
        };
        this.app.GetData('/Customers/GetCustomers').subscribe(
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
        this.app.GetData('/OrderHeads/GetOrderNumber').subscribe(
            (s) => {
                if (s.success) {
                    this.formData = s.data;
                    this.formData.OrderType = 0;
                    this.CreateNumberInfoVal = new CreateNumberInfo();
                    this.CreateNumberInfoVal.CreateNumber = s.data.OrderNo;
                    this.CreateNumberInfoVal.CreateTime = s.data.CreateTime;
                }
            }
        );
        this.MaterialList = await SendService.sendRequest(this.http, '/Materials/GetMaterials');
        this.MaterialBasicList = await SendService.sendRequest(this.http, '/Materials/GetMaterialBasics');
        if (this.modval === 'clone') {
            this.app.GetData('/OrderHeads/GetOrderHead/' + this.itemkeyval).subscribe(
                (s) => {
                    console.log(s);
                    if (s.success) {
                        this.formData = s.data;
                        this.formData.OrderType = 0;
                    }
                }
            );
            this.app.GetData(this.controller + '/GetOrderDetailsByOrderId?OrderId=' + this.itemkeyval).subscribe(
                (s) => {
                    if (s.success) {
                        this.SerialNo = 0;
                        this.dataGridRowIndex = 0;
                        s.data.forEach(item => {
                            if (item.MaterialBasicId === 0) {
                                item.MaterialBasicId = null;
                            } else {
                                this.SerialNo++;
                                this.dataGridRowIndex++;
                                item.RowIndex = this.dataGridRowIndex;
                            }
                        });
                        this.dataSourceDB = s.data;
                    }
                }
            );
            // this.dataSourceDB = new CustomStore({
            //     key: 'Id',
            //     load: () => SendService.sendRequest(this.http, this.controller + '/GetOrderDetailsByOrderId?OrderId=' + this.itemkeyval),
            //     byKey: () => SendService.sendRequest(this.http, this.controller + '/GetOrderDetail'),
            //     insert: (values) => SendService.sendRequest(this.http, this.controller + '/PostOrderDetail', 'POST', { values }),
            //     update: (key, values) => SendService.sendRequest(this.http, this.controller + '/PutOrderDetail', 'PUT', { key, values }),
            //     remove: (key) => SendService.sendRequest(this.http, this.controller + '/DeleteOrderDetail', 'DELETE')
            // });
        } else if (this.modval === 'excel') {
            debugger;
            let MaterialPricrErr = '';
            if (this.exceldata.Customer === 0) {
                this.exceldata.Customer = null;
            }

            this.SerialNo = 0;
            this.MaterialPricrErrList = [];
            this.exceldata.OrderDetails.forEach(item => {
                this.SerialNo++;
                if (item.MaterialBasicId === 0) {
                    item.MaterialBasicId = null;
                } else {
                    this.dataGridRowIndex++;
                    item.RowIndex = this.dataGridRowIndex;
                    const Material = this.MaterialBasicList.filter(x => x.Id === item.MaterialBasicId)[0];
                    item.DBOriginPrice = Material.Price;
                    item.DBPrice = Material.Price * item.Quantity;
                    if (Material.Price !== item.OriginPrice && !this.MaterialPricrErrList.some(x => x === Material.MaterialNo)) {
                        this.MaterialPricrErrList.push(Material.MaterialNo);
                        MaterialPricrErr += Material.MaterialNo + '：' + Material.Price + '=>' + item.OriginPrice + '<br/>';
                    }
                }
            });
            debugger;
            this.formData = this.exceldata;
            this.formData.OrderNo = this.CreateNumberInfoVal.CreateNumber;
            this.formData.CreateTime = this.CreateNumberInfoVal.CreateTime;
            this.formData.OrderType = 0;
            this.dataSourceDB = this.exceldata.OrderDetails;
            if (MaterialPricrErr) {
                Swal.fire({
                    allowEnterKey: false,
                    allowOutsideClick: false,
                    title: '金額不同',
                    icon: 'info',
                    html: '品項=>訂單<br/>' + MaterialPricrErr,
                    confirmButtonText: '確認'
                });
            }
        }
    }
    CreateTimeValueChange = async function (e) {
        // this.formData = this.myform.instance.option('formData');
        if (this.formData.CreateTime != null) {
            this.CreateNumberInfoVal = new CreateNumberInfo();
            this.CreateNumberInfoVal.CreateNumber = this.formData.OrderNo;
            this.CreateNumberInfoVal.CreateTime = this.formData.CreateTime;
            // tslint:disable-next-line: max-line-length
            const sendRequest = await SendService.sendRequest(this.http, '/OrderHeads/GetOrderNumberByInfo', 'POST', { values: this.CreateNumberInfoVal });
            if (sendRequest) {
                this.formData.OrderNo = sendRequest.CreateNumber;
                // this.formData.CreateTime = sendRequest.CreateTime;
            }
        }
    };
    onValueChanged(e, data) {
        data.setValue(e.value);
        const Material = this.MaterialBasicList.filter(x => x.Id === e.value)[0];
        this.Quantity = 1;
        this.DBOriginPrice = Material.Price;
        this.DBPrice = this.Quantity * Material.Price;
        this.OriginPrice = Material.Price;
        this.Price = this.Quantity * this.OriginPrice;
    }
    QuantityonValueChanged(e, data) {
        data.setValue(e.value);
        this.Quantity = e.value;
        this.DBPrice = this.Quantity * this.DBOriginPrice;
        this.Price = this.Quantity * this.OriginPrice;
    }
    OriginPriceonValueChanged(e, data) {
        data.setValue(e.value);
        this.OriginPrice = e.value;
        this.DBPrice = this.Quantity * this.DBOriginPrice;
        this.Price = this.Quantity * this.OriginPrice;
    }
    saveBtn(e) {
        this.btnMod = 'save';
    }
    removeBtn(e) {
        this.btnMod = 'remove';
    }
    onInitNewRow(e) {
        // debugger;
        this.SerialNo++;
        e.data.Serial = this.SerialNo;
        this.dataGridRowIndex++;
        e.data.RowIndex = this.dataGridRowIndex;
        e.data.Unit = 'EA';
        this.Quantity = 1;
        e.data.Quantity = this.Quantity;

        this.DBOriginPrice = 0;
        this.DBPrice = 0;
        this.OriginPrice = 0;
        this.Price = 0;
        e.data.DBOriginPrice = this.DBOriginPrice;
        e.data.DBPrice = this.DBPrice;
        e.data.OriginPrice = this.OriginPrice;
        e.data.Price = this.Price;
    }
    onEditingStart(e) {
        this.Quantity = e.data.Quantity;
        this.DBOriginPrice = e.data.DBOriginPrice;
        this.DBPrice = e.data.DBPrice;
        this.OriginPrice = e.data.OriginPrice;
        this.Price = e.data.Price;
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
        // debugger;
        try {
            this.gridsaveCheck = true;
            if (this.btnMod === 'save') {
                // this.buttondisabled = true;
                if (this.validate_before() === false) {
                    this.buttondisabled = false;
                    return;
                }
                this.dataGrid.instance.saveEditData();

                if (!this.gridsaveCheck) {
                    this.buttondisabled = false;
                    return;
                }
                this.formData = this.myform.instance.option('formData');
                const hnull = this.dataSourceDB.find(item => item.MaterialBasicId == null);
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
                        this.modval = null;
                        e.preventDefault();
                        this.childOuter.emit(sendRequest);
                    }
                }
            } else if (this.btnMod === 'remove') {
                this.dataSourceDB = [];
            }
        } catch (error) {
        }
        this.buttondisabled = false;
    }
    onRowValidating(e) {
        debugger;
        if (!e.isValid) {
            this.gridsaveCheck = false;
        }
    }
    QuantitysetCellValue(newData, value, currentRowData) {
        newData.Quantity = value;
        newData.Price = value * currentRowData.OriginPrice;
        newData.DBPrice = value * currentRowData.DBOriginPrice;
        if (isNaN(newData.Price)) {
            newData.Price = null;
        }
        if (isNaN(newData.DBPrice)) {
            newData.DBPrice = null;
        }
    }
    OriginPricesetCellValue(newData, value, currentRowData) {
        newData.OriginPrice = value;
        newData.Price = currentRowData.Quantity * value;
        if (isNaN(newData.Price)) {
            newData.Price = null;
        }
    }
    DBOriginPricesetCellValue(newData, value, currentRowData) {
        newData.DBOriginPrice = value;
        newData.DBPrice = currentRowData.Quantity * value;
        if (isNaN(newData.DBPrice)) {
            newData.DBPrice = null;
        }
    }

    screen(width) {
        return width < 720 ? "sm" : "md";
    }
}
