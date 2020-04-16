import { Component, OnInit, EventEmitter, Output, Input, ViewChild, OnChanges } from '@angular/core';
import { DxFormComponent } from 'devextreme-angular';
import { SendService } from '../shared/mylib';
import { Observable } from 'rxjs';
import { APIResponse } from '../app.module';
import { HttpClient } from '@angular/common/http';
import notify from 'devextreme/ui/notify';

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
    url = location.origin + '/api';
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
    Seallist: any;
    selectBoxOptions: any;
    buttonOptions: any =  {text: '存檔', type: 'success', useSubmitBehavior: true};
    ngOnChanges() {
        if (this.modval === 'add') {
            this.showdisabled = false;
        } else {
            this.showdisabled = true;
        }
        this.GetData(this.url + '/Sales/GetSales?status=0').subscribe(
            (s) => {
                console.log(s);
                if (s.success) {
                    this.Seallist = s.data;
                    this.selectBoxOptions = {
                        searchMode: 'startswith',
                        searchEnabled: true,
                        items: this.Seallist,
                        displayExpr: 'SaleNo',
                        valueExpr: 'Id',
                        onValueChanged: this.onCustomerSelectionChanged.bind(this)
                    };
                }
            }
        );
    }
    constructor(private http: HttpClient) {
        this.formData = null;
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 100;
        this.colCount = 3;

        this.startTimeInput = {
            min: new Date().toDateString(),
            value: new Date().toDateString()
        };
    }
    public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>(apiUrl);
    }
    ngOnInit() {
    }
    onCustomerSelectionChanged(e) {
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
        this.formData.Orderlist = this.itemkeyval;
        const sendRequest = await SendService.sendRequest(this.http, '/ToSale/OrderToSale', 'POST', { values: this.formData });
        if (sendRequest) {
            this.myform.instance.resetValues();
            e.preventDefault();
            this.childOuter.emit(true);
        }
        this.buttondisabled = false;

    };
}
