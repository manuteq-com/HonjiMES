import { Component, OnInit, EventEmitter, Output, Input, ViewChild } from '@angular/core';
import { DxFormComponent, DxDataGridComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from '../shared/mylib';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { APIResponse } from '../app.module';
import { InventoryChange } from '../model/viewmodels';

@Component({
    selector: 'app-inventory-change',
    templateUrl: './inventory-change.component.html',
    styleUrls: ['./inventory-change.component.css']
})
export class InventoryChangeComponent implements OnInit {
    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() exceldata: any;
    @Input() modval: any;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    buttondisabled = false;
    formData: InventoryChange;
    dataSourceDB: any;
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
    editorOptions: any;
    minval: number;
    ngOnInit() {
    }
    // tslint:disable-next-line: use-lifecycle-interface
    ngOnChanges() {
        if (this.myform) {
            this.myform.instance.resetValues();
        }
        this.itemval1 = '';
        this.itemval2 = '';
        this.itemval3 = '';
        this.minval = 0;
        if (this.modval === 'material') {
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
                        this.editorOptions = {showSpinButtons: true, mode: 'number', format: '#0', value: 0, min: this.minval};
                    }
                }
            );
        } else if (this.modval === 'product') {

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
                        this.editorOptions = {showSpinButtons: true, mode: 'number', format: '#0', value: 0, min: this.minval};
                    }
                }
            );
        }
    }
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
        this.colCount = 1;
        this.url = location.origin + '/api';
        this.dataSourceDB = [];
        this.controller = '/OrderDetails';

    }
    public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>('/api' + apiUrl);
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
        this.postval = this.formData as InventoryChange;
        this.postval.id = this.itemkeyval;
        this.postval.mod = this.modval;
        const sendRequest = await SendService.sendRequest(this.http, '/Inventory/inventorychange', 'POST', { values: this.postval });
        if (sendRequest) {
            this.dataSourceDB = null;
            this.myform.instance.resetValues();
            e.preventDefault();
            this.childOuter.emit(true);
        }
        this.buttondisabled = false;

    };

}
