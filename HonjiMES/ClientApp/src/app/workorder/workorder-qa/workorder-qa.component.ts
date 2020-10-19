import { Component, OnInit, OnChanges, ViewChild, Input, HostListener } from '@angular/core';
import { APIResponse } from 'src/app/app.module';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { createStore } from 'devextreme-aspnet-data-nojquery';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from 'src/app/shared/mylib';
import DataSource from 'devextreme/data/data_source';
import notify from 'devextreme/ui/notify';
import { DxDataGridComponent, DxFormComponent } from 'devextreme-angular';
import { AppComponent } from 'src/app/app.component';
import { mBillOfMaterial, workOrderQcData } from 'src/app/model/viewmodels';
import { Myservice } from 'src/app/service/myservice';
import Swal from 'sweetalert2';
import { Title } from '@angular/platform-browser';

@Component({
    selector: 'app-workorder-qa',
    templateUrl: './workorder-qa.component.html',
    styleUrls: ['./workorder-qa.component.css']
})
export class WorkorderQaComponent implements OnInit, OnChanges {
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild('dataGrid2') dataGrid2: DxDataGridComponent;
    dataSourceDB: any;


    formData: any;
    labelLocation: string;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    colCount: number;
    width: any;

    buttondisabled: boolean;
    QcTypeList: any;
    QcResultList: any;
    selectCreateUser: any;
    selectQcType: any;
    selectQcResult: { items: any; displayExpr: string; valueExpr: string; searchEnabled: boolean; value: number; };
    CountEditorOptions: any;
    UserList: any[];
    UserListAll: any;













    dataSourceDB_Process: any[];
    Controller = '/WorkOrders';
    remoteOperations = true;
    editOnkeyPress: boolean;
    enterKeyAction: string;
    enterKeyDirection: string;
    detailfilter = [];
    listStatus: any;
    ProcessBasicList: any;
    postval: any;
    logpopupVisible: boolean;
    stockpopupVisible: boolean;
    itemkey: any;
    btnDisabled: boolean;
    workOrderHeadNo: any;
    workOrderHeadId: any;
    workOrderHeadDataNo: any;
    workOrderHeadDataName: any;
    workOrderHeadStatus: any;
    workOrderHeadCount: any;
    WorkOrderNoInputVal: any;
    modkey: string;
    workOrderHeadDataType: any;
    workOrderHeadDataId: any;
    randomkey: number;
    keyup = '';
    userkey: any;
    closepopupVisible: boolean;
    clearCheck: any;
    listType: any;

