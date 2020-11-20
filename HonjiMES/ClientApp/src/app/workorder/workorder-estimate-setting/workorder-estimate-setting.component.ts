import { Component, OnInit, OnChanges, ViewChild, Input, EventEmitter, Output } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AppComponent } from 'src/app/app.component';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from 'src/app/shared/mylib';
import notify from 'devextreme/ui/notify';
import { DxDataGridComponent, DxFormComponent } from 'devextreme-angular';
import { Myservice } from 'src/app/service/myservice';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-workorder-estimate-setting',
  templateUrl: './workorder-estimate-setting.component.html',
  styleUrls: ['./workorder-estimate-setting.component.css']
})
export class WorkorderEstimateSettingComponent implements OnInit, OnChanges {
    @Output() childOuter = new EventEmitter();
    @ViewChild('dataGrid1') dataGrid1: DxDataGridComponent;
    @Input() randomkeyval: any;
    Controller = '/WorkScheduler';
    dataSourceDB1: any;
    remoteOperations: boolean;
    detailfilter: any;
    UserList: any;
    SettingList: any;

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent) {
        this.remoteOperations = true;
        this.SettingList = myservice.getMachineWorkDateVal();
    }
    ngOnInit() {
    }
    ngOnChanges() {
        this.app.GetData('/Users/GetUsers').subscribe(
            (s2) => {
                if (s2.success) {
                    this.UserList = s2.data;
                }
            }
        );
        this.dataSourceDB1 = new CustomStore({
            key: 'Id',
            load: (loadOptions) => SendService.sendRequest(
                this.http,
                this.Controller + '/GetMachineWorkDate',
                'GET', { loadOptions, remote: this.remoteOperations, detailfilter: this.detailfilter }),
            // byKey: (key) => SendService.sendRequest(this.http, this.Controller + '/GetOrderData', 'GET', { key }),
            insert: (values) => SendService.sendRequest(this.http, this.Controller + '/PostMachineWorkDate', 'POST', { values }),
            update: (key, values) => SendService.sendRequest(this.http, this.Controller + '/PutMachineWorkDate', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(this.http, this.Controller + '/DeleteMachineWorkDate/' + key, 'DELETE')
        });
    }
    onFocusedRowChanging(e) {
        const rowsCount = e.component.getVisibleRows().length;
        const pageCount = e.component.pageCount();
        const pageIndex = e.component.pageIndex();
        const key = e.event && e.event.key;

        if (key && e.prevRowIndex === e.newRowIndex) {
            if (e.newRowIndex === rowsCount - 1 && pageIndex < pageCount - 1) {
                // tslint:disable-next-line: only-arrow-functions
                e.component.pageIndex(pageIndex + 1).done(function() {
                    e.component.option('focusedRowIndex', 0);
                });
            } else if (e.newRowIndex === 0 && pageIndex > 0) {
                // tslint:disable-next-line: only-arrow-functions
                e.component.pageIndex(pageIndex - 1).done(function() {
                    e.component.option('focusedRowIndex', rowsCount - 1);
                });
            }
        }
    }
    onRowPrepared(e) {
        if (e.rowType === 'data') {
        }
    }
    cellClick(e) {
        if (e.rowType === 'header') {
            if (e.column.type === 'buttons') {
                if (e.column.cssClass === 'addmod') {
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
    onEditingStart(e) {
    }
    onFocusedRowChanged(e) {
    }
    onCellPrepared(e) {
    }
    onEditorPreparing(e) {
        // if (e.row && e.row.isSelected === false && (e.dataField === 'SaleDate' || e.dataField === 'SaleQuantity')) {
        //     e.editorOptions.readOnly = true;
        // }
    }
}
