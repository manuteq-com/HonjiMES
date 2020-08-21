import { Component, OnInit, OnChanges, Output, EventEmitter, ViewChild } from '@angular/core';
import { DxFormComponent, DxDataGridComponent } from 'devextreme-angular';
import { Customer } from 'src/app/model/viewmodels';
import CustomStore from 'devextreme/data/custom_store';
import { HttpClient } from '@angular/common/http';
import { AppComponent } from 'src/app/app.component';
import { SendService } from 'src/app/shared/mylib';
import notify from 'devextreme/ui/notify';

@Component({
    selector: 'app-creatreback',
    templateUrl: './creatreback.component.html',
    styleUrls: ['./creatreback.component.css']
})
export class CreatrebackComponent implements OnInit, OnChanges {

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
    Controller = '/Rebacks';
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
        this.Warehouselist = new CustomStore({
            key: 'Id',
            load: () =>
                SendService.sendRequest(this.http, '/Warehouses/GetWarehouses')
        });
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
            this.buttondisabled = false;
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
        this.app.PostData(this.Controller + '/PostRebacksDetailAll', postdata).toPromise()
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
        this.app.GetData(this.Controller + '/GetRebacksDetailMaterialByWorkOrderNo/' + e.value).subscribe(
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
        this.buttondisabled = true;
        let msg = '';
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
            }
        }
        this.buttondisabled = false;
        return true;
    }
    onToolbarPreparing(e) {
        const toolbarItems = e.toolbarOptions.items;
        toolbarItems.forEach(item => {
            if (item.name === 'saveButton') {
                item.options.icon = '';
                item.options.text = '退料';
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
