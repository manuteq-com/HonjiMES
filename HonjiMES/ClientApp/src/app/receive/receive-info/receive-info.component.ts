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
    @Input() itemkeyval: any;
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
    editorOptions: any;
    dataSourceDB1: any;
    dataSourceDB2: any;
    Warehouselist: CustomStore;
    MaterialBasicList: any;
    MaterialBasicSelectBoxOptions: any;
    constructor(private http: HttpClient, public app: AppComponent) {
        this.formData = {};
        // this.editOnkeyPress = true;
        // this.enterKeyAction = 'moveFocus';
        // this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 300;
        this.colCount = 2;
        this.NumberBoxOptions = { showSpinButtons: true, mode: 'number', min: 0, value: 0 };

        //this.MaterialBasicList = SendService.sendRequest(this.http, '/Materials/GetMaterialBasics');
        this.app.GetData('/MaterialBasics/GetMaterialBasicsAsc').subscribe(
            (s) => {
                if (s.success) {
                    this.MaterialBasicList = s.data;
                    // this.MaterialBasicList.forEach(x => {
                    //     x.Name = x.MaterialNo + '_' + x.Name;
                    // });
                    this.MaterialBasicSelectBoxOptions = {
                        dataSource: { paginate: true, store: { type: 'array', data: this.MaterialBasicList, key: 'Id' } },
                        searchEnabled: true,
                        displayExpr: 'MaterialNo',
                        valueExpr: 'Id',
                        onValueChanged: this.onValueChangedDataNo.bind(this),
                    };

                }
            }
        );

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
                    return SendService.sendRequest(this.http, '/WorkOrders/GetWorkOrderHeads/', 'GET', { loadOptions, remote: true });
                },
                byKey: (key) => {
                    return SendService.sendRequest(this.http, '/WorkOrders/GetWorkOrderHeadskey/' + key);
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
        console.log("afff");
        this.formData = {};
        if (this.itemkeyval !== null) {
            this.formData.WorkOrderNo = this.itemkeyval;
            this.GetRequisitionsDetail(this.itemkeyval);
        } else {
            this.dataSourceDB1 = [];
            this.dataSourceDB2 = [];
        }
    }
    GetRequisitionsDetail(key) {
        this.app.GetData('/Users/GetUsers').subscribe(
            (s2) => {
                if (s2.success) {
                    this.app.GetData(this.Controller + '/GetRequisitionsDetailMaterialByWorkOrderNoId/' + key).subscribe(
                        (s) => {
                            if (s.success) {
                                this.dataSourceDB1 = s.data;
                            }
                        }
                    );
                    this.app.GetData(this.Controller + '/GetRequisitionsDetailByWorkOrderNoId/' + key).subscribe(
                        (s) => {
                            if (s.success) {
                                s.data.forEach(element => {
                                    element.CreateUser = s2.data.find(x => x.Id === element.CreateUser).Realname;
                                    if (element.ReceiveUser) {
                                        element.ReceiveUser = s2.data.find(x => x.Id === element.ReceiveUser).Realname;
                                    }
                                    element.ReceiveQty = element.ReceiveQty === 0 ? null : element.ReceiveQty;
                                    element.RbackQty = element.RbackQty === 0 ? null : element.RbackQty;
                                });
                                this.dataSourceDB2 = s.data;
                            }
                        }
                    );
                }
                if (this.formData.WorkOrderNo) {
                    this.formData.MaterialBasicId = undefined;
                }
            }
        );
    }
    async onValueChanged(e) {
        console.log("清除品號", this.formData);
        // this.buttondisabled = false;
        this.GetRequisitionsDetail(e.value);

    }

    async onValueChangedDataNo(e) {
        console.log("清除工單", this.formData);
        if (this.formData.MaterialBasicId) {
            this.formData.WorkOrderNo = undefined;
        }
        let key = e.value;
        this.app.GetData('/Users/GetUsers').subscribe(
            (s2) => {
                if (s2.success) {
                    this.app.GetData(this.Controller + '/GetRequisitionsDetailMaterialByProductBasicId/' + key).subscribe(
                        (s) => {
                            if (s.success) {
                                this.dataSourceDB1 = s.data;
                            }
                        }
                    );
                    this.app.GetData(this.Controller + '/GetRequisitionsDetailByProductBasicId/' + key).subscribe(
                        (s) => {
                            if (s.success) {
                                s.data.forEach(element => {
                                    element.CreateUser = s2.data.find(x => x.Id === element.CreateUser).Realname;
                                    if (element.ReceiveUser) {
                                        element.ReceiveUser = s2.data.find(x => x.Id === element.ReceiveUser).Realname;
                                    }
                                    element.ReceiveQty = element.ReceiveQty === 0 ? null : element.ReceiveQty;
                                    element.RbackQty = element.RbackQty === 0 ? null : element.RbackQty;
                                });
                                this.dataSourceDB2 = s.data;
                            }
                        }
                    );
                }
            }
        );
    }
}
