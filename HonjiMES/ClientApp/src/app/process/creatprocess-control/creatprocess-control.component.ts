import { OrderDetail, WorkOrderHead } from './../../model/viewmodels';
import { Component, OnInit, OnChanges, Output, Input, EventEmitter, ViewChild } from '@angular/core';
import { DxFormComponent, DxDataGridComponent, DxButtonComponent } from 'devextreme-angular';
import { HttpClient } from '@angular/common/http';
import { APIResponse } from '../../app.module';
import { Observable } from 'rxjs';
import notify from 'devextreme/ui/notify';
import { SendService } from '../../shared/mylib';
import { Myservice } from '../../service/myservice';
import { Button } from 'primeng';
import { CreateNumberInfo } from 'src/app/model/viewmodels';
import Swal from 'sweetalert2';
import Buttons from 'devextreme/ui/button';
import { AppComponent } from 'src/app/app.component';
import { promise } from 'selenium-webdriver';
import moment from 'moment';
moment.locale('zh-tw');

@Component({
    selector: 'app-creatprocess-control',
    templateUrl: './creatprocess-control.component.html',
    styleUrls: ['./creatprocess-control.component.css']
})
export class CreatprocessControlComponent implements OnInit, OnChanges {
    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() modval: any;
    @Input() checkBoxarray: any;
    @Input() randomkeyval: any;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild('dataGrid2') dataGrid2: DxDataGridComponent;
    @ViewChild('myButton') myButton: DxButtonComponent;
    @Input() popupClose: boolean;

    controller: string;
    formData: any;
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
    buttondisabled = false;
    itemkeyTemp: any;
    productBasicChange: boolean;

