import { Component, OnInit, OnChanges, EventEmitter, Output, Input, ViewChild } from '@angular/core';
import { DxFormComponent, DxDataGridComponent, DxTreeListComponent } from 'devextreme-angular';
import { HttpClient } from '@angular/common/http';
import notify from 'devextreme/ui/notify';
import { Observable } from 'rxjs';
import { APIResponse } from 'src/app/app.module';
import { SendService } from 'src/app/shared/mylib';
import CustomStore from 'devextreme/data/custom_store';
import { AppComponent } from 'src/app/app.component';
@Component({
    selector: 'app-bomverlist',
    templateUrl: './bomverlist.component.html',
    styleUrls: ['./bomverlist.component.css']
})
export class BomverlistComponent implements OnInit, OnChanges {
    @Input() bomverdata: any;
    Controller = '/BillOfMaterials';
    dataSourceDB: any;

    constructor(private http: HttpClient, public app: AppComponent) {

    }
    ngOnInit() {
    }
    ngOnChanges() {
        this.dataSourceDB = this.bomverdata;
    }

}
