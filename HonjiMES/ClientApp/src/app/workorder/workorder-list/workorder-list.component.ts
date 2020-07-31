import { Component, OnInit, ViewChild } from '@angular/core';
import { AppComponent } from 'src/app/app.component';
import notify from 'devextreme/ui/notify';
import { DxDataGridComponent } from 'devextreme-angular';

@Component({
  selector: 'app-workorder-list',
  templateUrl: './workorder-list.component.html',
  styleUrls: ['./workorder-list.component.css']
})
export class WorkorderListComponent implements OnInit {
    @ViewChild('basicTable') dataGrid: DxDataGridComponent;
    dataSourceDB: any = {};
    creatpopupVisible: any;
    itemkey: number;
    serialkey: number;
    mod: string;
    loadingVisible = false;
    ReportHeight: any;

    constructor(public app: AppComponent) {
        this.loadingVisible = true;
        this.creatpopupVisible = false;

        this.app.GetData('/Processes/GetWorkOrderByStatus/1').subscribe(
            (s) => {
                this.dataSourceDB = s.data;
                this.loadingVisible = false;
            }
        );
        // this.app.GetData('/Processes/GetProcessesStatus/1').subscribe(
        //     (s) => {
        //         debugger;
        //         this.dataSourceDB = s.data;
        //     }
        // );
    }
    ngOnInit() {
    }
    trclick(e) {
        // debugger;
        // this.creatpopupVisible = true;
        // this.itemkey = e.Key;
        // this.mod = 'report';
    }
    tdclick(e, colData) {
        this.itemkey = e.Key;
        this.serialkey = Number(colData.key.substring(4)) + 1;
        this.mod = 'report';
        this.creatpopupVisible = true;

        if (e[colData.key].value3 === 1) {
            this.ReportHeight = 710;
        } else if (e[colData.key].value3 === 2) {
            this.ReportHeight = 760;
        } else if (e[colData.key].value3 === 3) {
            this.ReportHeight = 760;
        }
    }
    getBlueClass(data) {
        // if (data.Status === 1) {
        //     return 'process_started';
        // } else if (data.Status === 3) {
        //     return 'process_ended';
        // } else {
            return '';
        // }
    }
    getBlue2Class(data) {
        if (data === 2) {
            return 'process_start';
        } else if (data === 3) {
            return 'process_end';
        }
    }
    creatdata() {
        this.creatpopupVisible = true;
        this.itemkey = null;
        this.mod = 'new';
    }
    creatpopup_result(e) {
        this.creatpopupVisible = false;
        this.itemkey = null;
        // this.dataGrid.instance.refresh();
        this.loadingVisible = true;
        this.app.GetData('/Processes/GetWorkOrderByStatus/1').subscribe(
            (s) => {
                this.dataSourceDB = s.data;
                this.loadingVisible = false;
            }
        );
        notify({
            message: '更新完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
    }
}
