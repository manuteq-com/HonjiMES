import { Component, OnInit, EventEmitter, Output, Input, ViewChild, OnChanges } from '@angular/core';
import { DxFormComponent, DxDataGridComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import CustomStore from 'devextreme/data/custom_store';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { APIResponse } from 'src/app/app.module';
import { InventoryChange } from 'src/app/model/viewmodels';
import { SendService } from 'src/app/shared/mylib';

@Component({
    selector: 'app-inventory-change',
    templateUrl: './inventory-change.component.html',
    styleUrls: ['./inventory-change.component.css']
})
export class InventoryChangeComponent implements OnInit, OnChanges {
    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() exceldata: any;
    @Input() modval: any;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    buttondisabled = false;
    formData: any;
    postval: any;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    colCount: number;
    width: any;
    ProductList: any;
    url: string;
    editOnkeyPress: boolean;
    enterKeyAction: string;
    enterKeyDirection: string;
    Customerlist: any;
    buttonOptions: any = {
        text: '存檔',
        type: 'success',
        useSubmitBehavior: true,
        icon: 'save'
    };
    selectBoxOptions: any;
    controller: string;
    CustomerVal: string;
    labelLocation: string;
    itemval1: string;
    itemval2: string;
    itemval3: string;
    itemval4: string;
    minval: number;
    QuantityEditorOptions: any;
    PriceEditorOptions: any;
    UnitCountEditorOptions: any;

    constructor(private http: HttpClient) {
        // debugger;
        this.CustomerVal = null;
        this.formData = null;
        this.editOnkeyPress = true;
        this.enterKeyAction = 'moveFocus';
        this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 100;
        this.colCount = 2;
        this.url = location.origin + '/api';
        this.controller = '/OrderDetails';
        this.PriceEditorOptions = {showSpinButtons: true, mode: 'number', onValueChanged: this.PriceValueChanged.bind(this)};
        this.UnitCountEditorOptions = {showSpinButtons: true, mode: 'number', onValueChanged: this.UnitCountValueChanged.bind(this)};

    }
    public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>('/api' + apiUrl);
    }
    ngOnInit() {
    }
    ngOnChanges() {
        if (this.myform) {
            this.myform.instance.resetValues();
        }
        this.itemval1 = '';
        this.itemval2 = '';
        this.itemval3 = '';
        this.minval = 0;
        debugger;
        if (this.modval === 'material') {
            this.GetData('/Inventory/GetMaterialAdjustNo').subscribe(
                (s) => {
                    if (s.success) {
                        s.data.AdjustNo = '';
                        this.formData = s.data;
                    }
                }
            );
            this.GetData('/Materials/GetMaterial/' + this.itemkeyval).subscribe(
                (s) => {
                    console.log(s);
                    if (s.success) {
                        this.itemval1 = '元件品號：' + s.data.MaterialNo;
                        this.itemval2 = '元件品名：' + s.data.Name;
                        this.itemval3 = '　庫存數：' + s.data.Quantity;
                        this.itemval4 = '　　倉別：' + s.data.Warehouse.Name;
                        if (s.data.Quantity >= 0) {
                            this.minval = (-s.data.Quantity);
                        }
                        this.QuantityEditorOptions = {
                            showSpinButtons: true,
                            mode: 'number',
                            format: '#0',
                            value: 0,
                            min: this.minval,
                            onValueChanged: this.QuantityValueChanged.bind(this)
                        };
                    }
                }
            );
        } else if (this.modval === 'product') {
            this.GetData('/Inventory/GetProductAdjustNo').subscribe(
                (s) => {
                    if (s.success) {
                        s.data.AdjustNo = '';
                        this.formData = s.data;
                    }
                }
            );
            this.GetData('/Products/GetProduct/' + this.itemkeyval).subscribe(
                (s) => {
                    console.log(s);
                    if (s.success) {
                        this.itemval1 = '主件品號：' + s.data.ProductNo;
                        this.itemval2 = '主件品名：' + s.data.Name;
                        this.itemval3 = '　庫存數：' + s.data.Quantity;
                        this.itemval4 = '　　倉別：' + s.data.Warehouse.Name;
                        if (s.data.Quantity >= 0) {
                            this.minval = (-s.data.Quantity);
                        }
                        this.QuantityEditorOptions = {
                            showSpinButtons: true,
                            mode: 'number',
                            format: '#0',
                            value: 0,
                            min: this.minval,
                            onValueChanged: this.QuantityValueChanged.bind(this)
                        };
                    }
                }
            );
        }
    }
    QuantityValueChanged(e) {
        this.formData.PriceAll = this.formData.Price * e.value;
        this.formData.UnitCountAll = this.formData.UnitCount * e.value;
    }
    PriceValueChanged(e) {
        this.formData.PriceAll = this.formData.Quantity * e.value;
    }
    UnitCountValueChanged(e) {
        this.formData.UnitCountAll = this.formData.Quantity * e.value;
    }
    refreshAdjustNo() {
        if (this.modval === 'material') {
            this.GetData('/Inventory/GetMaterialAdjustNo').subscribe(
                (s) => {
                    if (s.success) {
                        this.formData.AdjustNo = s.data.AdjustNo;
                    }
                }
            );
        } else if (this.modval === 'product') {
            this.GetData('/Inventory/GetProductAdjustNo').subscribe(
                (s) => {
                    if (s.success) {
                        this.formData.AdjustNo = s.data.AdjustNo;
                    }
                }
            );
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
    onFormSubmit = async function(e) {
        // this.buttondisabled = true;
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }
        this.formData = this.myform.instance.option('formData');
        this.postval = new InventoryChange();
        if (this.modval === 'material') {
            this.postval.MaterialLog = this.formData;
        } else if (this.modval === 'product') {
            this.postval.ProductLog = this.formData;
        }
        this.postval.id = this.itemkeyval;
        this.postval.mod = this.modval;
        const sendRequest = await SendService.sendRequest(this.http, '/Inventory/inventorychange', 'POST', { values: this.postval });
        if (sendRequest) {
            this.myform.instance.resetValues();
            e.preventDefault();
            this.childOuter.emit(true);
        }
        this.buttondisabled = false;

    };

}
