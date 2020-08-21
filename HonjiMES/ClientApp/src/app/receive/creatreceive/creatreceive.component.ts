import { Component, OnInit, Output, EventEmitter, ViewChild, OnChanges } from '@angular/core';
import { DxFormComponent, DxDataGridComponent } from 'devextreme-angular';
import { Customer } from 'src/app/model/viewmodels';
import { HttpClient } from '@angular/common/http';
import { AppComponent } from 'src/app/app.component';
import notify from 'devextreme/ui/notify';
import { SendService } from 'src/app/shared/mylib';
import CustomStore from 'devextreme/data/custom_store';

@Component({
    selector: 'app-creatreceive',
    templateUrl: './creatreceive.component.html',
    styleUrls: ['./creatreceive.component.css']
})
export class CreatreceiveComponent implements OnInit, OnChanges {

    @Output() childOuter = new EventEmitter();
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
    editorOptions: {};
    dataSourceAllDB: any;
    Warehouselist: CustomStore;
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
        this.colCount = 1;
        this.NumberBoxOptions = { showSpinButtons: true, mode: 'number', min: 0, value: 0 };
        this.editorOptions = {
            onValueChanged: this.onValueChanged.bind(this),
            dataSource: new CustomStore({
                key: 'Id',
                load: (loadOptions) => {
                    loadOptions.take = 20;
                    loadOptions.sort = [{ selector: 'WorkOrderNo', desc: true }];
                    if (loadOptions.searchValue) {
                        loadOptions.filter = [loadOptions.searchExpr, loadOptions.searchOperation, loadOptions.searchValue];
                    }
                    return SendService.sendRequest(this.http, '/WorkOrders/GetWorkOrderHeadsRun/', 'GET', { loadOptions, remote: true });
                },
            }),
            valueExpr: 'Id',
            displayExpr: 'WorkOrderNo',
            searchEnabled: true,
            searchExpr: 'WorkOrderNo',
            // searchMode: 'startswith',


        };
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
    }
    ngOnChanges() {

    }
    ngOnInit() {
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
    async onFormSubmit(e) {
        let cansave = true;
        let msg = '';
        this.buttondisabled = true;
        if (this.validate_before() === false) {
            return;
        }
        this.dataGrid.instance.saveEditData();
        this.dataSourceAllDB.forEach(x => {
            if (x.RQty > 0 && (!x.WarehouseId || x.WarehouseId < 0)) {
                msg += x.NameNo + ':請選擇倉庫\r\n';
                this.buttondisabled = false;
                cansave = false;
                return false;
            } else if (x.WarehouseId > 0 && (!x.RQty || x.RQty < 1)) {
                msg += x.NameNo + ':請輸入數量 \r\n';
                this.buttondisabled = false;
                cansave = false;
                return false;
            }

        });
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
    async onValueChanged(e) {
        this.buttondisabled = false;
        this.app.GetData(this.Controller + '/GetRequisitionsDetailMaterialByWorkOrderNo/' + e.value).subscribe(
            (s) => {
                if (s.success) {
                    this.dataSourceAllDB = s.data;
                }
            }
        );
    }
    cellClick(e) {
        if (e.rowType === 'header') {
            if (e.column.type === 'buttons') {
                if (e.column.cssClass === 'addmod') {
                    // tslint:disable-next-line: deprecation
                    this.dataGrid.instance.insertRow();
                }
            }
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
            return data.data.WarehouseList.find(x => x.ID === data.value).StockQty ?? 0;
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
            if (e.data.RQty == null || e.data.RQty < 1) {
                msg = e.data.NameNo + '領取數量 必須大於0';
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
}