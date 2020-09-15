import { Component, OnInit, OnChanges, EventEmitter, Output, Input, ViewChild } from '@angular/core';
import { DxFormComponent, DxDataGridComponent, DxTreeListComponent } from 'devextreme-angular';
import { HttpClient } from '@angular/common/http';
import notify from 'devextreme/ui/notify';
import { Observable } from 'rxjs';
import { APIResponse } from 'src/app/app.module';
import { SendService } from 'src/app/shared/mylib';
import CustomStore from 'devextreme/data/custom_store';
import { AppComponent } from 'src/app/app.component';
import { Myservice } from 'src/app/service/myservice';

@Component({
    selector: 'app-resource-process',
    templateUrl: './resource-process.component.html',
    styleUrls: ['./resource-process.component.css']
})
export class ResourceProcessComponent implements OnInit, OnChanges {

    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild(DxFormComponent, { static: false }) form: DxFormComponent;
    @Input() masterkey: any;
    Controller = '/WorkOrders';
    dataSourceDB: any;
    listAdjustStatus: any;
    WorkOrderTypeList: any;
    ReportTypeList: any;
    itemkeyval: string;
    formData: any;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    width: any;
    colCount: number;
    labelLocation: string;
    ProcessList: any;
    selectStatus: any;
    ProcessEditorOptions: {
        dataSource: { store: { type: string; data: any; key: string; }; }; searchEnabled: boolean;
        // items: this.MaterialList,
        displayExpr: string; valueExpr: string;
    };
    StatusList: any;

    constructor(private http: HttpClient, myservice: Myservice, private app: AppComponent) {
        this.WorkOrderTypeList = myservice.getWorkOrderStatus();
        this.StatusList = myservice.getResourceWorkOrderStatus();
        this.dataSourceDB = [];
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 300;
        this.colCount = 4;
        this.selectStatus = {
            items: this.StatusList,
            displayExpr: 'Name',
            valueExpr: 'Id',
            searchEnabled: true,
            onValueChanged: this.onValueChanged.bind(this)
        };

    }
    ngOnInit() {
    }
    ngOnChanges() {
        this.formData = this.masterkey;
        this.dataSourceDB = new CustomStore({
            key: 'ProcessNo',
            load: () => SendService.sendRequest(this.http, this.Controller
                + '/GetProcessByMachineName?machine=' + this.masterkey.ProducingMachine),
        });
    }
    onValueChanged(e) {
        debugger;
        if (e.value === 0) {
            this.dataGrid.instance.clearFilter();
        } else {
            this.dataGrid.instance.filter(['WorkOrderHead.Status', '=', e.value]);
        }
    }
    // tslint:disable-next-line: only-arrow-functions
    onFormSubmit = function (e) { };
}
