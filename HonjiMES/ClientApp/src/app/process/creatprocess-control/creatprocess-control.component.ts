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

@Component({
  selector: 'app-creatprocess-control',
  templateUrl: './creatprocess-control.component.html',
  styleUrls: ['./creatprocess-control.component.css']
})
export class CreatprocessControlComponent implements OnInit, OnChanges {
    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() modval: any;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild('dataGrid2') dataGrid2: DxDataGridComponent;
    @ViewChild('myButton') myButton: DxButtonComponent;

    controller: string;
    formData: any;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    width: any;
    colCount: number;
    url: string;
    dataSourceDB: any[];
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
    ProcessLeadTime: any;
    ProcessTime: any;
    ProcessCost: any;
    ProducingMachine: any;
    Remark: any;
    DrawNo: any;
    Manpower: any;
    saveCheck: boolean;
    onCellPreparedLevel: any;

    constructor(private http: HttpClient, myservice: Myservice) {
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
        this.colCount = 4;
        this.url = location.origin + '/api';
        this.dataSourceDB = [];
        this.controller = '/OrderDetails';
        this.saveDisabled = true;
        this.modCheck = false;
        this.modName = 'new';

        this.ProcessLeadTime = null;
        this.ProcessTime = null;
        this.ProcessCost = null;
        this.ProducingMachine = '';

        this.CreateTimeDateBoxOptions = {
            onValueChanged: this.CreateTimeValueChange.bind(this)
        };
        this.NumberBoxOptions = { showSpinButtons: true, mode: 'number', min: 1, value: 1 };

        this.GetData('/Processes/GetProcesses').subscribe(
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
        this.GetData('/ProductBasics/GetProductBasicsAsc').subscribe(
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
        this.GetData('/Processes/GetWorkOrderNumber').subscribe(
            (s) => {
                if (s.success) {
                    this.formData = s.data;
                    this.formData.Count = 1;
                }
            }
        );

    }
    public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>(location.origin + '/api' + apiUrl);
    }
    ngOnInit() {
    }
    ngOnChanges() {
        // debugger;
        this.dataSourceDB = [];
        if (this.modval === 'new') {
            this.modName = 'new';
            this.modVisible = true;
            this.runVisible = false;
            this.saveDisabled = true;
            this.GetData('/Processes/GetWorkOrderNumber').subscribe(
                (s) => {
                    if (s.success) {
                        this.formData = s.data;
                        this.formData.Count = 1;
                    }
                }
            );
        } else if (this.itemkeyval != null) {
            this.modVisible = false;
            this.GetData('/Processes/GetProcessByWorkOrderId/' + this.itemkeyval).subscribe(
                (s) => {
                    if (s.success) {
                        this.modCheck = true; // 避免製程資訊被刷新
                        this.dataSourceDB = s.data.WorkOrderDetail;
                        this.formData.WorkOrderHeadId = s.data.WorkOrderHead.Id;
                        this.formData.WorkOrderNo = s.data.WorkOrderHead.WorkOrderNo;
                        this.formData.CreateTime = s.data.WorkOrderHead.CreateTime;
                        this.formData.ProductBasicId = s.data.WorkOrderHead.DataId;
                        this.formData.Count = s.data.WorkOrderHead.Count;
                        this.formData.MachineNo = s.data.WorkOrderHead.MachineNo;
                        // this.formData.Remarks = s.data[0].Remarks;
                        if (s.data.WorkOrderHead.Status === 0) {
                            this.runVisible = true;
                        } else {
                            this.runVisible = false;
                        }
                    }
                }
            );
        }
        this.onCellPreparedLevel = 0;
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
    CreateTimeValueChange = async function(e) {
        // this.formData = this.myform.instance.option('formData');
        if (this.formData.CreateTime != null) {
            this.CreateNumberInfoVal = new CreateNumberInfo();
            this.CreateNumberInfoVal.CreateNumber = this.formData.WorkOrderNo;
            this.CreateNumberInfoVal.CreateTime = this.formData.CreateTime;
            // tslint:disable-next-line: max-line-length
            const sendRequest = await SendService.sendRequest(this.http, '/Processes/GetWorkOrderNumberByInfo', 'POST', { values: this.CreateNumberInfoVal });
            if (sendRequest) {
                this.formData.WorkOrderNo = sendRequest.CreateNumber;
                this.formData.CreateTime = sendRequest.CreateTime;
            }
        }
    };
    onProductBasicSelectionChanged(e) {
        // debugger;
        if (this.modCheck) {
            this.modCheck = false;
        } else {
            this.saveDisabled = false;
            if (e.value !== 0 && e.value !== null && e.value !== undefined) {
                this.GetData('/BillOfMaterials/GetProcessByProductBasicId/' + e.value).subscribe(
                    (s) => {
                        if (s.success) {
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
        const today = new Date();
        this.ProcessBasicList.forEach(x => {
            if (x.Id === e.value) {
                this.ProcessLeadTime = x?.LeadTime ?? 0;
                this.ProcessTime = x?.WorkTime ?? 0;
                this.ProcessCost = x?.Cost ?? 0;
                this.ProducingMachine = x.ProducingMachine;
                this.Remark = x.Remark;
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
        this.ProcessLeadTime = 0;
        this.ProcessTime = 0;
        this.ProcessCost = 0;
        this.ProducingMachine = '';
        this.Remark = '';
        this.DrawNo = '';
        this.Manpower = 0;
    }
    onEditingStart(e) {
        this.saveCheck = false;
        this.onCellPreparedLevel = 1;
        this.ProcessLeadTime = e.data.ProcessLeadTime;
        this.ProcessTime = e.data.ProcessTime;
        this.ProcessCost = e.data.ProcessCost;
        this.ProducingMachine = e.data.ProducingMachine;
        this.Remark = e.data.Remark;
        this.DrawNo = e.data.DrawNo;
        this.Manpower = e.data.Manpower;
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
    DeleteOnClick(e) {
        this.modName = 'delete';
    }
    UpdateOnClick(e) {
        this.modName = 'update';
    }
    RunOnClick(e) {
        this.modName = 'run';
    }
    onFormSubmit = async function(e) {
        // debugger;
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
                DataType: 1,
                DataId: this.formData.ProductBasicId,
                Count: this.formData.Count,
                MachineNo: this.formData.MachineNo,
            },
            WorkOrderDetail: this.dataSourceDB
        };

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
                        const sendRequest = await SendService.sendRequest(this.http, '/Processes/DeleteWorkOrderList/' + this.formData.WorkOrderHeadId, 'DELETE');
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
            this.formData.CreateTime = new Date();
            this.formData.ProductBasicId = null;
            this.formData.Count = 1;
            this.formData.ProducingMachine = '';
            this.formData.Remarks = '';
            this.dataSourceDB = [];
            e.preventDefault();
            this.childOuter.emit(true);
        }
    }
}
