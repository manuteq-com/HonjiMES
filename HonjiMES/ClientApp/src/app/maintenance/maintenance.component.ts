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
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild(DxFormComponent, { static: false }) form: DxFormComponent;
    dataSourceDB;
    creatpopupVisible: boolean;
    buttondisabled = false;
    Controller = '/MachineMaintenance';
    detailfilter: any;
    itemkey: any;
    bentest:any;
    autoNavigateToFocusedRow = true;
    remoteOperations : boolean;
    MachineList: any
    constructor(private http: HttpClient, public app: AppComponent) {
        this.remoteOperations = true;
        this.getdata();
        this.app.GetData('/Machines/GetMachines').subscribe(
            (s) => {
                console.log(s);
                this.MachineList = s.data;
                if (s.success) {

                }
            }
        );
    }

    ngOnInit(): void {


    }

    getdata() {
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) => {
                // loadOptions.sort = [{ selector: 'WorkOrderNo', desc: true }];
                // if (loadOptions.searchValue) {
                // loadOptions.filter = [
                //     ['CreateTime', '>=', oldDay],
                //     'and',
                //     ['CreateTime', '<=', toDay],
                // ];
                // }
                return SendService.sendRequest(
                    this.http,
                    this.Controller + '/GetMachineMaintenances',
                    'GET', { loadOptions, remote: this.remoteOperations, detailfilter: this.detailfilter });
            },
            byKey: (key) => SendService.sendRequest(this.http, this.Controller + '/GetMachineMaintenance', 'GET', { key }),
            insert: (values) => SendService.sendRequest(this.http, this.Controller + '/PostMachineMaintenance', 'POST', { values }),
            update: (key, values) => SendService.sendRequest(this.http, this.Controller + '/PutMachineMaintenance', 'PUT', { key, values }),
            //remove: (key) => SendService.sendRequest(this.http, this.Controller + '/Delete/' + key, 'DELETE')
        });
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
    onRowChanged() {
        this.dataGrid.instance.refresh();
    }
    onDataErrorOccurred(e){}
    onFocusedRowChanging(e){}
    onRowPrepared(e){}
    onFocusedRowChanged(e){}
    onEditorPreparing(e){}
    onEditingStart(e){}
    selectionChanged(e){}
    onCellPrepared(e){}

}
