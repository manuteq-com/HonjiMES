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

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent) {
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
    }
    ngOnInit() {
    }
    ngOnChanges() {
        // debugger;
        this.dataSourceDB = [];
        this.app.GetData('/Users/GetUsers').subscribe(
            (s2) => {
                if (s2.success) {
                    this.UserList = s2.data;
                    if (this.itemkeyval != null) {
                        this.app.GetData('/WorkOrders/GetWorkOrderLogByWorkOrderDetailId/' + this.itemkeyval).subscribe(
                            (s) => {
                                if (s.success) {
                                    s.data.forEach(element => {
                                        element.CreateUser = s2.data.find(x => x.Id === element.CreateUser).Realname;
                                        element.ReCount = element.ReCount !== 0 ? element.ReCount : null;
                                        element.NgCount = element.NgCount !== 0 ? element.NgCount : null;
                                    });
                                    this.dataSourceDB = s.data;
                                }
                            }
                        );
                    }
                }
            }
        );
    }
}
