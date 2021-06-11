import { Component, OnInit, Output, EventEmitter, ViewChild, OnChanges, Input, HostListener } from '@angular/core';
import { DxFormComponent, DxDataGridComponent } from 'devextreme-angular';
import { Customer } from 'src/app/model/viewmodels';
import { HttpClient } from '@angular/common/http';
import { AppComponent } from 'src/app/app.component';
import notify from 'devextreme/ui/notify';
import { SendService } from 'src/app/shared/mylib';
import CustomStore from 'devextreme/data/custom_store';
import { APIResponse } from 'src/app/app.module';

@Component({
    selector: 'app-creatreceive',
    templateUrl: './creatreceive.component.html',
    styleUrls: ['./creatreceive.component.css']
})
export class CreatreceiveComponent implements OnInit, OnChanges {

    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() randomkeyval: any;
    @Input() popupkeyval: any;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    buttondisabled = false;
    formData: any;
    postval: Customer;
    labelLocation: string;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    colCount: number;
    width: any;
    Controller = '/Requisitions';
    buttonOptions: any = {
        text: '存檔',
        type: 'success',
        useSubmitBehavior: true,
        icon: 'save'
    };
    NumberBoxOptions: { showSpinButtons: boolean; mode: string; min: number; value: number; };
    dataSourceAllDB: any;
    Warehouselist: any;
    RQtyEditorOptions: { showSpinButtons: boolean; mode: string; format: string; value: number; min: number; };
    popupVisible: boolean;
    randomkey: number;
    popupMod: string;
    newVisible: boolean;
    CreateUserList: any;
    ReceiveUserList: any;
    editorOptions: {};
    keyup = '';
    UserTurn: number;
    selectUserDefault: any;
    selectUser: any;
    AttrValue: string;
    tempCreateUserId: number;

