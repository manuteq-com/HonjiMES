import { NgModule, Component, OnInit, ViewChild, Input, OnChanges, EventEmitter, Output } from '@angular/core';
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
import moment from 'moment';

@Component({
    selector: 'app-maintenance-detail',
    templateUrl: './maintenance-detail.component.html',
    styleUrls: ['./maintenance-detail.component.css']
})
export class MaintenanceDetailComponent implements OnInit {
    dataSourceDB:any;
    @Output() childOuter = new EventEmitter();
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild(DxFormComponent, { static: false }) form: DxFormComponent;
    @Input() masterkey: any;
    @Input() itemval: any;
    @Input() bentest:any;
    UserList:any;
    editorOptions:any;

    Controller = '/MachineMaintenance';
    detailfilter: any;
    autoNavigateToFocusedRow = true;
    constructor(private http: HttpClient, public app: AppComponent) {
        this.app.GetData('/Users/GetUsers').subscribe(
            (s2) => {
                if (s2.success) {
                    this.UserList = s2.data;
                    //this.getdata();
                }
            }
        );
    }
    ngOnInit(): void {

    }
    ngOnChanges(){
        debugger;
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: () => SendService.sendRequest(this.http, this.Controller + '/GetMaintenanceDetails/' + this.masterkey),
            byKey: (key) => SendService.sendRequest(this.http, this.Controller + '/GetMaterial', 'GET', { key }),
            insert: (values) => {
                values.MaintenanceId = this.masterkey;
                values.item = this.itemval.data.Item;
                values.MachineId = this.itemval.data.MachineId;
                let end_date = moment(new Date());
                let start_date  = moment(new Date(values.RecentTime));
                let diffday = end_date.diff(start_date,'days');
                if(diffday > 30){
                    notify({
                        message: '執行日期超過30天前不可新增!! ',
                        position: {
                            my: 'center top',
                            at: 'center top'
                        }
                    }, 'warning', 6000);
                    return;
                }
                return SendService.sendRequest(this.http, this.Controller + '/PostMaintenanceDetail', 'POST', { values })
            },
            update: (key, values) => {
                let end_date = moment(new Date());
                let start_date  = moment(new Date(values.RecentTime));
                let diffday = end_date.diff(start_date,'days');
                if(diffday > 30){
                    notify({
                        message: '執行日期超過30天前不可更新!! ',
                        position: {
                            my: 'center top',
                            at: 'center top'
                        }
                    }, 'warning', 6000);
                    return;
                }
                return SendService.sendRequest(this.http, this.Controller + '/PutMaintenanceDetail', 'PUT', { key, values });
            },
            //remove: (key) => SendService.sendRequest(http, this.Controller + '/Delete/' + key, 'DELETE')
        });
    }
    cancelClickHandler(e) { }
    onDataErrorOccurred(e) { }
    onFocusedRowChanging(e) { }
    onFocusedRowChanged(e) { }
    onEditorPreparing(e) { }
    selectionChanged(e) { }
    cellClick(e) { }
    onRowUpdated(e) { }
    onRowPrepared(e) { }



}
