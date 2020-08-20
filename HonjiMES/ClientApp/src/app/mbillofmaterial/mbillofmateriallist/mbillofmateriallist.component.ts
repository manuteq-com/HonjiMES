import { Component, OnInit, OnChanges, ViewChild, ɵangular_packages_core_core_y } from '@angular/core';
import { APIResponse } from 'src/app/app.module';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { createStore } from 'devextreme-aspnet-data-nojquery';
import CustomStore from 'devextreme/data/custom_store';
import { SendService, Guid } from 'src/app/shared/mylib';
import DataSource from 'devextreme/data/data_source';
import notify from 'devextreme/ui/notify';
import { DxDataGridComponent } from 'devextreme-angular';
import { AppComponent } from 'src/app/app.component';
import { mBillOfMaterial } from 'src/app/model/viewmodels';
import { ɵangular_packages_forms_forms_y } from '@angular/forms';

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
    // newRow: number;

    // ProcessLeadTime: any;
    // ProcessTime: any;
    // ProcessCost: any;
    // ProducingMachine: any;
    // Remarks: any;
    // DrawNo: any;
    // Manpower: any;

    modelpopupVisible: boolean;
    itemkey: any;
    OnChangeValue: number;
    allowAdding: any;
    nProcess = [];
    constructor(private http: HttpClient, public app: AppComponent) {
        this.onReorder = this.onReorder.bind(this);
        this.onRowRemoved = this.onRowRemoved.bind(this);
        this.onInitNewRow = this.onInitNewRow.bind(this);
        this.editOnkeyPress = true;
        this.enterKeyAction = 'moveFocus';
        this.enterKeyDirection = 'row';
        this.bomMod = 'MBOM';
        this.saveDisabled = true;

        this.dataSourceDB_Process = [];

        // this.ProcessLeadTime = null;
        // this.ProcessTime = null;
        // this.ProcessCost = null;
        // this.ProducingMachine = '';

        this.allowAdding = false;

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
    ngOnChanges() {
        this.dataSourceDB_Process = [];

    }
    onInitNewRow(e) {
        this.SerialNo = this.dataSourceDB_Process.length;
        this.SerialNo++;
        // this.newRow = this.SerialNo;

        const key = Guid.newGuid();
        e.data.SerialNumber = this.SerialNo;
        e.data.key = key;
        const commentData = { SerialNumber: this.SerialNo, key };
        this.nProcess.push(commentData);
        // e.data.ProcessLeadTime = null;
        // e.data.ProcessTime = null;
        // e.data.ProcessCost = null;
        // e.data.ProducingMachine = '';
        // e.data.Remark = '';
        // e.data.DrawNo = '';
        // e.data.Manpower = 1;
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
        this.itemkey = null;
        this.OnChangeValue = 0;
        this.allowAdding = true;
        this.app.GetData('/BillOfMaterials/GetProcessByProductBasicId/' + this.productbasicId).subscribe(
            (s) => {
                if (s.success) {
                    this.dataSourceDB_Process = s.data;
                    this.nProcess = [];
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
        this.app.GetData('/BillOfMaterials/GetProcessByBomId/' + this.bomId).subscribe(
            (s) => {
                if (s.success) {
                    this.allowAdding = true;
                    this.dataSourceDB_Process = s.data;
                }
            }
        );

    }
    TemplatesetValue(e, data) {
        this.nProcess.forEach(x => {
            if (x.key === data.data.key) {
                x[data.column.dataField] = e.value;
            }
        });
        data.setValue(e.value);
    }
    selectvalueChanged(e, data) {

        if (data.row.isNewRow) {
            const ProcessBasic = this.ProcessBasicList.find(x => x.Id === e.value);
            // data.row.data.DrawNo = ProcessBasic.DrawNo;
            // data.row.data.Manpower = ProcessBasic.Manpower;
            // data.row.data.ProcessCost = ProcessBasic.Cost;
            // data.row.data.ProcessLeadTime = ProcessBasic.LeadTime;
            // data.row.data.ProcessTime = ProcessBasic.WorkTime;
            // data.row.data.ProducingMachine = ProcessBasic.ProducingMachine;
            // data.row.data.Remark = ProcessBasic.Remark;
            data.data.DrawNo = ProcessBasic.DrawNo;
            data.data.Manpower = ProcessBasic.Manpower;
            data.data.ProcessCost = ProcessBasic?.Cost ?? 0;
            data.data.ProcessLeadTime = ProcessBasic?.LeadTime ?? 0;
            data.data.ProcessTime = ProcessBasic?.WorkTime ?? 0;
            data.data.ProducingMachine = ProcessBasic.ProducingMachine;
            data.data.Remark = ProcessBasic.Remark;

            this.nProcess.forEach(x => {
                if (x.key === data.row.data.key) {
                    x.DrawNo = ProcessBasic.DrawNo;
                    x.Manpower = ProcessBasic.Manpower;
                    x.ProcessCost = ProcessBasic?.Cost ?? 0;
                    x.ProcessLeadTime = ProcessBasic?.LeadTime ?? 0;
                    x.ProcessTime = ProcessBasic?.WorkTime ?? 0;
                    x.ProducingMachine = ProcessBasic.ProducingMachine;
                    x.Remark = ProcessBasic.Remark;
                }
            });
            // data.DrawNo = ProcessBasic.DrawNo;
            // data.Manpower = ProcessBasic.Manpower;
            // data.ProcessCost = ProcessBasic.Cost;
            // data.ProcessLeadTime = ProcessBasic.LeadTime;
            // data.ProcessTime = ProcessBasic.WorkTime;
            // data.ProducingMachine = ProcessBasic.ProducingMachine;
            // data.Remark = ProcessBasic.Remark;

            // this.ProcessLeadTime = ProcessBasic.LeadTime;
            // this.ProcessTime = ProcessBasic.WorkTime;
            // this.ProcessCost = ProcessBasic.Cost;
            // this.ProducingMachine = ProcessBasic.ProducingMachine;
            // this.Remarks = ProcessBasic.Remark;
            // this.DrawNo = ProcessBasic.DrawNo;
            // this.Manpower = ProcessBasic.Manpower;


        }
        data.setValue(e.value);
        //this.dataGrid2.instance.saveEditData();
    }
    popup_model() {
        this.modelpopupVisible = true;
        this.OnChangeValue += 1;
        this.itemkey = this.OnChangeValue;
    }
    async savedata() {
        debugger;
        this.dataGrid2.instance.saveEditData();
        this.dataSourceDB_Process.forEach(x => {
            this.nProcess.forEach(y => {
                if (x.key === y.key) {
                    x.DrawNo = y.DrawNo;
                    x.Manpower = y.Manpower;
                    x.ProcessCost = y.ProcessCost;
                    x.ProcessLeadTime = y.ProcessLeadTime;
                    x.ProcessTime = y.ProcessTime;
                    x.ProducingMachine = y.ProducingMachine;
                    x.Remark = y.Remark;
                }
            });
        });
        this.postval = {
            ProductBasicId: this.productbasicId,
            BomId: this.bomId,
            MBillOfMaterialList: this.dataSourceDB_Process
        };
        // tslint:disable-next-line: max-line-length
        const sendRequest = await SendService.sendRequest(this.http, '/BillOfMaterials/PostMbomlist', 'POST', { values: this.postval });
        // let data = this.client.POST( this.url + '/OrderHeads/PostOrderMaster_Detail').toPromise();
        if (sendRequest) {
            this.nProcess = [];
            notify({
                message: '更新成功',
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'success', 2000);
        }
    }
    modelpopup_result(e) {
        this.modelpopupVisible = false;

        if (e.length !== 0) {
            e.forEach(element => {
                this.SerialNo = this.dataSourceDB_Process.length;
                this.SerialNo++;

                // tslint:disable-next-line: new-parens
                const tempData = new mBillOfMaterial;
                tempData.SerialNumber = this.SerialNo;
                tempData.ProcessId = element.ProcessId;
                tempData.ProcessLeadTime = element.ProcessLeadTime;
                tempData.ProcessTime = element.ProcessTime;
                tempData.ProcessCost = element.ProcessCost;
                tempData.Remarks = element.Remarks;

                this.dataSourceDB_Process.push(tempData);
            });
        }
        notify({
            message: '完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
    }
    showcell(data) {
        if (data.row.isNewRow || !data.value) {
            const dProcess = this.nProcess.find(x => x.key === data.data.key);
            if (dProcess) {
                data.value = dProcess[data.column.dataField];
                // return dProcess[data.column.dataField];
            }
        }
        return data.value;
    }
    onEditorPreparing(e) {
        if (e.parentType === 'dataRow') {

        }
    }
    onContentReady(e) {

    }
    onRowUpdating(e) {

    }
    onRowInserting(e) {

    }
    onFocusedCellChanging(e) {
        if (e.rows[0].isNewRow) {
            // e.rows[0].data.ProcessLeadTime = this.ProcessLeadTime;
            // e.rows[0].data.ProcessTime = this.ProcessTime;
            // e.rows[0].data.ProcessCost = this.ProcessCost;
            // e.rows[0].data.ProducingMachine = this.ProducingMachine;
            // e.rows[0].data.Remark = this.Remarks;
            // e.rows[0].data.DrawNo = this.DrawNo;
            // e.rows[0].data.Manpower = this.Manpower;
        }
    }
    onFocusedCellChanged(e) {
        if (e.rowIndex === -1) {
            if (this.nProcess) {
                // this.dataGrid2.instance.saveEditData();
                this.dataSourceDB_Process.forEach(x => {
                    this.nProcess.forEach(y => {
                        if (x.key === y.key) {
                            x.DrawNo = y.DrawNo;
                            x.Manpower = y.Manpower;
                            x.ProcessCost = y.ProcessCost;
                            x.ProcessLeadTime = y.ProcessLeadTime;
                            x.ProcessTime = y.ProcessTime;
                            x.ProducingMachine = y.ProducingMachine;
                            x.Remark = y.Remark;
                        }
                    });
                });
            }

        }
    }
}
