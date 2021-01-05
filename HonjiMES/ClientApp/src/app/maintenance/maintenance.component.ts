import { NgModule, Component, OnInit, ViewChild } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import CustomStore from 'devextreme/data/custom_store';
import DataSource from 'devextreme/data/data_source';
import ArrayStore from 'devextreme/data/array_store';
import { DxDataGridComponent, DxFormComponent, DxPopupComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { APIResponse } from 'src/app/app.module';
import { SendService } from 'src/app/shared/mylib';
import { AppComponent } from 'src/app/app.component';
import { Title } from '@angular/platform-browser';

@Component({
    selector: 'app-maintenance',
    templateUrl: './maintenance.component.html',
    styleUrls: ['./maintenance.component.css']
})
export class MaintenanceComponent implements OnInit {
    dataSourceDB;
    creatpopupVisible: boolean;
    buttondisabled = false;
    constructor() {

        this.dataSourceDB = [{ "Id":1,"item": "A", "cycle": 1, "machine": "A1","lastTime":"2020-12-15T00:30:00","nextTime":"2020-12-15T00:40:00" }, {"Id":2, "item": "A", "cycle": 1, "machine": "A2","nextTime":"2020-12-15T00:00:00","lastTime":"2020-12-15T00:00:00" }, { "Id":3,"item": "AB", "cycle": 1, "machine": "A3","lastTime":"2020-12-16T00:01:00","nextTime":"2020-12-15T00:03:00" }]
    }

    ngOnInit(): void {


    }

    cancelClickHandler(e) {
        //this.dataGridnobom.instance.cancelEditData();
    }
    saveClickHandler(e) {
        //this.dataGridnobom.instance.saveEditData();
    }
    creatdata(){
        this.creatpopupVisible = true;
    }
    creatpopup_result(){
        this.creatpopupVisible = false;
    }

}
