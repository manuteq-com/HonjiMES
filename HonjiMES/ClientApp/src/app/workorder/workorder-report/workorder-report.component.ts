import { Component, OnInit, Input, Output, EventEmitter, ViewChild, OnChanges } from '@angular/core';
import { DxFormComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { SendService } from 'src/app/shared/mylib';
import { HttpClient } from '@angular/common/http';
import { APIResponse } from 'src/app/app.module';
import { Observable } from 'rxjs';
import { workOrderReportData } from 'src/app/model/viewmodels';

@Component({
  selector: 'app-workorder-report',
  templateUrl: './workorder-report.component.html',
  styleUrls: ['./workorder-report.component.css']
})
export class WorkorderReportComponent implements OnInit, OnChanges {
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() serialkeyval: any;
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
    ProcessesList: any;

    itemval1: string;
    itemval3: string;
    itemval2: string;
    itemval4: string;
    itemval5: string;
    itemval6: string;
    itemval7: string;
    itemval8: string;
    itemval9: string;
    itemval10: string;
    itemval11: string;
    itemval12: string;
    itemval13: string;
    itemval14: string;
    itemval15: string;
    itemval16: string;
    itemval17: string;
    itemval18: string;
    itemval19: string;

    ProcessEditorOptions: any;
    startBtnVisible: boolean;
    endBtnVisible: boolean;
    restartedBtnVisible: boolean;
    ReCountVisible: boolean;
    RemarkVisible: boolean;
    DateBoxOptions: any;

    constructor(private http: HttpClient) {
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 300;
        this.colCount = 3;
        this.labelLocation = 'left';

        this.DateBoxOptions = {
            displayFormat: 'yyyy/MM/dd HH:mm:ss',
            // onValueChanged: this.PurchaseDateValueChange.bind(this)
        };

        this.GetData('/Processes/GetProcesses').subscribe(
            (s) => {
                if (s.success) {
                    s.data.forEach(element => {
                        element.Name = element.Code + '_' + element.Name;
                    });
                    this.ProcessEditorOptions = {
                        dataSource: { paginate: true, store: { type: 'array', data: s.data, key: 'Id' } },
                        searchEnabled: true,
                        // items: this.MaterialList,
                        displayExpr: 'Name',
                        valueExpr: 'Id',
                        // onValueChanged: this.ProcessEditorOptions.bind(this)
                    };
                }
            }
        );
    }
    public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>('/api' + apiUrl);
    }
    ngOnInit() {
    }
    ngOnChanges() {
        this.ReCountVisible = false;
        this.RemarkVisible = false;
        this.startBtnVisible = false;
        this.endBtnVisible = false;
        this.restartedBtnVisible = false;

        if (this.itemkeyval != null) {
            this.GetData('/Processes/GetProcessByWorkOrderId/' + this.itemkeyval).subscribe(
                (s) => {
                    if (s.success) {
                        this.itemval1 = '　　　　　　　　　　　工單號：　' + s.data.WorkOrderHead.WorkOrderNo;
                        this.itemval2 = '　　　　　　　　　　　　品號：　' + s.data.WorkOrderHead.DataNo;
                        this.itemval3 = '　　　　　　　　　　　　名稱：　' + s.data.WorkOrderHead.DataName;
                        this.itemval4 = '　　　　　　　　　　　　機號：　' + (s.data.WorkOrderHead?.MachineNo ?? '');
                        // this.itemval5 = '　　　　　預計／實際完工數量：　' + s.data.WorkOrderHead.Count + ' / ' + s.data.WorkOrderHead.ReCount;
                        this.itemval5 = '　　　　　　　　　　　　　　　';
                        this.itemval6 = '';

                        let findProcess = false;
                        s.data.WorkOrderDetail.forEach(element => {
                            if (element.SerialNumber === this.serialkeyval && findProcess === false) {
                                this.itemval7 = '　　　　　　　　　　製程序號：　' + element.SerialNumber;
                                this.itemval8 = '　　　　　　　　　　製程名稱：　[' + element.ProcessNo + '] ' + element.ProcessName;
                                this.itemval9 = '　　　　　　　　　　　　圖號：　' + (element?.DrawNo ?? '');
                                this.itemval10 = '　　　　　　　　　　所需人力：　' + (element?.Manpower ?? '');
                                this.itemval11 = '　　　　　　　　　　　　備註：　' + (element?.Remarks ?? '');
                                this.itemval12 = '　　　　　　　前置時間（分）：　' + (element?.ProcessLeadTime ?? '');
                                this.itemval13 = '　　　　　　　標準工時（分）：　' + (element?.ProcessTime ?? '');
                                this.itemval14 = '　　　　　　　　　預計開工日：　' + (element?.DueStartTime ?? '');
                                this.itemval15 = '　　　　　　　　　預計完工日：　' + (element?.DueEndTime ?? '');
                                this.itemval16 = '　　　　　　　　　實際開工日：　' + (element?.ActualStartTime ?? '');
                                this.itemval17 = '　　　　　　　　　實際完工日：　' + (element?.ActualEndTime ?? '');
                                this.itemval18 = '　　　　　　　　　　需求數量：　' + (element?.Count ?? '0');
                                this.itemval19 = '　　　　　　　　　　完工數量：　' + (element?.ReCount ?? '0');

                                this.formData = element;
                                this.formData.ReCount = element.Count;
                                findProcess = true;

                                if (element.Status === 1) {
                                    this.startBtnVisible = true;
                                } else if (element.Status === 2) {
                                    this.ReCountVisible = true;
                                    this.RemarkVisible = true;
                                    this.endBtnVisible = true;
                                } else if (element.Status === 3) {
                                    this.restartedBtnVisible = true;
                                    this.RemarkVisible = true;
                                }
                            }
                        });
                    }
                }
            );
        }
    }
    onStartClick(e) {
        this.modval = 'start';
    }
    onEndClick(e) {
        this.modval = 'end';
    }
    onRestartClick(e) {
        this.modval = 'restart';
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
        // this.formData = this.myform.instance.option('formData');
        // this.postval = this.formData;
        // tslint:disable-next-line: new-parens
        this.postval = new workOrderReportData;
        this.postval.WorkOrderID = this.itemkeyval;
        this.postval.WorkOrderSerial = this.serialkeyval;
        this.postval.ReCount = this.formData.ReCount;
        this.postval.Remarks = this.formData.Remark;
        try {
            if (this.modval === 'start') {
                // tslint:disable-next-line: max-line-length
                const sendRequest = await SendService.sendRequest(this.http, '/WorkOrders/WorkOrderReportStart', 'POST', { values: this.postval });
                this.viewRefresh(e, sendRequest);
            } else if (this.modval === 'end') {
                // tslint:disable-next-line: max-line-length
                const sendRequest = await SendService.sendRequest(this.http, '/WorkOrders/WorkOrderReportEnd', 'POST', { values: this.postval });
                this.viewRefresh(e, sendRequest);
            } else if (this.modval === 'restart') {
                // tslint:disable-next-line: max-line-length
                const sendRequest = await SendService.sendRequest(this.http, '/WorkOrders/WorkOrderReportRestart', 'POST', { values: this.postval });
                this.viewRefresh(e, sendRequest);
            }
        } catch (error) {

        }
        this.buttondisabled = false;
    }
    viewRefresh(e, result) {
        if (result) {
            this.myform.instance.resetValues();
            this.CustomerVal = null;
            e.preventDefault();
            this.childOuter.emit(true);
        }
    }

}
