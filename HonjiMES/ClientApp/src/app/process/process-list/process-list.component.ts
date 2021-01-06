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
import { Myservice } from 'src/app/service/myservice';

@Component({
    selector: 'app-process-list',
    templateUrl: './process-list.component.html',
    styleUrls: ['./process-list.component.css']
})
export class ProcessListComponent implements OnInit {

    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild(DxFormComponent, { static: false }) form: DxFormComponent;
    autoNavigateToFocusedRow = true;
    dataSourceDB: any;
    formData: any;
    Controller = '/Processes';
    creatpopupVisible: boolean;
    editpopupVisible: boolean;
    itemkey: number;
    mod: string;
    uploadUrl: string;
    exceldata: any;
    Supplierlist: any;
    ProcessTypeList: any;
    NumberBoxOptions: any;
    ToolsVisible: boolean;
    ProcessId: any;

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent, private titleService: Title) {
        this.ToolsVisible = false;
        this.ProcessTypeList = myservice.getProcessType();
        this.Inventory_Change_Click = this.Inventory_Change_Click.bind(this);
        this.cancelClickHandler = this.cancelClickHandler.bind(this);
        this.saveClickHandler = this.saveClickHandler.bind(this);
        this.NumberBoxOptions = { showSpinButtons: true, mode: 'number', min: 0, value: 0 };
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: () => SendService.sendRequest(http, this.Controller + '/GetProcesses'),
            byKey: (key) => SendService.sendRequest(http, this.Controller + '/GetProcess', 'GET', { key }),
            insert: (values) => SendService.sendRequest(http, this.Controller + '/PostProcess', 'POST', { values }),
            update: (key, values) => {
                if (values.LeadTime === null) {
                    values.LeadTime = 0;
                }
                if (values.WorkTime === null) {
                    values.WorkTime = 0;
                }
                if (values.Cost === null) {
                    values.Cost = 0;
                }
                if (values.Manpower === null) {
                    values.Manpower = 0;
                }
                return SendService.sendRequest(http, this.Controller + '/PutProcess', 'PUT', { key, values })
            },
            remove: (key) => SendService.sendRequest(http, this.Controller + '/DeleteProcess/' + key, 'DELETE')
        });
    }
    creatdata() {
        this.creatpopupVisible = true;
    }
    creatpopup_result(e) {
        this.creatpopupVisible = false;
        this.dataGrid.instance.refresh();
        notify({
            message: '製程新增完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
    }
    onUploaded(e) {
        //    debugger;
        const response = JSON.parse(e.request.response) as APIResponse;
        if (response.success) {
            this.mod = 'excel';
            this.creatpopupVisible = true;
            this.exceldata = response.data;
        } else {
            notify({
                message: 'Excel 檔案讀取失敗:' + response.message,
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'error', 3000);
        }
    }
    Inventory_Change_Click(e) {
        this.itemkey = e.row.key;
        this.mod = 'customer';
        this.editpopupVisible = true;
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
    ngOnInit() {
        this.titleService.setTitle('製程基本資料');
    }
    cancelClickHandler(e) {
        this.dataGrid.instance.cancelEditData();
    }
    saveClickHandler(e) {
        this.dataGrid.instance.saveEditData();
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
        // debugger;
        if (e.parentType === 'dataRow' && e.dataField === 'Code') {
            e.editorOptions.readOnly = true;
        }
    }
    selectionChanged(e) {
    }
    readTools(e, data) {
        this.ProcessId = data.key
        this.ToolsVisible = true;

    }
}
