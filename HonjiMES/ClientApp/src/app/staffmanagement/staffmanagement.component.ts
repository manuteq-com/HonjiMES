import { NgModule, Component, OnInit, ViewChild, Output, Input} from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import CustomStore from 'devextreme/data/custom_store';
import DataSource from 'devextreme/data/data_source';
import ArrayStore from 'devextreme/data/array_store';
import { DxDataGridComponent, DxFormComponent, DxPopupComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { APIResponse } from 'src/app/app.module';
import { SendService } from 'src/app/shared/mylib';
import { AppComponent } from 'src/app/app.component';
import { Title } from '@angular/platform-browser';
import { Myservice } from 'src/app/service/myservice';
import { CreateNumberInfo } from 'src/app/model/viewmodels';

@Component({
  selector: 'app-staffmanagement',
  templateUrl: './staffmanagement.component.html',
  styleUrls: ['./staffmanagement.component.css']
})
export class StaffmanagementComponent implements OnInit {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    dataSource: any;
    modval: any;
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
    WarehouseIdAVisible: boolean;
    WarehouseIdBVisible: boolean;
    DeliveryTime: Date;
    TypeDisabled: boolean;
    gridsaveCheck: boolean;

    ProcessBasicList: any;
    MachineList: any;
    WorkOrderNoList: any;
    DueStartTime: any;
    DueEndTime: any;

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent, private titleService: Title) {
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
        this.DueStartTime = new Date();
        this.DueEndTime = new Date();

        this.CreateTimeDateBoxOptions = {
            onValueChanged: this.CreateTimeValueChange.bind(this)
        };
        this.app.GetData('/WorkOrders/GetWorkOrderDetailByWorkOrderNo').subscribe(
            (s) => {
                if (s.success) {
                    this.WorkOrderNoList = s.data;
                }
            }
        );
        this.app.GetData('/Processes/GetProcesses').subscribe(
            (s) => {
                if (s.success) {
                    if (s.success) {
                        this.ProcessBasicList = s.data;
                        this.ProcessBasicList.forEach(x => {
                            x.Name = x.Code + '_' + x.Name;
                        });
                    }
                }
            }
        );
        this.app.GetData('/Machines/GetMachines').subscribe(
            (s) => {
                if (s.success) {
                    if (s.success) {
                        s.data.unshift({ Id: null, Name: '' }); // 加入第一行
                        this.MachineList = s.data;
                    }
                }
            }
        );
    }
    ngOnInit() {
        this.titleService.setTitle('人員管理');
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
                    this.formData.PurchaseDate = new Date();
                    if (this.modval === 'workorder') {
                        this.formData.Type = 30;
                    } else if (this.modval === 'add-outside') {
                        this.formData.Type = 20;
                        this.TypeDisabled = true;
                    }
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

    selectvalueChanged(e, data) {
        data.setValue(e.value);
        const basicData = this.BasicDataList.find(z => z.TempId === e.value);
        this.DataType = basicData.DataType;
        const dataId = basicData.DataId;
        this.Quantityvalmax = 999;
        this.Quantityval = 1;
        this.OriginPriceval = basicData.Price ? basicData.Price : 0;
        this.Priceval = basicData.Price ? basicData.Price : 0;

        // if (this.DataType === 1) {   // 查詢原料
        this.app.GetData('/Warehouses/GetWarehouseListByMaterialBasic/' + dataId).subscribe(
            (s) => {
                this.WarehouseList = s.data;
                this.UpdateVal();
            }
        );
    }
    UpdateVal() {
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
    onInitNewRow(e) {
        debugger;
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
        if (this.DataType === 1 && this.formData.Type === 10) {
            e.data.WarehouseId = this.WarehouseListAll.find(x => x.Code === '100')?.Id ?? null;
        } else if (this.DataType === 1) {
            e.data.WarehouseId = this.WarehouseListAll.find(x => x.Code === '101')?.Id ?? null;
        } else {
            e.data.WarehouseId = this.WarehouseListAll.find(x => x.Code === '301')?.Id ?? null; // 預設成品倉301
        }

        if (this.formData.Type === 30) { // 表處採購
            e.data.WarehouseIdA = this.WarehouseListAll.find(x => x.Code === '201')?.Id ?? null; // 預設轉出倉201
            e.data.WarehouseIdB = this.WarehouseListAll.find(x => x.Code === '202')?.Id ?? null; // 預設轉入倉202
        } else if (this.formData.Type === 40) { // 傳統銑床採購
            e.data.WarehouseIdA = this.WarehouseListAll.find(x => x.Code === '100')?.Id ?? null; // 預設轉出倉100
        }
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
            this.app.GetData('/Warehouses/GetWarehouseListByMaterialBasic/' + dataId).subscribe(
                (s) => {
                    this.WarehouseList = s.data;
                }
            );
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
    selectMachineChanged(e, data) {
        data.setValue(e.value);
    }
}
