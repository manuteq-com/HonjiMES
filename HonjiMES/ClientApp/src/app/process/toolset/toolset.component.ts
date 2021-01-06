import { HttpClient } from '@angular/common/http';
import { Component, EventEmitter, Input, OnChanges, OnInit, Output, ViewChild } from '@angular/core';
import { DxDataGridComponent } from 'devextreme-angular';
import CustomStore from 'devextreme/data/custom_store';
import { AppComponent } from 'src/app/app.component';
import { SendService } from 'src/app/shared/mylib';

@Component({
    selector: 'app-toolset',
    templateUrl: './toolset.component.html',
    styleUrls: ['./toolset.component.css']
})
export class ToolsetComponent implements OnInit, OnChanges {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @Output() childOuter = new EventEmitter();
    @Input() ProcessId: number;
    Controller = '/Toolsets';
    autoNavigateToFocusedRow = true;
    dataSourceDB: any;
    HolderList = [];
    ToolList = [];
    List: any;
    constructor(private http: HttpClient, public app: AppComponent) {

        this.app.GetData('/ToolManagement/GetToolManagements').subscribe(s => {
            if (s.success) {
                this.List = s.data;
                s.data.forEach(x => {
                    if (x.Type === 1) {
                        this.ToolList.push(x);
                    } else if (x.Type === 2) {
                        this.HolderList.push(x);
                    }
                });
                this.getdata();
            }
        });


    }
    ngOnChanges() {
        this.getdata();

    }
    ngOnInit() {
        debugger
    }
    onInitNewRow(e) {
        e.data.ProcessId = this.ProcessId;
    }
    getdata() {
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: () => SendService.sendRequest(this.http, this.Controller + '/GetToolsetsByProcessId/' + this.ProcessId),
            byKey: (key) => SendService.sendRequest(this.http, this.Controller + '/GetToolset', 'GET', { key }),
            insert: (values) => SendService.sendRequest(this.http, this.Controller + '/PostToolset', 'POST', { values }),
            update: (key, values) => SendService.sendRequest(this.http, this.Controller + '/PutToolset', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(this.http, this.Controller + '/DeleteToolset/' + key, 'DELETE')
        });
    }
}
