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
  selector: 'app-creatprocess-control',
  templateUrl: './creatprocess-control.component.html',
  styleUrls: ['./creatprocess-control.component.css']
})
export class CreatprocessControlComponent implements OnInit, OnChanges {
    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() modval: any;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild('dataGrid2') dataGrid2: DxDataGridComponent;

    controller: string;
    formData: any;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    width: any;
    colCount: number;
    url: string;
    dataSourceDB: any[];
    labelLocation: string;
    editOnkeyPress: boolean;
    enterKeyAction: string;
    enterKeyDirection: string;
    buttondisabled = false;





    CreateTimeDateBoxOptions: any;
    ProductBasicSelectBoxOptions: any;
    ProductBasicList: any;
    ProcessBasicList: any;
    NumberBoxOptions: any;
    SerialNo: any;
    productbasicId: number;
    saveDisabled: boolean;








    // buttondisabled = false;
    // CustomerVal: any;
    //
    //
    // PurchaseDateBoxOptions: any;
    //
    // ProductselectBoxOptions: any;
    // SupplierList: any;
    // ProductBasicList: any;
    // WarehouseList: any;
    // WarehouseListAll: any;
    // PurchasetypeList: any;
    // Warehouseval: any;
    // Quantityval: number;
    // OriginPriceval: number;
    // Priceval: number;
    // addRow = true;
    // eformData: any;
    // CreateNumberInfoVal: any;
    // Quantityvalmax: number;
    // saveCheck = true;
    // Serial: number;
    // Purchaselist: any;

