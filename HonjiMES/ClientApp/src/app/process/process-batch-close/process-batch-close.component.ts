import { HttpClient } from '@angular/common/http';
import { Component, OnInit, ViewChild } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { AppComponent } from 'src/app/app.component';
import { DxDataGridComponent, DxFileUploaderComponent, DxFormComponent } from 'devextreme-angular';
import { AuthService } from 'src/app/service/auth.service';
import { Myservice } from 'src/app/service/myservice';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from 'src/app/shared/mylib';
import notify from 'devextreme/ui/notify';
import { InventoryChange, workOrderReportData } from 'src/app/model/viewmodels';

@Component({
    selector: 'app-process-batch-close',
    templateUrl: './process-batch-close.component.html',
    styleUrls: ['./process-batch-close.component.css']
})
export class ProcessBatchCloseComponent implements OnInit {
    @ViewChild('dataGrid1') dataGrid1: DxDataGridComponent;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    listStatus: any;
    selectedRowKeys: any[];
    uploadUrl: string;
    dataSourceDB: any;
    Controller = '/WorkOrders';
    detailfilter = [];
    remoteOperations = true;
    closepopupVisible: boolean;
    selectedData: any[] & Promise<any> & JQueryPromise<any>;
    selectedKeys: any[] & Promise<any> & JQueryPromise<any>;
    UserList: any[];
    width: any;
    UserEditorOptions: { items: any; displayExpr: string; valueExpr: string; value: any; searchEnabled: boolean; disabled: boolean; };

    constructor(public http: HttpClient, myservice: Myservice, public app: AppComponent, private titleService: Title) {
        const authenticationService = new AuthService(http);
        const currentUser = authenticationService.currentUserValue;

        this.listStatus = myservice.getWorkOrderStatus();
        this.uploadUrl = location.origin + '/api/WorkOrders/PostWorkOrdeByExcel';
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) => SendService.sendRequest(
                this.http,
                this.Controller + '/GetWorkOrderHeads',
                'GET', { loadOptions, remote: this.remoteOperations, detailfilter: this.detailfilter }),

        });
        this.app.GetData('/Users/GetUsers').subscribe(
            (s) => {
                if (s.success) {
                    this.UserList = [];
                    // 此寫法無法過濾帳戶身分。(因此畫面是使用共用帳戶，但登記人員必須是個人身分)
                    s.data.forEach(element => {
                        this.UserList.push(element);
                        console.log("auser", this.UserList);
                    });
                    this.SetUserEditorOptions(this.UserList, undefined);
                }
            }
        );
    }

    ngOnInit() {
        this.selectedRowKeys = [];
        this.titleService.setTitle('工單管理');

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

    toSave() {
        this.closepopupVisible = true;
        this.selectedData = this.dataGrid1.instance.getSelectedRowsData();
        this.selectedKeys = this.dataGrid1.instance.getSelectedRowKeys();
        //console.log("mykeys", this.selectedKeys);
        //console.log("mydata", this.selectedData);

    }

    SetUserEditorOptions(List, IdVal) {
        debugger;
        this.UserEditorOptions = {
            items: List,
            displayExpr: 'Realname',
            valueExpr: 'Id',
            value: IdVal,
            searchEnabled: true,
            disabled: false
        };
    }

    onFormSubmit = async function (e) {
        // this.buttondisabled = true;
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }
        this.formData = this.myform.instance.option('formData');
        const Data = {
            "CreateUser": this.formData.CreateUser,
            "KeyList": this.selectedKeys
        }
        //Data.CreateUser = this.formData.CreateUser;
        // tslint:disable-next-line: max-line-length
        debugger;
        const sendRequest = SendService.sendRequest(this.http, '/WorkOrders/BatchCloseWorkOrder', 'PUT', { key: 1 , values: Data });
        if (sendRequest) {
            this.closepopupVisible = false;
            this.dataGrid1.instance.refresh();
            notify({
                message: '更新完成',
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'success', 3000);
        }
    };

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
