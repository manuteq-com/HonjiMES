import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import CustomStore from 'devextreme/data/custom_store';
import notify from 'devextreme/ui/notify';
import { AppComponent } from '../app.component';
import { Myservice } from '../service/myservice';
import { SendService } from '../shared/mylib';


@Component({
    selector: 'app-machine-process-time',
    templateUrl: './machine-process-time.component.html',
    styleUrls: ['./machine-process-time.component.css']
})
export class MachineProcessTimeComponent implements OnInit {
    autoNavigateToFocusedRow = true;
    dataSourceDB: any;
    Controller = '/MachineProcessTimes';
    constructor(private http: HttpClient, myservice: Myservice, private app: AppComponent, private titleService: Title) {
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: () => SendService.sendRequest(this.http, this.Controller + '/GetMachineProcessTimes', 'GET'),
        });


    }

    ngOnInit() {
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
    onCellPrepared(e) {

    }
    onEditorPreparing(e) {
    }
    selectionChanged(e) {
    }
    onFocusedRowChanging(e) {
        const rowsCount = e.component.getVisibleRows().length;
        const pageCount = e.component.pageCount();
        const pageIndex = e.component.pageIndex();
        const key = e.event && e.event.key;

        if (key && e.prevRowIndex === e.newRowIndex) {
            if (e.newRowIndex === rowsCount - 1 && pageIndex < pageCount - 1) {
                // tslint:disable-next-line: only-arrow-functions
                e.component.pageIndex(pageIndex + 1).done(function () {
                    e.component.option('focusedRowIndex', 0);
                });
            } else if (e.newRowIndex === 0 && pageIndex > 0) {
                // tslint:disable-next-line: only-arrow-functions
                e.component.pageIndex(pageIndex - 1).done(function () {
                    e.component.option('focusedRowIndex', rowsCount - 1);
                });
            }
        }
    }
}
