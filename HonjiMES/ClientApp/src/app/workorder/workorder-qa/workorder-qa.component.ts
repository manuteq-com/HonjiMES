import { Component, OnInit, OnChanges, ViewChild, Input, HostListener } from '@angular/core';
import { APIResponse } from 'src/app/app.module';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { createStore } from 'devextreme-aspnet-data-nojquery';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from 'src/app/shared/mylib';
import DataSource from 'devextreme/data/data_source';
import notify from 'devextreme/ui/notify';
import { DxDataGridComponent } from 'devextreme-angular';
import { AppComponent } from 'src/app/app.component';
import { mBillOfMaterial, workOrderReportData } from 'src/app/model/viewmodels';
import { Myservice } from 'src/app/service/myservice';
import Swal from 'sweetalert2';
import { Title } from '@angular/platform-browser';

@Component({
    selector: 'app-workorder-qa',
    templateUrl: './workorder-qa.component.html',
    styleUrls: ['./workorder-qa.component.css']
})
export class WorkorderQaComponent implements OnInit, OnChanges {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild('dataGrid2') dataGrid2: DxDataGridComponent;
    dataSourceDB: any;
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

    @HostListener('window:keyup', ['$event']) keyUp(e: KeyboardEvent) {
        if (!this.stockpopupVisible && !this.closepopupVisible && !this.logpopupVisible) {
            this.stockpopupVisible = false;
            this.closepopupVisible = false;
            if (e.key === 'Enter') {
                const key = this.keyup;
                const promise = new Promise((resolve, reject) => {
                    this.app.GetData('/WorkOrders/GetWorkOrderHeadByWorkOrderNo?DataNo=' + key).toPromise().then((res: APIResponse) => {
                        if (res.success) {
                            this.btnDisabled = true;
                            this.dataSourceDB_Process = [];
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
            } else if (e.key === 'Shift') {

            } else {
                this.keyup += e.key.toLocaleUpperCase();
            }
        }
    }

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent, private titleService: Title) {
        this.onReorder = this.onReorder.bind(this);
        this.editOnkeyPress = true;
        this.enterKeyAction = 'moveFocus';
        this.enterKeyDirection = 'row';
        this.listStatus = myservice.getWorkOrderStatus();
        this.btnDisabled = true;
        this.dataSourceDB_Process = [];
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) => SendService.sendRequest(
                this.http,
                this.Controller + '/GetWorkOrderHeadsRun',
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
        this.titleService.setTitle('工單入庫確認');
    }
    ngOnChanges() {
    }
    valueChanged(e) {
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
    async searchdata() {
        this.itemkey = 0;
        const SearchValue = { WorkOrderNo: this.WorkOrderNoInputVal };
        // tslint:disable-next-line: max-line-length
        const sendRequest = await SendService.sendRequest(this.http, '/WorkOrders/GetWorkOrderDetailByWorkOrderNo', 'POST', { values: SearchValue });
        if (sendRequest) {
            this.dataSourceDB_Process = sendRequest.WorkOrderDetail;
            this.btnDisabled = false;
            this.workOrderHeadId = sendRequest.WorkOrderHead.Id;
            this.workOrderHeadDataNo = sendRequest.WorkOrderHead.DataNo;
            this.workOrderHeadDataName = sendRequest.WorkOrderHead.DataName;
            this.workOrderHeadStatus = this.listStatus.find(x => x.Id === sendRequest.WorkOrderHead.Status)?.Name ?? '';
            this.workOrderHeadCount = (sendRequest.WorkOrderHead?.ReCount ?? '0') + ' / ' + sendRequest.WorkOrderHead.Count;
        }
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
        //// 以下舊方法
        // Swal.fire({
        //     showCloseButton: true,
        //     allowEnterKey: false,
        //     allowOutsideClick: false,
        //     title: '確認結案?',
        //     html: '狀態將無法復原!',
        //     icon: 'question',
        //     showCancelButton: true,
        //     confirmButtonColor: '#296293',
        //     cancelButtonColor: '#CE312C',
        //     confirmButtonText: '確認',
        //     cancelButtonText: '取消'
        // }).then(async (result) => {
        //     if (result.value) {
        //         // tslint:disable-next-line: max-line-length
        //         const sendRequest = await SendService.sendRequest(this.http, '/WorkOrders/CloseWorkOrder', 'PUT', { key: this.workOrderHeadId, values: '' });
        //         // let data = this.client.POST( this.url + '/OrderHeads/PostOrderMaster_Detail').toPromise();
        //         if (sendRequest) {
        //             this.dataGrid.instance.refresh();
        //             notify({
        //                 message: '更新成功',
        //                 position: {
        //                     my: 'center top',
        //                     at: 'center top'
        //                 }
        //             }, 'success', 2000);
        //         }
        //     }
        // });
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
    stockdataOld() {
        Swal.fire({
            title: '請確認入庫數量!',
            input: 'number',
            inputAttributes: {
                autocapitalize: 'off',
                min: '0',
                step: '1'
            },
            showCancelButton: true,
            confirmButtonColor: '#296293',
            cancelButtonColor: '#CE312C',
            confirmButtonText: '確認',
            cancelButtonText: '取消',
            showLoaderOnConfirm: true,
            // preConfirm: (login) => {
            //     return fetch(`//api.github.com/users/${login}`)
            //     .then(response => {
            //         if (!response.ok) {
            //             throw new Error(response.statusText)
            //         }
            //         return response.json()
            //     })
            //     .catch(error => {
            //         Swal.showValidationMessage(
            //             `Request failed: ${error}`
            //         )
            //     })
            // },
            // allowOutsideClick: () => !Swal.isLoading()
            inputValidator: (value) => {
                if (!Number.isInteger(Number(value))) {
                    return '數量需為整數!';
                } else if (Number(value) === 0) {
                    return '數量不能為0!';
                }
            }
        }).then(async (result) => {
            if (result.value) {
                const Data = new workOrderReportData();
                Data.ReCount = result.value;
                // tslint:disable-next-line: max-line-length
                const sendRequest = await SendService.sendRequest(this.http, '/WorkOrders/StockWorkOrder', 'PUT', { key: this.workOrderHeadId, values: Data });
                // let data = this.client.POST( this.url + '/OrderHeads/PostOrderMaster_Detail').toPromise();
                if (sendRequest) {
                    this.dataGrid.instance.refresh();
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
