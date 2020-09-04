import { Component, OnInit, ViewChild } from '@angular/core';
import { AppComponent } from 'src/app/app.component';
import notify from 'devextreme/ui/notify';
import { DxDataGridComponent } from 'devextreme-angular';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from 'src/app/shared/mylib';
import { HttpClient } from '@angular/common/http';
import { Myservice } from 'src/app/service/myservice';
import Swal from 'sweetalert2';

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

    constructor(public http: HttpClient, myservice: Myservice, public app: AppComponent) {
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
    async runProcess(e, data) {
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
                        Id: data.key,
                    }
                };
                try {
                    // tslint:disable-next-line: max-line-length
                    const sendRequest = await SendService.sendRequest(this.http, '/WorkOrders/toWorkOrder', 'POST', { values: this.postval });
                    if (sendRequest) {
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
    editProcess(e, data) {
        this.creatpopupVisible = true;
        this.itemkey = data.key;
        this.mod = 'edit';
        this.randomkey = new Date().getTime();
    }
    readProcess(e) {
        if (!this.creatpopupVisible) {
            this.itemkey = 0;
            this.workOrderHeadId = e.key;
            this.app.GetData('/WorkOrders/GetWorkOrderDetailByWorkOrderHeadId/' + e.key).subscribe(
                (s) => {
                    if (s.success) {
                        this.dataSourceDB_Process = s.data.WorkOrderDetail;
                        this.btnDisabled = false;
                        this.workOrderHeadNo = s.data.WorkOrderHead.WorkOrderNo;
                        // this.workOrderHeadId = s.data.WorkOrderHead.Id;
                        // this.workOrderHeadDataNo = s.data.WorkOrderHead.DataNo;
                        // this.workOrderHeadDataName = s.data.WorkOrderHead.DataName;
                        // this.workOrderHeadStatus = this.listStatus.find(x => x.Id === s.data.WorkOrderHead.Status)?.Name ?? '';
                        // this.workOrderHeadCount = (s.data.WorkOrderHead?.ReCount ?? '0') + ' / ' + s.data.WorkOrderHead.Count;
                    }
                }
            );
        }
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
    handleCancel() {
        this.viewpopupVisible = false;
    }
}
