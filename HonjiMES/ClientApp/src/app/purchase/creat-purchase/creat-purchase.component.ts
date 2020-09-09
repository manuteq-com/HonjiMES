import { Component, OnInit, OnChanges, Output, Input, EventEmitter, ViewChild } from '@angular/core';
import { DxFormComponent, DxDataGridComponent, DxButtonComponent } from 'devextreme-angular';
import { HttpClient } from '@angular/common/http';
import { APIResponse } from '../../app.module';
import { Observable } from 'rxjs';
import notify from 'devextreme/ui/notify';
import { SendService } from '../../shared/mylib';
import { Myservice } from '../../service/myservice';
import { Button } from 'primeng';
import { CreateNumberInfo } from 'src/app/model/viewmodels';
import { AppComponent } from 'src/app/app.component';

@Component({
    selector: 'app-creat-purchase',
    templateUrl: './creat-purchase.component.html',
    styleUrls: ['./creat-purchase.component.css']
})
export class CreatPurchaseComponent implements OnInit, OnChanges {

    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() exceldata: any;
    @Input() dataSource: any;
    @Input() modval: any;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild('dataGrid2') dataGrid2: DxDataGridComponent;
    @ViewChild('myButton') myButton: DxButtonComponent;
    buttondisabled = false;
    CustomerVal: any;
    formData: any;
    editOnkeyPress: boolean;
    enterKeyAction: string;
    enterKeyDirection: string;
    labelLocation: string;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    width: any;
    colCount: number;
    url: string;
    dataSourceDB: any[];
    controller: string;
    CreateTimeDateBoxOptions: any;
    PurchaseDateBoxOptions: any;
    SupplierselectBoxOptions: any;
    MaterialselectBoxOptions: any;
    SupplierList: any;
    MaterialBasicList: any;
    BasicDataList: any;
    WarehouseList: any;
    WarehouseListAll: any;
    PurchasetypeList: any;
    TypeselectBoxOptions: any;
    Warehouseval: any;
    WarehousevalA: any;
    WarehousevalB: any;
    DataType: number;
    Quantityval: number;
    OriginPriceval: number;
    Priceval: number;
    addRow = true;
    eformData: any;
    CreateNumberInfoVal: any;
    Quantityvalmax: number;
    saveCheck = true;
    showdisabled: boolean;
    Serial: number;
    Purchaselist: any;
    selectBoxOptions: any;
    // onCellPreparedLevel: number;
    listAdjustStatus: any;
    OrderTypeVisible: boolean;
    DeliveryTime: Date;

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent) {
        this.listAdjustStatus = myservice.getlistAdjustStatus();
        this.PurchasetypeList = myservice.getpurchasetypes();
        this.CustomerVal = null;
        this.formData = null;
        this.editOnkeyPress = true;
        this.enterKeyAction = 'moveFocus';
        this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 300;
        this.colCount = 3;
        this.dataSourceDB = [];
        this.controller = '/OrderDetails';
        this.OrderTypeVisible = false;
        this.CreateTimeDateBoxOptions = {
            onValueChanged: this.CreateTimeValueChange.bind(this)
        };
        this.PurchaseDateBoxOptions = {
            // displayFormat: 'yyyyMMdd',
            onValueChanged: this.PurchaseDateValueChange.bind(this)
        };
        this.app.GetData('/Suppliers/GetSuppliers').subscribe(
            (s) => {
                if (s.success) {
                    this.SupplierList = s.data;
                    this.SupplierList.forEach(x => {
                        x.Name = x.Code + '_' + x.Name;
                    });
                    this.SupplierselectBoxOptions = {
                        items: this.SupplierList,
                        displayExpr: 'Name',
                        valueExpr: 'Id',
                        searchEnabled: true,
                        onValueChanged: this.onSupplierSelectionChanged.bind(this)
                    };
                }
            }
        );
        // this.app.GetData('/MaterialBasics/GetMaterialBasicsAsc').subscribe(
        //     (s) => {
        //         if (s.success) {
        //             this.MaterialBasicList = s.data;
        //         }
        //     }
        // );
        this.app.GetData('/Inventory/GetBasicsData').subscribe(
            (s) => {
                if (s.success) {
                    this.BasicDataList = s.data;
                }
            }
        );
        this.app.GetData('/Warehouses/GetWarehouses').subscribe(
            (s) => {
                s.data.forEach(e => {
                    e.Name = e.Code + e.Name;
                });
                this.WarehouseListAll = s.data;
            }
        );
        this.TypeselectBoxOptions = {
            items: this.PurchasetypeList,
            displayExpr: 'Name',
            valueExpr: 'Id',
            searchEnabled: true,
            onValueChanged: this.onTypeSelectionChanged.bind(this)
        };

    }
    ngOnInit() {

    }
    ngOnChanges() {
        this.dataSourceDB = [];
        this.Purchaselist = [];
        if (this.dataSource !== undefined) {
            this.dataSourceDB = this.dataSource;
        }
        if (this.modval === 'merge') {
            this.showdisabled = true;
            this.app.GetData('/PurchaseHeads/GetPurchasesByStatus?status=0').subscribe(
                (s) => {
                    console.log(s);
                    if (s.success) {
                        this.Purchaselist = s.data;
                        this.selectBoxOptions = {
                            // searchMode: 'startswith',
                            searchEnabled: true,
                            items: this.Purchaselist,
                            displayExpr: 'PurchaseNo',
                            valueExpr: 'Id',
                            onValueChanged: this.onSelectionChanged.bind(this)
                        };
                    }
                }
            );
        } else if (this.modval === 'merge-outside') {
            this.showdisabled = true;
            this.app.GetData('/PurchaseHeads/GetPurchasesOutsideByStatus?status=0').subscribe(
                (s) => {
                    console.log(s);
                    if (s.success) {
                        this.Purchaselist = s.data;
                        this.selectBoxOptions = {
                            // searchMode: 'startswith',
                            searchEnabled: true,
                            items: this.Purchaselist,
                            displayExpr: 'PurchaseNo',
                            valueExpr: 'Id',
                            onValueChanged: this.onSelectionChanged.bind(this)
                        };
                    }
                }
            );
        } else {
            this.showdisabled = false;
        }
        this.app.GetData('/PurchaseHeads/GetPurchaseNumber').subscribe(
            (s) => {
                if (s.success) {
                    this.formData = s.data;
                    this.formData.Id = 0;
                    this.formData.SupplierId = null;
                    this.formData.PurchaseDate = new Date();
                    if (this.modval === 'workorder') {
                        this.formData.Type = 30;
                    } else if (this.modval === 'add-outside') {
                        this.formData.Type = 20;
                    }
                }
            }
        );
    }
    onInitialized(value, data) {
        data.setValue(value);
    }
    onFocusedCellChanging(e) {
    }
    onSelectionChanged(e) {
        if (this.Purchaselist.length !== 0 && e.value !== 0) {
            this.formData.SupplierId = this.Purchaselist.find(x => x.Id === e.value).SupplierId;
        }
    }
    onTypeSelectionChanged(e) {
        this.CreateTimeValueChange(e);
    }
    CreateTimeValueChange = async function (e) {
        // this.formData = this.myform.instance.option('formData');
        if (this.formData.CreateTime != null) {
            if (this.formData.Type === 30) {
                this.OrderTypeVisible = true;
            } else {
                this.OrderTypeVisible = false;
            }
            this.CreateNumberInfoVal = new CreateNumberInfo();
            this.CreateNumberInfoVal.Type = this.formData.Type;
            this.CreateNumberInfoVal.CreateNumber = this.formData.PurchaseNo;
            this.CreateNumberInfoVal.CreateTime = this.formData.CreateTime;
            // tslint:disable-next-line: max-line-length
            const sendRequest = await SendService.sendRequest(this.http, '/PurchaseHeads/GetPurchaseNumberByInfo', 'POST', { values: this.CreateNumberInfoVal });
            if (sendRequest) {
                this.formData.Type = sendRequest.Type;
                this.formData.PurchaseNo = sendRequest.CreateNumber;
                // this.formData.CreateTime = sendRequest.CreateTime;
            }
        }
    };
    PurchaseDateValueChange(e) {
    }
    onSupplierSelectionChanged(e) {//依照供應商ID去進貨單查詢，可進貨的原料項目。
        // this.app.GetData('/ToPurchase/GetCanPurchase/' + e.value).subscribe(
        //     (s) => {
        //         if (s.success) {
        //             this.MaterialBasicList = s.data;
        //             this.Quantityvalmax = null;
        //             this.Quantityval = null;
        //             this.OriginPriceval = null;
        //             this.Priceval = null;
        //             if (this.addRow) {
        //                 this.dataGrid.instance.addRow();
        //                 this.addRow = false;
        //             }
        //         }
        //     }
        // );
    }
    selectvalueChanged(e, data) {
        data.setValue(e.value);
        const basicData = this.BasicDataList.find(z => z.TempId === e.value);
        this.DataType = basicData.DataType;
        const dataId = basicData.DataId;
        this.Quantityvalmax = 999;
        this.Quantityval = 1;
        this.OriginPriceval = basicData.Price ? basicData.Price : 0;
        this.Priceval = basicData.Price ? basicData.Price : 0;

        if (this.DataType === 1) {   // 查詢原料
            this.app.GetData('/Warehouses/GetWarehouseListByMaterialBasic/' + dataId).subscribe(
                (s) => {
                    this.WarehouseList = s.data;
                    this.UpdateVal();
                }
            );
        } else if (this.DataType === 2) {    // 查詢成品
            this.app.GetData('/Warehouses/GetWarehouseListByProductBasic/' + dataId).subscribe(
                (s) => {
                    this.WarehouseList = s.data;
                    this.UpdateVal();
                }
            );
        } else if (this.DataType === 3) {    // 查詢半成品
            this.app.GetData('/Warehouses/GetWarehouseListByWiproductBasic/' + dataId).subscribe(
                (s) => {
                    this.WarehouseList = s.data;
                    this.UpdateVal();
                }
            );
        }
    }
    UpdateVal() {
        this.Warehouseval = this.WarehouseList.find(x => x.Code === '301')?.Id ?? null; // 預設成品倉301
        this.WarehousevalA = this.WarehouseList.find(x => x.Code === '201')?.Id ?? null; // 預設轉出倉201
        this.WarehousevalB = this.WarehouseList.find(x => x.Code === '202')?.Id ?? null; // 預設轉入倉202
    }
    WarehousevalvalueChanged(e, data) {
        data.setValue(e.value);
    }
    QuantityValueChanged(e, data) {
        data.setValue(e.value);
        this.Quantityval = e.value;
        this.Priceval = this.Quantityval * this.OriginPriceval;
    }
    OriginValueChanged(e, data) {
        data.setValue(e.value);
        this.OriginPriceval = e.value;
        this.Priceval = this.Quantityval * this.OriginPriceval;
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
    onInitNewRow(e) {
        this.saveCheck = false;
        // this.onCellPreparedLevel = 1;
        this.Quantityval = e.data.Quantity;
        this.OriginPriceval = e.data.OriginPrice;
        this.Priceval = e.data.Price;
        this.WarehouseList = null;
        this.DeliveryTime = new Date();
    }
    onEditingStart(e) {
        this.saveCheck = false;
        // this.onCellPreparedLevel = 1;
        const basicData = this.BasicDataList.find(z => z.TempId === e.data.TempId);
        this.DataType = basicData.DataType;
        const dataId = basicData.DataId;
        this.Quantityval = e.data.Quantity;
        this.OriginPriceval = e.data.OriginPrice;
        this.Priceval = e.data.Price;
        this.Warehouseval = e.data.WarehouseId;
        this.WarehousevalA = e.data.WarehouseIdA;
        this.WarehousevalB = e.data.WarehouseIdB;
        this.DeliveryTime = e.data.DeliveryTime;

        if (this.DataType === 1) {   // 查詢原料
            this.app.GetData('/Warehouses/GetWarehouseListByMaterialBasic/' + dataId).subscribe(
                (s) => {
                    this.WarehouseList = s.data;
                }
            );
        } else if (this.DataType === 2) {    // 查詢成品
            this.app.GetData('/Warehouses/GetWarehouseListByProductBasic/' + dataId).subscribe(
                (s) => {
                    this.WarehouseList = s.data;
                }
            );
        } else if (this.DataType === 3) {    // 查詢半成品
            this.app.GetData('/Warehouses/GetWarehouseListByWiproductBasic/' + dataId).subscribe(
                (s) => {
                    this.WarehouseList = s.data;
                }
            );
        }
    }
    onCellPrepared(e) {
        if (e.column.command === 'edit') {
            this.saveCheck = true;
            // if (this.onCellPreparedLevel === 1) {
            //     this.onCellPreparedLevel = 2;
            // } else if (this.onCellPreparedLevel === 2) {
            //     this.onCellPreparedLevel = 3;
            //     this.saveCheck = true;
            // } else if (this.onCellPreparedLevel === 3) {
            //     this.myButton.instance.focus();
            // }
        }
    }
    onFormSubmit = async function(e) {
        // debugger;
        this.buttondisabled = true;
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }
        if (!this.saveCheck) {
            this.buttondisabled = false;
            return;
        }
        this.dataGrid.instance.saveEditData();
        this.formData = this.myform.instance.option('formData');
        let dataCheck = true;
        this.dataSourceDB.forEach(element => {
            const basicData = this.BasicDataList.find(z => z.TempId === element.TempId);
            element.DataType = basicData.DataType;
            element.DataId = basicData.DataId;

            // 如採購單種類為[表處]，則需確認倉別資訊。
            if (this.formData.Type === 30) {
                if (element.WarehouseIdA === undefined || element.WarehouseIdB === undefined) {
                    dataCheck = false;
                }
            }
        });
        if (!dataCheck) {
            notify({
                message: '請注意訂單內容必填的欄位',
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'error', 3000);
            this.buttondisabled = false;
            return;
        } else {
            // if (this.formData.SupplierId == null) {
            //     this.formData.SupplierId = 0;
            // }
            this.postval = {
                PurchaseHead: this.formData,
                PurchaseDetails: this.dataSourceDB
            };
            try {
                // tslint:disable-next-line: max-line-length
                const sendRequest = await SendService.sendRequest(this.http, '/ToPurchase/PostPurchaseMaster_Detail', 'POST', { values: this.postval });
                if (sendRequest) {
                    this.dataSourceDB = [];
                    this.dataGrid.instance.refresh();
                    // this.myform.instance.resetValues();
                    this.formData.CreateTime = new Date();
                    this.formData.PurchaseDate = new Date();
                    this.formData.SupplierId = null;
                    this.formData.Type = 10;
                    this.formData.Remarks = '';
                    e.preventDefault();
                    this.childOuter.emit(true);
                }
            } catch (error) {
            }
        }
        this.buttondisabled = false;
    };
}
