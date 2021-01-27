import { HttpClient } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import CustomStore from 'devextreme/data/custom_store';
import notify from 'devextreme/ui/notify';
import { AppComponent } from 'src/app/app.component';
import { Myservice } from 'src/app/service/myservice';
import { SendService } from 'src/app/shared/mylib';

@Component({
    selector: 'app-machine-logs-details',
    templateUrl: './machine-logs-details.component.html',
    styleUrls: ['./machine-logs-details.component.css']
})
export class MachineLogsDetailsComponent implements OnInit {
    @Input() itemkey: number;
    dataSourceDB: any;
    Controller = "/MaterialLogs";
    remoteOperations = false;
    detailfilter: any;
    autoNavigateToFocusedRow = true;
    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent) {


    }

    ngOnInit() {
    }
    ngOnChanges() {
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) => {
                return SendService.sendRequest(
                    this.http,
                    this.Controller + '/GetMachineLogsDetails/' + this.itemkey,
                    'GET', { loadOptions, remote: this.remoteOperations, detailfilter: this.detailfilter });
            }

        });
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
    onFocusedRowChanging(e) { }
    onRowPrepared(e) { }
    onFocusedRowChanged(e) { }
    onEditorPreparing(e) { }
    onEditingStart(e) { }
    selectionChanged(e) { }
    onCellPrepared(e) { }
}
