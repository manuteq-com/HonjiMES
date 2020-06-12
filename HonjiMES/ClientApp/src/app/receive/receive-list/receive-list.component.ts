import { Component, OnInit, ViewChild } from '@angular/core';
import { DxDataGridComponent } from 'devextreme-angular';
import { Observable } from 'rxjs';
import { APIResponse } from 'src/app/app.module';
import { HttpClient } from '@angular/common/http';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from 'src/app/shared/mylib';
import notify from 'devextreme/ui/notify';

@Component({
    selector: 'app-receive-list',
    templateUrl: './receive-list.component.html',
    styleUrls: ['./receive-list.component.css']
})
export class ReceiveListComponent implements OnInit {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    dataSourceDB: any;
    apiurl = location.origin + '/api';
    Controller = '/Requisitions';
    remoteOperations = true;
    url: string;
    selectBoxOptions: { searchEnabled: boolean; items: any; displayExpr: string; valueExpr: string; };
    public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>(location.origin + '/api' + apiUrl);
    }
    constructor(private http: HttpClient) {
        const remote = this.remoteOperations;
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) =>
                SendService.sendRequest(this.http, this.Controller + '/GetRequisitions', 'GET', { loadOptions, remote }),
            byKey: (key) =>
                SendService.sendRequest(this.http, this.Controller + '/GetRequisition', 'GET', { key }),
            insert: (values) =>
                SendService.sendRequest(this.http, this.Controller + '/PostRequisition', 'POST', { values }),
            update: (key, values) =>
                SendService.sendRequest(this.http, this.Controller + '/PutRequisition', 'PUT', { key, values }),
            remove: (key) =>
                SendService.sendRequest(this.http, this.Controller + '/DeleteRequisition', 'DELETE')
        });
        this.GetData('/BillOfMaterials/GetProductBasicsDrowDown').subscribe(
            (s) => {
                if (s.success) {
                    this.selectBoxOptions = {
                        searchEnabled: true,
                        items: s.data,
                        displayExpr: 'Name',
                        valueExpr: 'Id',
                    };
                }
            }
        );
    }
    ngOnInit() {
    }
    cellClick(e) {
        if (e.rowType === 'header') {
            if (e.column.type === 'buttons') {
                if (e.column.cssClass === 'addmod') {
                    // tslint:disable-next-line: deprecation
                    this.dataGrid.instance.insertRow();
                }
            }
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
    onContentReady(e) {

    }
    onInitNewRow(e) {

    }
    onRowInserting(e) {

    }
    onRowInserted(e) {

    }
    onEditingStart(e) {

    }
}
