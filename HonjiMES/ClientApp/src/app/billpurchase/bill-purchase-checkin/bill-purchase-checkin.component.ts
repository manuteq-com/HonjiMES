import { Component, OnInit, Input, Output, EventEmitter, ViewChild, OnChanges } from '@angular/core';
import { DxFormComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { SendService } from 'src/app/shared/mylib';
import { HttpClient } from '@angular/common/http';
import { APIResponse } from 'src/app/app.module';
import { Observable } from 'rxjs';

@Component({
    selector: 'app-bill-purchase-checkin',
    templateUrl: './bill-purchase-checkin.component.html',
    styleUrls: ['./bill-purchase-checkin.component.css']
})
export class BillPurchaseCheckinComponent implements OnInit, OnChanges {
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() modval: any;
    buttondisabled = false;
    CustomerVal: any;
    formData: any;
    postval: any;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    width: any;
    colCount: number;
    labelLocation: string;
    QuantityEditorOptions: any;
    PriceEditorOptions: any;
    UnitCountEditorOptions: any;

    constructor(private http: HttpClient) {
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 300;
        this.colCount = 2;
        this.labelLocation = 'left';
    }
    public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>('/api' + apiUrl);
    }
    ngOnInit() {
    }
    ngOnChanges() {
        this.formData = {
            Quantity: 0,
            Price: 0,
            PriceAll: 0,
            Unit: '',
            UnitCount: 0,
            UnitPrice: 0,
            UnitPriceAll: 0,
            WorkPrice: 0,
            Remarks: ''
        };
        this.GetData('/ToPurchase/CanCheckIn/' + this.itemkeyval).subscribe(
            (s) => {
                if (s.success) {
                    this.QuantityEditorOptions = {
                        showSpinButtons: true,
                        mode: 'number',
                        format: '#0',
                        value: s.data.Quantity,
                        min: 1,
                        max: s.data.Quantity,
                        onValueChanged: this.QuantityValueChanged.bind(this)
                    };
                }
            }
        );
        this.PriceEditorOptions = {showSpinButtons: true, mode: 'number', onValueChanged: this.PriceValueChanged.bind(this)};
        this.UnitCountEditorOptions = {showSpinButtons: true, mode: 'number', onValueChanged: this.UnitCountValueChanged.bind(this)};
    }
    QuantityValueChanged(e) {
        this.formData.PriceAll = this.formData.Price * e.value;
        // this.formData.UnitPrice = this.formData.UnitCount * e.value;
    }
    PriceValueChanged(e) {
        this.formData.PriceAll = this.formData.Quantity * e.value;
    }
    UnitCountValueChanged(e) {
        // this.formData.UnitPrice = this.formData.Quantity * e.value;
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
    async onFormSubmit(e) {
        this.buttondisabled = true;
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }
        this.formData = this.myform.instance.option('formData');
        this.postval = this.formData;
        this.postval.BillofPurchaseDetailId = this.itemkeyval;
        debugger;
        try {
            // tslint:disable-next-line: max-line-length
            const sendRequest = await SendService.sendRequest(this.http, '/ToPurchase/PostPurchaseCheckIn', 'POST', { values: this.postval });
            // let data = this.client.POST( this.url + '/OrderHeads/PostOrderMaster_Detail').toPromise();
            debugger;
            if (sendRequest) {
                this.myform.instance.resetValues();
                this.CustomerVal = null;
                e.preventDefault();
                this.childOuter.emit(true);
            }
        } catch (error) {

        }
        this.buttondisabled = false;

    }
}
