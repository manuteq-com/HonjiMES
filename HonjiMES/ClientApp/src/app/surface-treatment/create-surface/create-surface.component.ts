import { FormBuilder } from '@angular/forms';
import { element } from 'protractor';
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
  selector: 'app-create-surface',
  templateUrl: './create-surface.component.html',
  styleUrls: ['./create-surface.component.css']
})
export class CreateSurfaceComponent implements OnInit, OnChanges {

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
    supplierdisabled: boolean;
    Serial: number;
    Purchaselist: any;
    selectBoxOptions: any;
    // onCellPreparedLevel: number;
    listAdjustStatus: any;
    WarehouseIdAVisible: boolean;
    WarehouseIdBVisible: boolean;
    DeliveryTime: Date;
    TypeDisabled: boolean;
    gridsaveCheck: boolean;
    Okval: number;
    NotOkval: number;
    Repairval: number;
    Unrepairval: number;
    InNGval: number;
    OutNGval: number;
    Deliveredval: number;
    Undeliveredval: number;

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent) {
        this.requiredfun = this.requiredfun.bind(this);
        this.onRowValidating = this.onRowValidating.bind(this);
        this.onInitNewRow = this.onInitNewRow.bind(this);

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
        this.WarehouseIdAVisible = false;
        this.WarehouseIdBVisible = false;
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
                    this.TypeDisabled = false;
                    this.formData = s.data;
                    this.formData.Id = 0;
                    this.formData.SupplierId = null;
                    this.formData.CreateTime = new Date();
                    this.formData.PurchaseDate = new Date();
                    //新增表處單採購類別固定為表處
                    this.formData.Type = 30;
                    this.TypeDisabled = true;
                    // if (this.modval === 'workorder') {
                    //     this.formData.Type = 30;
                    // } else if (this.modval === 'add-outside') {
                    //     this.formData.Type = 20;
                    //     this.TypeDisabled = true;
                    // }
                    this.TypeselectBoxOptions = {
                        items: this.PurchasetypeList,
                        displayExpr: 'Name',
                        valueExpr: 'Id',
                        searchEnabled: true,
                        disabled: this.TypeDisabled,
                        onValueChanged: this.onTypeSelectionChanged.bind(this)
                    };
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
                this.WarehouseIdAVisible = true;
                this.WarehouseIdBVisible = true;
            } else if (this.formData.Type === 40) {
                this.WarehouseIdAVisible = true;
                this.WarehouseIdBVisible = false;
            } else {
                this.WarehouseIdAVisible = false;
                this.WarehouseIdBVisible = false;
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
        this.Deliveredval = 0;
        this.Undeliveredval = 0;
        this.Okval = 0;
        this.NotOkval = 0;
        this.Repairval = 0;
        this.Unrepairval = 0;
        this.InNGval = 0;
        this.OutNGval = 0;
        this.OriginPriceval = basicData.Price ? basicData.Price : 0;
        this.Priceval = basicData.Price ? basicData.Price : 0;

        // if (this.DataType === 1) {   // 查詢原料
        this.app.GetData('/Warehouses/GetWarehouseListByMaterialBasic/' + dataId).subscribe(
            (s) => {
                this.WarehouseList = s.data;
                this.UpdateVal();
            }
        );
        // } else if (this.DataType === 2) {    // 查詢成品
        //     this.app.GetData('/Warehouses/GetWarehouseListByProductBasic/' + dataId).subscribe(
        //         (s) => {
        //             this.WarehouseList = s.data;
        //             this.UpdateVal();
        //         }
        //     );
        // } else if (this.DataType === 3) {    // 查詢半成品
        //     this.app.GetData('/Warehouses/GetWarehouseListByWiproductBasic/' + dataId).subscribe(
        //         (s) => {
        //             this.WarehouseList = s.data;
        //             this.UpdateVal();
        //         }
        //     );
        // }
    }
    UpdateVal() {
        debugger;
        if (this.DataType === 1 && this.formData.Type === 10) {
            this.Warehouseval = this.WarehouseList.find(x => x.Code === '100')?.Id ?? null;
        } else if (this.DataType === 1) {
            this.Warehouseval = this.WarehouseList.find(x => x.Code === '101')?.Id ?? null;
        } else {
            this.Warehouseval = this.WarehouseList.find(x => x.Code === '301')?.Id ?? null; // 預設成品倉301
        }

        if (this.formData.Type === 30) { // 表處採購
            this.WarehousevalA = this.WarehouseList.find(x => x.Code === '201')?.Id ?? null; // 預設轉出倉201
            this.WarehousevalB = this.WarehouseList.find(x => x.Code === '202')?.Id ?? null; // 預設轉入倉202
        } else if (this.formData.Type === 40) { // 傳統銑床採購
            this.WarehousevalA = this.WarehouseList.find(x => x.Code === '100')?.Id ?? null; // 預設轉出倉100
        }
    }
    WarehousevalvalueChanged(e, data) {
        data.setValue(e.value);
    }
    QuantityValueChanged(e, data) {
        data.setValue(e.value);
        this.Quantityval = e.value;
        this.Priceval = this.Quantityval * this.OriginPriceval;
    }
    DeliveredValueChanged(e, data) {
        data.setValue(e.value);
        this.Deliveredval = e.value;
        this.Undeliveredval = this.Quantityval - this.Deliveredval;
    }
    UndeliveredValueChanged(e, data) {
        data.setValue(e.value);
        this.Undeliveredval = e.value;
        this.Deliveredval = this.Quantityval - this.Undeliveredval;
    }
    OkValueChanged(e, data) {
        data.setValue(e.value);
        this.Okval = e.value;
        // this.NotOkval = this.Quantityval - this.Okval;
    }
    NotOkValueChanged(e, data) {
        data.setValue(e.value);
        this.NotOkval = e.value;
        // this.Okval = this.Quantityval - this.NotOkval;
    }
    RepairValueChanged(e, data) {
        data.setValue(e.value);
        this.Repairval = e.value;
        // this.Unrepairval = this.NotOkval - this.Repairval;
    }
    UnRepairValueChanged(e, data) {
        data.setValue(e.value);
        this.Unrepairval = e.value;
        // this.Repairval = this.NotOkval - this.Unrepairval;
    }
    InNGValueChanged(e, data) {
        data.setValue(e.value);
        this.InNGval = e.value;
        // this.OutNGval = this.Unrepairval - this.InNGval;
    }
    OutNGValueChanged(e, data) {
        data.setValue(e.value);
        this.OutNGval = e.value;
        // this.InNGval = this.Unrepairval - this.OutNGval;
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
        // this.WarehouseList = null;
        this.DeliveryTime = new Date();
        //

        e.data.DeliveryTime = new Date();
        //
        // if (this.DataType === 1 && this.formData.Type === 10) {
        //     e.data.WarehouseId = this.WarehouseListAll.find(x => x.Code === '100')?.Id ?? null;
        // } else if (this.DataType === 1) {
        //     e.data.WarehouseId = this.WarehouseListAll.find(x => x.Code === '101')?.Id ?? null;
        // } else {
        //     e.data.WarehouseId = this.WarehouseListAll.find(x => x.Code === '301')?.Id ?? null; // 預設成品倉301
        // }

        // if (this.formData.Type === 30) { // 表處採購
        //     e.data.WarehouseIdA = this.WarehouseListAll.find(x => x.Code === '201')?.Id ?? null; // 預設轉出倉201
        //     e.data.WarehouseIdB = this.WarehouseListAll.find(x => x.Code === '202')?.Id ?? null; // 預設轉入倉202
        // } else if (this.formData.Type === 40) { // 傳統銑床採購
        //     e.data.WarehouseIdA = this.WarehouseListAll.find(x => x.Code === '100')?.Id ?? null; // 預設轉出倉100
        // }
    }
    onRowInserted(e) {
        if (this.dataSourceDB.length !== 0) {
            this.TypeDisabled = true;
        }
    }
    onEditingStart(e) {
        if (e.data.TempId) {
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

            // if (this.DataType === 1) {   // 查詢原料
            // this.app.GetData('/Warehouses/GetWarehouseListByMaterialBasic/' + dataId).subscribe(
            //     (s) => {
            //         this.WarehouseList = s.data;
            //     }
            // );
        }
        // } else if (this.DataType === 2) {    // 查詢成品
        //     this.app.GetData('/Warehouses/GetWarehouseListByProductBasic/' + dataId).subscribe(
        //         (s) => {
        //             this.WarehouseList = s.data;
        //         }
        //     );
        // } else if (this.DataType === 3) {    // 查詢半成品
        //     this.app.GetData('/Warehouses/GetWarehouseListByWiproductBasic/' + dataId).subscribe(
        //         (s) => {
        //             this.WarehouseList = s.data;
        //         }
        //     );
        // }
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
    onFormSubmit = async function (e) {
        this.buttondisabled = true;
        this.gridsaveCheck = true;
        this.dataGrid.instance.saveEditData();
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }
        if (!this.saveCheck || !this.gridsaveCheck) {
            this.buttondisabled = false;
            return;
        }
        this.formData = this.myform.instance.option('formData');
        let dataCheck = true;
        this.dataSourceDB.forEach(element => {
            const basicData = this.BasicDataList.find(z => z.TempId === element.TempId);
            element.DataType = basicData.DataType;
            element.DataId = basicData.DataId;
            element.DataNo = basicData.DataNo;
            element.DataName = basicData.Name;
            element.Specification = basicData.Specification;

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
            this.formData.UpdateTime = this.formData.CreateTime;
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
                    this.formData.Type = 30;
                    this.formData.Remarks = '';
                    e.preventDefault();
                    this.childOuter.emit(true);
                }
            } catch (error) {
            }
        }
        this.buttondisabled = false;
    };
    requiredfun(e) {
        if (e.rule.pattern === "WarehouseIdA" && this.WarehouseIdAVisible) {
            if (e.value) {
                return true;
            } else {
                this.gridsaveCheck = false;
                return false;
            }
        } else if (e.rule.pattern === "WarehouseIdB" && this.WarehouseIdBVisible) {
            if (e.value) {
                return true;
            } else {
                this.gridsaveCheck = false;
                return false;
            }
        }
        else {
            return true;
        }
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
        if (isNaN(newData.Price)) {
            newData.Price = null;
        }
    }
    OriginPricesetCellValue(newData, value, currentRowData) {
        newData.OriginPrice = value;
        newData.Price = currentRowData.Quantity * value;
        if (isNaN(newData.Price)) {
            newData.Price = null;
        }
    }
}