    @HostListener('window:keyup', ['$event']) keyUp(e: KeyboardEvent) {
        if (this.popupkeyval) {
            if (e.key === 'Enter') {
                const key = this.keyup;
                const promise = new Promise((resolve, reject) => {
                    this.app.GetData('/Users/GetUserByUserNo?DataNo=' + key).toPromise().then((res: APIResponse) => {
                        if (res.success) {
                            // if (res.data.Permission === 60 || res.data.Permission === 20) {
                            this.app.GetData('/Users/GetUsers').subscribe(
                                (s) => {
                                    if (s.success) {
                                        const temp = [];
                                        s.data.forEach(element => {
                                            if (element.Permission > 10) {
                                                temp.push(element);
                                            }
                                        });
                                        this.CreateUserList = [];
                                        this.ReceiveUserList = temp;
                                        temp.forEach(element => {
                                            // tslint:disable-next-line: max-line-length
                                            if (element.Permission === 20 && element.Id === res.data.Id || element.Id === this.tempCreateUserId) {
                                                this.CreateUserList.push(element);
                                            } else if (element.Permission === 60) {
                                                this.CreateUserList.push(element);
                                            }
                                        });
                                        if (this.UserTurn === 0) {
                                            this.UserTurn = 1;
                                            this.tempCreateUserId = res.data.Id + 0;
                                            this.SetCreateUserEditorOptions(this.CreateUserList, res.data.Id, '');
                                            this.SetReceiveUserEditorOptions(this.ReceiveUserList, null, this.AttrValue);
                                        } else {
                                            this.UserTurn = 2;
                                            this.SetCreateUserEditorOptions(this.CreateUserList, this.tempCreateUserId, '');
                                            this.SetReceiveUserEditorOptions(this.ReceiveUserList, res.data.Id, '');
                                        }
                                    }
                                }
                            );
                            // } else {
                            //     this.showMessage('warning', '請勿越權使用!', 3000);
                            // }
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
            } else if (e.key === 'Shift' || e.key === 'CapsLock') {

            } else {
                this.keyup += e.key.toLocaleUpperCase();
            }
        }
    }

    constructor(private http: HttpClient, public app: AppComponent) {
        this.RQtyValidation = this.RQtyValidation.bind(this);
        this.formData = null;
        // this.editOnkeyPress = true;
        // this.enterKeyAction = 'moveFocus';
        // this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 100;
        this.colCount = 3;
        this.dataSourceAllDB = [];
        this.NumberBoxOptions = { showSpinButtons: true, mode: 'number', min: 0, value: 0 };
        this.RQtyEditorOptions = {
            showSpinButtons: true,
            mode: 'number',
            format: '#0.0',
            value: 0,
            min: 0
        };
        this.AttrValue = 'background-color: yellow; text-transform: uppercase';
        this.tempCreateUserId = 0;
        // this.Warehouselist = new CustomStore({
        //     key: 'Id',
        //     load: () =>
        //         SendService.sendRequest(this.http, '/Warehouses/GetWarehouses')
        // });
        this.app.GetData('/Warehouses/GetWarehouses').subscribe(
            (s) => {
                if (s.success) {
                    s.data.forEach(e => {
                        e.Name = e.Code + e.Name;
                    });
                    this.Warehouselist = s.data;
                }
            }
        );
        this.newVisible = false;
    }
    ngOnInit() {
    }
    ngOnChanges() {
        if (this.popupkeyval) {
            this.UserTurn = 0;
            this.dataSourceAllDB = [];
            this.newVisible = false;
            this.GetWorkOrderList();
            this.app.GetData('/Users/GetUsers').subscribe(
                (s) => {
                    if (s.success) {
                        const temp = [];
                        s.data.forEach(element => {
                            if (element.Permission > 10) {
                                temp.push(element);
                            }
                        });
                        // this.CreateUserList = s.data;
                        this.SetReceiveUserEditorOptions(temp, null, '');
                        this.CreateUserList = [];
                        temp.forEach(element => {
                            if (element.Permission === 60) {
                                this.CreateUserList.push(element);
                            }
                        });
                        this.SetCreateUserEditorOptions(this.CreateUserList, this.app.GetUserId(), this.AttrValue);
                    }
                }
            );
            if (this.itemkeyval && this.itemkeyval > 0) {
                const val = { value: this.itemkeyval };
                this.onValueChanged(val);
            }
        }
    }
    GetWorkOrderList() {
        this.editorOptions = {
            onValueChanged: this.onValueChanged.bind(this),
            dataSource: new CustomStore({
                key: 'Id',
                load: (loadOptions) => {
                    // loadOptions.take = 20;
                    loadOptions.sort = [{ selector: 'WorkOrderNo', desc: true }];
                    if (loadOptions.searchValue) {
                        loadOptions.filter = [loadOptions.searchExpr, loadOptions.searchOperation, loadOptions.searchValue];
                    }
                    if (this.itemkeyval !== null) {
                        loadOptions.filter = ['Id', '=', this.itemkeyval];
                    }
                    return SendService.sendRequest(this.http, '/WorkOrders/GetWorkOrderHeadsRun/', 'GET', { loadOptions, remote: true });
                },
                byKey: (key) => {
                    if (this.itemkeyval !== null) {
                        return SendService.sendRequest(this.http, '/WorkOrders/GetWorkOrderHeadsRunById', 'GET', { key });
                    }
                },
            }),
            valueExpr: 'Id',
            displayExpr: 'WorkOrderNo',
            searchEnabled: true,
            searchExpr: 'WorkOrderNo',
            // searchMode: 'startswith',
            value: this.itemkeyval
        };
    }
    SetReceiveUserEditorOptions(List, IdVal, AttrVal) {
        this.selectUser = {
            items: List,
            displayExpr: 'Realname',
            valueExpr: 'Id',
            value: IdVal,
            searchEnabled: true,
            inputAttr: { style: AttrVal },
            onValueChanged: this.onSelectUserSelectionChanged.bind(this)
        };
    }
    SetCreateUserEditorOptions(List, IdVal, AttrVal) {
        this.selectUserDefault = {
            items: List,
            displayExpr: 'Realname',
            valueExpr: 'Id',
            value: IdVal,
            searchEnabled: true,
            inputAttr: { style: AttrVal }
        };
    }
    onSelectUserSelectionChanged(e) {
        this.buttondisabled = false;
    }
    validate_before(): boolean {
        // 表單驗證
        if (this.myform.instance.validate().isValid === false) {
            notify({
                message: '請注意內容必填的欄位',
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'error', 3000);
            return false;
        }
        return true;
    }
    async onValueChanged(e) {
        this.buttondisabled = false;
        this.app.GetData(this.Controller + '/GetRequisitionsDetailMaterialByWorkOrderNoId/' + e.value).subscribe(
            (s) => {
                if (s.success) {
                    s.data.forEach(element => {
                        // 注意! 預設自動帶倉別
                        if (element.NameType === '成品') {
                            element.WarehouseId = this.Warehouselist.find(x => x.Code === '301').Id;
                        } else if (element.NameType === '元件') {
                            element.WarehouseId = this.Warehouselist.find(x => x.Code === '101').Id;
                        }
                        element.RQty = 0;

                        if (element.Master === 1) {
                            element.NameNo += ' (主要用料)';
                        }
                    });
                    this.dataSourceAllDB = s.data;
                    this.newVisible = true;
                }
            }
        );
    }
    allowDeleting(e) {
        if (e.row.data.Quantity !== null) {
            return false;
        } else {
            return true;
        }
    }
    cellClick(e) {
        if (e.rowType === 'header') {
            if (e.column.type === 'buttons') {
                if (e.column.cssClass === 'addmod') {
                    this.popupVisible = true;
                    this.randomkey = new Date().getTime();
                    this.popupMod = 'receive';
                }
            }
        } else if (e.rowType === 'header' && e.rowType === 'data') {
            // // tslint:disable-next-line: deprecation
            // this.dataGrid.instance.insertRow();
        }
    }
    WarehouseselectvalueChanged(e, data) {
        data.setValue(e.value);
    }
    GetWarehouselistbyNo(data) {
        return data.data.WarehouseList;
    }
    GetWarehouseStockQty(data) {
        if (data.value) {
            if (data.data.WarehouseList.find(x => x.ID === data.value) !== undefined) {
                return data.data.WarehouseList.find(x => x.ID === data.value).StockQty ?? 0;
            }
        } else {
            return data.value;
        }
    }
    WarehouseValidation(e) {
        let msg = '';
        this.buttondisabled = true;
        if (e.data.RQty > 0) {
            if (!e.data.WarehouseId || e.data.WarehouseId < 1) {
                msg = '請選擇倉庫';
                e.rule.message = msg;
                notify({
                    message: msg,
                    position: {
                        my: 'center top',
                        at: 'center top'
                    }
                }, 'error', 3000);

                return false;
            }
        }
        this.buttondisabled = false;
        return true;
    }
    RQtyValidation(e) {
        let msg = '';
        this.buttondisabled = true;
        if (e.data.WarehouseId > 0) {
            if (e.data.RQty == null || e.data.RQty < 0) {
                msg = e.data.NameNo + '領取數量 必須大於等於0';
                e.rule.message = msg;
                notify({
                    message: msg,
                    position: {
                        my: 'center top',
                        at: 'center top'
                    }
                }, 'error', 3000);

                return false;
            } else {
                const StockQty = e.data.WarehouseList.find(x => x.ID === e.data.WarehouseId).StockQty;
                if (e.data.RQty > StockQty) {
                    msg = e.data.NameNo + ' 庫存數不足';
                    e.rule.message = msg;
                    notify({
                        message: msg,
                        position: {
                            my: 'center top',
                            at: 'center top'
                        }
                    }, 'error', 3000);

                    return false;
                }
            }
        }
        //  else if (e.data.RQty > 0) {
        //     msg = '請選擇倉庫';
        //     notify({
        //         message: msg,
        //         position: {
        //             my: 'center top',
        //             at: 'center top'
        //         }
        //     }, 'error', 3000);
        //     return false;
        // }
        this.buttondisabled = false;
        return true;
    }
    onToolbarPreparing(e) {
        const toolbarItems = e.toolbarOptions.items;
        toolbarItems.forEach(item => {
            if (item.name === 'saveButton') {
                item.options.icon = '';
                item.options.text = '領料';
                item.showText = 'always';
                item.visible = false;

            } else if (item.name === 'revertButton') {
                item.options.icon = '';
                item.options.text = '取消';
                item.showText = 'always';
            }
        });
    }
    popup_result(e) {
        const newData = {
            Id: e.Id,
            // Ismaterial: e.,
            // Lv: e.,
            // MaterialBasic: e.,
            MaterialBasicId: e.MaterialBasicId,
            // MaterialName: e.,
            // MaterialNo: e.,
            // MaterialSpecification: e.,
            // Name: e.,
            NameNo: e.NameNo,
            NameType: e.NameType,
            // ProductBasic: e.,
            ProductBasicId: e.ProductBasicId,
            // ProductName: e.,
            // ProductNo: e.,
            // ProductNumber: e.,
            // ProductSpecification: e.,
            Quantity: null,
            RQty: e.RQty,
            // RbackQty: e.,
            ReceiveQty: e.ReceiveQty,
            // Receives: e.,
            // Remarks: e.,
            // Requisition: e.,
            // RequisitionId: e.,
            // StockQty: e.,
            // UpdateTime: e.,
            // UpdateUser: e.,
            WarehouseId: e.WarehouseId,
            WarehouseList: e.WarehouseList
        };
        if (this.dataSourceAllDB.findIndex(x => x.NameNo === newData.NameNo) === -1) {
            this.dataSourceAllDB.push(newData);
            this.popupVisible = false;
        } else {
            notify({
                message: '該品號已存在清單!',
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'error', 3000);
        }
    }
    async onFormSubmit(e) {
        let cansave = true;
        let msg = '';
        this.buttondisabled = true;
        if (this.validate_before() === false) {
            return;
        }
        this.dataGrid.instance.saveEditData();
        let allCount = 0;
        this.dataSourceAllDB.forEach(x => {
            if (x.RQty > 0 && (!x.WarehouseId || x.WarehouseId < 0)) {
                msg += x.NameNo + ':請選擇倉庫\r\n';
                this.buttondisabled = false;
                cansave = false;
                return false;
            } else if (x.WarehouseId > 0 && x.RQty < 0) {
                msg += x.NameNo + ':請輸入數量 \r\n';
                this.buttondisabled = false;
                cansave = false;
                return false;
            }
            if (x.RQty) {
                allCount += x.RQty;
            }
        });
        if (allCount === 0) {
            msg += '請輸入數量! \r\n';
            this.buttondisabled = false;
            cansave = false;
        }
        if (!cansave) {
            notify({
                message: msg,
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'error', 3000);
            return;
        }
        this.formData = this.myform.instance.option('formData');
        // 新增資料
        const postdata = this.formData;
        postdata.ReceiveList = this.dataSourceAllDB;
        this.buttondisabled = true;
        this.app.PostData(this.Controller + '/PostRequisitionsDetailAll', postdata).toPromise()
            .then((ReturnData: any) => {
                if (ReturnData.success) {
                    this.myform.instance.resetValues();
                    this.dataSourceAllDB = null;
                    e.preventDefault();
                    this.childOuter.emit(true);
                } else {
                    notify({
                        message: ReturnData.message,
                        position: {
                            my: 'center top',
                            at: 'center top'
                        }
                    }, 'error');
                    this.buttondisabled = false;
                }
            })
            .catch(ex => {
                notify({
                    message: ex.message,
                    position: {
                        my: 'center top',
                        at: 'center top'
                    }
                }, 'error');

            });
        this.buttondisabled = false;
    }
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
