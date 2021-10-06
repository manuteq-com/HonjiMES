import { first } from 'rxjs/operators';
import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { AppComponent } from 'src/app/app.component';
import notify from 'devextreme/ui/notify';
import { DxDataGridComponent, DxFormComponent } from 'devextreme-angular';
import Swal from 'sweetalert2';
import { HubMessage, workOrderReportData } from 'src/app/model/viewmodels';
import { SendService } from 'src/app/shared/mylib';
import { HttpClient } from '@angular/common/http';
import { APIResponse } from 'src/app/app.module';
import { Title } from '@angular/platform-browser';
import { SignalRService } from 'src/app/service/signal-r.service';
import { HubConnectionBuilder } from '@aspnet/signalr';
@Component({
    selector: 'app-workorder-list',
    templateUrl: './workorder-list.component.html',
    styleUrls: ['./workorder-list.component.css']
})
export class WorkorderListComponent implements OnInit {
    @ViewChild('basicTable') dataGrid: DxDataGridComponent;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    dataSourceDB: any = {};
    dataSourceDBDisplay: any = {};
    creatpopupVisible: any;
    itemtrkey: number;
    itemtdkey: any;
    serialkey: number;
    mod: string;
    loadingVisible = false;
    ReportHeight: any;
    keyup = '';
    editpopupVisible: boolean;
    checkVisible: boolean;
    randomkey: number;
    iteminfokey: any;
    infopopupVisible: boolean;
    editorOptions: any;
    formData: any;
    detailfilter: any={};
    @HostListener('window:keyup', ['$event']) keyUp(e: KeyboardEvent) {
        if (!this.creatpopupVisible && !this.editpopupVisible) {
            if (e.key === 'Enter') {
                const key = this.keyup;
                const selectdata = this.dataSourceDB.ProcessesDataList.find((value) => key.endsWith(value.WorkOrderNo));
                if (selectdata) {
                    this.itemtrkey = selectdata;
                    this.mod = 'report';
                    this.editpopupVisible = true;
                    this.ReportHeight = 710;
                } else {
                    const promise2 = new Promise((resolve2, reject2) => {
                        this.app.GetData('/Users/GetUserByUserNo?DataNo=' + key).toPromise().then((res2: APIResponse) => {
                            if (res2.success) {
                                if (!this.creatpopupVisible && !this.editpopupVisible) {
                                    if (res2.data.Permission === 80 || res2.data.Permission === 20) {
                                        this.showMessage('warning', '請先掃描工單碼!', 3000);
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
                }
                this.keyup = '';
                this.getWorkOrderData(this.detailfilter);
            } else if (e.key === 'Shift' || e.key === 'CapsLock') {

            } else {
                this.keyup += e.key.toLocaleUpperCase();
            }
        }
    }

    constructor(private http: HttpClient, public app: AppComponent, private titleService: Title, private SignalRService: SignalRService) {
        this.loadingVisible = true;
        this.creatpopupVisible = false;
        this.onValueChanged = this.onValueChanged.bind(this);
        this.editorOptions = { onValueChanged: this.onValueChanged };



        // this.app.GetData('/Processes/GetProcessesStatus/1').subscribe(
        //     (s) => {
        //         debugger;
        //         this.dataSourceDB = s.data;
        //     }
        // );

    }
    ngOnInit() {
        this.getWorkOrderData(this.detailfilter);
        this.titleService.setTitle('生產看板');
    }
    getWorkOrderData(qfilter) {
        this.app.GetData('/Processes/GetWorkOrderByMode/1').subscribe(
            (s) => {
                this.dataSourceDB = JSON.parse(JSON.stringify(s.data));
                this.dataSourceDBDisplay = s.data;
                // console.log("this.dataSourceDBDisplay",this.dataSourceDBDisplay);
                // var rawdata = this.dataSourceDB.ProcessesDataList;
                // console.log('this.dataSourceDB',this.dataSourceDB);
                // if(Object.keys(qfilter).length>0){
                //         Object.keys(qfilter).forEach(function(v,k){
                //             rawdata = rawdata.filter(function(x){
                //                 return x[v].toLowerCase().includes(qfilter[v].toLowerCase())
                //             })
                //         })
                //         this.dataSourceDB.ProcessesDataList=rawdata;
                // }
                this.loadingVisible = false;
            }
        );
    }

    filterdataSourceDB(qfilter) {
        let rawdata = this.dataSourceDB.ProcessesDataList;
        if (Object.keys(qfilter).length > 0) {
            Object.keys(qfilter).forEach(function (v, k) {
                rawdata = rawdata.filter(function (x) {
                    return x[v].toLowerCase().includes(qfilter[v].toLowerCase())
                })
            })
            this.dataSourceDBDisplay.ProcessesDataList = rawdata;
        }
    }

    onValueChanged(q) {
        this.detailfilter = this.myform.instance.option('formData');
        this.filterdataSourceDB(this.detailfilter);
    }
    trclick(e) {
        if (!this.checkVisible) {
            this.itemtrkey = e;
            this.serialkey = 1;
            this.mod = 'report';
            this.editpopupVisible = true;
            this.ReportHeight = 710;
        } else {
            this.checkVisible = false;
        }
        this.getWorkOrderData(this.detailfilter);
    }
    tdclick(e, colData) {
        // debugger;
        this.checkVisible = true;
        if (e[colData.key] != null) {
            this.itemtdkey = e.Key;
            this.serialkey = Number(colData.key.substring(4)) + 1;
            this.mod = 'report';
            this.randomkey = new Date().getTime();
            // 判斷是否為委外(Type)
            if (e[colData.key].value4 === 1) { // 委外(含採購單)
                this.creatpopupVisible = true;
                // 判斷該工序目前狀態(Status)
                if (e[colData.key].value3 === 3 || e[colData.key].value3 === 4) {
                    this.ReportHeight = 800;
                    // this.ReportByPurchaseNo(this.itemkey, this.serialkey);
                } else {
                    this.ReportHeight = 800;
                    // this.ReportByPurchaseNo(this.itemtdkey, this.serialkey);
                }
            } else if (e[colData.key].value4 === 2) { // 委外(無採購單)
                this.creatpopupVisible = true;
                // 判斷該工序目前狀態(Status)，決定顯示內容
                if (e[colData.key].value3 === 1) {
                    this.ReportHeight = 770;
                } else if (e[colData.key].value3 === 2) {
                    this.ReportHeight = 870;
                } else if (e[colData.key].value3 === 3) {
                    this.ReportHeight = 820;
                } else if (e[colData.key].value3 === 4) {
                    this.ReportHeight = 870;
                }
            } else {
                this.creatpopupVisible = true;
                if (e[colData.key].value5 === 20) { // 判斷製程種類 (10)NC加工(20)QC檢驗
                    if (e[colData.key].value3 === 1) {
                        this.ReportHeight = 700;
                    } else if (e[colData.key].value3 === 2) {
                        this.ReportHeight = 870;
                    } else if (e[colData.key].value3 === 3) {
                        this.ReportHeight = 700;
                    }
                } else if (e[colData.key].value5 === null || e[colData.key].value5 === 10) {
                    // 判斷該工序目前狀態(Status)，決定顯示內容
                    if (e[colData.key].value3 === 1) {
                        this.ReportHeight = 750;
                    } else if (e[colData.key].value3 === 2) {
                        this.ReportHeight = 820;
                    } else if (e[colData.key].value3 === 3) {
                        this.ReportHeight = 820;
                    } else if (e[colData.key].value3 === 4) {
                        this.ReportHeight = 820;
                    } else if (e[colData.key].value3 === 6) {
                        this.ReportHeight = 820;
                    } else if (e[colData.key].value3 === 7) {
                        this.ReportHeight = 770;
                    }
                }
            }
        }
        this.getWorkOrderData(this.detailfilter);
    }
    getBlueClass(data) {
        // if (data.Status === 1) {
        //     return 'process_started';
        // } else if (data.Status === 3) {
        //     return 'process_ended';
        // } else {
        return '';
        // }
    }
    getBlue2Class(data) {
        if (data === 2) { // 開工
            return 'process_start';
        } else if (data === 3) { // 完工
            return 'process_end';
        } else if (data === 7) { // 工序暫停
            return 'process_stop';
        } else if (data === 6) { // 超時完工
            return 'process_alarm';
        }
    }
    ReportByPurchaseNo(workOrderHeadId, serial) {
        Swal.fire({
            title: '請輸入採購單號!',
            input: 'text',
            inputAttributes: {
                autocapitalize: 'off'
            },
            showCancelButton: true,
            confirmButtonColor: '#296293',
            cancelButtonColor: '#CE312C',
            confirmButtonText: '確認',
            cancelButtonText: '取消',
            showLoaderOnConfirm: true,
            preConfirm: (purchaseNo) => {
                return this.app.GetData('/PurchaseHeads/GetPurchasesByPurchaseNo?DataNo=' + purchaseNo).toPromise()
                    .then(response => {
                        if (!response.success) {
                            // throw new Error(response.message);
                            Swal.showValidationMessage(response.message);
                        }
                        return { purchaseId: response.data, purchaseNo };
                    })
                    .catch(error => {
                        Swal.showValidationMessage(
                            `Request failed: ${error}`
                        );
                    });
            },
            allowOutsideClick: () => !Swal.isLoading()
        }).then(async (result) => {
            if (result.value) {
                const Data = new workOrderReportData();
                Data.WorkOrderID = workOrderHeadId;
                Data.WorkOrderSerial = serial;
                Data.PurchaseId = result.value.purchaseId;
                Data.PurchaseNo = result.value.purchaseNo;
                // tslint:disable-next-line: max-line-length
                const sendRequest = await SendService.sendRequest(this.http, '/WorkOrders/ReportWorkOrderByPurchase', 'PUT', { key: workOrderHeadId, values: Data });
                // let data = this.client.POST( this.url + '/OrderHeads/PostOrderMaster_Detail').toPromise();
                if (sendRequest) {
                    this.getWorkOrderData(this.detailfilter);
                    notify({
                        message: '更新成功',
                        position: {
                            my: 'center top',
                            at: 'center top'
                        }
                    }, 'success', 2000);
                }
            }
        });
    }
    infodata() {
        this.iteminfokey = null;
        this.infopopupVisible = true;
        this.randomkey = new Date().getTime();
        this.getWorkOrderData(this.detailfilter);
    }
    creatdata() {
        // this.creatpopupVisible = true;
        // this.checkVisible = true;
        // this.itemtrkey = null;
        // this.mod = 'new';
    }
    creatpopup_result(e) {
        this.app.startBillboardReload();
        this.creatpopupVisible = false;
        this.editpopupVisible = false;
        this.checkVisible = false;
        this.loadingVisible = true;
        this.getWorkOrderData(this.detailfilter);
        this.showMessage('success', '更新完成', 3000);

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
