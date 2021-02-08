import { NgModule, Component, OnInit, ViewChild, Output, Input } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import CustomStore from 'devextreme/data/custom_store';
import DataSource from 'devextreme/data/data_source';
import ArrayStore from 'devextreme/data/array_store';
import { DxDataGridComponent, DxFormComponent, DxPopupComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { APIResponse } from 'src/app/app.module';
import { SendService } from 'src/app/shared/mylib';
import { AppComponent } from 'src/app/app.component';
import { Title } from '@angular/platform-browser';
import { CreateNumberInfo } from 'src/app/model/viewmodels';

@Component({
    selector: 'app-staffmanagement',
    templateUrl: './staffmanagement.component.html',
    styleUrls: ['./staffmanagement.component.css']
})
export class StaffmanagementComponent implements OnInit {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    // @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    dataSource: any;
    dataSourceDB: any;
    modval: any;
    formData: any;
    editOnkeyPress: boolean;
    enterKeyAction: string;
    enterKeyDirection: string;
    labelLocation: string;
    readOnly: boolean;
    showColon: boolean;
    CreateTimeDateBoxOptions: any;
    CreateNumberInfoVal: any;
    saveCheck = true;
    Purchaselist: any;
    TypeDisabled: boolean;
    gridsaveCheck: boolean;
    selectedOperation: string = "between";

    Controller = '/StaffManagement';
    StaffList: any;
    ProcessBasicList: any;
    MachineList: any;
    WorkOrderIdList: any;
    WorkOrderNo: any;
    CreateTime: any;
    EndTime: any;

    constructor(private http: HttpClient, public app: AppComponent, private titleService: Title) {
        this.onRowValidating = this.onRowValidating.bind(this);
        this.onInitNewRow = this.onInitNewRow.bind(this);
        this.formData = null;
        this.editOnkeyPress = true;
        this.enterKeyAction = 'moveFocus';
        this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.CreateTime = new Date();
        this.EndTime = new Date();

        this.CreateTimeDateBoxOptions = {
            onValueChanged: this.CreateTimeValueChange.bind(this)
        };
        this.getData();
        this.app.GetData('/Users/GetUsers').subscribe(
            (s) => {
                if (s.success) {
                    debugger;
                    this.StaffList = s.data;
                }
            }
        );
        this.app.GetData('/WorkOrders/GetWorkOrderHeads').subscribe(
            (s) => {
                if (s.success) {
                    //debugger;
                    this.WorkOrderIdList = s.data.data;
                }
            }
        );
        this.app.GetData('/Processes/GetProcesses').subscribe(
            (s) => {
                if (s.success) {
                    this.ProcessBasicList = s.data;
                    this.ProcessBasicList.forEach(x => {
                        x.Name = x.Code + '_' + x.Name;
                    });
                }
            }
        );
        this.app.GetData('/Machines/GetMachines').subscribe(
            (s) => {
                if (s.success) {
                    s.data.unshift({ Id: null, Name: '' }); // 加入第一行
                    this.MachineList = s.data;
                }
            }
        );
    }
    getData() {
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: () => SendService.sendRequest(this.http, this.Controller + '/GetStaffManagements'),
            byKey: (key) => SendService.sendRequest(this.http, this.Controller + '/GetStaffManagement', 'GET', { key }),
            insert: (values) => SendService.sendRequest(this.http, this.Controller + '/PostStaffManagement', 'POST', { values }),
            update: (key, values) => SendService.sendRequest(this.http, this.Controller + '/PutStaffManagement', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(this.http, this.Controller + '/DeleteStaffManagement/' + key, 'DELETE')
        });
    }
    ngOnInit() {
        this.titleService.setTitle('人員管理');
    }
    ngOnChanges() {
    }
    onInitialized(value, data) {
        data.setValue(value);
    }
    onFocusedCellChanging(e) {
    }
    onSelectionChanged(e) {
        if (this.Purchaselist.length !== 0 && e.value !== 0) {
            this.formData.SupplierId = this.Purchaselist.find(x => x.Id === e.value).SupplierId;
        }
    }
    onTypeSelectionChanged(e) {
        this.CreateTimeValueChange(e);
    }
    CreateTimeValueChange = async function (e) {
        if (this.formData.CreateTime != null) {
            if (this.formData.Type === 30) {
                this.WarehouseIdAVisible = true;
                this.WarehouseIdBVisible = true;
            } else if (this.formData.Type === 40) {
                this.WarehouseIdAVisible = true;
                this.WarehouseIdBVisible = false;
            } else {
                this.WarehouseIdAVisible = false;
                this.WarehouseIdBVisible = false;
            }
            this.CreateNumberInfoVal = new CreateNumberInfo();
            this.CreateNumberInfoVal.Type = this.formData.Type;
            this.CreateNumberInfoVal.CreateNumber = this.formData.PurchaseNo;
            this.CreateNumberInfoVal.CreateTime = this.formData.CreateTime;
            // tslint:disable-next-line: max-line-length
            const sendRequest = await SendService.sendRequest(this.http, '/PurchaseHeads/GetPurchaseNumberByInfo', 'POST', { values: this.CreateNumberInfoVal });
            if (sendRequest) {
                this.formData.Type = sendRequest.Type;
                this.formData.PurchaseNo = sendRequest.CreateNumber;
                // this.formData.CreateTime = sendRequest.CreateTime;
            }
        }
    };

    selectvalueChanged(e, data) {
        data.setValue(e.value);
    }

    onInitNewRow(e) {
        debugger;
        this.saveCheck = false;
        this.CreateTime = new Date();
        e.data.CreateTime = new Date();
    }
    onRowInserted(e) {
        if (this.dataSourceDB.length !== 0) {
            this.TypeDisabled = true;
        }
    }
    onEditingStart(e) {
        if (e.data.TempId) {
            this.saveCheck = false;
            this.CreateTime = e.data.CreateTime;
        }
    }
    onCellPrepared(e) {
        if (e.column.command === 'edit') {
            this.saveCheck = true;
        }
    }
    onRowValidating(e) {
        debugger;
        if (!e.isValid) {
            this.gridsaveCheck = false;
        }
    }
    selectMachineChanged(e, data) {
        data.setValue(e.value);
    }
    selectWorkOrderNoValueChanged(e, data) {
        data.setValue(e.value);
    }
}
