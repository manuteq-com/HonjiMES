import { Component, OnInit, OnChanges, Output, Input, EventEmitter, ViewChild } from '@angular/core';
import { DxFormComponent, DxDataGridComponent } from 'devextreme-angular';
import { HttpClient } from '@angular/common/http';
import { APIResponse } from '../../app.module';
import { Observable } from 'rxjs';
import notify from 'devextreme/ui/notify';
import { SendService } from '../../shared/mylib';
import { Myservice } from '../../service/myservice';
import { Button } from 'primeng';
import { CreateNumberInfo } from 'src/app/model/viewmodels';

@Component({
    selector: 'app-creat-purchase',
    templateUrl: './creat-purchase.component.html',
    styleUrls: ['./creat-purchase.component.css']
})
export class CreatPurchaseComponent implements OnInit, OnChanges {

    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() exceldata: any;
    @Input() modval: any;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
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
    WarehouseList: any;
    WarehouseListAll: any;
    PurchasetypeList: any;
    TypeselectBoxOptions: any;
    Warehouseval: any;
    Quantityval: number;
    OriginPriceval: number;
    Priceval: number;
    addRow = true;
    eformData: any;
    CreateNumberInfoVal: any;
    Quantityvalmax: number;
    saveCheck = true;
    showdisabled: boolean;

