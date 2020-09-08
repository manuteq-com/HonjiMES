import { Component, OnInit, OnChanges, EventEmitter, Output, Input, ViewChild } from '@angular/core';
import { DxFormComponent, DxDataGridComponent } from 'devextreme-angular';
import { HttpClient } from '@angular/common/http';
import notify from 'devextreme/ui/notify';
import { Observable } from 'rxjs';
import { APIResponse } from 'src/app/app.module';
import { SendService } from 'src/app/shared/mylib';
import $ from 'jquery';
import { BillofPurchaseDetail, CreateNumberInfo } from 'src/app/model/viewmodels';
import { AppComponent } from 'src/app/app.component';
@Component({
    selector: 'app-creat-bill-purchase',
    templateUrl: './creat-bill-purchase.component.html',
    styleUrls: ['./creat-bill-purchase.component.css']
})
export class CreatBillPurchaseComponent implements OnInit, OnChanges {
    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() exceldata: any;
    @Input() modval: any;
    @Input() DetailDataList: any;
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
    // dataSourceDB: {
    //     TempId: number, Id: number, SupplierId: number, PurchaseId: number,
    //     DataType: number, DataId: number, Quantity: number, OriginPrice: number, Price: number, PriceAll: number
    // }[] = [];
    dataSourceDB: any;
    controller: string;
    selectBoxOptions: { items: any; displayExpr: string; valueExpr: string; onValueChanged: any; };
    SupplierList: any;
    SupplierListEdit: any[];
    PurchaseList: any;
    PurchaseTempList: any[];
    BasicDataList: any;
    BasicDataListTemp: any[];
    PurchaseselectBoxOptions: any;
    changeMode: boolean;
    topurchase: any[] & Promise<any> & JQueryPromise<any>;
    Quantityval: any;
    OriginPriceval: any;
    Priceval: any;
    PriceAllval: number;
    UnitQuantityval: any;
    UnitPriceval: any;
    UnitPriceAllval: any;
    allMode: string;
    checkBoxesMode: string;
    postval: any;
    CreateTimeDateBoxOptions: any;
    Supplierval: any;
    Purchaseval: any;
    Warehouseval: any;
    WarehouseList: any;
    WarehouseListAll: any;
    SupplierIdVal: any;
    PurchaseIdVal: any;

