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
    ProcessList: any;
    NumberBoxOptions: any;








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
        this.formData = null;
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
        this.CreateTimeDateBoxOptions = {
            onValueChanged: this.CreateTimeValueChange.bind(this)
        };
        this.NumberBoxOptions = { showSpinButtons: true, mode: 'number', min: 1, value: 1 };

        this.GetData('/Processes/GetProcesses').subscribe(
            (s) => {
                if (s.success) {
                    this.ProcessList = s.data;
                    // this.ProcessList.forEach(x => {
                    //     x.Name = x.Code + '_' + x.Name;
                    // });
                    // this.ProductBasicSelectBoxOptions = {
                    //     items: this.ProcessList,
                    //     displayExpr: 'Name',
                    //     valueExpr: 'Id',
                    //     searchEnabled: true,
                    //     onValueChanged: this.onProductBasicSelectionChanged.bind(this)
                    // };
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

    }
    public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>(location.origin + '/api' + apiUrl);
    }
    ngOnInit() {
    }
    ngOnChanges() {
        this.dataSourceDB = [];
        this.GetData('/Processes/GetWorkOrderNumber').subscribe(
            (s) => {
                if (s.success) {
                    this.formData = s.data;
                    this.formData.Count = 1;
                }
            }
        );
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
        // this.GetData('/ToPurchase/GetCanPurchase/' + e.value).subscribe(
        //     (s) => {
        //         if (s.success) {
        //             this.ProductBasicList = s.data;
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
        this.ProductBasicList.forEach(x => {
            if (x.Id === e.value) {
                // this.Quantityvalmax = 999;
                // this.Quantityval = 1;
                // this.OriginPriceval = x.Price ? x.Price : 0;
                // this.Priceval = x.Price ? x.Price : 0;
                // this.GetData('/Warehouses/GetWarehouseByProductBasic/' + x.Id).subscribe(
                //     (s) => {
                //         this.WarehouseList = s.data;
                //         this.Warehouseval = s.data[0].Id ? s.data[0].Id : null;
                //     }
                // );
            }
        });
    }

    // QuantityValueChanged(e, data) {
    //     data.setValue(e.value);
    //     this.Quantityval = e.value;
    //     this.Priceval = this.Quantityval * this.OriginPriceval;
    // }
    // OriginValueChanged(e, data) {
    //     data.setValue(e.value);
    //     this.OriginPriceval = e.value;
    //     this.Priceval = this.Quantityval * this.OriginPriceval;
    // }
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
        // this.saveCheck = false;
        // this.Quantityval = e.data.Quantity;
        // this.OriginPriceval = e.data.OriginPrice;
        // this.Priceval = e.data.Price;
        // this.WarehouseList = null;
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
        // if (!this.saveCheck) {
        //     this.buttondisabled = false;
        //     return;
        // }
        this.dataGrid.instance.saveEditData();
        this.formData = this.myform.instance.option('formData');
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
                this.formData.PurchaseDate = null;
                this.formData.SupplierId = null;
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
