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
import { id_ID } from 'ng-zorro-antd';
@Component({
    selector: 'app-resource-workorder',
    templateUrl: './resource-workorder.component.html',
    styleUrls: ['./resource-workorder.component.css']
})
export class ResourceWorkorderComponent implements OnInit, OnChanges {
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
    ProcessEditorOptions: {
        dataSource: { store: { type: string; data: any; key: string; }; }; searchEnabled: boolean;
        // items: this.MaterialList,
        displayExpr: string; valueExpr: string;
    };

    constructor(private http: HttpClient, myservice: Myservice, private app: AppComponent) {
        this.WorkOrderTypeList = myservice.getWorkOrderStatus();
        this.ReportTypeList = myservice.getReportType();
        this.dataSourceDB = [];
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 300;
        this.colCount = 4;

    }
    ngOnInit() {
    }
    ngOnChanges() {
        this.formData = this.masterkey;
        this.dataSourceDB = new CustomStore({
            key: 'WorkOrderNo',
            load: () => SendService.sendRequest(this.http, this.Controller
                + '/GetWorkOrderReportLogByNum?machine=' + this.masterkey.ProducingMachine),
        });
    }
    // tslint:disable-next-line: only-arrow-functions
    onFormSubmit = function(e) {};
}
