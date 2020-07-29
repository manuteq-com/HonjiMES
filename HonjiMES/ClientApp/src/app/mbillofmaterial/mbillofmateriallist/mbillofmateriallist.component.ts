import { Component, OnInit, OnChanges, ViewChild } from '@angular/core';
import { APIResponse } from 'src/app/app.module';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { createStore } from 'devextreme-aspnet-data-nojquery';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from 'src/app/shared/mylib';
import DataSource from 'devextreme/data/data_source';
import notify from 'devextreme/ui/notify';
import { DxDataGridComponent } from 'devextreme-angular';

@Component({
  selector: 'app-mbillofmateriallist',
  templateUrl: './mbillofmateriallist.component.html',
  styleUrls: ['./mbillofmateriallist.component.css']
})
export class MbillofmateriallistComponent implements OnInit, OnChanges {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild('dataGrid2') dataGrid2: DxDataGridComponent;
    dataSourceDB: any;
    dataSourceDB_Process: any[];
    apiurl = location.origin + '/api';
    Controller = '/BillOfMaterials';
    remoteOperations = true;
    bomMod: any;

    editOnkeyPress: boolean;
    enterKeyAction: string;
    enterKeyDirection: string;
    ProcessBasicList: any;
    SerialNo = 0;
    productbasicId: any;
    bomId: any;
    bomNo: any;
    bomName: any;
    postval: any;
    saveDisabled: boolean;

    ProcessLeadTime: any;
    ProcessTime: any;
    ProcessCost: any;
    ProducingMachine: any;
    Remarks: any;
    DrawNo: any;
    Manpower: any;

    constructor(private http: HttpClient) {
        this.onReorder = this.onReorder.bind(this);
        this.onRowRemoved = this.onRowRemoved.bind(this);
        this.editOnkeyPress = true;
        this.enterKeyAction = 'moveFocus';
        this.enterKeyDirection = 'row';
        this.bomMod = 'MBOM';
        this.saveDisabled = true;

        this.dataSourceDB_Process = [];

        this.ProcessLeadTime = null;
        this.ProcessTime = null;
        this.ProcessCost = null;
        this.ProducingMachine = '';

        const remote = this.remoteOperations;
        // this.dataSourceDB = createStore({
        //     key: 'Id',
        //     loadUrl: this.apiurl + this.Controller + '/GetProducts',
        //     insertUrl: this.apiurl + this.Controller + '/PostBillofPurchaseDetail',
        //     updateUrl: this.apiurl + this.Controller + '/PutProduct',
        //     deleteUrl: this.apiurl + this.Controller + '/DeleteBillofPurchaseDetail',
        // });
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) =>
                SendService.sendRequest(this.http, this.Controller + '/GetProductBasics', 'GET', { loadOptions, remote }),
            byKey: (key) =>
                SendService.sendRequest(this.http, this.Controller + '/GetBillofPurchaseDetail', 'GET', { key }),
            insert: (values) =>
                SendService.sendRequest(this.http, this.Controller + '/PostBillofPurchaseDetail', 'POST', { values }),
            update: (key, values) =>
                SendService.sendRequest(this.http, this.Controller + '/PutProduct', 'PUT', { key, values }),
            remove: (key) =>
                SendService.sendRequest(this.http, this.Controller + '/DeleteBillofPurchaseDetail', 'DELETE')
        });
        this.GetData('/Processes/GetProcesses').subscribe(
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
    public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>(location.origin + '/api' + apiUrl);
    }
    ngOnInit() {
    }
    ngOnChanges() {
        this.dataSourceDB_Process = [];
    }
    onInitNewRow(e) {
        // debugger;
        this.SerialNo = this.dataSourceDB_Process.length;
        this.SerialNo++;
        e.data.SerialNumber = this.SerialNo;
        e.data.ProcessLeadTime = null;
        e.data.ProcessTime = null;
        e.data.ProcessCost = null;
        e.data.ProducingMachine = '';
        e.data.Remark = '';
        e.data.DrawNo = '';
        e.data.Manpower = 1;

        this.ProcessLeadTime = null;
        this.ProcessTime = null;
        this.ProcessCost = null;
        this.ProducingMachine = '';
        this.Remarks = '';
        this.DrawNo = '';
        this.Manpower = 0;
    }
    onInitialized(value, e) {
        // data.setValue(value);
        e.component.option('value', value);
    }
    onRowRemoved(e) {
        this.dataSourceDB_Process.forEach((element, index) => {
            element.SerialNumber = index + 1;
        });
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
    readBomProcess(e, data) {
        this.productbasicId = data.data.Id;
        this.bomId = 0;
        this.bomNo = data.data.ProductNo;
        this.bomName = data.data.Name;
        this.saveDisabled = false;
        this.GetData('/BillOfMaterials/GetProcessByProductBasicId/' + this.productbasicId).subscribe(
            (s) => {
                if (s.success) {
                    this.dataSourceDB_Process = s.data;
                }
            }
        );
    }
    onChangeVar(variable: any) {
        this.productbasicId = 0;
        this.bomId = variable.Id;
        if (variable.Ismaterial) {
            this.bomNo = variable.MaterialNo;
            this.bomName = variable.MaterialName;
        } else {
            this.bomNo = variable.ProductNo;
            this.bomName = variable.ProductName;
        }
        this.saveDisabled = false;
        this.GetData('/BillOfMaterials/GetProcessByBomId/' + this.bomId).subscribe(
            (s) => {
                if (s.success) {
                    debugger;
                    this.dataSourceDB_Process = s.data;
                }
            }
        );

    }
    selectvalueChanged(e, data) {
        // debugger;
        data.setValue(e.value);
        const today = new Date();
        this.ProcessBasicList.forEach(x => {
            if (x.Id === e.value) {
                this.ProcessLeadTime = x?.LeadTime ?? 0;
                this.ProcessTime = x?.WorkTime ?? 0;
                this.ProcessCost = x?.Cost ?? 0;
                this.ProducingMachine = x.ProducingMachine;
                this.Remarks = x.Remark;
                this.DrawNo = x.DrawNo;
                this.Manpower = x?.Manpower ?? 1;
            }
        });
    }
    async savedata() {
        this.dataGrid2.instance.saveEditData();
        this.postval = {
            ProductBasicId: this.productbasicId,
            BomId: this.bomId,
            MBillOfMaterialList: this.dataSourceDB_Process
        };
        // tslint:disable-next-line: max-line-length
        const sendRequest = await SendService.sendRequest(this.http, '/BillOfMaterials/PostMbomlist', 'POST', { values: this.postval });
        // let data = this.client.POST( this.url + '/OrderHeads/PostOrderMaster_Detail').toPromise();
        if (sendRequest) {
            notify({
                message: '更新成功',
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'success', 2000);
        }
    }

    customizeText1(e) {
        return e.value + '分';
    }

    customizeText2(e) {
        return e.value + '分';
    }

    customizeText3(e) {
        return e.value + '元';
    }
}
