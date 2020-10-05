import { Component, OnInit, ViewChild } from '@angular/core';
import { AppComponent } from 'src/app/app.component';
import notify from 'devextreme/ui/notify';
import { DxDataGridComponent } from 'devextreme-angular';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from 'src/app/shared/mylib';
import { HttpClient } from '@angular/common/http';
import { Myservice } from 'src/app/service/myservice';
import Swal from 'sweetalert2';
import { Title } from '@angular/platform-browser';

@Component({
    selector: 'app-process-control',
    templateUrl: './process-control.component.html',
    styleUrls: ['./process-control.component.css']
})
export class ProcessControlComponent implements OnInit {
    @ViewChild('basicTable') dataGrid: DxDataGridComponent;
    @ViewChild('dataGrid1') dataGrid1: DxDataGridComponent;
    Controller = '/WorkOrders';
    dataSourceDB: any;
    dataSourceDB_Process: any[];
    detailfilter = [];
    editOnkeyPress: boolean;
    enterKeyAction: string;
    enterKeyDirection: string;
    creatpopupVisible: any;
    qrcodepopupVisible: any;
    itemkey: number;
    randomkey: number;
    mod: string;
    viewpopupVisible: boolean;
    remoteOperations = true;
    ProcessBasicList: any;
    listStatus: any;
    listWorkOrderTypes: any;
    btnDisabled: boolean;
    workOrderHeadId: any;
    workOrderHeadNo: any;
    logpopupVisible: boolean;
    postval: any;
    Url = '';
    runVisible: boolean;
    editVisible: boolean;
    closepopupVisible: boolean;
    workOrderHeadDataType: any;
    workOrderHeadDataId: any;
    modkey: string;
    userkey: any;

    constructor(public http: HttpClient, myservice: Myservice, public app: AppComponent, private titleService: Title) {
        this.listStatus = myservice.getWorkOrderStatus();
        this.listWorkOrderTypes = myservice.getWorkOrderTypes();
        this.editOnkeyPress = true;
        this.enterKeyAction = 'moveFocus';
        this.enterKeyDirection = 'row';
        this.creatpopupVisible = false;
        this.qrcodepopupVisible = false;
        this.dataSourceDB_Process = [];
        this.btnDisabled = true;
        this.postval = {};

        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) => SendService.sendRequest(
                this.http,
                this.Controller + '/GetWorkOrderHeads',
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
        this.titleService.setTitle('工單管理');
        this.runVisible = false;
        this.editVisible = false;
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
    async runProcess() {
        Swal.fire({
            showCloseButton: true,
            allowEnterKey: false,
            allowOutsideClick: false,
            title: '確認派工?',
            html: '派工後將無法復原!',
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#296293',
            cancelButtonColor: '#CE312C',
            confirmButtonText: '確認',
            cancelButtonText: '取消'
        }).then(async (result) => {
            if (result.value) {
                this.postval = {
                    WorkOrderHead: {
                        Id: this.workOrderHeadId,
                    }
                };
                try {
                    // tslint:disable-next-line: max-line-length
                    const sendRequest = await SendService.sendRequest(this.http, '/WorkOrders/toWorkOrder', 'POST', { values: this.postval });
                    if (sendRequest) {
                        this.runVisible = false;
                        this.dataGrid1.instance.refresh();
                        notify({
                            message: '更新完成',
                            position: {
                                my: 'center top',
                                at: 'center top'
                            }
                        }, 'success', 3000);
                    }
                } catch (error) {

                }
            }
        });
    }
    editProcess() {
        this.creatpopupVisible = true;
        this.itemkey = this.workOrderHeadId;
        this.mod = 'edit';
        this.randomkey = new Date().getTime();
    }
    readProcess(e, data) {
        if (!this.creatpopupVisible) {
            this.itemkey = 0;
            this.workOrderHeadId = data.data.Id;
            this.app.GetData('/WorkOrders/GetWorkOrderDetailByWorkOrderHeadId/' + data.data.Id).subscribe(
                (s) => {
                    if (s.success) {
                        this.editVisible = true;
                        if (s.data.WorkOrderHead.Status === 0 || s.data.WorkOrderHead.Status === 4) {
                            this.runVisible = true;
                        } else {
                            this.runVisible = false;
                        }

                        // 是否結案
                        if (s.data.WorkOrderHead.Status === 5) { // 已結案
                            this.btnDisabled = true;
                        } else {
                            this.btnDisabled = false;
                        }

                        s.data.WorkOrderDetail.forEach(element => {
                            element.Count = (element?.ReCount ?? 0) + ' / ' + element.NgCount;
                        });
                        this.dataSourceDB_Process = s.data.WorkOrderDetail;
                        this.workOrderHeadNo = s.data.WorkOrderHead.WorkOrderNo;
                        // this.workOrderHeadId = s.data.WorkOrderHead.Id;
                        // this.workOrderHeadDataNo = s.data.WorkOrderHead.DataNo;
                        // this.workOrderHeadDataName = s.data.WorkOrderHead.DataName;
                        // this.workOrderHeadStatus = this.listStatus.find(x => x.Id === s.data.WorkOrderHead.Status)?.Name ?? '';
                        // this.workOrderHeadCount = (s.data.WorkOrderHead?.ReCount ?? '0') + ' / ' + s.data.WorkOrderHead.Count;

                        this.workOrderHeadDataType = s.data.WorkOrderHead.DataType;
                        this.workOrderHeadDataId = s.data.WorkOrderHead.DataId;
                    }
                }
            );
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
    }
    readLog(e, data) {
        this.itemkey = data.data.Id;
        this.logpopupVisible = true;
    }
    creatdata() {
        this.creatpopupVisible = true;
        this.itemkey = null;
        this.mod = 'new';
        this.randomkey = new Date().getTime();
    }
    viewdata() {
        this.viewpopupVisible = true;
        this.randomkey = new Date().getTime();
        // this.mod = 'view';
    }
    qrcodedata() {
        this.qrcodepopupVisible = true;
        this.randomkey = new Date().getTime();
    }
    creatpopup_result(e) {
        this.creatpopupVisible = false;
        this.itemkey = null;
        this.dataGrid1.instance.refresh();
        if (this.workOrderHeadId !== undefined) {
            this.app.GetData('/WorkOrders/GetWorkOrderDetailByWorkOrderHeadId/' + this.workOrderHeadId).subscribe(
                (s) => {
                    if (s.success) {
                        this.dataSourceDB_Process = s.data.WorkOrderDetail;
                        this.btnDisabled = false;
                        this.workOrderHeadNo = s.data.WorkOrderHead.WorkOrderNo;
                    }
                }
            );
        }
        notify({
            message: '更新完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
    }
    closepopup_result(e) {
        this.btnDisabled = true;
        // this.dataSourceDB_Process = [];
        // this.workOrderHeadNo = '';
        this.dataGrid1.instance.refresh();
        this.closepopupVisible = false;
        notify({
            message: '存檔完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
    }
    handleCancel() {
        this.viewpopupVisible = false;
    }
    downloadWorkOrder(e) {
        debugger;
        if (this.workOrderHeadId === undefined) {
            Swal.fire({
                allowEnterKey: false,
                allowOutsideClick: false,
                title: '沒有選擇任何工單項目',
                html: '請點選需產生報表之工單項目',
                icon: 'warning',
                timer: 5000
            });
            return false;
        } else {
            this.Url = '/Api/Report/GetWorkOrderPDF/' + this.workOrderHeadId;
            window.open(this.Url, '_blank');
        }
    }
}
