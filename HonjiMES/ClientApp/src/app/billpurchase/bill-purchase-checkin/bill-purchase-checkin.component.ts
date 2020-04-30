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
    editorOptions: { showSpinButtons: boolean; mode: string; format: string; value: number; min: number; max: number;};

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
        debugger;
        this.GetData('/ToPurchase/CanCheckIn/' + this.itemkeyval).subscribe(
            (s) => {
                if (s.success) {
                    this.editorOptions = { showSpinButtons: true, mode: 'number', format: '#0', value: 1, min: 1, max: s.data.Quantity };
                }
            }
        );

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
        this.postval =  this.formData;
        this.postval.BillofPurchaseDetailId = this.itemkeyval;
        debugger;
        try {
            // tslint:disable-next-line: max-line-length
            const sendRequest = await SendService.sendRequest(this.http, '/ToPurchase/PostPurchaseCheckIn', 'POST', { values: this.postval });
            // let data = this.client.POST( this.url + '/OrderHeads/PostOrderMaster_Detail').toPromise();
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
