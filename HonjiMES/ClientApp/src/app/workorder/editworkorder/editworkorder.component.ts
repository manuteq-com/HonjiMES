import { Component, OnInit, OnChanges, Output, Input, ViewChild, EventEmitter } from '@angular/core';
import { DxFormComponent, DxDataGridComponent, DxButtonComponent } from 'devextreme-angular';
import { HttpClient } from '@angular/common/http';
import { Myservice } from 'src/app/service/myservice';
import { AppComponent } from 'src/app/app.component';
import { SendService } from 'src/app/shared/mylib';
import notify from 'devextreme/ui/notify';
import Swal from 'sweetalert2';
import CustomStore from 'devextreme/data/custom_store';

@Component({
    selector: 'app-editworkorder',
    templateUrl: './editworkorder.component.html',
    styleUrls: ['./editworkorder.component.css']
})
export class EditworkorderComponent implements OnInit, OnChanges {
    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() modval: any;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild('dataGrid2') dataGrid2: DxDataGridComponent;
    @ViewChild('myButton') myButton: DxButtonComponent;

    formData: any;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    width: any;
    colCount: number;
    dataSourceDB: any;
    labelLocation: string;
    editOnkeyPress: boolean;
    enterKeyAction: string;
    enterKeyDirection: string;
    buttondisabled = false;


