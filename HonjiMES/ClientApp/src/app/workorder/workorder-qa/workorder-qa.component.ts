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
import { AppComponent } from 'src/app/app.component';
import { mBillOfMaterial } from 'src/app/model/viewmodels';
import { Myservice } from 'src/app/service/myservice';

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

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent) {
        this.onReorder = this.onReorder.bind(this);
        this.onRowRemoved = this.onRowRemoved.bind(this);
        this.editOnkeyPress = true;
        this.enterKeyAction = 'moveFocus';
        this.enterKeyDirection = 'row';
        this.listStatus = myservice.getWorkOrderType();

        this.dataSourceDB_Process = [];

        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) => SendService.sendRequest(
                this.http,
                this.Controller + '/GetWorkOrderListRun',
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
    ngOnChanges() {
        this.dataSourceDB_Process = [];
    }
    onInitNewRow(e) {
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
        // this.productbasicId = data.data.Id;

        // this.app.GetData('/BillOfMaterials/GetProcessByProductBasicId/' + this.productbasicId).subscribe(
        //     (s) => {
        //         if (s.success) {
        //             this.dataSourceDB_Process = s.data;
        //         }
        //     }
        // );
    }
    async savedata() {
        // this.dataGrid2.instance.saveEditData();
        // this.postval = {
        //     ProductBasicId: this.productbasicId,
        //     BomId: this.bomId,
        //     MBillOfMaterialList: this.dataSourceDB_Process
        // };
        // // tslint:disable-next-line: max-line-length
        // const sendRequest = await SendService.sendRequest(this.http, '/BillOfMaterials/PostMbomlist', 'POST', { values: this.postval });
        // // let data = this.client.POST( this.url + '/OrderHeads/PostOrderMaster_Detail').toPromise();
        // if (sendRequest) {
        //     notify({
        //         message: '更新成功',
        //         position: {
        //             my: 'center top',
        //             at: 'center top'
        //         }
        //     }, 'success', 2000);
        // }
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