    CreateTimeDateBoxOptions: any;
    MaterialBasicSelectBoxOptions: any;
    MaterialBasicList: any;
    ProcessBasicList: any;
    NumberBoxOptions: any;
    OrderNumberOptions: any;
    zNumberBoxOptions: any;
    SerialNo: any;
    saveDisabled: boolean;
    runVisible: boolean;
    newVisible: boolean;
    modVisible: boolean;
    editVisible: boolean;
    modCheck: boolean;
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
    saveCheck: boolean;
    onCellPreparedLevel: any;
    allowReordering: boolean;
    listWorkOrderTypes: any;
    MachineList: any;
    processVisible: boolean;
    countVal: number;
    editVisible2: boolean;
    cm3Machine: any;
    StaffList: any;
    DateOptions: {};
    DateOptionsDSt: { type: string; displayFormat: string; onOpened: (e: any) => void; onValueChanged: any; };
    DateOptionsDEnd: { type: string; displayFormat: string; onOpened: (e: any) => void; onValueChanged: any; };


    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent) {
        this.listWorkOrderTypes = myservice.getWorkOrderTypes();
        this.onReorder = this.onReorder.bind(this);
        this.onRowRemoved = this.onRowRemoved.bind(this);
        this.setDateTime = this.setDateTime.bind(this);
        this.ProcessLeadTimesetCellValue = this.ProcessLeadTimesetCellValue.bind(this);
        this.ProcessTimesetCellValue = this.ProcessTimesetCellValue.bind(this);
        this.Countchange = this.Countchange.bind(this);

        // this.CustomerVal = null;
        this.formData = {};
        this.editOnkeyPress = true;
        this.enterKeyAction = 'moveFocus';
        this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 300;
        this.colCount = 24;
        this.dataSourceDB = [];
        this.controller = '/OrderDetails';
        this.saveDisabled = true;
        this.modCheck = false;
        this.modName = 'new';
        this.saveCheck = true;
        this.allowReordering = true;
        this.NumberBoxOptions = { showSpinButtons: true, mode: 'number', min: 1 };
        this.OrderNumberOptions = { showSpinButtons: true, mode: 'number', min: 1 };
        //this.DateOptions = {type: "datetime", displayFormat: "yyyy/MM/dd HH:mm", onOpened: this.setDateTime};
        this.DateOptionsDSt = {
            type: "date", displayFormat: "yyyy/MM/dd", onOpened: this.setDateTime,
            onValueChanged: this.setDueStDate.bind(this)
        };
        this.DateOptionsDEnd = {
            type: "date", displayFormat: "yyyy/MM/dd", onOpened: this.setDateTime,
            onValueChanged: this.setDueEndDate.bind(this)
        };

        this.ProcessLeadTime = null;
        this.ProcessTime = null;
        this.ProcessCost = null;
        this.ProducingMachine = '';
        this.DueStartTime = new Date();
        this.DueEndTime = new Date();

        this.CreateTimeDateBoxOptions = {
            onValueChanged: this.CreateTimeValueChange.bind(this)
        };
        this.NumberBoxOptions = { showSpinButtons: true, mode: 'number', min: 1, value: 1, onValueChanged: this.Countchange };
        this.OrderNumberOptions = { showSpinButtons: true, mode: 'number', min: 1, value: 1, onValueChanged: this.Countchange };
        this.zNumberBoxOptions = { showSpinButtons: true, mode: 'number', min: 0, value: 0 };
        this.app.GetData('/Machines/GetMachines').subscribe(
            (s) => {
                if (s.success) {
                    if (s.success) {
                        s.data.sort(function (a, b) {
                            if (a.Name < b.Name) {
                                return -1;
                            }
                            if (a.Name > b.Name) {
                                return 1;
                            }
                            return 0;
                        })
                        s.data.unshift({ Id: null, Name: '' }); // 加入第一行
                        this.MachineList = s.data;
                    }
                }
            }
        );
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
        this.app.GetData('/MaterialBasics/GetMaterialBasicsAsc').subscribe(
            (s) => {
                if (s.success) {
                    this.MaterialBasicList = s.data;
                    this.MaterialBasicList.forEach(x => {
                        x.Name = x.MaterialNo + '_' + x.Name;
                    });
                    this.MaterialBasicSelectBoxOptions = {
                        dataSource: { paginate: true, store: { type: 'array', data: this.MaterialBasicList, key: 'Id' } },
                        searchEnabled: true,
                        displayExpr: 'Name',
                        valueExpr: 'Id',
                        onValueChanged: this.onMaterialBasicSelectionChanged.bind(this)
                    };

                }
            }
        );
        // this.app.GetData('/Processes/GetWorkOrderNumber').subscribe(
        //     (s) => {
        //         if (s.success) {
        //             console.log("Processes/GetWorkOrderNumber0", s.data);
        //             let rawData = s.data;
        //                 //this.formData = s.data;
        //                 rawData.Count = 1;
        //                 console.log("this.formData",this.formData, rawData);
        //                 this.formData = rawData;

        //         }
        //     }
        // );
        this.app.GetData('/Users/GetUsers').subscribe(
            (s) => {
                if (s.success) {
                    this.StaffList = s.data;
                    //console.log("this.StaffList",this.StaffList);
                }
            }
        );

    }
    ngOnInit() {
    }
    ngOnChanges() {
        this.modName = this.modval;
        this.cm3Machine = undefined;
        // console.log('itemkeyval', this.itemkeyval);
        // console.log('modval', this.modval);
        // console.log('checkBoxarray', this.checkBoxarray);
        // console.log('randomkeyval', this.randomkeyval);
        // console.log('itemkeyTemp',this.itemkeyTemp);
        this.dataSourceDB = [];
        this.newVisible = false;
        this.modVisible = false;
        this.runVisible = false;
        this.editVisible = false;
        this.saveDisabled = false;
        this.processVisible = false;
        this.allowReordering = false;
        this.editVisible2 = false;
        if (this.modval === 'new') {
            this.newVisible = true;
            this.modVisible = true;
            this.processVisible = true;
            this.allowReordering = true;
            this.editVisible2 = false;
            this.app.GetData('/Processes/GetWorkOrderNumber').subscribe(
                (s) => {
                    if (s.success) {
                        //console.log("Processes/GetWorkOrderNumber1",s.data);
                        let rawData = s.data;
                        rawData.Count = 1;
                        rawData.OrderCount = 1;
                        rawData.DueStartTime = new Date();
                        rawData.DueEndTime = new Date();
                        //console.log("this.formDataNEW",this.formData, rawData);
                        this.formData = rawData;
                    }
                }
            );
        } else if (this.itemkeyval != null && this.itemkeyval !== 0) {
            // 按編輯工單時 讀取資料
            this.app.GetData('/Processes/GetProcessByWorkOrderId/' + this.itemkeyval).subscribe(
                (s) => {
                    if (s.success) {
                        console.log("Processes/GetProcessByWorkOrderIdEdit", s.data);
                        let rawData0 = {};
                        this.dataSourceDB = s.data.WorkOrderDetail;
                        rawData0["WorkOrderHeadId"] = s.data.WorkOrderHead.Id;
                        rawData0["WorkOrderNo"] = s.data.WorkOrderHead.WorkOrderNo;
                        rawData0["CreateTime"] = s.data.WorkOrderHead.CreateTime;
                        rawData0["MaterialBasicId"] = s.data.WorkOrderHead.DataId;
                        rawData0["Count"] = s.data.WorkOrderHead.Count;
                        rawData0["OrderCount"] = s.data.WorkOrderHead.OrderCount;
                        rawData0["DrawNo"] = s.data.WorkOrderHead.DrawNo;
                        rawData0["MachineNo"] = s.data.WorkOrderHead.MachineNo;
                        rawData0["DueStartTime"] = s.data.WorkOrderHead.DueStartTime;
                        rawData0["DueEndTime"] = s.data.WorkOrderHead.DueEndTime;
                        rawData0["CreateUser"] = s.data.WorkOrderHead.CreateUser;

                        this.formData = rawData0;
                        //console.log("rawData", rawData0);
                        this.myform.instance.repaint(); //重新popup時舊資料要刷新
                        // this.formData.Remarks = s.data[0].Remarks;
                        if (s.data.WorkOrderHead.Status === 4) { // 工單為[轉單]

                            this.runVisible = true;
                            this.processVisible = true;
                            this.allowReordering = true;
                            this.newVisible = false;
                            this.editVisible2 = false;
                        }
                        else if (s.data.WorkOrderHead.Status === 0) { // 工單為[新建]
                            this.runVisible = true;
                            this.processVisible = true;
                            this.allowReordering = true;
                            this.newVisible = false;
                            this.editVisible2 = true;
                        } else if (s.data.WorkOrderHead.Status === 5) { // 工單為[結案]，不能編輯
                            this.editVisible = true;
                            this.modVisible = true;
                            this.editVisible2 = true;
                        } else {
                            this.newVisible = false;
                            this.editVisible2 = true;
                            this.editVisible = true;
                            this.processVisible = true;
                            this.allowReordering = true;
                        }
                    }
                }
            );
        }
        this.onCellPreparedLevel = 0;
    }

    setDateTime(e) {
        //console.log("opentime",e.component.option("value"));
    }

    validateNumber(e) {
        var test = new Date(e.value);
        console.log("validateNumber", new Date(e.value), test instanceof Date, isNaN(test.valueOf()));
        let isNull = false;
        isNull = (e.value == null) || (e.value == undefined) || (e.value == '');
        return (test instanceof Date && !isNaN(test.valueOf())) || isNull;
    }

    allowEdit(e) {
        if (e.row.data.Status !== undefined && e.row.data.WorkOrderHead !== undefined && e.row.data.WorkOrderHead !== null) {
            if (e.row.data.Status === 2 || e.row.data.Status === 3 || e.row.data.WorkOrderHead.Status === 5) {
                return false;
            } else {
                return true;
            }
        } else {
            return true;
        }
    }
    cellClick(e) {
        if (e.rowType === 'header') {
            if (e.column.type === 'buttons') {
                if (e.column.cssClass === 'addmod') {
                    this.dataGrid2.instance.saveEditData();
                    // tslint:disable-next-line: deprecation
                    this.dataGrid2.instance.addRow();
                }
            }
        } else if (e.rowType === 'header' && e.rowType === 'data') {
            // // tslint:disable-next-line: deprecation
            // this.dataGrid.instance.insertRow();
        }
    }
    onInitialized(value, data) {
        data.setValue(value);
    }
    onRowRemoved(e) {
        this.dataGrid2.instance.saveEditData();
        this.dataSourceDB.forEach((element, index) => {
            element.SerialNumber = index + 1;
        });
    }
    onReorder(e) {
        this.dataGrid2.instance.saveEditData();
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
    CreateTimeValueChange = async function (e) {
        // this.formData = this.myform.instance.option('formData');
        if (this.formData.CreateTime != null) {
            this.CreateNumberInfoVal = new CreateNumberInfo();
            this.CreateNumberInfoVal.CreateNumber = this.formData.WorkOrderNo;
            this.CreateNumberInfoVal.CreateTime = this.formData.CreateTime;
            // tslint:disable-next-line: max-line-length
            const sendRequest = await SendService.sendRequest(this.http, '/Processes/GetWorkOrderNumberByInfo', 'POST', { values: this.CreateNumberInfoVal });
            if (sendRequest) {
                this.formData.WorkOrderNo = sendRequest.CreateNumber;
                // this.formData.CreateTime = sendRequest.CreateTime;
            }
        }
    };
    onMaterialBasicSelectionChanged(e) {
        // console.log("herechange", this.modCheck, e, this.modval);
        if (this.modCheck) {
            this.modCheck = false;
        } else {
            this.saveDisabled = false;
            //第一次載入不需觸發
            if (e.value !== 0 && e.value !== null && e.value !== undefined && e.event !== undefined) {

                Swal.fire({
                    showCloseButton: true,
                    allowEnterKey: false,
                    allowOutsideClick: false,
                    title: '載入該圖號製程?',
                    html: '製程將會讀取新選的圖號的製程!',
                    icon: 'question',
                    showCancelButton: true,
                    confirmButtonColor: '#296293',
                    cancelButtonColor: '#CE312C',
                    confirmButtonText: '確認',
                    cancelButtonText: '取消'
                }).then(async (result) => {
                    if (result.value) {
                        // tslint:disable-next-line: max-line-length
                        this.app.GetData('/BillOfMaterials/GetProcessByMaterialBasicId/' + e.value).subscribe(
                            (s) => {
                                if (s.success) {
                                    // console.log("griddata0", s.data);
                                    s.data.forEach(element => {
                                        element.Id = 0;
                                        element.CreateUser = undefined;
                                        element.DueStartTime = new Date();
                                        element.DueEndTime = new Date();
                                    });
                                    this.dataSourceDB = s.data;

                                    this.productBasicChange = true;
                                }
                            }
                        );
                    } else {
                        return false;
                    }
                });


            }
        }
    }
    selectvalueChanged(e, data) {
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
    selectMachineChanged(e, data) {
        data.setValue(e.value);
        // const today = new Date();
        // this.ProcessBasicList.forEach(x => {
        //     if (x.Id === e.value) {
        //         this.ProcessLeadTime = x?.LeadTime ?? 0;
        //         this.ProcessTime = x?.WorkTime ?? 0;
        //         this.ProcessCost = x?.Cost ?? 0;
        //         this.ProducingMachine = x.ProducingMachine;
        //         this.Remarks = x.Remark;
        //         this.DrawNo = x.DrawNo;
        //         this.Manpower = x?.Manpower ?? 1;
        //     }
        // });
    }

    selectUserChanged(e, data) {
        //console.log("edata",e,data)
        data.setValue(e.value);
    }
    validate_before(): boolean {
        // 表單驗證
        if (this.myform.instance.validate().isValid === false) {
            notify({
                message: '請注意訂單內容必填的欄位及時間格式',
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
        this.saveCheck = false;
        this.onCellPreparedLevel = 1;

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
        this.saveCheck = false;
        this.onCellPreparedLevel = 1;

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
        if (e.column.command === 'edit') {
            if (this.onCellPreparedLevel === 1) {
                this.onCellPreparedLevel = 2;
            } else if (this.onCellPreparedLevel === 2) {
                this.onCellPreparedLevel = 3;
                this.saveCheck = true;
            } else if (this.onCellPreparedLevel === 3) {
                this.myButton.instance.focus();
            }
        }
    }
    editorPreparing(e) {
        if (e.parentType === 'dataRow' && e.dataField === 'Type') {
            e.editorOptions.readOnly = true;
            if (e.row.data.ProcessType == null || e.row.data.ProcessType === 10) { // 如工序的[種類]為'null'或'NC'則可以選擇是否委外。
                e.editorOptions.readOnly = false;
            }
        }
    }
    onToolbarPreparing(e) {
        e.toolbarOptions.visible = false;
        // const toolbarItems = e.toolbarOptions.items;
        // toolbarItems.forEach(item => {
        //     if (item.name === 'saveButton') {
        //         // item.options.icon = '';
        //         // item.options.text = '退料';
        //         // item.showText = 'always';
        //         item.visible = false;
        //     } else if (item.name === 'revertButton') {
        //         // item.options.icon = '';
        //         // item.options.text = '取消';
        //         // item.showText = 'always';
        //         item.visible = false;
        //     }
        // });
    }
    DeleteOnClick(e) {
        this.modName = 'delete';
    }
    ResetOnClick(e) {
        this.modName = 'reset';
    }
    UpdateOnClick(e) {
        this.dataGrid2.instance.saveEditData();
        this.modName = 'update';
    }
    RunOnClick(e) {
        this.modName = 'run';
    }
    CancelOnClick(e) {
        this.modName = 'cancel';
    }
    onFormSubmit = async function (e) {
        // debugger;

        if (this.modName === 'reset') {
            Swal.fire({
                showCloseButton: true,
                allowEnterKey: false,
                allowOutsideClick: false,
                title: '確認重設製程?',
                html: '重設製程後將無法復原!',
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#296293',
                cancelButtonColor: '#CE312C',
                confirmButtonText: '確認',
                cancelButtonText: '取消'
            }).then(async (result) => {
                if (result.value) {
                    // tslint:disable-next-line: max-line-length
                    const sendRequest = await SendService.sendRequest(this.http, '/Processes/ResetWorkOrderList/' + this.formData.WorkOrderHeadId);
                    this.viewRefresh(e, sendRequest);
                } else {
                    return false;
                }
            });
        } else {

            if (this.modName !== 'cancel') {
                this.dataGrid2.instance.saveEditData();
            } else {
                this.dataGrid2.instance.cancelEditData();
                this.childOuter.emit(true);
            }
            this.buttondisabled = true;
            if (this.validate_before() === false) {
                this.buttondisabled = false;
                return;
            }
            if (!this.saveCheck) {
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
            this.postval = {
                WorkOrderHead: {
                    Id: this.formData.WorkOrderHeadId,
                    WorkOrderNo: this.formData.WorkOrderNo,
                    CreateTime: this.formData.CreateTime,
                    // DataType: 2,
                    DataId: this.formData.MaterialBasicId,
                    Count: this.formData.Count,
                    OrderCount: this.formData.OrderCount,
                    DrawNo: this.formData.DrawNo,
                    MachineNo: this.formData.MachineNo,
                    DueStartTime: this.formData.DueStartTime,
                    DueEndTime: this.formData.DueEndTime,
                    CreateUser: this.formData.CreateUser
                },
                WorkOrderDetail: this.dataSourceDB
            };
            //console.log("this.postvalpro",this.postval);

            try {
                if (this.modName === 'run') {
                    // tslint:disable-next-line: max-line-length
                    const sendRequest = await SendService.sendRequest(this.http, '/WorkOrders/toWorkOrder', 'POST', { values: this.postval });
                    this.viewRefresh(e, sendRequest);
                } else if (this.modName === 'new') {
                    // tslint:disable-next-line: max-line-length
                    const sendRequest = await SendService.sendRequest(this.http, '/Processes/PostWorkOrderList', 'POST', { values: this.postval });
                    this.viewRefresh(e, sendRequest);
                } else if (this.modName === 'update') {
                    // tslint:disable-next-line: max-line-length
                    const sendRequest = await SendService.sendRequest(this.http, '/Processes/PutWorkOrderList', 'PUT', { key: this.formData.WorkOrderHeadId, values: this.postval });
                    // this.viewRefresh(e, sendRequest);
                    if (sendRequest) {
                        this.dataGrid2.instance.refresh();
                        e.preventDefault();
                        this.childOuter.emit(true);
                        notify({
                            message: '更新完成',
                            position: {
                                my: 'center top',
                                at: 'center top'
                            }
                        }, 'success', 3000);
                    }
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
                            const sendRequest = await SendService.sendRequest(this.http, '/Processes/DeleteWorkOrderList/' + this.formData.WorkOrderHeadId, 'DELETE');
                            this.viewRefresh(e, sendRequest);
                        }
                    });
                }
            } catch (error) {

            }
        }
        this.buttondisabled = false;
    };
    viewRefresh(e, result) {
        if (result) {
            this.dataSourceDB = [];
            this.dataGrid2.instance.refresh();
            this.dataSourceDB = [];
            e.preventDefault();
            this.childOuter.emit(true);
            notify({
                message: '更新完成',
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'success', 3000);
        }
    }
    ProcessLeadTimesetCellValue(newData, value, currentRowData) {
        newData.ProcessLeadTime = value;
        newData.ExpectedlTotalTime = (currentRowData.ProcessTime + value) * this.formData.Count
        if (isNaN(newData.ExpectedlTotalTime)) {
            newData.ExpectedlTotalTime = 0;
        }
    }
    ProcessTimesetCellValue(newData, value, currentRowData) {
        newData.ProcessTime = value;
        newData.ExpectedlTotalTime = (currentRowData.ProcessLeadTime + value) * this.formData.Count;
        if (isNaN(newData.ExpectedlTotalTime)) {
            newData.ExpectedlTotalTime = 0;
        }
    }
    Countchange(e) {
        this.dataGrid2.instance.saveEditData();
        this.dataSourceDB.forEach(item => {
            item.ExpectedlTotalTime = (item.ProcessLeadTime + item.ProcessTime) * e.value;
        });
        this.dataGrid2.instance.refresh();
    }

    onMachineSelectionChanged(e) {
        let newMachine = e.component.option('text');
        this.dataSourceDB.forEach(function (v, k) {
            if (v.ProcessNo.slice(0, 3)?.toUpperCase() == "CM3") {
                v.ProducingMachine = newMachine;
            }
        })
    }

    onStaffSelectionChanged(e) {
        this.dataSourceDB.forEach(function (v, k) {
            v.CreateUser = e.value;
        })
    }

    setDueStDate(e) {
        // console.log("eeest=>",e);
        if (e.previousValue || e.event) {
            let newDate = e.component.option('text');
            if (moment(new Date(newDate)).isValid()) {
                this.dataSourceDB.forEach(function (v, k) {
                    v.DueStartTime = new Date(newDate);
                })
            }
        }
    }
    setDueEndDate(e) {
        if (e.previousValue || e.event) {
            let newDate = e.component.option('text');
            if (moment(new Date(newDate)).isValid()) {
                this.dataSourceDB.forEach(function (v, k) {
                    v.DueEndTime = new Date(newDate);
                })
            }
        }
    }
}