    CreateTimeDateBoxOptions: any;
    ProductBasicSelectBoxOptions: any;
    ProductBasicList: any;
    ProcessBasicList: any;
    NumberBoxOptions: any;
    SerialNo: any;
    saveDisabled: boolean;
    runVisible: boolean;
    modVisible: boolean;
    modCheck: boolean;
    modName: any;
    saveCheck: boolean;
    onCellPreparedLevel: any;
    Controller = '/WorkOrders';
    WorkStatusList: any;


    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent) {
        this.onReorder = this.onReorder.bind(this);
        this.onRowRemoved = this.onRowRemoved.bind(this);
        this.validateNumber = this.validateNumber.bind(this);
        // this.CustomerVal = null;
        // this.formData = null;
        this.editOnkeyPress = true;
        this.enterKeyAction = 'moveFocus';
        this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 300;
        this.colCount = 4;
        this.dataSourceDB = [];
        this.saveDisabled = true;
        this.modCheck = false;
        this.modName = 'new';
        this.saveCheck = true;
        this.WorkStatusList = myservice.getWorkOrderType();


        this.CreateTimeDateBoxOptions = {
            onValueChanged: this.CreateTimeValueChange.bind(this)
        };
        this.NumberBoxOptions = { showSpinButtons: true, mode: 'number', min: 1, value: 1 };

        this.app.GetData('/Processes/GetProcesses').subscribe(
            (s) => {
                if (s.success) {
                    if (s.success) {
                        this.ProcessBasicList = s.data;
                        this.ProcessBasicList.forEach(x => {
                            x.Name = x.Code + '_' + x.Name;
                        });
                    }
                }
            }
        );
        this.app.GetData('/ProductBasics/GetProductBasicsAsc').subscribe(
            (s) => {
                if (s.success) {
                    this.ProductBasicList = s.data;
                    this.ProductBasicList.forEach(x => {
                        x.Name = x.ProductNo + '_' + x.Name;
                    });
                    this.ProductBasicSelectBoxOptions = {
                        dataSource: { paginate: true, store: { type: 'array', data: this.ProductBasicList, key: 'Id' } },
                        searchEnabled: true,
                        displayExpr: 'Name',
                        valueExpr: 'Id',
                        onValueChanged: this.onProductBasicSelectionChanged.bind(this)
                    };

                }
            }
        );
        this.app.GetData('/Processes/GetWorkOrderNumber').subscribe(
            (s) => {
                if (s.success) {
                    this.formData = s.data;
                    this.formData.Count = 1;
                }
            }
        );

    }
    ngOnInit() {
    }
    ngOnChanges() {
        // this.dataSourceDB = new CustomStore({
        //     key: 'Id',
        //     load: () => SendService.sendRequest(this.http, '/Processes/GetProcessByWorkOrderDetail/' + this.itemkeyval.Key),
        // });
        debugger;
        this.dataSourceDB = this.app.GetData('/Processes/GetProcessByWorkOrderDetail/' + this.itemkeyval.Key).subscribe(
            (s) => {
                if (s.success) {
                    this.dataSourceDB = s.data;
                }
            }
        );
        this.modVisible = false;
        this.app.GetData('/Processes/GetProcessByWorkOrderHead/' + this.itemkeyval.Key).subscribe(
            (s) => {
                if (s.success) {
                    this.modCheck = true; // 避免製程資訊被刷新
                    this.formData = s.data;
                }
            }
        );

        this.onCellPreparedLevel = 0;
    }
    allowEdit(e) {
        if (e.row.data.Status > 1) {
            return false;
        } else {
            return true;
        }
    }
    onInitialized(value, data) {
        data.setValue(value);
    }
    onRowRemoved(e) {
        this.dataSourceDB.forEach((element, index) => {
            element.SerialNumber = index + 1;
        });
    }
    onReorder(e) {
        const visibleRows = e.component.getVisibleRows();
        const toIndex = this.dataSourceDB.indexOf(visibleRows[e.toIndex].data);
        const fromIndex = this.dataSourceDB.indexOf(e.itemData);

        this.dataSourceDB.splice(fromIndex, 1);
        this.dataSourceDB.splice(toIndex, 0, e.itemData);
        this.dataSourceDB.forEach((element, index) => {
            element.SerialNumber = index + 1;
        });
    }
    onFocusedCellChanging(e) {
    }
    async CreateTimeValueChange(e) {

    }
    onProductBasicSelectionChanged(e) {
        // debugger;
        if (this.modCheck) {
            this.modCheck = false;
        } else {
            this.saveDisabled = false;
            if (e.value !== 0 && e.value !== null && e.value !== undefined) {
                this.app.GetData('/BillOfMaterials/GetProcessByProductBasicId/' + e.value).subscribe(
                    (s) => {
                        if (s.success) {
                            s.data.forEach(e => {
                                e.DueStartTime = new Date();
                                e.DueEndTime = new Date();
                            });
                            this.dataSourceDB = s.data;
                        }
                    }
                );
            }
        }
    }
    selectvalueChanged(e, data) {
        // debugger;
        data.setValue(e.value);
    }

    onEditingStart(e) {
        this.saveCheck = false;
        this.onCellPreparedLevel = 1;
    }
    onInitNewRow(e) {
    }
    editorPreparing(e) {
        if (e.parentType === 'dataRow' && e.dataField === 'ReCount') {
            e.editorOptions.readOnly = true;
            const SelectedRows = this.dataGrid2.instance.getSelectedRowsData().find(x => x.Id === e.row.data.Id);
            if (SelectedRows) {
                e.editorOptions.readOnly = e.row.data.Status !== 2;
            }
        }
    }
    onCellPrepared(e) {
        // if (e.rowType === 'data') {
        //     if (e.column.dataField === 'ReCount') {
        //         debugger;
        //         if (e.row.data.Status === 2) {
        //             e.column.allowEditing = true;
        //         } else {
        //             e.column.allowEditing = false;
        //         }
        //     }
        // }
    }
    onSelectionChanged() {

    }
    onToolbarPreparing(e) {
        const toolbarItems = e.toolbarOptions.items;
        toolbarItems.forEach(item => {
            if (item.name === 'saveButton') {
                item.options.icon = '';
                item.options.text = '批次報工';
                item.showText = 'always';
                item.visible = false;

            } else if (item.name === 'revertButton') {
                item.options.icon = '';
                item.options.text = '取消';
                item.showText = 'always';
            }
        });
    }
    validateNumber(e) {
        const SelectedRows = this.dataGrid2.instance.getSelectedRowsData().find(x => x.Id === e.data.Id);
        if (SelectedRows) {
            if (e.data.Status === 2 && e.value < 1) {
                this.buttondisabled = true;
                return false;
            } else {
                this.buttondisabled = false;
            }
        }
        return true;
    }
    async onFormSubmit(e) {
        let saveok = true;
        const saveEditData = this.dataGrid2.instance.saveEditData();
        const SelectedRows = this.dataGrid2.instance.getSelectedRowsData();
        // tslint:disable-next-line: forin
        for (const x in SelectedRows) {
            if (SelectedRows[x].Status === 2 && SelectedRows[x].ReCount < 1) {
                return;
            }
            const sendRequest = await SendService.sendRequest(
                this.http, this.Controller + '/WorkOrderReportAll', 'PUT',
                { key: SelectedRows[x].Id, values: SelectedRows[x] });
            if (sendRequest) {
                debugger;
            }
            else {
                debugger;
                saveok = false;
            }
        }
        if (saveok) {
            debugger;
            this.dataGrid2.instance.refresh();
            e.preventDefault();
            this.childOuter.emit(true);
        }
    }
}
