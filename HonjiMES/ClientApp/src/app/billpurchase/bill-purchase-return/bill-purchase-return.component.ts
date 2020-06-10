import { Component, OnInit, Input, Output, EventEmitter, ViewChild, OnChanges } from '@angular/core';
import { DxFormComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { SendService } from 'src/app/shared/mylib';
import { HttpClient } from '@angular/common/http';
import { APIResponse } from 'src/app/app.module';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-bill-purchase-return',
  templateUrl: './bill-purchase-return.component.html',
  styleUrls: ['./bill-purchase-return.component.css']
})
export class BillPurchaseReturnComponent implements OnInit {
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
    selectBoxOptions: any;
    NumberBoxOptions: any;
    editorOptions: { showSpinButtons: boolean; mode: string; format: string; value: number; min: number; max: number; };

    constructor(private http: HttpClient) {
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 300;
        this.colCount = 1;
        this.labelLocation = 'left';
    }
    public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>('/api' + apiUrl);
    }
    ngOnInit() {
    }
    ngOnChanges() {
        this.GetData('/ToPurchase/GetBillofPurchaseReturnNo').subscribe(
            (s) => {
                if (s.success) {
                    this.formData = s.data;
                }
            }
        );
        this.GetData('/Warehouses/GetWarehouseByMaterial/' + this.itemkeyval.DataId).subscribe(
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
        this.GetData('/ToPurchase/CanCheckIn/' + this.itemkeyval.Id).subscribe(
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
    refreshReturnNo() {
        this.GetData('/ToPurchase/GetBillofPurchaseReturnNo').subscribe(
            (s) => {
                if (s.success) {
                    this.formData.ReturnNo = s.data.ReturnNo;
                }
            }
        );
    }
    async onFormSubmit(e) {
        this.buttondisabled = true;
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }
        this.formData = this.myform.instance.option('formData');
        this.postval =  this.formData;
        this.postval.BillofPurchaseDetailId = this.itemkeyval.Id;
        debugger;
        try {
            // tslint:disable-next-line: max-line-length
            const sendRequest = await SendService.sendRequest(this.http, '/ToPurchase/PostPurchaseCheckReturn', 'POST', { values: this.postval });
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
