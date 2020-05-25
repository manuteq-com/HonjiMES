import { Component, OnInit, ViewChild, EventEmitter, Output, Input, OnChanges } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DxFormComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { SendService } from '../../shared/mylib';
import { APIResponse } from '../../app.module';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-to-order-sale',
  templateUrl: './to-order-sale.component.html',
  styleUrls: ['./to-order-sale.component.css']
})
export class ToOrderSaleComponent implements OnInit {
    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    url = location.origin + '/api';
    buttondisabled: boolean;
    formData: any;
    labelLocation: string;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    colCount: number;
    width: any;
    buttonOptions: any = { text: '存檔', type: 'success', useSubmitBehavior: true };
    selectBoxOptions: any;
    NumberBoxOptions: any;
    constructor(private http: HttpClient) {
        this.formData = null;
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 100;
        this.colCount = 1;
    }
    public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>(apiUrl);
    }
    ngOnChanges() {
        debugger;
        this.NumberBoxOptions = { showSpinButtons: true, mode: 'number', max: this.itemkeyval.qty, min: 1, value: this.itemkeyval.qty};
        this.GetData(this.url + '/Warehouses/GetWarehouseByProductBasic/' + this.itemkeyval.ProductBasicId).subscribe(
            (s) => {
                debugger;
                if (s.success) {
                    this.selectBoxOptions = {
                        items: s.data,
                        displayExpr: 'Name',
                        valueExpr: 'Id',
                    };
                }
            }
        );
    }
    ngOnInit() {
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
        debugger;
        this.buttondisabled = true;
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }
        this.formData = this.myform.instance.option('formData');
        this.formData.SaleDID = this.itemkeyval.key;

        const sendRequest = await SendService.sendRequest(this.http, '/ToSale/OrderSale', 'POST', { values: this.formData });
        if (sendRequest) {
            this.myform.instance.resetValues();
            e.preventDefault();
            this.childOuter.emit(true);
            notify({
                message: '銷貨完成',
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'success', 3000);
            this.dataGrid.instance.refresh();
        }
        this.buttondisabled = false;

    };
}