    @HostListener('window:keyup', ['$event']) keyUp(e: KeyboardEvent) {
        if (!this.stockpopupVisible && !this.closepopupVisible && !this.logpopupVisible) {
            this.stockpopupVisible = false;
            this.closepopupVisible = false;
            if (e.key === 'Enter') {
                const key = this.keyup;
                const promise = new Promise((resolve, reject) => {
                    this.app.GetData('/WorkOrders/GetWorkOrderHeadByWorkOrderNo?DataNo=' + key).toPromise().then((res: APIResponse) => {
                        this.workOrderHeadNo = '';
                        if (res.success) {
                            this.btnDisabled = true;
                            this.dataSourceDB_Process = [];
                            this.clearCheck = false;
                            this.workOrderHeadNo = res.data.WorkOrderNo;
                            if (res.data.Status === 0) {
                                this.showMessage('warning', '[ ' + res.data.WorkOrderNo + ' ]　工單尚未派工!', 3000);
                            } else if (res.data.Status === 5) {
                                this.showMessage('warning', '[ ' + res.data.WorkOrderNo + ' ]　工單已經結案!', 3000);
                            } else {
                                this.readProcess(null, res);
                            }
                        } else {
                            const promise2 = new Promise((resolve2, reject2) => {
                                this.app.GetData('/Users/GetUserByUserNo?DataNo=' + key).toPromise().then((res2: APIResponse) => {
                                    if (res2.success) {
                                        if (!this.btnDisabled) {
                                            if (res2.data.Permission === 70 || res2.data.Permission === 20) {
                                                this.stockdata(res2.data.Id);
                                            } else {
                                                this.showMessage('warning', '請勿越權使用!', 3000);
                                            }
                                        } else {
                                            this.showMessage('warning', '請先掃描工單碼!', 3000);
                                        }
                                    } else {
                                        this.showMessage('error', '查無資料!', 3000);
                                    }
                                },
                                    err => {
                                        // Error
                                        reject2(err);
                                    }
                                );
                            });
                            // this.showMessage('error', '[ ' + res.data.WorkOrderNo + ' ]　查無資料!', 3000);
                        }
                    },
                        err => {
                            // Error
                            reject(err);
                        }
                    );
                });
                this.keyup = '';
            } else if (e.key === 'Shift' || e.key === 'CapsLock') {

            } else {
                this.keyup += e.key.toLocaleUpperCase();
            }
        }
    }

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent, private titleService: Title) {
        this.QcTypeList = myservice.getQcType();
        this.QcResultList = myservice.getQcResult();
        this.onReorder = this.onReorder.bind(this);
        this.buttondisabled = true;

        this.formData = null;
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 100;
        this.colCount = 1;
        this.selectQcType = {
            items: this.QcTypeList,
            displayExpr: 'Name',
            valueExpr: 'Id',
            searchEnabled: true,
            value: null
        };
        this.selectQcResult = {
            items: this.QcResultList,
            displayExpr: 'Name',
            valueExpr: 'Id',
            searchEnabled: true,
            value: null
        };
        this.CountEditorOptions = {
            showSpinButtons: true,
            mode: 'number',
            format: '#0',
            value: '0',
            min: '0',
            // onValueChanged: this.QuantityValueChanged.bind(this)
        };
        this.app.GetData('/Users/GetUsers').subscribe(
            (s2) => {
                if (s2.success) {
                    this.UserListAll = s2.data;
                }
            }
        );

        //// 測試用暫時加入，可選人員
        this.app.GetData('/Users/GetUsers').subscribe(
            (s) => {
                if (s.success) {
                    this.buttondisabled = false;
                    this.UserList = [];
                    // 過濾帳戶身分。(因此畫面是使用共用帳戶，但登記人員必須是個人身分)
                    s.data.forEach(element => {
                        if (element.Permission === 20 || element.Permission === 30 || element.Permission === 40 ||
                            element.Permission === 50 || element.Permission === 60 || element.Permission === 70 ||
                            element.Permission === 80) {
                            this.UserList.push(element);
                        }
                    });
                    this.SetUserEditorOptions(this.UserList, null);
                }
            }
        );
















        this.editOnkeyPress = true;
        this.enterKeyAction = 'moveFocus';
        this.enterKeyDirection = 'row';
        this.listStatus = myservice.getWorkOrderStatus();
        this.listType = myservice.getStockType();
        this.btnDisabled = true;
        this.dataSourceDB_Process = [];
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) => SendService.sendRequest(
                this.http,
                this.Controller + '/GetWorkOrderQcLog',
                'GET', { loadOptions, remote: this.remoteOperations, detailfilter: this.detailfilter }),

        });
        this.app.GetData('/Processes/GetProcesses').subscribe(
            (s) => {
                if (s.success) {
                    if (s.success) {
                        this.ProcessBasicList = s.data;
                        this.ProcessBasicList.forEach(x => {
                            x.Name = x.Code + '_' + x.Name;
                        });
                    }
                }
            }
        );
    }
    ngOnInit() {
        this.titleService.setTitle('品質管理');
    }
    ngOnChanges() {

    }
    QuantityValueChanged(e) {
        // this.formData.PriceAll = this.formData.Price * e.value;
        // this.formData.UnitPrice = this.formData.UnitCount * e.value;
    }
    SetUserEditorOptions(List, IdVal) {
        this.selectCreateUser = {
            items: List,
            displayExpr: 'Realname',
            valueExpr: 'Id',
            value: IdVal,
            searchEnabled: true,
            disable: false
        };
    }
    async searchdata() {
        this.dataSourceDB_Process = [];
        this.buttondisabled = true;
        this.btnDisabled = true;
        this.workOrderHeadId = '';
        this.workOrderHeadDataNo = '';
        this.workOrderHeadDataName = '';
        this.workOrderHeadStatus = '';
        this.workOrderHeadCount = '';

        this.itemkey = 0;
        const SearchValue = { WorkOrderNo: this.WorkOrderNoInputVal };
        // tslint:disable-next-line: max-line-length
        const sendRequest = await SendService.sendRequest(this.http, '/WorkOrders/GetWorkOrderDetailByWorkOrderNo', 'POST', { values: SearchValue });
        if (sendRequest) {
            this.buttondisabled = false;
            this.dataSourceDB_Process = sendRequest.WorkOrderDetail;
            this.btnDisabled = false;
            this.workOrderHeadId = sendRequest.WorkOrderHead.Id;
            this.workOrderHeadDataNo = sendRequest.WorkOrderHead.DataNo;
            this.workOrderHeadDataName = sendRequest.WorkOrderHead.DataName;
            this.workOrderHeadStatus = this.listStatus.find(x => x.Id === sendRequest.WorkOrderHead.Status)?.Name ?? '';
            this.workOrderHeadCount = (sendRequest.WorkOrderHead?.ReCount ?? '0') + ' / ' + sendRequest.WorkOrderHead.Count;

            this.workOrderHeadDataType = sendRequest.WorkOrderHead.DataType;
            this.workOrderHeadDataId = sendRequest.WorkOrderHead.DataId;
        }
    }
    validate_before(): boolean {
        // 表單驗證
        if (this.myform.instance.validate().isValid === false) {
            this.showMessage('error', '請注意訂單內容必填的欄位', 3000);
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
        // tslint:disable-next-line: new-parens
        this.postval = new workOrderQcData;
        this.postval.WorkOrderHeadId = this.workOrderHeadId;
        // this.postval.WorkOrderDetailId = this.formData.ReCount;
        this.postval.ReportType = this.formData.ReportType;
        this.postval.ReCount = this.formData.ReCount;
        this.postval.CkCount = this.formData.CkCount;
        this.postval.OkCount = this.formData.OkCount;
        this.postval.NgCount = this.formData.NgCount;
        this.postval.NcCount = this.formData.NcCount;
        this.postval.DrawNo = this.formData.DrawNo;
        this.postval.CheckResult = this.formData.CheckResult;
        this.postval.CreateUser = this.formData.CreateUser;
        this.postval.Message = this.formData.Message;
        try {
            // tslint:disable-next-line: max-line-length
            const sendRequest = await SendService.sendRequest(this.http, '/WorkOrders/WorkOrderReportQC', 'POST', { values: this.postval });
            this.viewRefresh(e, sendRequest);
        } catch (error) {

        }
        this.buttondisabled = false;
    }
    viewRefresh(e, result) {
        if (result) {
            this.myform.instance.resetValues();
            this.dataGrid.instance.refresh();
            this.buttondisabled = true;
            this.workOrderHeadId = '';
            this.workOrderHeadDataNo = '';
            this.workOrderHeadDataName = '';
            this.workOrderHeadStatus = '';
            this.workOrderHeadCount = '';
            // this.CustomerVal = null;
            e.preventDefault();
            // this.childOuter.emit(true);
            this.showMessage('success', '回報完成', 3000);
        }
    }













    valueChanged(e) {
        if (this.clearCheck) {
            this.dataSourceDB_Process = [];
            this.btnDisabled = true;
            this.workOrderHeadId = '';
            this.workOrderHeadDataNo = '';
            this.workOrderHeadDataName = '';
            this.workOrderHeadStatus = '';
            this.workOrderHeadCount = '';
        } else {
            this.clearCheck = true;
        }

        this.WorkOrderNoInputVal = e.value;
    }
    onReorder(e) {
        debugger;
        const visibleRows = e.component.getVisibleRows();
        const toIndex = this.dataSourceDB_Process.indexOf(visibleRows[e.toIndex].data);
        const fromIndex = this.dataSourceDB_Process.indexOf(e.itemData);

        this.dataSourceDB_Process.splice(fromIndex, 1);
        this.dataSourceDB_Process.splice(toIndex, 0, e.itemData);
        this.dataSourceDB_Process.forEach((element, index) => {
            element.SerialNumber = index + 1;
        });
    }
    readProcess(e, data) {
        this.itemkey = 0;
        this.app.GetData('/WorkOrders/GetWorkOrderDetailByWorkOrderHeadId/' + data.data.Id).subscribe(
            (s) => {
                if (s.success) {
                    this.clearCheck = false;
                    s.data.WorkOrderDetail.forEach(element => {
                        element.Count = (element?.ReCount ?? 0) + ' / ' + element.NgCount;
                    });
                    this.dataSourceDB_Process = s.data.WorkOrderDetail;
                    this.btnDisabled = false;
                    this.workOrderHeadNo = s.data.WorkOrderHead.WorkOrderNo;
                    this.workOrderHeadId = s.data.WorkOrderHead.Id;
                    this.workOrderHeadDataNo = s.data.WorkOrderHead.DataNo;
                    this.workOrderHeadDataName = s.data.WorkOrderHead.DataName;
                    this.workOrderHeadStatus = this.listStatus.find(x => x.Id === s.data.WorkOrderHead.Status)?.Name ?? '';
                    this.workOrderHeadCount = (s.data.WorkOrderHead?.ReCount ?? '0') + ' / ' + s.data.WorkOrderHead.Count;

                    this.workOrderHeadDataType = s.data.WorkOrderHead.DataType;
                    this.workOrderHeadDataId = s.data.WorkOrderHead.DataId;
                }
            }
        );
    }
    readLog(e, data) {
        this.itemkey = data.data.Id;
        this.logpopupVisible = true;
    }
    overdata() {
        if (this.workOrderHeadDataType === 1) {
            this.modkey = 'material';
        } else if (this.workOrderHeadDataType === 2) {
            this.modkey = 'product';
        } else if (this.workOrderHeadDataType === 3) {
            this.modkey = 'wiproduct';
        }
        this.userkey = null;
        this.randomkey = new Date().getTime();
        this.itemkey = this.workOrderHeadDataId;
        this.closepopupVisible = true;
    }
    stockdata(userId) {
        if (this.workOrderHeadDataType === 1) {
            this.modkey = 'material';
        } else if (this.workOrderHeadDataType === 2) {
            this.modkey = 'product';
        } else if (this.workOrderHeadDataType === 3) {
            this.modkey = 'wiproduct';
        }
        this.userkey = userId;
        this.randomkey = new Date().getTime();
        this.itemkey = this.workOrderHeadDataId;
        this.stockpopupVisible = true;
    }
    stockpopup_result(e) {
        this.dataGrid.instance.refresh();
        this.stockpopupVisible = false;
        notify({
            message: '存檔完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
    }
    closepopup_result(e) {
        this.btnDisabled = true;
        this.dataSourceDB_Process = [];
        this.workOrderHeadNo = '';
        this.dataGrid.instance.refresh();
        this.closepopupVisible = false;
        notify({
            message: '存檔完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
    }
    showMessage(type, data, val) {
        notify({
            message: data,
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, type, val);
    }

}
