import { Component, OnInit, OnChanges, Output, Input, ViewChild, EventEmitter, HostListener, AfterViewInit } from '@angular/core';
import { DxFormComponent, DxDataGridComponent, DxButtonComponent } from 'devextreme-angular';
import { HttpClient } from '@angular/common/http';
import { Myservice } from 'src/app/service/myservice';
import { AppComponent } from 'src/app/app.component';
import { SendService } from 'src/app/shared/mylib';
import notify from 'devextreme/ui/notify';
import CheckBox from 'devextreme/ui/check_box';
import Swal from 'sweetalert2';
import { CreateNumberInfo } from 'src/app/model/viewmodels';
import CustomStore from 'devextreme/data/custom_store';
import { workOrderReportData } from 'src/app/model/viewmodels';
import { APIResponse } from 'src/app/app.module';

@Component({
    selector: 'app-editworkorder',
    templateUrl: './editworkorder.component.html',
    styleUrls: ['./editworkorder.component.css']
})
export class EditworkorderComponent implements OnInit, OnChanges, AfterViewInit {
    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() modval: any;
    @Input() randomkeyval: any;
    @Input() popupkeyval: any;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild('dataGrid2') dataGrid2: DxDataGridComponent;
    @ViewChild('myButton') myButton: DxButtonComponent;
    @ViewChild(DxButtonComponent, { static: false }) button: DxButtonComponent;

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
    buttondisabled = true;

    disabledValues: number[];

    CreateTimeDateBoxOptions: any;
    ProductBasicSelectBoxOptions: any;
    ProductBasicList: any;
    ProcessBasicList: any;
    MaterialBasicList: any;
    MaterialBasicSelectBoxOptions: any;
    ProcessLeadTime: any;
    ProcessTime: any;
    ProcessCost: any;
    ProducingMachine: any;
    Remarks: any;
    DrawNo: any;
    Manpower: any;
    DueStartTime: any;
    DueEndTime: any;
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
    Controller2 = '/Processes';
    WorkStatusList: any;
    listWorkOrderTypes: any;
    ReportHeight: number;
    mod: string;
    serialkey: number;
    creatpopupVisible: boolean;
    randomkey: number;
    itemkey: any;
    editorOptions: { showSpinButtons: boolean; mode: string; min: number; };
    UserList: any[];
    keyup = '';
    UserEditorOptions: { items: any; displayExpr: string; valueExpr: string; value: number; searchEnabled: boolean; disable: boolean; };
    MachineList: any;
    hasUser: boolean;
    stopVisible: boolean;
    SubmitVal: string;
    keyupEnter: boolean;
    HasPermission: boolean;
    productBasicChange: boolean;

