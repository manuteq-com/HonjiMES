import { Component, OnInit, Input, Output, EventEmitter, ViewChild, OnChanges } from '@angular/core';
import { DxFormComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { SendService } from 'src/app/shared/mylib';
import { HttpClient } from '@angular/common/http';
import { APIResponse } from 'src/app/app.module';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-workorder-report',
  templateUrl: './workorder-report.component.html',
  styleUrls: ['./workorder-report.component.css']
})
export class WorkorderReportComponent implements OnInit, OnChanges {
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
    UnitPriceEditorOptions: any;

    itemval1: string;
    itemval3: string;
    itemval2: string;
    itemval4: string;
    itemval5: string;
    itemval6: string;
    itemval7: string;
    itemval8: string;
    itemval9: string;

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
        if (this.itemkeyval != null) {
            this.GetData('/Processes/GetProcessByWorkOrderId/' + this.itemkeyval).subscribe(
                (s) => {
                    if (s.success) {
                        debugger;
                        this.itemval1 = '　　　　　　　工單號：' + s.data.WorkOrderHead.WorkOrderNo;
                        this.itemval2 = '　　　　　　　　品號：' + s.data.WorkOrderHead.DataNo;
                        this.itemval3 = '　　　　　　　　名稱：' + s.data.WorkOrderHead.DataName;
                        this.itemval4 = '　　　　　　　　機號：' + s.data.WorkOrderHead.MachineNo;
                        this.itemval5 = '　預計／實際完工數量：' + s.data.WorkOrderHead.Count + ' / ' + s.data.WorkOrderHead.ReCount;
                        this.itemval6 = '　　　　　預計開工日：' + s.data.WorkOrderHead?.DueStartTime ?? '';
                        this.itemval7 = '　　　　　預計完工日：' + s.data.WorkOrderHead?.DueEndTime ?? '';
                        this.itemval8 = '　　　　　實際開工日：' + s.data.WorkOrderHead?.ActualStartTime ?? '';
                        this.itemval9 = '　　　　　實際完工日：' + s.data.WorkOrderHead?.ActualEndTime ?? '';

                        s.data.WorkOrderDetail.forEach(element => {
                            if (element.Status === 1) {
                                this.formData = element;
                            }
                        });
                        // this.dataSourceDB = s.data.WorkOrderDetail;
                        // this.formData.WorkOrderHeadId = s.data.WorkOrderHead.Id;
                        // this.formData.WorkOrderNo = s.data.WorkOrderHead.WorkOrderNo;
                        // this.formData.CreateTime = s.data.WorkOrderHead.CreateTime;
                        // this.formData.ProductBasicId = s.data.WorkOrderHead.DataId;
                        // this.formData.Count = s.data.WorkOrderHead.Count;
                        // this.formData.MachineNo = s.data.WorkOrderHead.MachineNo;
                        // this.formData.Remarks = s.data[0].Remarks;

                    }
                }
            );
        }

        // this.PriceEditorOptions = {showSpinButtons: true, mode: 'number', onValueChanged: this.PriceValueChanged.bind(this)};
        // this.UnitCountEditorOptions = {showSpinButtons: true, mode: 'number', onValueChanged: this.UnitCountValueChanged.bind(this)};
        // this.UnitPriceEditorOptions = {showSpinButtons: true, mode: 'number', onValueChanged: this.UnitPriceValueChanged.bind(this)};
    }
    // QuantityValueChanged(e) {
    //     this.formData.PriceAll = this.formData.Price * e.value;
    //     // this.formData.UnitPrice = this.formData.UnitCount * e.value;
    // }
    // PriceValueChanged(e) {
    //     this.formData.PriceAll = this.formData.Quantity * e.value;
    // }
    // UnitCountValueChanged(e) {
    //     this.formData.UnitPriceAll = this.formData.UnitPrice * e.value;
    // }
    // UnitPriceValueChanged(e) {
    //     this.formData.UnitPriceAll = this.formData.UnitCount * e.value;
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
    async onFormSubmit(e) {
        // this.buttondisabled = true;
        // if (this.validate_before() === false) {
        //     this.buttondisabled = false;
        //     return;
        // }
        // this.formData = this.myform.instance.option('formData');
        // this.postval = this.formData;
        // this.postval.BillofPurchaseDetailId = this.itemkeyval;
        // debugger;
        // try {
        //     // tslint:disable-next-line: max-line-length
        //     const sendRequest = await SendService.sendRequest(this.http, '/ToPurchase/PostPurchaseCheckIn', 'POST', { values: this.postval });
        //     // let data = this.client.POST( this.url + '/OrderHeads/PostOrderMaster_Detail').toPromise();
        //     debugger;
        //     if (sendRequest) {
        //         this.myform.instance.resetValues();
        //         this.CustomerVal = null;
        //         e.preventDefault();
        //         this.childOuter.emit(true);
        //     }
        // } catch (error) {

        // }
        // this.buttondisabled = false;

    }
}