    constructor(private http: HttpClient, public app: AppComponent) {
        this.allMode = 'allPages';
        this.checkBoxesMode = 'always'; // 'onClick';
        this.CustomerVal = null;
        this.formData = null;
        this.editOnkeyPress = true;
        this.enterKeyAction = 'moveFocus';
        this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 300;
        this.colCount = 4;
        this.controller = '/OrderDetails';
        this.dataSourceDB = [];
        this.CreateTimeDateBoxOptions = {
            onValueChanged: this.CreateTimeValueChange.bind(this)
        };
        this.PurchaseList = [];
        this.PurchaseTempList = [];
        this.BasicDataList = [];
        this.BasicDataListTemp = [];

        this.app.GetData('/Warehouses/GetWarehouses').subscribe(
            (s) => {
                s.data.forEach(e => {
                    e.Name = e.Code + e.Name;
                });
                this.WarehouseListAll = s.data;
            }
        );
        this.app.GetData('/Inventory/GetBasicsData').subscribe(
            (s) => {
                if (s.success) {
                    this.BasicDataList = s.data;
                    if (this.dataSourceDB.length !== 0) {
                        this.dataSourceDB.forEach(element => {
                            // tslint:disable-next-line: prefer-const
                            let BasicData = this.BasicDataList.find(x => x.DataType === element.DataType && x.DataId === element.DataId);
                            element.TempId = BasicData.TempId;
                        });
                    }
                }
            }
        );
    }
    ngOnInit() {
    }
    ngOnChanges() {
        this.dataSourceDB = [];
        if (this.DetailDataList !== undefined) {
            this.DetailDataList.forEach(element => {
                element.PriceAll = element.Price;
                element.Price = element.OriginPrice;

                this.dataSourceDB.push({
                    DataId: element.DataId,
                    DataType: element.DataType,
                    OriginPrice: element.OriginPrice,
                    Price: element.Price,
                    PriceAll: element.PriceAll,
                    PurchaseId: element.PurchaseId,
                    Quantity: element.Quantity - element.PurchaseCount,
                    SupplierId: element.SupplierId,
                    TempId: element.TempId,
                    UnitCount: null,
                    UnitPrice: null,
                    UnitPriceAll: 0,
                    WarehouseId: element.WarehouseId,
                });
            });
            if (this.BasicDataList.length !== 0 && this.dataSourceDB.length !== 0) {
                this.dataSourceDB.forEach(element => {
                    // tslint:disable-next-line: prefer-const
                    let BasicData = this.BasicDataList.find(x => x.DataType === element.DataType && x.DataId === element.DataId);
                    element.TempId = BasicData.TempId;
                });
            }
        }
        this.app.GetData('/BillofPurchaseHeads/GetBillofPurchaseNumber').subscribe(
            (s) => {
                if (s.success) {
                    this.formData = s.data;
                }
            }
        );
        this.app.GetData('/Suppliers/GetSuppliers').subscribe(
            (s) => {
                if (s.success) {
                    this.SupplierList = s.data;
                    this.SupplierListEdit = [];
                    this.SupplierList.forEach(x => {
                        x.Name = x.Name.replace('有限公司', '');
                        this.SupplierListEdit.push({
                            Id: x.Id,
                            Name: x.Code + '_' + x.Name
                        });
                    });
                }
            }
        );
        this.app.GetData('/PurchaseHeads/GetPurchasesByStatus?status=0').subscribe(
            (s) => {
                if (s.success) {
                    this.PurchaseList = s.data;
                }
            }
        );
    }
    CreateTimeValueChange = async function (e) {
        // this.formData = this.myform.instance.option('formData');
        if (this.formData.CreateTime != null) {
            this.CreateNumberInfoVal = new CreateNumberInfo();
            this.CreateNumberInfoVal.CreateNumber = this.formData.BillofPurchaseNo;
            this.CreateNumberInfoVal.CreateTime = this.formData.CreateTime;
            // tslint:disable-next-line: max-line-length
            const sendRequest = await SendService.sendRequest(this.http, '/BillofPurchaseHeads/GetBillofPurchaseNumberByInfo', 'POST', { values: this.CreateNumberInfoVal });
            if (sendRequest) {
                this.formData.BillofPurchaseNo = sendRequest.CreateNumber;
                // this.formData.CreateTime = sendRequest.CreateTime;
            }
        }
    };
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
    onContentReady(e) {
        // debugger;
        // const _dataGrid = $(e.element);
        // const dataGrid = e.component;
        // if (this.changeMode && !_dataGrid.find('.dx-row-inserted').length) {
        //     // debugger;
        //     dataGrid.beginUpdate();
        //     e.component.option('editing.mode', 'form');
        //     this.changeMode = false;
        //     dataGrid.columnOption('SupplierId', { allowEditing: false });
        //     dataGrid.columnOption('DataId', { allowEditing: false });
        //     dataGrid.endUpdate();
        // }
    }
    to_purchaseClick(e) {
        debugger;
        this.topurchase = this.dataGrid.instance.getSelectedRowsData();

    }
    onInitialized(value, data) {
        data.setValue(value);
    }
    selectSupplierValueChanged(e, data) {
        data.setValue(e.value);
        this.Supplierval = e.value;

        this.Purchaseval = null;
        this.Warehouseval = null;
        this.Quantityval = 0;
        this.OriginPriceval = 0;
        this.Priceval = 0;
        this.PriceAllval = 0;
        this.UnitQuantityval = null;
        this.UnitPriceval = null;
        this.UnitPriceAllval = null;
        // this.BasicDataList = null;
        this.BasicDataListTemp = [];
        this.WarehouseList = null;

        this.GetPurchasesBySupplier(e.value);
    }
    selectPurchaseValueChanged(e, data) {
        data.setValue(e.value);
        this.Purchaseval = e.value;
        this.WarehouseList = null;
        this.GetBasicDatasByPurchase(e.value);
    }
    selectMateriaValueChanged(e, data) {
        data.setValue(e.value);
        this.BasicDataListTemp.find(x => {
            if (x.TempId === e.value) {
                this.Quantityval = x.Quantity;
                this.OriginPriceval = x.OriginPrice;
                this.Priceval = x.OriginPrice;
                this.PriceAllval = x.Quantity * x.OriginPrice;
                this.Warehouseval = x.WarehouseId;
                this.GetWarehouseByBasicData(x.DataType, x.DataId);
            }
        });
    }
    // WarehousevalvalueChanged(e, data) {
    //     data.setValue(e.value);
    // }
    QuantityValueChanged(e, data) {
        data.setValue(e.value);
        this.Quantityval = e.value;
        this.PriceAllval = this.Quantityval * this.Priceval;
    }
    PriceValueChanged(e, data) {
        data.setValue(e.value);
        this.Priceval = e.value;
        this.PriceAllval = this.Quantityval * this.Priceval;
    }
    UnitQuantityValueChanged(e, data) {
        data.setValue(e.value);
        this.UnitQuantityval = e.value;
        this.UnitPriceAllval = this.UnitQuantityval * this.UnitPriceval;
    }
    UnitPriceValueChanged(e, data) {
        data.setValue(e.value);
        this.UnitPriceval = e.value;
        this.UnitPriceAllval = this.UnitQuantityval * this.UnitPriceval;
    }
    onInitNewRow(e) {
        this.Supplierval = this.SupplierIdVal;
        this.Purchaseval = this.PurchaseIdVal;
        this.Quantityval = e.data.Quantity;
        this.OriginPriceval = e.data.OriginPrice;
        this.Priceval = e.data.Price;
        this.PriceAllval = e.data.PriceAll;
        this.UnitQuantityval = e.data.UnitQuantityval;
        this.UnitPriceval = e.data.UnitPriceval;
        this.UnitPriceAllval = e.data.UnitPriceAllval;
        this.WarehouseList = null;
        // debugger;
        // const dataGrid = e.component;
        // dataGrid.beginUpdate();
        // dataGrid.option('editing.mode', 'form');
        // dataGrid.columnOption('SupplierId', { allowEditing: true });
        // dataGrid.columnOption('DataId', { allowEditing: true });
        // dataGrid.endUpdate();
        // this.changeMode = true;
    }
    onRowInserting(e) {
        this.SupplierIdVal = e.data.SupplierId;
        this.PurchaseIdVal = e.data.PurchaseId;
        const TempId = e.data.TempId;
        const datas = this.dataSourceDB;
        datas.forEach(element => {// 阻擋重複新增
            if (element.SupplierId === this.SupplierIdVal &&
                element.PurchaseId === this.PurchaseIdVal &&
                element.TempId === TempId) {
                this.showMessage('新增項目已存在!!');
                e.cancel = true;
            }
        });
        if (e.cancel === false) {
            this.Warehouseval = 0;
            this.Quantityval = 0;
            this.OriginPriceval = 0;
            this.Priceval = 0;
            this.PriceAllval = 0;
            this.UnitQuantityval = null;
            this.UnitPriceval = null;
            this.UnitPriceAllval = null;
            this.WarehouseList = null;
        }
    }
    onRowInserted(e) {
        debugger;
    }
    onEditingStart(e) {
        debugger;
        this.Supplierval = e.data.SupplierId;
        this.Purchaseval = e.data.PurchaseId;
        this.Quantityval = e.data.Quantity;
        this.OriginPriceval = e.data.OriginPrice;
        this.Priceval = e.data.Price;
        this.PriceAllval = e.data.PriceAll;
        this.UnitQuantityval = e.data.UnitQuantityval;
        this.UnitPriceval = e.data.UnitPriceval;
        this.UnitPriceAllval = e.data.UnitPriceAllval;
        this.Warehouseval = e.data.WarehouseId;
        this.GetPurchasesBySupplier(e.data.SupplierId);
        this.GetBasicDatasByPurchase(e.data.PurchaseId);
        const result = this.BasicDataList.find(x => x.TempId === e.data.TempId);
        if (result) {
            this.GetWarehouseByBasicData(result.DataType, result.DataId);
        }
    }
    GetPurchasesBySupplier(Id) {
        this.app.GetData('/PurchaseHeads/GetPurchasesBySupplier/' + Id).subscribe(
            (s) => {
                if (s.success) {
                    this.PurchaseTempList = s.data;
                    s.data.forEach(element => {
                        if (!this.PurchaseList.some(x => x.PurchaseNo === element.PurchaseNo)) {
                            this.PurchaseList.push(element);
                        }
                    });
                }
            }
        );
    }
    GetBasicDatasByPurchase(Id) {
        this.app.GetData('/PurchaseDetails/GetBasicsDataByPurchase/' + Id).subscribe(
            (s) => {
                if (s.success) {
                    // this.BasicDataList = s.data;
                    this.BasicDataListTemp = [];
                    s.data.forEach(element => {
                        const result = this.BasicDataList.find(x => x.DataType === element.DataType && x.DataId === element.DataId);
                        if (result && element.Quantity !== 0) {
                            this.BasicDataListTemp.push({
                                TempId: result.TempId,
                                DataId: element.DataId,
                                DataName: element.DataName,
                                DataNo: element.DataNo,
                                DataType: element.DataType,
                                DeleteFlag: element.DeleteFlag,
                                DeliveryTime: element.DeliveryTime,
                                Id: element.Id,
                                OrderId: element.OrderId,
                                OriginPrice: element.OriginPrice,
                                Price: element.Price,
                                Purchase: element.Purchase,
                                PurchaseCount: element.PurchaseCount,
                                PurchaseId: element.PurchaseId,
                                PurchaseType: element.PurchaseType,
                                Quantity: element.Quantity,
                                Remarks: element.Remarks,
                                Specification: element.Specification,
                                SupplierId: element.SupplierId,
                                WarehouseId: element.WarehouseId
                            });
                        }
                    });
                }
            }
        );
    }
    GetWarehouseByBasicData(Type, Id) {
        if (Type === 1) {
            this.app.GetData('/Warehouses/GetWarehouseByMaterialBasic/' + Id).subscribe(
                (s) => {
                    this.WarehouseList = [];
                    s.data.forEach((element, index) => {
                        element.Warehouse.Name = element.Warehouse.Code + element.Warehouse.Name + ' (庫存 ' + element.Quantity + ')';
                        this.WarehouseList[index] = element.Warehouse;
                    });
                    this.BasicDataListTemp.forEach(element => {
                        if (element.Id === Id) {
                            if (element.WarehouseId !== null) {
                                this.Warehouseval = element.WarehouseId;
                            } else {
                                this.Warehouseval = this.WarehouseList[0].Id;
                            }
                        }
                    });
                }
            );
        } else if (Type === 2) {
            this.app.GetData('/Warehouses/GetWarehouseByProductBasic/' + Id).subscribe(
                (s) => {
                    this.WarehouseList = [];
                    s.data.forEach((element, index) => {
                        element.Warehouse.Name = element.Warehouse.Code + element.Warehouse.Name + ' (庫存 ' + element.Quantity + ')';
                        this.WarehouseList[index] = element.Warehouse;
                    });
                    this.BasicDataListTemp.forEach(element => {
                        if (element.Id === Id) {
                            if (element.WarehouseId !== null) {
                                this.Warehouseval = element.WarehouseId;
                            } else {
                                this.Warehouseval = this.WarehouseList[0].Id;
                            }
                        }
                    });
                }
            );
        } else if (Type === 3) {
            this.app.GetData('/Warehouses/GetWarehouseByWiproductBasic/' + Id).subscribe(
                (s) => {
                    this.WarehouseList = [];
                    s.data.forEach((element, index) => {
                        element.Warehouse.Name = element.Warehouse.Code + element.Warehouse.Name + ' (庫存 ' + element.Quantity + ')';
                        this.WarehouseList[index] = element.Warehouse;
                    });
                    this.BasicDataListTemp.forEach(element => {
                        if (element.Id === Id) {
                            if (element.WarehouseId !== null) {
                                this.Warehouseval = element.WarehouseId;
                            } else {
                                this.Warehouseval = this.WarehouseList[0].Id;
                            }
                        }
                    });
                }
            );
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
    showMessage(data) {
        notify({
            message: data,
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'error', 3000);
    }
    async onFormSubmit(e) {
        debugger;
        this.buttondisabled = true;
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }
        this.dataGrid.instance.saveEditData();
        this.formData = this.myform.instance.option('formData');
        this.dataSourceDB.forEach(element => {
            const basicData = this.BasicDataList.find(z => z.TempId === element.TempId);
            element.DataType = basicData.DataType;
            element.DataId = basicData.DataId;
        });
        this.postval = {
            BillofPurchaseHead: this.formData,
            BillofPurchaseDetail: this.dataSourceDB
        };
        this.buttondisabled = false;

        // tslint:disable-next-line: max-line-length
        const sendRequest = await SendService.sendRequest(this.http, '/BillofPurchaseHeads/PostBillofPurchaseHead_Detail', 'POST', { values: this.postval });
        // let data = this.client.POST( this.url + '/OrderHeads/PostOrderMaster_Detail').toPromise();
        if (sendRequest) {
            this.dataSourceDB = [];
            this.dataGrid.instance.refresh();
            // this.myform.instance.resetValues();
            this.formData.CreateTime = new Date();
            this.formData.BillofPurchaseDate = null;
            this.formData.Remarks = '';
            this.CustomerVal = null;
            e.preventDefault();
            this.childOuter.emit(true);
        }
        this.buttondisabled = false;

    }
}
