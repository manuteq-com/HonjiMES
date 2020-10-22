import { NgModule, Component, OnInit, ViewChild, Input, OnChanges } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import CustomStore from 'devextreme/data/custom_store';
import DataSource from 'devextreme/data/data_source';
import ArrayStore from 'devextreme/data/array_store';
import { DxDataGridComponent, DxFormComponent, DxPopupComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { APIResponse } from 'src/app/app.module';
import { SendService } from 'src/app/shared/mylib';
import { Myservice } from 'src/app/service/myservice';
import { AppComponent } from 'src/app/app.component';
import { Title } from '@angular/platform-browser';


@Component({
    selector: 'app-quality-record',
    templateUrl: './quality-record.component.html',
    styleUrls: ['./quality-record.component.css']
})
export class QualityRecordComponent implements OnInit, OnChanges {
    @Input() itemkeyval: any;
    @Input() randomkey: any;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    autoNavigateToFocusedRow = true;
    Controller = '/WorkOrders';
    formData: any;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    width: any;
    colCount: number;
    dataSourceDB: any;
    labelLocation: string;
    editOnkeyPress: boolean;
    enterKeyAction: string;
    enterKeyDirection: string;
    buttondisabled = false;
    ReportTypeList: any;
    UserList: any;
    QcTypeList: any;
    QCVisible: boolean;
    NCVisible: boolean;
    remoteOperations: any;
    detailfilter: any;

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent, private titleService: Title) {


        this.QcTypeList = myservice.getQcType();
        this.ReportTypeList = myservice.getReportType();
        this.editOnkeyPress = true;
        this.enterKeyAction = 'moveFocus';
        this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 300;
        this.colCount = 4;
        this.dataSourceDB = [];
        this.QCVisible = false;
        this.NCVisible = false;
        this.getdata();
        this.app.GetData('/Users/GetUsers').subscribe(
            (s2) => {
                if (s2.success) {
                    this.UserList = s2.data;
                }
            }
        );
    }
    getdata() {
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) => SendService.sendRequest(
                this.http,
                this.Controller + '/GetWorkOrderQCLogs',
                'GET', { loadOptions, remote: this.remoteOperations, detailfilter: this.detailfilter }),
            byKey: (key) => SendService.sendRequest(this.http, this.Controller + '/GetWorkOrderQCLog', 'GET', { key }),
            insert: (values) => SendService.sendRequest(this.http, this.Controller + '/PostAdjustList', 'POST', { values }),
            update: (key, values) => SendService.sendRequest(this.http, this.Controller + '/PutAdjustList', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(this.http, this.Controller + '/DeleteAdjustList/' + key, 'DELETE')
        });
    }
    ngOnInit() {
        this.titleService.setTitle('品質紀錄查詢');
    }
    ngOnChanges() { }

    download() { }

    onValueChanged(e) {
        if (e.value === '全部資料') {
            this.dataGrid.instance.clearFilter();
        } else {
            this.dataGrid.instance.filter(['Message', '=', e.value]);
        }
    }
    onDataErrorOccurred(e) {
        notify({
            message: e.error.message,
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'error', 3000);
    }
    onFocusedRowChanged(e) {
    }
    onEditorPreparing(e) {
    }
    selectionChanged(e) {
    }
}
