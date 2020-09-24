import { Component, OnInit, OnChanges, Output, Input, EventEmitter, ViewChild } from '@angular/core';
import { DxFormComponent, DxDataGridComponent, DxButtonComponent } from 'devextreme-angular';
import { HttpClient } from '@angular/common/http';
import { APIResponse } from '../../app.module';
import { Observable } from 'rxjs';
import notify from 'devextreme/ui/notify';
import { SendService } from '../../shared/mylib';
import { Myservice } from '../../service/myservice';
import { Button } from 'primeng';
import { CreateNumberInfo, MbomModelData } from 'src/app/model/viewmodels';
import Swal from 'sweetalert2';
import Buttons from 'devextreme/ui/button';
import { AppComponent } from 'src/app/app.component';

@Component({
    selector: 'app-mbillofmaterial-model',
    templateUrl: './mbillofmaterial-model.component.html',
    styleUrls: ['./mbillofmaterial-model.component.css']
})
export class MbillofmaterialModelComponent implements OnInit, OnChanges {
    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() modval: any;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild('dataGrid2') dataGrid2: DxDataGridComponent;
    @ViewChild('myButton') myButton: DxButtonComponent;

    controller: string;
    formData: { MbomModelHeadId: number, ModelCode: string, ModelName: string };
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    width: any;
    colCount: number;
    dataSourceDB: any[];
    labelLocation: string;
    editOnkeyPress: boolean;
    enterKeyAction: string;
    enterKeyDirection: string;

    modeVisible: boolean;
    saveVisible: boolean;
    NewVisible: boolean;
    UpdateVisible: boolean;
    DeleteVisible: boolean;
    InsertVisible: boolean;
    CancelVisible: boolean;

