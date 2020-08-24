import { Component, OnInit, Output, EventEmitter, ViewChild, OnChanges, Input } from '@angular/core';
import { DxFormComponent, DxDataGridComponent } from 'devextreme-angular';
import { Customer } from 'src/app/model/viewmodels';
import { HttpClient } from '@angular/common/http';
import { AppComponent } from 'src/app/app.component';
import notify from 'devextreme/ui/notify';
import { SendService } from 'src/app/shared/mylib';
import CustomStore from 'devextreme/data/custom_store';

@Component({
    selector: 'app-receive-info',
    templateUrl: './receive-info.component.html',
    styleUrls: ['./receive-info.component.css']
})
export class ReceiveInfoComponent implements OnInit, OnChanges {
    @Output() childOuter = new EventEmitter();
    @Input() randomkeyval: any;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    buttondisabled = false;
    formData: any;
    postval: Customer;
    labelLocation: string;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    colCount: number;
    width: any;
    Controller = '/Requisitions';
    buttonOptions: any = {
        text: '存檔',
        type: 'success',
        useSubmitBehavior: true,
        icon: 'save'
    };
    NumberBoxOptions: { showSpinButtons: boolean; mode: string; min: number; value: number; };
    editorOptions: {};
    dataSourceDB1: any;
    dataSourceDB2: any;
    Warehouselist: CustomStore;
    constructor(private http: HttpClient, public app: AppComponent) {
        this.formData = null;
        // this.editOnkeyPress = true;
        // this.enterKeyAction = 'moveFocus';
        // this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 100;
        this.colCount = 1;
        this.NumberBoxOptions = { showSpinButtons: true, mode: 'number', min: 0, value: 0 };
        this.editorOptions = {
            onValueChanged: this.onValueChanged.bind(this),
            dataSource: new CustomStore({
                key: 'Id',
                load: (loadOptions) => {
                    loadOptions.take = 20;
                    loadOptions.sort = [{ selector: 'WorkOrderNo', desc: true }];
                    if (loadOptions.searchValue) {
                        loadOptions.filter = [loadOptions.searchExpr, loadOptions.searchOperation, loadOptions.searchValue];
                    }
                    return SendService.sendRequest(this.http, '/WorkOrders/GetWorkOrderHeadsRun/', 'GET', { loadOptions, remote: true });
                },
            }),
            valueExpr: 'Id',
            displayExpr: 'WorkOrderNo',
            searchEnabled: true,
            searchExpr: 'WorkOrderNo',
            // searchMode: 'startswith',
        };
    }
    ngOnInit() {

    }
    ngOnChanges() {
        if (this.formData !== null) {
            this.formData = {
                WorkOrderNo: 0
            };
        }
    }
    async onValueChanged(e) {
        this.buttondisabled = false;
        this.app.GetData(this.Controller + '/GetRequisitionsDetailMaterialByWorkOrderNo/' + e.value).subscribe(
            (s) => {
                if (s.success) {
                    this.dataSourceDB1 = s.data;
                }
            }
        );
        this.app.GetData(this.Controller + '/GetRequisitionsDetailByWorkOrderNo/' + e.value).subscribe(
            (s) => {
                if (s.success) {
                    this.dataSourceDB2 = s.data;
                }
            }
        );
    }
}
