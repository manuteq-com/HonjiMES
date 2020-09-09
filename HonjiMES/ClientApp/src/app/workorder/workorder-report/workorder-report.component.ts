import { Component, OnInit, Input, Output, EventEmitter, ViewChild, OnChanges } from '@angular/core';
import { DxFormComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { SendService } from 'src/app/shared/mylib';
import { HttpClient } from '@angular/common/http';
import { APIResponse } from 'src/app/app.module';
import { Observable } from 'rxjs';
import { workOrderReportData } from 'src/app/model/viewmodels';
import { AppComponent } from 'src/app/app.component';

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
    @Input() randomkey: any;
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
    NgEditorOptions: any;
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
    NgCountVisible: boolean;
    RemarkVisible: boolean;
    DateBoxOptions: any;
    NoPurchaseVisible: any;
    selectBoxOptions: any;
    HasPurchaseVisible: boolean;
    PurchaseHeadList: any;
    editorOptions: any;
    creatpopupVisible: boolean;
    itemkey: any;
    mod: any;
    dataSource: any[];
    ProductBasicList: any;

    constructor(private http: HttpClient, public app: AppComponent) {
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 300;
        this.colCount = 4;
        this.labelLocation = 'left';

        this.DateBoxOptions = {
            displayFormat: 'yyyy/MM/dd HH:mm:ss',
            // onValueChanged: this.PurchaseDateValueChange.bind(this)
        };
        this.PriceEditorOptions = {
            showSpinButtons: true,
            mode: 'number',
            // onValueChanged: this.PriceValueChanged.bind(this)
        };
    }
    ngOnInit() {
    }
    ngOnChanges() {
        // debugger;
        this.ReCountVisible = false;
        this.NgCountVisible = false;
        this.RemarkVisible = false;
        this.startBtnVisible = false;
        this.endBtnVisible = false;
        this.restartedBtnVisible = false;
        this.NoPurchaseVisible = false;
        this.HasPurchaseVisible = false;
        this.PurchaseHeadList = [];

        this.app.GetData('/Processes/GetProcesses').subscribe(
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
        this.app.GetData('/Suppliers/GetSuppliers').subscribe(
            (s) => {
                if (s.success) {
                    s.data.forEach(x => {
                        x.Name = x.Code + '_' + x.Name;
                    });
                    this.selectBoxOptions = {
                        items: s.data,
                        displayExpr: 'Name',
                        valueExpr: 'Id',
                        searchEnabled: true,
                        onValueChanged: this.onSupplierSelectionChanged.bind(this)
                    };
                }
            }
        );
        // this.app.GetData('/PurchaseHeads/GetNotEndPurchaseHeads').subscribe(
        //     (s) => {
        //         if (s.success) {
        //             this.PurchaseHeadList = s.data;
        this.editorOptions = {
            dataSource: { paginate: true, store: { type: 'array', data: this.PurchaseHeadList, key: 'Id' } },
            searchEnabled: true,
            displayExpr: 'PurchaseNo',
            valueExpr: 'Id'
        };
        //         }
        //     }
        // );
        this.NgEditorOptions = {
            showSpinButtons: true,
            mode: 'number',
            format: '#0',
            value: '0',
            min: '0',
            // onValueChanged: this.QuantityValueChanged.bind(this)
        };

        if (this.itemkeyval != null) {
            this.app.GetData('/Processes/GetProcessByWorkOrderId/' + this.itemkeyval).subscribe(
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
                                this.itemval19 = '　　　　　　　　　　完工數量：　' + (element?.ReCount ?? '0') + '　　( NG數量：' + element?.NgCount + ' )';

                                this.QuantityEditorOptions = {
                                    showSpinButtons: true,
                                    mode: 'number',
                                    format: '#0',
                                    value: (element?.Count ?? '0'),
                                    min: -(element?.ReCount ?? '0'),
                                    // onValueChanged: this.QuantityValueChanged.bind(this)
                                };

                                this.formData = element;
                                this.formData.ReCount = element.Count;
                                this.formData.NgCount = 0;
                                findProcess = true;

                                if (element.Status === 1) {
                                    // 如工序為委外(有採購單)，則需選擇採購單or新建採購單
                                    if (element.Type === 1) {
                                        this.ReCountVisible = true;
                                        this.HasPurchaseVisible = true;
                                    } else {
                                        this.startBtnVisible = true;
                                    }
                                } else if (element.Status === 2) {
                                    this.ReCountVisible = true;
                                    this.NgCountVisible = true;
                                    this.RemarkVisible = true;
                                    this.endBtnVisible = true;

                                    // 如工序為委外(無採購單)，則需填入供應商、金額
                                    if (element.Type === 2) {
                                        this.NoPurchaseVisible = true;
                                    }
                                } else if (element.Status === 3) {
                                    // 如工序為委外(有採購單)，則需選擇採購單or新建採購單
                                    if (element.Type === 1) {
                                        this.ReCountVisible = true;
                                        this.HasPurchaseVisible = true;
                                    } else {
                                        this.restartedBtnVisible = true;
                                        this.RemarkVisible = true;
                                    }
                                }
                            }
                        });
                    }
                }
            );
        }
    }
    PurchaseNoValueChanged(e) {

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
    onPurchaseClick(e) {
        this.modval = 'purchase';
    }
    onNewPurchaseClick(e) {
        this.modval = 'newpurchase';
    }
    creatpopup_result(e) {
        this.creatpopupVisible = false;
        this.ShowMessage('存檔完成', 'success', 3000);
    }
    onSupplierSelectionChanged(e) {
        this.app.GetData('/PurchaseHeads/GetNotEndPurchaseHeadsSurfaceBySupplier/' + e.value).subscribe(
            (s) => {
                if (s.success) {
                    this.PurchaseHeadList = s.data;
                    this.editorOptions = {
                        dataSource: { paginate: true, store: { type: 'array', data: this.PurchaseHeadList, key: 'Id' } },
                        searchEnabled: true,
                        displayExpr: 'PurchaseNo',
                        valueExpr: 'Id'
                    };
                }
            }
        );
    }
    validate_before(): boolean {
        // 表單驗證
        if (this.myform.instance.validate().isValid === false) {
            this.ShowMessage('請注意訂單內容必填的欄位', 'error', 3000);
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
        // this.postval = this.formData;
        // tslint:disable-next-line: new-parens
        this.postval = new workOrderReportData;
        this.postval.WorkOrderID = this.itemkeyval;
        this.postval.WorkOrderSerial = this.serialkeyval;
        this.postval.ReCount = this.formData.ReCount;
        this.postval.RePrice = this.formData?.RePrice ?? 0;
        this.postval.NgCount = this.formData.NgCount;
        this.postval.Message = this.formData.Message;
        this.postval.ProducingMachine = this.formData.ProducingMachine;
        this.postval.SupplierId = this.formData.SupplierId;
        this.postval.PurchaseId = this.formData.PurchaseId;
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
            } else if (this.modval === 'purchase') {
                if (this.postval.PurchaseId != null) {
                    const result = this.PurchaseHeadList.find(x => x.Id === this.postval.PurchaseId);
                    if (result) {
                        this.postval.PurchaseNo = result.PurchaseNo;
                        // tslint:disable-next-line: max-line-length
                        const sendRequest = await SendService.sendRequest(this.http, '/WorkOrders/ReportWorkOrderByPurchase', 'PUT', { key: this.postval.WorkOrderID, values: this.postval });
                        this.viewRefresh(e, sendRequest);
                    }
                } else {
                    this.ShowMessage('請選擇採購單號', 'error', 3000);
                }
            } else if (this.modval === 'newpurchase') {
                this.GetPurchaseDetailData();
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
    ShowMessage(text, type, val) {
        notify({
            message: text,
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, type, val);
    }
    GetPurchaseDetailData() {
        this.dataSource = [];
        this.app.GetData('/Inventory/GetBasicsData').subscribe(
            (s) => {
                if (s.success) {
                    const result = s.data.find(x =>
                        x.DataType === this.formData.WorkOrderHead.DataType &&
                        x.DataId === this.formData.WorkOrderHead.DataId
                    );
                    if (result) {
                        this.dataSource.push({
                            Serial: 1,
                            TempId: result.TempId,
                            DataType: result.DataType,
                            DataId: result.DataId,
                            WarehouseId: result.WarehouseId,
                            WarehouseIdA: result.WarehouseIdA,
                            WarehouseIdB: result.WarehouseIdB,
                            Quantity: this.postval.ReCount,
                            OriginPrice: result.Price,
                            Price: this.postval.ReCount * result.Price,
                            DeliveryTime: new Date()
                        });
                    }
                    this.mod = 'workorder';
                    this.creatpopupVisible = true;
                }
            }
        );
    }
}