    MbomModelHeadSelectBoxOptions: any;
    MbomModelList: any;
    ProcessBasicList: any;
    SerialNo: any;
    saveDisabled: boolean;
    modName: any;
    ProcessLeadTime: any;
    ProcessTime: any;
    ProcessCost: any;
    ProducingMachine: any;
    Remarks: any;
    DrawNo: any;
    Manpower: any;
    DueStartTime: any;
    DueEndTime: any;
    CopyVisible: boolean;

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent) {
        this.onReorder = this.onReorder.bind(this);
        this.onRowRemoved = this.onRowRemoved.bind(this);
        // this.CustomerVal = null;
        // this.formData = null;
        this.editOnkeyPress = true;
        this.enterKeyAction = 'moveFocus';
        this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 300;
        this.colCount = 3;
        this.dataSourceDB = [];
        this.MbomModelList = [];
        this.controller = '/OrderDetails';
        this.saveDisabled = true;
        this.modName = 'new';

        this.modeVisible = false;
        this.saveVisible = false;
        this.NewVisible = true;
        this.UpdateVisible = false;
        this.DeleteVisible = false;
        this.InsertVisible = false;
        this.CancelVisible = false;
        this.CopyVisible = false;

        this.ProcessLeadTime = null;
        this.ProcessTime = null;
        this.ProcessCost = null;
        this.ProducingMachine = '';
        this.DueStartTime = new Date();
        this.DueEndTime = new Date();

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
    }
    ngOnInit() {
    }
    ngOnChanges() {
        // debugger;
        this.formData = new MbomModelData();
        this.dataSourceDB = [];
        this.GetMbomModelHeads();
        this.modeVisible = false;
        this.saveVisible = false;
        this.UpdateVisible = false;
        this.DeleteVisible = false;
        this.InsertVisible = false;
        this.CancelVisible = false;
        this.CopyVisible = false;
        this.NewVisible = true;
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
        debugger;
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
    onMbomModelSelectionChanged(e) {
        // debugger;
        this.saveDisabled = false;
        if (e.value !== 0 && e.value !== null && e.value !== undefined) {
            this.app.GetData('/MbomModels/GetProcessByMbomModelHeadId/' + e.value).subscribe(
                (s) => {
                    if (s.success) {
                        const info = this.MbomModelList.find(x => x.Id === e.value);
                        this.formData.ModelCode = info.ModelCode;
                        this.formData.ModelName = info.ModelName.replace(info.ModelCode + '_', '');

                        this.dataSourceDB = s.data;
                        this.saveVisible = false;
                        this.UpdateVisible = true;
                        this.DeleteVisible = true;
                        this.CopyVisible = true;
                        this.InsertVisible = true;
                        this.CancelVisible = false;
                    }
                }
            );
        }
    }
    selectvalueChanged(e, data) {
        // debugger;
        data.setValue(e.value);
        const today = new Date();
        this.ProcessBasicList.forEach(x => {
            if (x.Id === e.value) {
                this.ProcessLeadTime = x?.LeadTime ?? 0;
                this.ProcessTime = x?.WorkTime ?? 0;
                this.ProcessCost = x?.Cost ?? 0;
                this.ProducingMachine = x.ProducingMachine;
                this.Remarks = x.Remark;
                this.DrawNo = x.DrawNo;
                this.Manpower = x?.Manpower ?? 1;
            }
        });
    }
    validate_before(): boolean {
        // 表單驗證
        if (this.myform.instance.validate().isValid === false) {
            notify({
                message: '請注意訂單內容必填的欄位',
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'error', 3000);
            return false;
        }
        return true;
    }
    onInitNewRow(e) {
        // debugger;
        this.SerialNo = this.dataSourceDB.length;
        this.SerialNo++;
        e.data.SerialNumber = this.SerialNo;
        e.data.ProcessLeadTime = 0;
        e.data.ProcessTime = 0;
        e.data.ProcessCost = 0;
        e.data.ProducingMachine = '';
        e.data.Remark = '';
        e.data.DrawNo = '';
        e.data.Manpower = 1;
        e.data.DueStartTime = new Date();
        e.data.DueEndTime = new Date();
        // e.data.ActualStartTime = new Date();
        // e.data.ActualEndTime = new Date();

        this.ProcessLeadTime = 0;
        this.ProcessTime = 0;
        this.ProcessCost = 0;
        this.ProducingMachine = '';
        this.Remarks = '';
        this.DrawNo = '';
        this.Manpower = 0;
        this.DueStartTime = new Date();
        this.DueEndTime = new Date();
    }
    onEditingStart(e) {
        this.ProcessLeadTime = e.data.ProcessLeadTime;
        this.ProcessTime = e.data.ProcessTime;
        this.ProcessCost = e.data.ProcessCost;
        this.ProducingMachine = e.data.ProducingMachine;
        this.Remarks = e.data.Remarks;
        this.DrawNo = e.data.DrawNo;
        this.Manpower = e.data.Manpower;
        this.DueStartTime = e.data.DueStartTime;
        this.DueEndTime = e.data.DueEndTime;
    }
    onCellPrepared(e) {

    }
    PostOnClick(e) {
        this.modName = 'post';
    }
    DeleteOnClick(e) {
        this.modName = 'delete';
    }
    NewOnClick(e) {
        this.modName = 'new';
        this.modeVisible = true;
        this.saveDisabled = false;
        this.InsertVisible = false;
        this.NewVisible = false;
        this.UpdateVisible = false;
        this.DeleteVisible = false;
        this.saveVisible = true;
        this.CancelVisible = true;
        this.CopyVisible = false;
        this.dataSourceDB = [];
        this.formData.MbomModelHeadId = 0;
        this.formData.ModelCode = null;
        this.formData.ModelName = null;
    }
    CopyOnClick(e) {
        this.modName = 'copy';
        this.modeVisible = true;
        this.saveDisabled = false;
        this.InsertVisible = false;
        this.NewVisible = false;
        this.UpdateVisible = false;
        this.DeleteVisible = false;
        this.saveVisible = true;
        this.CancelVisible = true;
        this.CopyVisible = false;
        // this.dataSourceDB = [];
    }
    UpdateOnClick(e) {
        this.modName = 'update';
    }
    InsertOnClick(e) {
        this.modName = 'insert';
        this.formData = this.myform.instance.option('formData');
        if (this.dataSourceDB.length !== 0) {
            this.formData.MbomModelHeadId = 0;
            this.formData.ModelCode = null;
            this.formData.ModelName = null;
            this.modeVisible = false;
            this.saveVisible = false;
            this.UpdateVisible = false;
            this.DeleteVisible = false;
            this.InsertVisible = false;
            this.childOuter.emit(this.dataSourceDB);
            this.dataSourceDB = [];
            this.dataGrid2.instance.refresh();
        } else {
            notify({
                message: '製程內容不能為空!',
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'error', 3000);
        }
    }
    CancelOnClick(e) {
        this.modeVisible = false;
        this.NewVisible = true;
        this.CopyVisible = false;
        this.CancelVisible = false;
        this.saveVisible = false;
        this.formData = this.myform.instance.option('formData');
        this.dataSourceDB = [];
        this.dataGrid2.instance.refresh();
        this.formData.MbomModelHeadId = 0;
        this.formData.ModelCode = null;
        this.formData.ModelName = null;
    }
    GetMbomModelHeads() {
        this.app.GetData('/MbomModels/GetMbomModelHeads').subscribe(
            (s) => {
                if (s.success) {
                    this.MbomModelList = s.data;
                    this.MbomModelList.forEach(x => {
                        x.ModelName = x.ModelCode + '_' + x.ModelName;
                    });
                    this.MbomModelHeadSelectBoxOptions = {
                        dataSource: { paginate: true, store: { type: 'array', data: this.MbomModelList, key: 'Id' } },
                        searchEnabled: true,
                        displayExpr: 'ModelName',
                        valueExpr: 'Id',
                        onValueChanged: this.onMbomModelSelectionChanged.bind(this)
                    };
                }
            }
        );
    }
    onFormSubmit = async function(e) {
        if (this.modName === 'new' || this.modName === 'insert' || this.modName === 'copy') {
            return;
        }
        this.buttondisabled = true;
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }

        this.dataGrid2.instance.saveEditData();
        if (this.dataSourceDB.length === 0) {
            notify({
                message: '製程內容不能為空!',
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'error', 3000);
            return false;
        }
        this.formData = this.myform.instance.option('formData');
        this.postval = {
            MbomModelHead: {
                Id: this.formData?.MbomModelHeadId ?? 0,
                ModelCode: this.formData.ModelCode,
                ModelName: this.formData.ModelName
            },
            MbomModelDetail: this.dataSourceDB
        };

        try {
            if (this.modName === 'post') {
                // tslint:disable-next-line: max-line-length
                const sendRequest = await SendService.sendRequest(this.http, '/MbomModels/PostMbomModels', 'POST', { values: this.postval });
                this.viewRefresh(e, sendRequest);
            } else if (this.modName === 'update') {
                // tslint:disable-next-line: max-line-length
                const sendRequest = await SendService.sendRequest(this.http, '/MbomModels/PutMbomModels', 'PUT', { key: this.formData.MbomModelHeadId, values: this.postval });
                this.viewRefresh(e, sendRequest);
            } else if (this.modName === 'delete') {
                Swal.fire({
                    showCloseButton: true,
                    allowEnterKey: false,
                    allowOutsideClick: false,
                    title: '確認刪除?',
                    html: '刪除後將無法復原!',
                    icon: 'question',
                    showCancelButton: true,
                    confirmButtonColor: '#296293',
                    cancelButtonColor: '#CE312C',
                    confirmButtonText: '確認',
                    cancelButtonText: '取消'
                }).then(async (result) => {
                    if (result.value) {
                        // tslint:disable-next-line: max-line-length
                        const sendRequest = await SendService.sendRequest(this.http, '/MbomModels/DeleteMbomModels/' + this.formData.MbomModelHeadId, 'DELETE');
                        this.viewRefresh(e, sendRequest);
                    }
                });
            }
        } catch (error) {

        }
        this.buttondisabled = false;
    };
    viewRefresh(e, result) {
        if (result) {
            this.dataSourceDB = [];
            this.dataGrid2.instance.refresh();
            this.formData.MbomModelHeadId = 0;
            this.formData.ModelCode = null;
            this.formData.ModelName = null;
            this.modeVisible = false;
            this.saveVisible = false;
            this.NewVisible = true;
            this.UpdateVisible = false;
            this.DeleteVisible = false;
            this.InsertVisible = false;
            this.CancelVisible = false;
            this.dataSourceDB = [];
            e.preventDefault();
            // this.childOuter.emit(true);
            this.GetMbomModelHeads();
        }
    }



}
