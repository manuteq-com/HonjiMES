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
    selector: 'app-inventory-list',
    templateUrl: './inventory-list.component.html',
    styleUrls: ['./inventory-list.component.css']
})
export class InventoryListComponent implements OnInit, OnChanges {
    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() exceldata: any;
    @Input() modval: any;
    @Input() headData: any;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild("myGrid", { static: false }) dataGrid: DxDataGridComponent;
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
    dataSourceDB: any[];
    addRow = true;
    eformData: any;
    saveCheck = true;
    showdisabled: boolean;
    Quantityval: any;
    Priceval: any;
    PriceAllval: number;
    UnitQuantityval: any;
    UnitPriceval: any;
    UnitPriceAllval: any;
    BasicDataList: any;
    WarehouseList: any;
    WarehouseListAll: any;
    DataType: any;
    DataId: any;
    Warehouseval: any;
    onCellPreparedLevel: number;
    minValue: number;
    gridsaveCheck: boolean;

    constructor(private http: HttpClient, public app: AppComponent) {
        this.CustomerVal = null;
        this.formData = null;
        this.editOnkeyPress = true;
        this.enterKeyAction = 'moveFocus';
        this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 200;
        this.colCount = 3;
        this.dataSourceDB = [];
        this.onRowValidating = this.onRowValidating.bind(this);
    }
    ngOnInit() {
        this.app.GetData('/Inventory/GetBasicsData').subscribe(
            (s) => {
                if (s.success) {
                    this.BasicDataList = s.data;
                }
            }
        );
    }
    ngOnChanges() {
        this.dataSourceDB = [];
        this.app.GetData('/Warehouses/GetWarehouses').subscribe(
            (s) => {
                s.data.forEach(e => {
                    e.Name = e.Code + e.Name;
                });
                this.WarehouseListAll = s.data;
                this.WarehouseList = s.data;
            }
        );
        switch (this.modval) {
            case "edit":
                this.formData = this.headData[0];
                this.dataSourceDB = this.exceldata;
                //console.log("this.exceldata",this.exceldata, this.headData);
                break;
            case "new":
                this.app.GetData('/Inventory/GetAdjustNo').subscribe(
                    (s) => {
                        if (s.success) {
                            this.formData = s.data;
                        }
                    }
                );
                break;
            default:
        }
    }
    onInitialized(value, data) {
        data.setValue(value);
    }
    refreshAdjustNo() {
        this.app.GetData('/Inventory/GetAdjustNo').subscribe(
            (s) => {
                if (s.success) {
                    this.formData = s.data;
                }
            }
        );
    }
    onRowValidating(e) {
        if (!e.isValid) {
            this.gridsaveCheck = false;
        }
    }
    onFocusedCellChanging(e) {
    }
    TempIdValueChanged(e, data) {
        data.setValue(e.value);
        const basicData = this.BasicDataList.find(z => z.DataId === e.value);
        const dataType = basicData.DataType;
        const dataId = basicData.DataId;
        // if (dataType === 1) {   // 查詢原料
        this.app.GetData('/Warehouses/GetWarehouseListByMaterialBasic/' + dataId).subscribe(
            (s) => {
                this.WarehouseList = s.data;
                // console.log("WarehouseListAll2",this.WarehouseListAll);
                // console.log("WarehouseList2",this.WarehouseList);
                this.UpdateVal(dataType);
            }
        );
        // } else if (dataType === 2) {    // 查詢成品
        //     this.app.GetData('/Warehouses/GetWarehouseListByProductBasic/' + dataId).subscribe(
        //         (s) => {
        //             this.WarehouseList = s.data;
        //             this.UpdateVal(dataType);
        //         }
        //     );
        // } else if (dataType === 3) {    // 查詢半成品
        //     this.app.GetData('/Warehouses/GetWarehouseListByWiproductBasic/' + dataId).subscribe(
        //         (s) => {
        //             this.WarehouseList = s.data;
        //             this.UpdateVal(dataType);
        //         }
        //     );
        // }
        // const basicData = this.BasicDataList.find(z => z.TempId === e.value);
        // this.DataType = basicData.DataType;
        // this.DataId = basicData.DataId;
    }
    UpdateVal(dataType) {
        if (dataType === 1) {
            this.Warehouseval = this.WarehouseList.find(x => x.Code === '101')?.Id ?? null;
        } else if (dataType === 2) {
            this.Warehouseval = this.WarehouseList.find(x => x.Code === '301')?.Id ?? null; // 預設成品倉301
        } else {
            this.Warehouseval = this.WarehouseList.find(x => x.Code === '201')?.Id ?? null;
        }
    }
    WarehouseIdValueChanged(e, data) {
        // debugger;
        data.setValue(e.value);
        this.minValue = -this.WarehouseList.find(x => x.Id === e.value).Quantity;
    }
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
    onRowInserting(e) {
        const datas = this.dataSourceDB;
        this.dataSourceDB.forEach(element => {// 阻擋重複新增
            if (element.TempId === e.data.TempId &&
                element.WarehouseId === e.data.WarehouseId) {
                this.showMessage('新增項目已存在!!');
                e.cancel = true;
            }
        });
        if (e.cancel === false) {
            this.DataType = 0;
            this.DataId = 0;
        }
    }
    onInitNewRow(e) {
        this.saveCheck = false;
        this.onCellPreparedLevel = 1;

        this.WarehouseList = null;
        this.Quantityval = null;
        this.Priceval = null;
        this.PriceAllval = null;
        this.UnitQuantityval = null;
        this.UnitPriceval = null;
        this.UnitPriceAllval = null;
    }
    onEditingStart(e) {
        this.saveCheck = false;
        this.onCellPreparedLevel = 1;
    }
    onCellPrepared(e) {
        if (e.column.command === 'edit') {
            if (this.onCellPreparedLevel === 1) {
                this.onCellPreparedLevel = 2;
            } else if (this.onCellPreparedLevel === 2) {
                this.onCellPreparedLevel = 3;
                this.saveCheck = true;
            } else if (this.onCellPreparedLevel === 3) {
                this.myButton.instance.focus();
            }
        }
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
    onFormSubmit = async function (e) {
        // debugger;
        this.buttondisabled = true;
        this.gridsaveCheck = true;
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }
        if (!this.saveCheck || !this.gridsaveCheck) {
            this.buttondisabled = false;
            return;
        }
        this.dataGrid.instance.saveEditData();
        this.formData = this.myform.instance.option('formData');
        this.dataSourceDB.forEach(element => {
            const basicData = this.BasicDataList.find(z => z.DataId === element.MaterialBasicId);
            element.DataType = basicData.DataType;
            element.DataId = basicData.DataId;
        });
        this.postval = {
            AdjustNo: this.formData.AdjustNo,
            LinkOrder: this.formData.LinkOrder,
            Remarks: this.formData.Remarks,
            AdjustDetailData: this.dataSourceDB
        };
        try {
            // tslint:disable-next-line: max-line-length
            if (this.modval == "edit") {
                const sendRequest = await SendService.sendRequest(this.http, '/Inventory/PutInventoryListChange', 'PUT', { values: this.postval });
                if (sendRequest) {
                    this.dataSourceDB = [];
                    this.dataGrid.instance.refresh();
                    // this.myform.instance.resetValues();
                    e.preventDefault();
                    this.childOuter.emit(true);
                    this.refreshAdjustNo();
                }
            } else {
                const sendRequest = await SendService.sendRequest(this.http, '/Inventory/inventoryListChange', 'POST', { values: this.postval });
                if (sendRequest) {
                    this.dataSourceDB = [];
                    this.dataGrid.instance.refresh();
                    // this.myform.instance.resetValues();
                    e.preventDefault();
                    this.childOuter.emit(true);
                    this.refreshAdjustNo();
                }
            }

        } catch (error) {
        }
        this.buttondisabled = false;
    };
}
