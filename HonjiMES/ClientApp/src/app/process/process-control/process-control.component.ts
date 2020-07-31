import { Component, OnInit, ViewChild } from '@angular/core';
import { AppComponent } from 'src/app/app.component';
import notify from 'devextreme/ui/notify';
import { DxDataGridComponent } from 'devextreme-angular';

@Component({
    selector: 'app-process-control',
    templateUrl: './process-control.component.html',
    styleUrls: ['./process-control.component.css']
})
export class ProcessControlComponent implements OnInit {
    @ViewChild('basicTable') dataGrid: DxDataGridComponent;
    dataSourceDB: any = {};
    creatpopupVisible: any;
    itemkey: number;
    mod: string;
    loadingVisible = false;
    constructor(public app: AppComponent) {
        this.loadingVisible = true;
        this.creatpopupVisible = false;
        this.app.GetData('/Processes/GetWorkOrderByStatus/0').subscribe(
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
        this.creatpopupVisible = true;
        this.itemkey = e.Key;
        this.mod = 'edit';
    }
    tdclick(e) {
        // notify({
        //     message: e.ProductNo,
        //     position: {
        //         my: 'center top',
        //         at: 'center top'
        //     }
        // }, 'error', 3000);
    }
    getBlueClass(data) {
        if (data.Status === 1) {
            return 'process_started';
        } else if (data.Status === 3) {
            return 'process_end';
        } else {
            return '';
        }
    }
    getBlue2Class(data) {
        if (data === 2) {
            return 'process_start';
        } else if (data === 3) {
            return 'process_end';
        } else {
            return '';
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
        this.app.GetData('/Processes/GetWorkOrderByStatus/0').subscribe(
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
