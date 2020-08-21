import { Component, OnInit, EventEmitter, Output, Input, ViewChild, OnChanges } from '@angular/core';
import { DxFormComponent, DxDataGridComponent } from 'devextreme-angular';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import notify from 'devextreme/ui/notify';
import { SendService } from 'src/app/shared/mylib';
import { APIResponse } from 'src/app/app.module';
import { CreateNumberInfo } from 'src/app/model/viewmodels';
import { AppComponent } from 'src/app/app.component';

@Component({
    selector: 'app-ordertosale',
    templateUrl: './ordertosale.component.html',
    styleUrls: ['./ordertosale.component.css']
})
export class OrdertosaleComponent implements OnInit, OnChanges {
    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() modval: any;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    buttondisabled: boolean;
    formData: any;
    labelLocation: string;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    colCount: number;
    width: any;
    startTimeInput: any;
    showdisabled: boolean;
    Salelist: any;
    selectBoxOptions: any;
    buttonOptions: any = { text: '存檔', type: 'success', useSubmitBehavior: true };
    editorOptions: any;
    ProductList: any;
    WarehouseList: any;
    dataSourceDB: any;
    ProductBasicList: any;
    ProductsAllList: any;
    CreateTimeDateBoxOptions: any;

    constructor(private http: HttpClient, public app: AppComponent) {
        this.formData = null;
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 100;
        this.colCount = 5;
        this.CreateTimeDateBoxOptions = {
            onValueChanged: this.CreateTimeValueChange.bind(this)
        };
    }
    ngOnInit() {
    }
    async ngOnChanges() {
        debugger;
        this.app.GetData('/ToSale/GetSaleNumber').subscribe(
            (s) => {
                if (s.success) {
                    this.formData = s.data;
                }
            }
        );
        this.startTimeInput = {
            min: new Date().toDateString()
        };
        this.dataSourceDB = [];
        this.ProductsAllList = [];
        this.itemkeyval.forEach(x => this.dataSourceDB.push(Object.assign({}, x)));
        this.dataSourceDB.forEach(async x => {
            x.Quantity = x.Quantity - x.SaleCount;
            this.app.GetData('/Products/GetProductsById/' + x.ProductBasicId).subscribe(
                (s) => {
                    if (s.success) {
                        s.data.forEach(element => {
                            this.ProductsAllList.push(element);
                        });
                    }
                }
            );
        });

        this.ProductList = await SendService.sendRequest(this.http, '/Products/GetProducts');
        // this.WarehouseList = await SendService.sendRequest(this.http, '/Warehouses/GetWarehouses');
        this.app.GetData('/Warehouses/GetWarehouses').subscribe(
            (s) => {
                if (s.success) {
                    s.data.forEach(e => {
                        e.Name = e.Code + e.Name;
                    });
                    this.WarehouseList = s.data;
                }
            }
        );
        this.ProductBasicList = await SendService.sendRequest(this.http, '/Products/GetProductBasics');
        if (this.modval === 'add') {
            this.showdisabled = false;
        } else {
            this.showdisabled = true;
        }
        this.editorOptions = { showSpinButtons: true, mode: 'number', min: 1 };
        this.app.GetData('/Sales/GetSalesByStatus?status=0').subscribe(
            (s) => {
                console.log(s);
                if (s.success) {
                    this.Salelist = s.data;
                    this.selectBoxOptions = {
                        // searchMode: 'startswith',
                        searchEnabled: true,
                        items: this.Salelist,
                        displayExpr: 'SaleNo',
                        valueExpr: 'Id',
                        onValueChanged: this.onCustomerSelectionChanged.bind(this)
                    };
                }
            }
        );
    }
    CreateTimeValueChange = async function (e) {
        // this.formData = this.myform.instance.option('formData');
        if (this.formData.CreateTime != null) {
            this.CreateNumberInfoVal = new CreateNumberInfo();
            this.CreateNumberInfoVal.CreateNumber = this.formData.SaleNo;
            this.CreateNumberInfoVal.CreateTime = this.formData.CreateTime;
            // tslint:disable-next-line: max-line-length
            const sendRequest = await SendService.sendRequest(this.http, '/ToSale/GetSaleNumberByInfo', 'POST', { values: this.CreateNumberInfoVal });
            if (sendRequest) {
                this.formData.SaleNo = sendRequest.CreateNumber;
                // this.formData.CreateTime = sendRequest.CreateTime;
            }
        }
    };
    onCustomerSelectionChanged(e) {
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
    onCellPrepared(e) {
    }
    onEditorPreparing(e) {
        if (e.parentType === 'dataRow' && e.dataField === 'Quantity') {
            const rowIndex = e.row.rowIndex;
            const originData = this.itemkeyval[rowIndex];
            e.editorOptions.max = originData.Quantity - originData.SaleCount;
        }
        if (e.parentType === 'dataRow' && e.dataField === 'WarehouseId') {
            const gg = this.ProductsAllList;
            debugger;
            const rowIndex = e.row.rowIndex;
            const originData = this.itemkeyval[rowIndex];
        }
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
        this.buttondisabled = true;
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }
        this.dataGrid.instance.saveEditData();
        this.formData = this.myform.instance.option('formData');
        this.formData.Orderlist = this.dataSourceDB;
        const sendRequest = await SendService.sendRequest(this.http, '/ToSale/OrderToSale', 'POST', { values: this.formData });
        if (sendRequest) {
            e.preventDefault();
            this.childOuter.emit(true);
            this.dataGrid.instance.refresh();
            this.myform.instance.resetValues();
            this.formData.CreateTime = new Date();
            this.formData.SaleDate = null;
            this.formData.Remarks = '';
        }
        this.buttondisabled = false;

    };
}
