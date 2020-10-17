import { Component, OnInit, OnChanges, Output, Input, EventEmitter, ViewChild } from '@angular/core';
import { DxFormComponent, DxDataGridComponent, DxButtonComponent } from 'devextreme-angular';
import { HttpClient } from '@angular/common/http';
import { Myservice } from '../../service/myservice';
import { AppComponent } from 'src/app/app.component';

@Component({
    selector: 'app-workorder-report-log',
    templateUrl: './workorder-report-log.component.html',
    styleUrls: ['./workorder-report-log.component.css']
})
export class WorkorderReportLogComponent implements OnInit, OnChanges {
    @Input() itemkeyval: any;
    @Input() randomkey: any;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;

    controller: string;
    formData: any;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    width: any;
    colCount: number;
    dataSourceDB: any[];
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

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent) {
        this.QcTypeList = myservice.getQcType();
        // this.CustomerVal = null;
        // this.formData = null;
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
        this.controller = '/OrderDetails';
        this.QCVisible = false;
        this.NCVisible = false;
    }
    ngOnInit() {
    }
    ngOnChanges() {
        // debugger;
        this.QCVisible = false;
        this.NCVisible = false;
        this.dataSourceDB = [];
        this.app.GetData('/Users/GetUsers').subscribe(
            (s2) => {
                if (s2.success) {
                    this.UserList = s2.data;
                    if (this.itemkeyval != null && this.itemkeyval !== 0) {
                        this.app.GetData('/WorkOrders/GetWorkOrderProcessByWorkOrderDetailId/' + this.itemkeyval).subscribe(
                            (s) => {
                                if (s.success) {
                                    if (s.data.Type === null || s.data.Type === 10) { // 顯示NC報工log
                                        this.ShowNCLogView(s2.data);
                                    } else if (s.data.Type === 20) { // 顯示QC報工log
                                        this.ShowQCLogView(s2.data);
                                    }
                                }
                            }
                        );
                    }
                }
            }
        );
    }
    ShowNCLogView(data) {
        this.NCVisible = true;
        this.app.GetData('/WorkOrders/GetWorkOrderLogByWorkOrderDetailId/' + this.itemkeyval).subscribe(
            (s) => {
                if (s.success) {
                    s.data.forEach(element => {
                        // element.CreateUser = data.find(x => x.Id === element.CreateUser).Realname;
                        element.ReCount = element.ReCount !== 0 ? element.ReCount : null;
                        element.NgCount = element.NgCount !== 0 ? element.NgCount : null;
                    });
                    this.dataSourceDB = s.data;
                }
            }
        );
    }
    ShowQCLogView(data) {
        this.QCVisible = true;
        this.app.GetData('/WorkOrders/GetWorkOrderQCLogByWorkOrderDetailId/' + this.itemkeyval).subscribe(
            (s) => {
                if (s.success) {
                    this.dataSourceDB = s.data;
                }
            }
        );
    }
}