    @HostListener('window:keyup', ['$event']) keyUp(e: KeyboardEvent) {
        if (this.popupkeyval && !this.creatpopupVisible) {
            if (e.key === 'Enter') {
                const key = this.keyup;
                if (key !== '') {
                    const promise = new Promise((resolve, reject) => {
                        this.app.GetData('/Users/GetUserByUserNo?DataNo=' + key).toPromise().then((res: APIResponse) => {
                            if (res.success) {
                                if (res.data.Permission === 80 || res.data.Permission === 20) {
                                    this.hasUser = true;
                                    this.app.GetData('/Users/GetUsers').subscribe(
                                        (s) => {
                                            if (s.success) {
                                                // this.buttondisabled = false;
                                                this.UserList = [];
                                                // 過濾帳戶身分。(因此畫面是使用共用帳戶，但登記人員必須是個人身分)
                                                s.data.forEach(element => {
                                                    if (element.Permission === 20 && element.Id === res.data.Id) {
                                                        this.stopVisible = true;
                                                        this.HasPermission = true;
                                                        this.UserList.push(element);
                                                        // s.data.forEach(element2 => {
                                                        //     if (element2.Permission === 80) {
                                                        //         this.UserList.push(element2);
                                                        //     }
                                                        // });
                                                    } else if (element.Permission === 80 && element.Id === res.data.Id) {
                                                        this.stopVisible = false;
                                                        this.HasPermission = false;
                                                        this.UserList.push(element);
                                                    }
                                                });
                                                this.disabledValues = [];
                                                this.dataGrid2.instance.clearSelection();
                                                this.dataGrid2.instance.refresh();
                                                this.SetUserEditorOptions(this.UserList, res.data.Id);
                                            }
                                        }
                                    );
                                } else {
                                    this.hasUser = false;
                                    this.UserList = [];
                                    this.SetUserEditorOptions(this.UserList, null);
                                    this.showMessage('warning', '請勿越權使用!', 3000);
                                }
                            } else {
                                this.showMessage('error', '查無資料!', 3000);
                            }
                        },
                            err => {
                                // Error
                                reject(err);
                            }
                        );
                    });
                }
                this.keyup = '';
            } else if (e.key === 'Shift' || e.key === 'CapsLock') {

            } else {
                this.keyup += e.key.toLocaleUpperCase();
            }
        }
    }

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent) {
        this.listWorkOrderTypes = myservice.getWorkOrderTypes();
        this.onReorder = this.onReorder.bind(this);
        this.onRowRemoved = this.onRowRemoved.bind(this);
        this.validateNumber = this.validateNumber.bind(this);
        // this.CustomerVal = null;
        // this.formData = null;
        this.stopVisible = false;
        this.editOnkeyPress = true;
        this.enterKeyAction = 'moveFocus';
        this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 300;
        this.colCount = 5;
        this.dataSourceDB = [];
        this.saveDisabled = true;
        this.modCheck = false;
        this.modName = 'new';
        this.saveCheck = true;
        this.WorkStatusList = myservice.getWorkOrderStatus();
        this.disabledValues = [];

        this.editorOptions = { showSpinButtons: true, mode: 'number', min: 0 };
        this.CreateTimeDateBoxOptions = {
            onValueChanged: this.CreateTimeValueChange.bind(this)
        };

        this.app.GetData('/Machines/GetMachines').subscribe(
            (s) => {
                if (s.success) {
                    if (s.success) {
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
        // this.app.GetData('/MaterialBasics/GetMaterialBasicsAsc').subscribe(
        //     (s) => {
        //         if (s.success) {
        //             this.MaterialBasicList = s.data;
        //             this.MaterialBasicList.forEach(x => {
        //                 x.Name = x.MaterialNo + '_' + x.Name;
        //             });
        //             this.MaterialBasicSelectBoxOptions = {
        //                 dataSource: { paginate: true, store: { type: 'array', data: this.MaterialBasicList, key: 'Id' } },
        //                 searchEnabled: true,
        //                 displayExpr: 'Name',
        //                 valueExpr: 'Id',
        //                 onValueChanged: this.onMaterialBasicSelectionChanged.bind(this)
        //             };

        //         }
        //     }
        // );
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
        this.app.GetData('/Machines/GetMachines').subscribe(
            (s) => {
                if (s.success) {
                    if (s.success) {
                        s.data.unshift({Id: null, Name: '<無>'}); // 加入第一行
                        this.MachineList = s.data;
                    }
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
        this.stopVisible = false;
        this.hasUser = false;
        this.disabledValues = [];
        // this.GetProcessInfo();
        this.modVisible = false;
        this.app.GetData('/Processes/GetProcessByWorkOrderId/' + this.itemkeyval.Key).subscribe(
            (s) => {
                if (s.success) {
                    this.modCheck = true; // 避免製程資訊被刷新
                    this.dataSourceDB = s.data.WorkOrderDetail;
                    this.formData.WorkOrderHeadId = s.data.WorkOrderHead.Id;
                        this.formData.WorkOrderNo = s.data.WorkOrderHead.WorkOrderNo;
                        this.formData.CreateTime = s.data.WorkOrderHead.CreateTime;
                        this.formData.MaterialBasicId = s.data.WorkOrderHead.DataId;
                        this.formData.Count = s.data.WorkOrderHead.Count;
                        this.formData.OrderCount = s.data.WorkOrderHead.OrderCount;
                        this.formData.DrawNo = s.data.WorkOrderHead.DrawNo;
                        this.formData.MachineNo = s.data.WorkOrderHead.MachineNo;
                        this.formData.DueStartTime = s.data.WorkOrderHead.DueStartTime;
                        this.formData.DueEndTime = s.data.WorkOrderHead.DueEndTime;
                        this.formData.DataNo = s.data.WorkOrderHead.DataNo;
                        this.NumberBoxOptions = { showSpinButtons: true, mode: 'number', min: 1, value: s.data.WorkOrderHead.Count };
                }
            }
        );

        // this.buttondisabled = true;
        // this.UserList = [];
        // this.SetUserEditorOptions(this.UserList, null);

        //// 測試用暫時加入，可選人員
        this.app.GetData('/Users/GetUsers').subscribe(
            (s) => {
                if (s.success) {
                    // this.buttondisabled = false;
                    this.UserList = [];
                    // 過濾帳戶身分。(因此畫面是使用共用帳戶，但登記人員必須是個人身分)
                    s.data.forEach(element => {
                        if (element.Permission === 20 || element.Permission === 30 || element.Permission === 40 ||
                            element.Permission === 50 || element.Permission === 60 || element.Permission === 70 ||
                            element.Permission === 80) {
                            this.UserList.push(element);
                        }
                    });
                    this.SetUserEditorOptions(this.UserList, null);
                }
            }
        );

        this.HasPermission = false;
        this.onCellPreparedLevel = 0;
    }
    ngAfterViewInit() {
        this.button.instance.registerKeyHandler('enter', function(e) {
            // console.log(this.keyupEnter);
            this.keyupEnter = true;
        });
    }
    // GetProcessInfo() {
    //     this.dataSourceDB = this.app.GetData('/Processes/GetProcessByWorkOrderDetail/' + this.itemkeyval.Key).subscribe(
    //         (s) => {
    //             if (s.success) {
    //                 s.data.forEach(e => {
    //                     e.ReportCount = null;
    //                     e.NgCount = e.NgCount !== 0 ? e.NgCount : null;
    //                 });
    //                 this.dataSourceDB = s.data;
    //                 console.log('dataSourceDB',this.dataSourceDB);
    //             }
    //         }
    //     );
    // }
    SetUserEditorOptions(List, IdVal) {
        this.UserEditorOptions = {
            items: List,
            displayExpr: 'Realname',
            valueExpr: 'Id',
            value: IdVal,
            searchEnabled: true,
            disable: false
        };
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
    };
    // onMaterialBasicSelectionChanged(e) {
    //     if (this.modCheck) {
    //         this.modCheck = false;
    //     } else {
    //         this.saveDisabled = false;
    //         if (e.value !== 0 && e.value !== null && e.value !== undefined) {
    //             this.app.GetData('/BillOfMaterials/GetProcessByMaterialBasicId/' + e.value).subscribe(
    //                 (s) => {
    //                     if (s.success) {
    //                         s.data.forEach(element => {
    //                             element.Id = 0;
    //                             element.DueStartTime = new Date();
    //                             element.DueEndTime = new Date();
    //                         });
    //                         this.dataSourceDB = s.data;
    //                         this.productBasicChange = true;
    //                     }
    //                 }
    //             );
    //         }
    //     }
    // }
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


    onEditingStart(e) {
        this.saveCheck = false;
        this.onCellPreparedLevel = 1;


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
    editorPreparing(e) {
        if (e.parentType === 'dataRow' && (e.dataField === 'ReportCount' || e.dataField === 'ReportNgCount')) {
            e.editorOptions.readOnly = true;
            const SelectedRows = this.dataGrid2.instance.getSelectedRowsData().find(x => x.Id === e.row.data.Id);
            if (SelectedRows) {
                e.editorOptions.readOnly = e.row.data.Status !== 2;
            }
        }
    }
    onCellPrepared(e: any) {
        if (e.rowType === 'data' && e.column.command === 'select') {
            if (e.data.Type === 1 || // 種類為委外
                (e.data.Type === 2 && e.data.Status === 2) || // 種類為委外(工單委外) // 已暫停使用
                (e.data.Status === 7 && !this.HasPermission) // 狀態為暫停
                ) {
                const instance = CheckBox.getInstance(e.cellElement.querySelector('.dx-select-checkbox'));
                instance.option('disabled', true);
                this.disabledValues.push(e.data.Id);
            }
        }
    }
    onSelectionChanged(e) {// CheckBox disabled還是會勾選，必須清掉，這是官方寫法

        const disabledKeys = e.currentDeselectedRowKeys.filter(i => this.disabledValues.indexOf(i) > -1);
        if (disabledKeys.length > 0) {
            e.component.deselectRows(disabledKeys);
        }
        if (e.currentDeselectedRowKeys.length !== 0) {
            e.currentDeselectedRowKeys.forEach(element => {
                const processData = this.dataSourceDB.find(z => z.Id === element);
                processData.ReportCount = null;
                processData.ReportNgCount = null;
                processData.Message = null;
            });
        }
        if (e.currentSelectedRowKeys.length !== 0) {
            e.currentSelectedRowKeys.forEach(element => {
                const processData = this.dataSourceDB.find(z => z.Id === element && z.Status === 2);
                if (processData !== undefined) {
                    const resultCount = (this.formData.Count - (processData?.ReCount ?? 0));
                    processData.ReportCount = resultCount >= 0 ? resultCount : 0;
                    processData.ReportNgCount = 0;
                }
            });
        }
    }
    onRowClick(e) {
        this.itemkey = e.data.WorkOrderHeadId;
        this.serialkey = e.data.SerialNumber;
        this.mod = 'report';
        this.randomkey = new Date().getTime();
        if (e.data.Type === 1) { // 委外(含採購單)
            this.creatpopupVisible = true;
            if (e.data.Status === 3) {
                this.ReportHeight = 800;
                // this.ReportByPurchaseNo(e.data.WorkOrderHeadId, e.data.SerialNumber);
            } else {
                this.ReportHeight = 800;
                // this.ReportByPurchaseNo(e.data.WorkOrderHeadId, e.data.SerialNumber);
            }
        } else if (e.data.Type === 2 && e.data.Status === 2) { // 委外(無採購單)
            this.creatpopupVisible = true;
            this.ReportHeight = 870;
        } else {
            const arr =  this.dataGrid2.instance.getSelectedRowKeys();
            if (e.isSelected) {
                const index = arr.indexOf(e.data.Id, 0);
                if (index > -1) {
                    arr.splice(index, 1);
                }
            } else {
                arr.push(e.data.Id);
            }
            e.component.selectRows(arr);
        }
    }
    onToolbarPreparing(e) {
        const toolbarItems = e.toolbarOptions.items;
        e.toolbarOptions.visible = false;
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
    onRowPrepared(e) {
        if (e.data !== undefined) {
            if (e.data.Status === 2) {
                e.rowElement.style.backgroundColor = '#f8f691';
            }
        }
    }
    validateNumber(e) {
        const SelectedRows = this.dataGrid2.instance.getSelectedRowsData().find(x => x.Id === e.data.Id);
        if (SelectedRows) {
            if (e.data.Status === 2 && e.value < 0) {
                this.buttondisabled = true;
                return false;
            } else {
                if (this.hasUser) {
                    // this.buttondisabled = false;
                }
            }
        }
        return true;
    }
    onStartClick(e) {
        this.SubmitVal = 'start';
    }
    onStopClick(e) {
        this.SubmitVal = 'stop';
    }
    onReportClick(e) {
        this.SubmitVal = 'report';
    }
    UpdateOnClick(e){
        this.dataGrid2.instance.saveEditData();
        this.SubmitVal = 'update';
        this.saveCheck = true;
    }
    CancelOnClick(e){
        this.SubmitVal = 'cancel';
    }
    ReportByPurchaseNo(workOrderHeadId, serial) {
        Swal.fire({
            title: '請輸入採購單號!',
            input: 'text',
            inputAttributes: {
                autocapitalize: 'off'
            },
            showCancelButton: true,
            confirmButtonColor: '#296293',
            cancelButtonColor: '#CE312C',
            confirmButtonText: '確認',
            cancelButtonText: '取消',
            showLoaderOnConfirm: true,
            preConfirm: (purchaseNo) => {
                return this.app.GetData('/PurchaseHeads/GetPurchasesByPurchaseNo?DataNo=' + purchaseNo).toPromise()
                .then(response => {
                    if (!response.success) {
                        // throw new Error(response.message);
                        Swal.showValidationMessage(response.message);
                    }
                    return {purchaseId: response.data, purchaseNo};
                })
                .catch(error => {
                    Swal.showValidationMessage(
                        `Request failed: ${error}`
                    );
                });
            },
            allowOutsideClick: () => !Swal.isLoading()
        }).then(async (result) => {
            if (result.value) {
                const Data = new workOrderReportData();
                Data.WorkOrderID = workOrderHeadId;
                Data.WorkOrderSerial = serial;
                Data.PurchaseId = result.value.purchaseId;
                Data.PurchaseNo = result.value.purchaseNo;
                // tslint:disable-next-line: max-line-length
                const sendRequest = await SendService.sendRequest(this.http, '/WorkOrders/ReportWorkOrderByPurchase', 'PUT', { key: workOrderHeadId, values: Data });
                // let data = this.client.POST( this.url + '/OrderHeads/PostOrderMaster_Detail').toPromise();
                if (sendRequest) {
                    this.dataGrid2.instance.refresh();
                    notify({
                        message: '更新成功',
                        position: {
                            my: 'center top',
                            at: 'center top'
                        }
                    }, 'success', 2000);
                }
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
    onFormSubmit = async function(e) {
        // console.log(this.keyupEnter);
        // if (this.keyupEnter) {
        //     this.keyupEnter = false;
        //     return;
        // }

        this.formData = this.myform.instance.option('formData');
        let saveok = true;
        let cansave = true;
        const saveEditData = this.dataGrid2.instance.saveEditData();
        const SelectedRows = this.dataGrid2.instance.getSelectedRowsData();
        console.log('SelectedRows',SelectedRows);
        this.postval = {
            WorkOrderHead: {
                // Id: this.itemkeyval.Key,
                Id: this.formData.WorkOrderHeadId,
                WorkOrderNo: this.formData.WorkOrderNo,
                CreateTime: this.formData.CreateTime,
                // DataType: 2,
                DataId: this.formData.MaterialBasicId,
                Count: this.formData.Count,
                OrderCount : this.formData.OrderCount,
                DrawNo: this.formData.DrawNo,
                MachineNo: this.formData.MachineNo,
                DueStartTime: this.formData.DueStartTime,
                DueEndTime: this.formData.DueEndTime
            },
            WorkOrderDetail: this.dataSourceDB
        };
        if(this.SubmitVal === 'update') {
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
        }
        else if (SelectedRows.length === 0) {
            Swal.fire({
                allowEnterKey: false,
                allowOutsideClick: false,
                title: '沒有勾選任何項目',

                html: '請勾選要回報的項目',
                icon: 'warning',
                timer: 3000
            });
        } else {
            // if (this.validate_before() === false) {
            //     this.buttondisabled = false;
            //     return;
            // }

            if (this.SubmitVal === 'stop' || this.SubmitVal === 'start') { // 回報 [工序暫停]/[回復加工]
                const reportResult = await this.ReportStopOrStart(SelectedRows);
                this.dataGrid2.instance.refresh();
                if (reportResult) {
                    this.dataGrid2.instance.clearSelection();
                    e.preventDefault();
                    this.childOuter.emit(true);
                }
            } else if (this.SubmitVal === 'report') {
                // tslint:disable-next-line: forin
                for (const x in SelectedRows) {
                    if (SelectedRows[x].Status === 2 && (SelectedRows[x].ReportCount <= 0 || SelectedRows[x].ReportCount == null)) {
                        const msg = SelectedRows[x].ProcessNo + SelectedRows[x].ProcessName + '：回報數量 必填! (不能為 0)';
                        notify({
                            message: msg,
                            position: {
                                my: 'center top',
                                at: 'center top'
                            }
                        }, 'error');
                        cansave = false;
                        return;
                    }
                }
                if (cansave) {
                    // tslint:disable-next-line: forin
                    for (const x in SelectedRows) {
                        SelectedRows[x].CreateUser = this.formData.CreateUser;
                        const sendRequest = await SendService.sendRequest(
                            this.http, this.Controller + '/WorkOrderReportAll', 'PUT',
                            { key: SelectedRows[x].Id, values: SelectedRows[x] });
                        if (sendRequest) {
                            this.dataGrid2.instance.clearSelection();
                        } else {
                            saveok = false;
                        }
                    }
                    this.dataGrid2.instance.refresh();
                    if (saveok) {
                        e.preventDefault();
                        this.childOuter.emit(true);
                    }
                }
            }
        }
    }
    creatpopup_result(e) {
        this.creatpopupVisible = false;
        // this.dataGrid2.instance.refresh();
        this.disabledValues = [];
        // this.GetProcessInfo();
        notify({
            message: '更新完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
    }
    showMessage(type, data, val) {
        notify({
            message: data,
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, type, val);
    }
    async ReportStopOrStart(SelectedRows) {
        let cansave = true;
        // tslint:disable-next-line: forin
        for (const x in SelectedRows) {
            if (SelectedRows[x].Status !== 2 && this.SubmitVal === 'stop') {
                if (SelectedRows[x].Status === 7) {
                    this.showMessage('warning', '[ ' + SelectedRows[x].ProcessNo + '_' + SelectedRows[x].ProcessName + ' ] 該工序已經暫停!', 3000);
                } else {
                    this.showMessage('warning', '[ ' + SelectedRows[x].ProcessNo + '_' + SelectedRows[x].ProcessName + ' ] 該工序尚未開工!', 3000);
                }
                cansave = false;
                return false;
            }
            if (SelectedRows[x].Status !== 7 && this.SubmitVal === 'start') {
                this.showMessage('warning', '[ ' + SelectedRows[x].ProcessNo + '_' + SelectedRows[x].ProcessName + ' ] 該工序不是[暫停]狀態!', 3000);
                cansave = false;
                return false;
            }
        }
        if (cansave) {
            let saveok = true;
            // tslint:disable-next-line: forin
            for (const x in SelectedRows) {
                SelectedRows[x].CreateUser = this.formData.CreateUser;
                const sendRequest = await SendService.sendRequest(
                    this.http, this.Controller + '/WorkOrderReportAllStopOrStart', 'PUT',
                    { key: SelectedRows[x].Id, values: SelectedRows[x] });
                if (sendRequest) {

                } else {
                    saveok = false;
                }
            }
            return saveok;
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
    Countchange(e) {
        this.dataGrid2.instance.saveEditData();
        this.dataSourceDB.forEach(item => {
            item.ExpectedlTotalTime = (item.ProcessLeadTime + item.ProcessTime) * e.value;
        });
        this.dataGrid2.instance.refresh();
    }
}
