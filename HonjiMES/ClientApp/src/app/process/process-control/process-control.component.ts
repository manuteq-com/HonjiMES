import { Component, OnInit, ViewChild } from '@angular/core';
import { AppComponent } from 'src/app/app.component';
import notify from 'devextreme/ui/notify';
import { DxDataGridComponent, DxFileUploaderComponent } from 'devextreme-angular';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from 'src/app/shared/mylib';
import { HttpClient } from '@angular/common/http';
import { Myservice } from 'src/app/service/myservice';
import Swal from 'sweetalert2';
import { Title } from '@angular/platform-browser';
import { AuthService } from 'src/app/service/auth.service';
import { APIResponse } from 'src/app/app.module';
import { CreatprocessControlComponent } from '../creatprocess-control/creatprocess-control.component';


@Component({
    selector: 'app-process-control',
    templateUrl: './process-control.component.html',
    styleUrls: ['./process-control.component.css']
})
export class ProcessControlComponent implements OnInit {
    @ViewChild('basicTable') dataGrid: DxDataGridComponent;
    @ViewChild('dataGrid1') dataGrid1: DxDataGridComponent;
    @ViewChild(DxFileUploaderComponent) uploader: DxFileUploaderComponent;
    @ViewChild(CreatprocessControlComponent) popUp: CreatprocessControlComponent;
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
    lograndomkey: number;
    showTitleValue: string;
    workOrderHeadDataNo: any;
    uploadUrl: string;
    uploadHeaders: { Authorization: string; routerLink: string; apitype: string; };
    islg = true;
    loadingVisible = false;
    selectedRowKeys: any[];
    popupClose: boolean;
    constructor(public http: HttpClient, myservice: Myservice, public app: AppComponent, private titleService: Title) {
        const authenticationService = new AuthService(http);
        const currentUser = authenticationService.currentUserValue;
        this.uploadHeaders = {
            Authorization: 'Bearer ' + currentUser.Token,
            routerLink: location.pathname,
            apitype: 'POST'
        };
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
        this.showTitleValue = '報工紀錄';
        this.uploadUrl = location.origin + '/api/WorkOrders/PostWorkOrdeByExcel';
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
        this.selectedRowKeys =[];
        this.titleService.setTitle('工單管理');
        this.runVisible = false;
        this.editVisible = false;
    }
    onReorder(e) {
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
    readProcess(e, dataId) {
        // console.log("dataId", dataId);
        if (!this.creatpopupVisible) {
            this.itemkey = 0;
            this.workOrderHeadId = dataId;
            this.app.GetData('/WorkOrders/GetWorkOrderDetailByWorkOrderHeadId/' + dataId).subscribe(
                (s) => {
                    if (s.success) {
                        if (this.app.checkUpdateRoles()) {
                            this.editVisible = true;
                        }
                        if (s.data.WorkOrderHead.Status === 0 || s.data.WorkOrderHead.Status === 4) {
                            if (this.app.checkUpdateRoles()) {
                                this.runVisible = true;
                            }
                        } else {
                            this.runVisible = false;
                        }

                        // 是否結案
                        if (s.data.WorkOrderHead.Status === 5) { // 已結案
                            this.btnDisabled = true;
                        } else {
                            this.btnDisabled = false;
                        }

                        s.data.WorkOrderDetailData.forEach(element => {
                            element.Count = (element?.ReCount ?? 0) + ' / ' + element.NgCount;
                        });
                        this.dataSourceDB_Process = s.data.WorkOrderDetailData;
                        this.workOrderHeadNo = s.data.WorkOrderHead.WorkOrderNo;
                        // this.workOrderHeadId = s.data.WorkOrderHead.Id;
                        this.workOrderHeadDataNo = s.data.WorkOrderHead.DataNo;
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
    onRowClick(e) {
        // console.log("onRowClick", e);
        if (!this.creatpopupVisible) {
            this.itemkey = 0;
            this.workOrderHeadId = e.data.Id;
            this.app.GetData('/WorkOrders/GetWorkOrderDetailByWorkOrderHeadId/' + e.data.Id).subscribe(
                (s) => {
                    if (s.success) {
                        if (this.app.checkUpdateRoles()) {
                            this.editVisible = true;
                        }
                        if (s.data.WorkOrderHead.Status === 0 || s.data.WorkOrderHead.Status === 4) {
                            if (this.app.checkUpdateRoles()) {
                                this.runVisible = true;
                            }
                        } else {
                            this.runVisible = false;
                        }

                        // 是否結案
                        if (s.data.WorkOrderHead.Status === 5) { // 已結案
                            this.btnDisabled = true;
                        } else {
                            this.btnDisabled = false;
                        }

                        s.data.WorkOrderDetailData.forEach(element => {
                            element.Count = (element?.ReCount ?? 0) + ' / ' + element.NgCount;
                        });
                        this.dataSourceDB_Process = s.data.WorkOrderDetailData;
                        this.workOrderHeadNo = s.data.WorkOrderHead.WorkOrderNo;
                        // this.workOrderHeadId = s.data.WorkOrderHead.Id;
                        this.workOrderHeadDataNo = s.data.WorkOrderHead.DataNo;
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
        this.showTitleValue = '報工紀錄　[ ' + this.workOrderHeadDataNo + ' ]';
        this.lograndomkey = new Date().getTime();
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
    popup_hidden(e) {
        this.dataGrid1.instance.refresh();
        if (this.workOrderHeadId !== undefined) {
            this.readProcess(null, this.workOrderHeadId);
        }
        this.popupClose = !this.popupClose;
        this.popUp.dataGrid2.instance.cancelEditData();
        this.popUp.myform.instance.resetValues();
    }
    creatpopup_result(e) {
        debugger;
        this.itemkey = null;
        this.dataGrid1.instance.refresh();
        if (this.workOrderHeadId !== undefined) {
            this.readProcess(null, this.workOrderHeadId);
            // this.app.GetData('/WorkOrders/GetWorkOrderDetailByWorkOrderHeadId/' + this.workOrderHeadId).subscribe(
            //     (s) => {
            //         if (s.success) {
            //             this.dataSourceDB_Process = s.data.WorkOrderDetail;
            //             this.btnDisabled = false;
            //             this.workOrderHeadNo = s.data.WorkOrderHead.WorkOrderNo;
            //         }
            //     }
            // );
        }
        this.creatpopupVisible = false;
    }
    closepopup_result(e) {
        debugger;
        this.btnDisabled = true;
        // this.dataSourceDB_Process = [];
        // this.workOrderHeadNo = '';
        this.dataGrid1.instance.refresh();
        this.closepopupVisible = false;
        // notify({
        //     message: '存檔完成',
        //     position: {
        //         my: 'center top',
        //         at: 'center top'
        //     }
        // }, 'success', 3000);
    }
    handleCancel() {
        this.viewpopupVisible = false;
    }
    downloadWorkOrder(e) {
        // debugger;
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
            this.app.downloadfile('/Report/GetWorkOrderPDF/' + this.workOrderHeadId);
            // this.Url = '/Api/Report/GetWorkOrderPDF/' + this.workOrderHeadId;
            // window.open(this.Url, '_blank');
        }
    }
    deleteWorkOrder(e) {
        //console.log("deleteWorkOrder",e);
        //console.log("this.workOrderHeadId",this.workOrderHeadId);
        if (this.workOrderHeadId) {
            Swal.fire({
                showCloseButton: true,
                allowEnterKey: false,
                allowOutsideClick: false,
                title: '確認刪除?',
                html: '刪除後將無法復原!',
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#296293',
                cancelButtonColor: '#CE312C',
                confirmButtonText: '確認',
                cancelButtonText: '取消'
            }).then(async (result) => {
                if (result.value) {
                    // tslint:disable-next-line: max-line-length
                    const sendRequest = await SendService.sendRequest(this.http, '/Processes/DeleteWorkOrderList/' + this.workOrderHeadId, 'DELETE');
                    this.viewRefresh(e, sendRequest);

                }
            });
        }

    }
    viewRefresh(e, result) {
        if (result) {
            this.editVisible = false;
            this.runVisible = false;
            this.btnDisabled = true;
            this.dataSourceDB_Process = [];
            this.dataGrid1.instance.refresh();
            //this.dataGrid1.instance.clearSelection();
            this.workOrderHeadNo = "";
            this.workOrderHeadDataNo = "";
            this.workOrderHeadDataType = "";
            this.workOrderHeadDataId = "";
            this.selectedRowKeys =[];
            //this.dataGrid2.instance.refresh();
            //this.dataSourceDB = [];
            e.preventDefault();
            notify({
                message: '更新完成',
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'success', 3000);
        }
    }
    onProgress(e) {
        this.loadingVisible = true;
    }
    onUploaded(e) {
        const response = JSON.parse(e.request.response) as APIResponse;
        this.loadingVisible = false;
        if (response.success) {
            this.dataGrid1.instance.refresh();
        } else {

        }
    }
}
