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
    selector: 'app-toolmanagement',
    templateUrl: './toolmanagement.component.html',
    styleUrls: ['./toolmanagement.component.css']
})
export class ToolmanagementComponent implements OnInit {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild(DxFormComponent, { static: false }) form: DxFormComponent;
    autoNavigateToFocusedRow = true;
    dataSourceDB: any;
    Controller = '/ToolManagement';
    editpopupVisible: boolean;
    itemkey: number;
    mod: string;
    uploadUrl: string;
    exceldata: any;
    ToolTypeList: any;

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent, private titleService: Title) {
        this.Inventory_Change_Click = this.Inventory_Change_Click.bind(this);
        this.cancelClickHandler = this.cancelClickHandler.bind(this);
        this.saveClickHandler = this.saveClickHandler.bind(this);
        this.ToolTypeList = myservice.getToolTypes();
        //debugger;
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: () => SendService.sendRequest(http, this.Controller + '/GetToolManagements'),
            byKey: (key) => SendService.sendRequest(http, this.Controller + '/GetToolManagement', 'GET', { key }),
            insert: (values) => SendService.sendRequest(http, this.Controller + '/PostToolManagement', 'POST', { values }),
            update: (key, values) => SendService.sendRequest(http, this.Controller + '/PutToolManagement', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(http, this.Controller + '/DeleteToolManagement/' + key, 'DELETE')
        });
        // this.ToolTypeList = {
        //     items: myservice.getToolTypes(),
        //     displayExpr: 'Name',
        //     valueExpr: 'Id',
        //     searchEnabled: true,
        //     // onValueChanged: this.onValueChanged.bind(this)
        // };
    }
    ngOnInit() {
        this.titleService.setTitle('刀具基本資料');
    }
    // creatpopup_result(e) {
    //     this.creatpopupVisible = false;
    //     this.dataGrid.instance.refresh();
    //     notify({
    //         message: '刀具新增完成',
    //         position: {
    //             my: 'center top',
    //             at: 'center top'
    //         }
    //     }, 'success', 3000);
    // }
    onUploaded(e) {
        //    debugger;
        const response = JSON.parse(e.request.response) as APIResponse;
        if (response.success) {
            this.mod = 'excel';
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
    allowEdit(e) {
        if (e.row.data.Status > 1) {
            return false;
        } else {
            return true;
        }
    }
    onFocusedRowChanged(e) {
    }
    onCellPrepared(e) {
    }
    onEditorPreparing(e) {
    }
    selectionChanged(e) {
    }
}
