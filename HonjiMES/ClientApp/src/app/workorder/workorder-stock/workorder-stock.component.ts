import { Component, OnInit, EventEmitter, Output, Input, ViewChild, OnChanges, HostListener } from '@angular/core';
import { DxFormComponent, DxDataGridComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import CustomStore from 'devextreme/data/custom_store';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { APIResponse } from 'src/app/app.module';
import { InventoryChange, workOrderReportData } from 'src/app/model/viewmodels';
import { SendService } from 'src/app/shared/mylib';
import { AppComponent } from 'src/app/app.component';

@Component({
    selector: 'app-workorder-stock',
    templateUrl: './workorder-stock.component.html',
    styleUrls: ['./workorder-stock.component.css']
})
export class WorkorderStockComponent implements OnInit, OnChanges {
    @Output() childOuter = new EventEmitter();
    @Input() workorderkeyval: any;
    @Input() itemkeyval: any;
    @Input() userkeyval: any;
    @Input() modkeyval: any;
    @Input() randomkeyval: any;
    @Input() popupkeyval: any;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    buttondisabled = false;
    formData: any;
    postval: any;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    colCount: number;
    width: any;
    ProductList: any;
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
    labelLocation: string;
    itemval1: string;
    itemval2: string;
    itemval3: string;
    itemval4: string;
    itemval5: string;
    minval: number;
    warehousesList: any;
    warehousesOptions: any;
    QuantityEditorOptions: any;
    keyup = '';
    UserList: any;
    UserEditorOptions: { items: any; displayExpr: string; valueExpr: string; value: number; searchEnabled: boolean; disable: boolean; };

    @HostListener('window:keyup', ['$event']) keyUp(e: KeyboardEvent) {
        if (this.popupkeyval) {
            if (e.key === 'Enter') {
                const key = this.keyup;
                const promise = new Promise((resolve, reject) => {
                    this.app.GetData('/Users/GetUserByUserNo?DataNo=' + key).toPromise().then((res: APIResponse) => {
                        if (res.success) {
                            if (res.data.Permission === 70 || res.data.Permission === 20) {
                                this.app.GetData('/Users/GetUsers').subscribe(
                                    (s) => {
                                        if (s.success) {
                                            this.UserList = [];
                                            // 過濾帳戶身分。(因此畫面是使用共用帳戶，但登記人員必須是個人身分)
                                            s.data.forEach(element => {
                                                if (element.Permission === 20 && element.Id === res.data.Id) {
                                                    this.UserList.push(element);
                                                    s.data.forEach(element2 => {
                                                        if (element2.Permission === 70) {
                                                            this.UserList.push(element2);
                                                        }
                                                    });
                                                } else if (element.Permission === 70 && element.Id === res.data.Id) {
                                                    this.UserList.push(element);
                                                }
                                            });
                                            this.SetUserEditorOptions(this.UserList, res.data.Id);
                                        }
                                    }
                                );
                            } else {
                                this.showMessage('warning', '請勿越權使用!', 3000);
                            }
                        } else {
                            this.showMessage('error', '查無資料!', 3000);
                        }
                    },
                        err => {
                            // Error
                            reject(err);
                        }
                    );
                });
                this.keyup = '';
            } else if (e.key === 'Shift') {

            } else {
                this.keyup += e.key.toLocaleUpperCase();
            }
        }
    }

    constructor(private http: HttpClient, public app: AppComponent) {
        // debugger;
        this.CustomerVal = null;
        this.formData = null;
        this.editOnkeyPress = true;
        this.enterKeyAction = 'moveFocus';
        this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 100;
        this.colCount = 1;
        this.warehousesList = [];
    }
    ngOnInit() {
    }
    ngOnChanges() {
        this.itemval1 = '';
        this.itemval2 = '';
        this.itemval3 = '';
        this.itemval4 = '';
        this.minval = 0;
        this.QuantityEditorOptions = {
            showSpinButtons: true,
            mode: 'number',
            format: '#0',
            value: 0,
            min: 0,
            // onValueChanged: this.QuantityValueChanged.bind(this)
        };

        if (this.itemkeyval !== 0) {
            this.app.GetData('/Warehouses/GetWarehouses').subscribe(
                (s) => {
                    if (s.success) {
                        s.data.forEach(e => {
                            e.Name = e.Code + e.Name;
                        });
                        this.warehousesList = s.data;
                        this.GetDataInfo();
                    }
                }
            );
        }
        this.app.GetData('/Users/GetUsers').subscribe(
            (s) => {
                if (s.success) {
                    this.UserList = [];
                    let tempUser = null;
                    if (this.userkeyval !== null) {
                        tempUser = this.userkeyval;
                    }

                    // 過濾帳戶身分。(因此畫面是使用共用帳戶，但登記人員必須是個人身分)
                    s.data.forEach(element => {
                        if (element.Permission === 20 && element.Id === tempUser) {
                            this.UserList.push(element);
                            s.data.forEach(element2 => {
                                if (element2.Permission === 70) {
                                    this.UserList.push(element2);
                                }
                            });
                        } else if (element.Permission === 70 && element.Id === tempUser) {
                            this.UserList.push(element);
                        }
                    });
                    this.SetUserEditorOptions(this.UserList, tempUser);
                }
            }
        );
    }
    GetDataInfo() {
        if (this.modkeyval === 'material') {
            this.app.GetData('/MaterialBasics/GetMaterialBasic/' + this.itemkeyval).subscribe(
                (s) => {
                    console.log(s);
                    if (s.success) {
                        const totalCount = this.GetTotalCount(s.data.Materials);
                        this.itemval1 = '　　　元件料號：' + s.data.MaterialNo;
                        this.itemval2 = '　　　元件品名：' + s.data.Name;
                        this.itemval3 = '　　　元件規格：' + s.data.Specification;
                        this.itemval4 = '　　　總庫存數：' + totalCount;
                    }
                }
            );
        } else if (this.modkeyval === 'product') {
            this.app.GetData('/ProductBasics/GetProductBasic/' + this.itemkeyval).subscribe(
                (s) => {
                    if (s.success) {
                        const totalCount = this.GetTotalCount(s.data.Products);
                        this.itemval1 = '　　　成品料號：' + s.data.ProductNo;
                        this.itemval2 = '　　　成品品名：' + s.data.Name;
                        this.itemval3 = '　　　成品規格：' + s.data.Specification;
                        this.itemval4 = '　　　總庫存數：' + totalCount;
                    }
                }
            );
        } else if (this.modkeyval === 'wiproduct') {
            this.app.GetData('/WiproductBasics/GetWiproductBasic/' + this.itemkeyval).subscribe(
                (s) => {
                    console.log(s);
                    if (s.success) {
                        const totalCount = this.GetTotalCount(s.data.Wiproducts);
                        this.itemval1 = '　　半成品料號：' + s.data.WiproductNo;
                        this.itemval2 = '　　半成品品名：' + s.data.Name;
                        this.itemval3 = '　　半成品規格：' + s.data.Specification;
                        this.itemval4 = '　　　總庫存數：' + totalCount;
                    }
                }
            );
        }
    }
    GetTotalCount(data) {
        let totalCount = 0;
        data.forEach(element => {
            totalCount = totalCount + element.Quantity;
            this.UpdateWarehousesList(element);
        });
        return totalCount;
    }
    SetUserEditorOptions(List, IdVal) {
        this.UserEditorOptions = {
            items: List,
            displayExpr: 'Realname',
            valueExpr: 'Id',
            value: IdVal,
            searchEnabled: true,
            disable: false
        };
    }
    UpdateWarehousesList(data) {
        if (data !== null) {
            const index = this.warehousesList.findIndex(x => x.Id === data.WarehouseId);
            this.warehousesList[index].Name += ' (庫存 ' + data.Quantity + ')';
        }
        this.warehousesOptions = {
            items: this.warehousesList,
            displayExpr: 'Name',
            valueExpr: 'Id',
            value: this.warehousesList.find(x => x.Code === '301')?.Id ?? null // 預設成品倉301
        };
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
    onFormSubmit = async function(e) {
        // this.buttondisabled = true;
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }
        this.formData = this.myform.instance.option('formData');
        const Data = new workOrderReportData();
        Data.CreateUser = this. formData.CreateUser;
        Data.ReCount = this.formData.Quantity;
        Data.WarehouseId = this.formData.WarehouseId;
        if (Data.ReCount === 0) {
            this.showMessage('warning', '增減數量不能為 "0" !', 3000);
            return;
        }
        // tslint:disable-next-line: max-line-length
        const sendRequest = await SendService.sendRequest(this.http, '/WorkOrders/StockWorkOrder', 'PUT', { key: this.workorderkeyval, values: Data });
        // let data = this.client.POST( this.url + '/OrderHeads/PostOrderMaster_Detail').toPromise();
        if (sendRequest) {
            this.childOuter.emit(true);
        }
    };
    showMessage(type, data, val) {
        notify({
            message: data,
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, type, val);
    }
}
