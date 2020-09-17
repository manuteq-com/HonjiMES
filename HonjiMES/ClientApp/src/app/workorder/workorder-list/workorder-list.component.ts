import { first } from 'rxjs/operators';
import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { AppComponent } from 'src/app/app.component';
import notify from 'devextreme/ui/notify';
import { DxDataGridComponent } from 'devextreme-angular';
import Swal from 'sweetalert2';
import { workOrderReportData } from 'src/app/model/viewmodels';
import { SendService } from 'src/app/shared/mylib';
import { HttpClient } from '@angular/common/http';
import { APIResponse } from 'src/app/app.module';
import { Title } from '@angular/platform-browser';

@Component({
    selector: 'app-workorder-list',
    templateUrl: './workorder-list.component.html',
    styleUrls: ['./workorder-list.component.css']
})
export class WorkorderListComponent implements OnInit {
    @ViewChild('basicTable') dataGrid: DxDataGridComponent;
    dataSourceDB: any = {};
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
                this.getWorkOrderData();
            } else if (e.key === 'Shift') {

            } else {
                this.keyup += e.key.toLocaleUpperCase();
            }
        }
    }

    constructor(private http: HttpClient, public app: AppComponent, private titleService: Title) {
        this.loadingVisible = true;
        this.creatpopupVisible = false;
        this.getWorkOrderData();
        // this.app.GetData('/Processes/GetProcessesStatus/1').subscribe(
        //     (s) => {
        //         debugger;
        //         this.dataSourceDB = s.data;
        //     }
        // );
    }
    ngOnInit() {
        // debugger;
        this.titleService.setTitle('生產看板');
    }
    getWorkOrderData() {
        this.app.GetData('/Processes/GetWorkOrderByStatus/1').subscribe(
            (s) => {
                this.dataSourceDB = s.data;
                this.loadingVisible = false;
            }
        );
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
        this.getWorkOrderData();
    }
    tdclick(e, colData) {
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
                    this.ReportHeight = 770;
                    // this.ReportByPurchaseNo(this.itemkey, this.serialkey);
                } else {
                    this.ReportHeight = 770;
                    // this.ReportByPurchaseNo(this.itemtdkey, this.serialkey);
                }
            } else if (e[colData.key].value4 === 2) { // 委外(無採購單)
                this.creatpopupVisible = true;
                // 判斷該工序目前狀態(Status)，決定顯示內容
                if (e[colData.key].value3 === 1) {
                    this.ReportHeight = 750;
                } else if (e[colData.key].value3 === 2) {
                    this.ReportHeight = 850;
                } else if (e[colData.key].value3 === 3) {
                    this.ReportHeight = 800;
                } else if (e[colData.key].value3 === 4) {
                    this.ReportHeight = 850;
                }
            } else {
                this.creatpopupVisible = true;
                // 判斷該工序目前狀態(Status)，決定顯示內容
                if (e[colData.key].value3 === 1) {
                    this.ReportHeight = 750;
                } else if (e[colData.key].value3 === 2) {
                    this.ReportHeight = 800;
                } else if (e[colData.key].value3 === 3) {
                    this.ReportHeight = 800;
                } else if (e[colData.key].value3 === 4) {
                    this.ReportHeight = 800;
                }
            }
        }
        this.getWorkOrderData();
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
        if (data === 2) {
            return 'process_start';
        } else if (data === 3) {
            return 'process_end';
        } else if (data === 4) {
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
                    return {purchaseId: response.data, purchaseNo};
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
                    this.getWorkOrderData();
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
    creatdata() {
        // this.creatpopupVisible = true;
        // this.checkVisible = true;
        // this.itemtrkey = null;
        // this.mod = 'new';
    }
    creatpopup_result(e) {
        this.creatpopupVisible = false;
        this.editpopupVisible = false;
        this.checkVisible = false;
        this.loadingVisible = true;
        this.getWorkOrderData();
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