    constructor(private http: HttpClient, myservice: Myservice) {
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
        this.url = location.origin + '/api';
        this.dataSourceDB = [];
        this.controller = '/OrderDetails';
        this.CreateTimeDateBoxOptions = {
            onValueChanged: this.CreateTimeValueChange.bind(this)
        };
        this.PurchaseDateBoxOptions = {
            // displayFormat: 'yyyyMMdd',
            onValueChanged: this.PurchaseDateValueChange.bind(this)
        };
        this.GetData('/Suppliers/GetSuppliers').subscribe(
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
        this.GetData('/MaterialBasics/GetMaterialBasics').subscribe(
            (s) => {
                if (s.success) {
                    this.MaterialBasicList = s.data;
                }
            }
        );
        this.GetData('/Warehouses/GetWarehouses').subscribe(
            (s) => {
                this.WarehouseListAll = s.data;
            }
        );
        this.PurchasetypeList = myservice.getpurchasetypes();
        this.TypeselectBoxOptions = {
            items: this.PurchasetypeList,
            displayExpr: 'Name',
            valueExpr: 'Id',
            searchEnabled: true
        };

    }
    public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>(location.origin + '/api' + apiUrl);
    }
    ngOnInit() {

    }
    ngOnChanges() {
        this.dataSourceDB = [];
        if (this.modval === 'merge') {
            this.showdisabled = false;
        } else {
            this.showdisabled = true;
        }
        if (this.itemkeyval != null) {
            this.itemkeyval.forEach(x => {

                this.GetData('/BillOfMaterials/GetBillOfMaterialByProductBasic/' + x.ProductBasicId).subscribe(
                    (s) => {
                        debugger;
                        if (s.success) {
                            this.dataSourceDB.push({
                                Serial: x.Serial,
                                DataId: s.data[0].MaterialBasicId,
                                WarehouseId: s.data[0].Id,
                                Quantity: x.Quantity,
                                OriginPrice: x.OriginPrice,
                                Price: x.Quantity * x.OriginPrice,
                                DeliveryTime: new Date()
                            });
                        }
                    }
                );
                // this.GetData('/Warehouses/GetWarehouseByProductBasic/' + x.ProductBasicId).subscribe(
                //     (s) => {
                //         if (s.success) {
                //             this.dataSourceDB.push({
                //                 Serial: x.Serial,
                //                 DataId: x.ProductBasicId,
                //                 WarehouseId: s.data[0].Id,
                //                 Quantity: x.Quantity,
                //                 OriginPrice: x.OriginPrice,
                //                 Price: x.Quantity * x.OriginPrice,
                //                 DeliveryTime: new Date()

                //                 // DataId: 394
                //                 // DeliveryTime: Wed Jun 17 2020 00:00:00 GMT+0800 (台北標準時間) {}
                //                 // OriginPrice: 0
                //                 // Price: 0
                //                 // Quantity: 1
                //                 // Serial: "1959f2f9-44dd-299b-552a-b1352e927bcc"
                //                 // WarehouseId: 1
                //             });
                //         }
                //     }
                // );
                // this.dataSourceDB.push(Object.assign({}, x));
            });
        }
        this.GetData('/PurchaseHeads/GetPurchaseNumber').subscribe(
            (s) => {
                if (s.success) {
                    this.formData = s.data;
                }
            }
        );
    }
    onFocusedCellChanging(e) {
    }
    CreateTimeValueChange = async function(e) {
        // this.formData = this.myform.instance.option('formData');
        if (this.formData.CreateTime != null) {
            this.CreateNumberInfoVal = new CreateNumberInfo();
            this.CreateNumberInfoVal.Type = this.formData.Type;
            this.CreateNumberInfoVal.CreateNumber = this.formData.PurchaseNo;
            this.CreateNumberInfoVal.CreateTime = this.formData.CreateTime;
            // tslint:disable-next-line: max-line-length
            const sendRequest = await SendService.sendRequest(this.http, '/PurchaseHeads/GetPurchaseNumberByInfo', 'POST', { values: this.CreateNumberInfoVal });
            if (sendRequest) {
                this.formData.Type = sendRequest.Type;
                this.formData.PurchaseNo = sendRequest.CreateNumber;
                this.formData.CreateTime = sendRequest.CreateTime;
            }
        }
    };
    PurchaseDateValueChange(e) {
    }
    onSupplierSelectionChanged(e) {//依照供應商ID去進貨單查詢，可進貨的原料項目。
        // this.GetData('/ToPurchase/GetCanPurchase/' + e.value).subscribe(
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
        // debugger;
        data.setValue(e.value);
        const today = new Date();
        this.MaterialBasicList.forEach(x => {
            if (x.Id === e.value) {
                this.Quantityvalmax = 999;
                this.Quantityval = 1;
                this.OriginPriceval = x.Price ? x.Price : 0;
                this.Priceval = x.Price ? x.Price : 0;
                this.Warehouseval = 0;
                this.GetData('/Warehouses/GetWarehouseByMaterialBasic/' + x.Id).subscribe(
                    (s) => {
                        this.WarehouseList = s.data;
                    }
                );
            }
        });
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
        this.Quantityval = e.data.Quantity;
        this.OriginPriceval = e.data.OriginPrice;
        this.Priceval = e.data.Price;
        this.WarehouseList = null;
    }
    onEditingStart(e) {
        this.saveCheck = false;
        this.Quantityval = e.data.Quantity;
        this.OriginPriceval = e.data.OriginPrice;
        this.Priceval = e.data.Price;
        this.Warehouseval = e.data.WarehouseId;
        this.GetData('/Warehouses/GetWarehouseByMaterialBasic/' + e.data.DataId).subscribe(
            (s) => {
                this.WarehouseList = s.data;
            }
        );
    }
    onCellPrepared(e) {
        if (e.column.command === 'edit') {
            this.saveCheck = true;
        }
    }
    onFormSubmit = async function (e) {
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
        this.postval = {
            PurchaseHead: this.formData,
            PurchaseDetails: this.dataSourceDB
        };
        this.buttondisabled = false;
        try {
            // tslint:disable-next-line: max-line-length
            const sendRequest = await SendService.sendRequest(this.http, '/ToPurchase/PostPurchaseMaster_Detail', 'POST', { values: this.postval });
            if (sendRequest) {
                this.dataSourceDB = [];
                this.dataGrid.instance.refresh();
                // this.myform.instance.resetValues();
                this.formData.CreateTime = new Date();
                this.formData.PurchaseDate = null;
                this.formData.SupplierId = 0;
                this.formData.Type = 10;
                this.formData.Remarks = '';
                e.preventDefault();
                this.childOuter.emit(true);
            }
        } catch (error) {
        }
        this.buttondisabled = false;
    };
}