    constructor(private http: HttpClient, myservice: Myservice) {
        // this.CustomerVal = null;
        // this.formData = null;
        this.editOnkeyPress = true;
        this.enterKeyAction = 'moveFocus';
        this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 300;
        this.colCount = 4;
        this.url = location.origin + '/api';
        this.dataSourceDB = [];
        this.controller = '/OrderDetails';
        this.saveDisabled = true;

        this.CreateTimeDateBoxOptions = {
            onValueChanged: this.CreateTimeValueChange.bind(this)
        };
        this.NumberBoxOptions = { showSpinButtons: true, mode: 'number', min: 1, value: 1 };

        this.GetData('/Processes/GetProcesses').subscribe(
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
        this.GetData('/ProductBasics/GetProductBasicsAsc').subscribe(
            (s) => {
                if (s.success) {
                    this.ProductBasicList = s.data;
                    this.ProductBasicList.forEach(x => {
                        x.Name = x.ProductNo + '_' + x.Name;
                    });
                    this.ProductBasicSelectBoxOptions = {
                        dataSource: { paginate: true, store: { type: 'array', data: this.ProductBasicList, key: 'Id' } },
                        searchEnabled: true,
                        displayExpr: 'Name',
                        valueExpr: 'Id',
                        onValueChanged: this.onProductBasicSelectionChanged.bind(this)
                    };

                }
            }
        );
        this.GetData('/Processes/GetWorkOrderNumber').subscribe(
            (s) => {
                if (s.success) {
                    this.formData = s.data;
                    this.formData.Count = 1;
                }
            }
        );

    }
    public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>(location.origin + '/api' + apiUrl);
    }
    ngOnInit() {
    }
    ngOnChanges() {
        debugger;
        this.dataSourceDB = [];
        if (this.itemkeyval != null) {
            this.GetData('/Processes/GetProcessByWorkOrderNo/' + this.itemkeyval).subscribe(
                (s) => {
                    if (s.success) {
                        this.dataSourceDB = s.data;
                        this.formData.CreateTime = s.data[0].CreateTime;
                        this.formData.ProductBasicId = s.data[0].ProductBasicId;
                        this.formData.Count = s.data[0].Count;
                        this.formData.MachineNo = s.data[0].MachineNo;
                        // this.formData.Remarks = s.data[0].Remarks;
                    }
                }
            );
        } else {
            this.GetData('/Processes/GetWorkOrderNumber').subscribe(
                (s) => {
                    if (s.success) {
                        this.formData = s.data;
                        this.formData.Count = 1;
                    }
                }
            );
        }
    }
    onInitialized(value, data) {
        data.setValue(value);
    }
    onFocusedCellChanging(e) {
    }
    CreateTimeValueChange = async function(e) {
        // this.formData = this.myform.instance.option('formData');
        if (this.formData.CreateTime != null) {
            this.CreateNumberInfoVal = new CreateNumberInfo();
            this.CreateNumberInfoVal.CreateNumber = this.formData.WorkOrderNo;
            this.CreateNumberInfoVal.CreateTime = this.formData.CreateTime;
            // tslint:disable-next-line: max-line-length
            const sendRequest = await SendService.sendRequest(this.http, '/Processes/GetWorkOrderNumberByInfo', 'POST', { values: this.CreateNumberInfoVal });
            if (sendRequest) {
                this.formData.WorkOrderNo = sendRequest.CreateNumber;
                this.formData.CreateTime = sendRequest.CreateTime;
            }
        }
    };
    onProductBasicSelectionChanged(e) {
        // debugger;
        this.productbasicId = e.value;
        this.saveDisabled = false;
        this.GetData('/BillOfMaterials/GetProcessByProductBasicId/' + this.productbasicId).subscribe(
            (s) => {
                if (s.success) {
                    this.dataSourceDB = s.data;
                }
            }
        );
    }
    selectvalueChanged(e, data) {
        // debugger;
        data.setValue(e.value);
        const today = new Date();
        this.ProcessBasicList.forEach(x => {
            if (x.Id === e.value) {
                // this.Quantityvalmax = 999;
                // this.Quantityval = 1;
                // this.OriginPriceval = x.Price ? x.Price : 0;
                // this.Priceval = x.Price ? x.Price : 0;
                // this.GetData('/Warehouses/GetWarehouseByMaterialBasic/' + x.Id).subscribe(
                //     (s) => {
                //         this.WarehouseList = s.data;
                //         this.Warehouseval = s.data[0].Id ? s.data[0].Id : null;
                //     }
                // );
            }
        });
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
        // debugger;
        this.SerialNo = this.dataSourceDB.length;
        this.SerialNo++;
        e.data.SerialNumber = this.SerialNo;
        e.data.ProcessLeadTime = 0;
        e.data.ProcessTime = 0;
        e.data.ProcessCost = 0;
        e.data.ProducingMachine = '';
    }
    onEditingStart(e) {
        // this.saveCheck = false;
        // this.Quantityval = e.data.Quantity;
        // this.OriginPriceval = e.data.OriginPrice;
        // this.Priceval = e.data.Price;
        // this.Warehouseval = e.data.WarehouseId;
        // this.GetData('/Warehouses/GetWarehouseByProductBasic/' + e.data.DataId).subscribe(
        //     (s) => {
        //         this.WarehouseList = s.data;
        //     }
        // );
    }
    onCellPrepared(e) {
        if (e.column.command === 'edit') {
            // this.saveCheck = true;
        }
    }
    onFormSubmit = async function (e) {
        // debugger;
        this.buttondisabled = true;
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }
        this.dataGrid2.instance.saveEditData();
        this.postval = {
            CreateTime: this.formData.CreateTime,
            ProductBasicId: this.productbasicId,
            Count: this.formData.Count,
            MachineNo: this.formData.MachineNo,
            Remarks: this.formData.Remarks,
            MBillOfMaterialList: this.dataSourceDB
        };
        try {
            // tslint:disable-next-line: max-line-length
            const sendRequest = await SendService.sendRequest(this.http, '/Processes/PostWorkOrderList', 'POST', { values: this.postval });
            if (sendRequest) {
                this.dataSourceDB = [];
                this.dataGrid2.instance.refresh();
                this.formData.CreateTime = new Date();
                this.productbasicId = null;
                this.formData.ProductBasicId = null;
                this.formData.Count = 1;
                this.formData.ProducingMachine = '';
                this.formData.Remarks = '';
                this.dataSourceDB = [];
                e.preventDefault();
                this.childOuter.emit(true);
            }
        } catch (error) {
        }
        this.buttondisabled = false;
    };
}
