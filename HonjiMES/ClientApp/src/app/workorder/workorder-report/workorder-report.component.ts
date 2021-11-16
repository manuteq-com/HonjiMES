import { WorkOrderHead } from './../../model/viewmodels';
import { Component, OnInit, Input, Output, EventEmitter, ViewChild, OnChanges, HostListener } from '@angular/core';
import { DxDataGridComponent, DxFormComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { SendService } from 'src/app/shared/mylib';
import { HttpClient } from '@angular/common/http';
import { APIResponse } from 'src/app/app.module';
import { Observable } from 'rxjs';
import { HubMessage, workOrderReportData } from 'src/app/model/viewmodels';
import { AppComponent } from 'src/app/app.component';
import { Myservice } from 'src/app/service/myservice';
import Swal from 'sweetalert2';
import { analyzeAndValidateNgModules } from '@angular/compiler';


@Component({
    selector: 'app-workorder-report',
    templateUrl: './workorder-report.component.html',
    styleUrls: ['./workorder-report.component.css']
})
export class WorkorderReportComponent implements OnInit, OnChanges {
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild('dataGrid1') dataGrid1: DxDataGridComponent;
    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() serialkeyval: any;
    @Input() modval: any;
    @Input() randomkey: any;
    @Input() popupkeyval: any;
    buttondisabled = false;
    CustomerVal: any;
    formData: any;
    postval: any;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    width: any;
    colCount: number;
    labelLocation: string;
    QuantityEditorOptions: any;
    NgEditorOptions: any;
    NcEditorOptions: any;
    PriceEditorOptions: any;
    UnitCountEditorOptions: any;
    UnitPriceEditorOptions: any;
    ProcessesList: any;

    itemval1: string;
    itemval3: string;
    itemval2: string;
    itemval4: string;
    itemval5: string;
    itemval6: string;
    itemval7: string;
    itemval8: string;
    itemval9: string;
    itemval10: string;
    itemval11: string;
    itemval12: string;
    itemval13: string;
    itemval14: string;
    itemval15: string;
    itemval16: string;
    itemval17: string;
    itemval18: string;
    itemval19: string;
    itemval20: string;
    itemval21: string;

    value: any;
    ReCount: any;
    NgCount: any;
    ProcessEditorOptions: any;
    startBtnVisible: boolean;
    endBtnVisible: boolean;
    restartedBtnVisible: boolean;
    ReCountVisible: boolean;
    NgCountVisible: boolean;
    RemarkVisible: boolean;
    DateBoxOptions: any;
    NoPurchaseVisible: any;
    selectBoxOptions: any;
    HasPurchaseVisible: boolean;
    HasStockVisible: boolean;
    PurchaseHeadList: any;
    editorOptions: any;
    creatpopupVisible: boolean;
    stockpopupVisible: boolean;
    itemkey: any;
    mod: any;
    dataSource: any[];
    ProductBasicList: any;
    UserEditorOptions: { items: any; displayExpr: string; valueExpr: string; value: any; searchEnabled: boolean; disable: boolean; };
    UserList: any;
    keyup = '';
    MachineEditorOptions: { items: any; displayExpr: string; valueExpr: string; searchEnabled: boolean; readOnly: boolean };
    HasProducingMachineVisible: boolean;
    HasCodeNoVisible: boolean;
    QcTypeList: any;
    QcResultList: any;
    selectQcType: { items: any; displayExpr: string; valueExpr: string; searchEnabled: boolean; value: any; };
    selectQcResult: { items: any; displayExpr: string; valueExpr: string; searchEnabled: boolean; value: any; };
    CountEditorOptions: any;
    HasQCVisible: boolean;
    dataSourceDB: any;
    autoNavigateToFocusedRow = true;
    remoteOperations: boolean;
    NcCountVisible: boolean;
    workorderkey: any;
    MCounteditorOptions: { max: any; min: number; };
    maxMCount: any;


    @HostListener('window:keyup', ['$event']) keyUp(e: KeyboardEvent) {
        if (this.popupkeyval) {
            if (e.key === 'Enter') {
                const key = this.keyup;
                if (key !== '') {
                    const promise = new Promise((resolve, reject) => {
                        this.app.GetData('/Users/GetUserByUserNo?DataNo=' + key).toPromise().then((res: APIResponse) => {
                            if (res.success) {
                                let PermissionTemp = 80; // 生產人員
                                if (this.formData.Type === 1) {
                                    PermissionTemp = 60; // 生管人員
                                }
                                if (res.data.Permission === PermissionTemp || res.data.Permission === 20) {
                                    this.app.GetData('/Users/GetUsers').subscribe(
                                        (s) => {
                                            if (s.success) {
                                                this.buttondisabled = false;
                                                this.UserList = [];
                                                // 過濾帳戶身分。(因此畫面是使用共用帳戶，但登記人員必須是個人身分)
                                                s.data.forEach(element => {
                                                    if (element.Permission === 20 && element.Id === res.data.Id) {
                                                        this.UserList.push(element);
                                                        s.data.forEach(element2 => {
                                                            if (element2.Permission === PermissionTemp) {
                                                                this.UserList.push(element2);
                                                            }
                                                        });
                                                    } else if (element.Permission === PermissionTemp && element.Id === res.data.Id) {
                                                        this.UserList.push(element);
                                                    }
                                                });
                                                this.SetUserEditorOptions(this.UserList, res.data.Id);
                                            }
                                        }
                                    );
                                } else {
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
        this.QcTypeList = myservice.getQcType();
        this.QcResultList = myservice.getQcResult();
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 300;
        this.colCount = 4;
        this.labelLocation = 'left';
        this.remoteOperations = true;
        // this.keyDown = this.keyDown.bind(this)

        this.DateBoxOptions = {
            displayFormat: 'yyyy/MM/dd HH:mm:ss',
            // onValueChanged: this.PurchaseDateValueChange.bind(this)
        };
        this.PriceEditorOptions = {
            showSpinButtons: true,
            mode: 'number',
            // onValueChanged: this.PriceValueChanged.bind(this)
        };
        this.selectQcType = {
            items: this.QcTypeList,
            displayExpr: 'Name',
            valueExpr: 'Id',
            searchEnabled: true,
            value: null
        };
        this.selectQcResult = {
            items: this.QcResultList,
            displayExpr: 'Name',
            valueExpr: 'Id',
            searchEnabled: true,
            value: null
        };
        this.CountEditorOptions = {
            showSpinButtons: true,
            mode: 'number',
            // format: '#0',
            value: '0',
            min: '0',
            // onValueChanged: this.QuantityValueChanged.bind(this)
        };
    }
    ngOnInit() {
    }
    // keyDown(e) {
    //     debugger;
    //     const event = e.event;
    //     const str = event.key || String.fromCharCode(event.which);
    //     if(/^[.,e]$/.test(str)) {
    //         event.preventDefault();
    //     }
    // }
    ngOnChanges() {
        this.HasCodeNoVisible = false;
        this.ReCountVisible = false;
        this.NgCountVisible = false;
        this.RemarkVisible = false;
        this.startBtnVisible = false;
        this.endBtnVisible = false;
        this.restartedBtnVisible = false;
        this.NoPurchaseVisible = false;
        this.HasPurchaseVisible = false;
        this.HasStockVisible = false;
        this.HasProducingMachineVisible = false;
        this.HasQCVisible = false;
        this.PurchaseHeadList = [];
        this.NcCountVisible = false;
        this.UserList = [];


        this.app.GetData('/Processes/GetProcesses').subscribe(
            (s) => {
                if (s.success) {
                    s.data.forEach(element => {
                        element.Name = element.Code + '_' + element.Name;
                    });
                    this.ProcessEditorOptions = {
                        dataSource: { paginate: true, store: { type: 'array', data: s.data, key: 'Id' } },
                        searchEnabled: true,
                        // items: this.MaterialList,
                        displayExpr: 'Name',
                        valueExpr: 'Id',
                        // onValueChanged: this.ProcessEditorOptions.bind(this)
                    };
                }
            }
        );
        this.app.GetData('/Suppliers/GetSuppliers').subscribe(
            (s) => {
                if (s.success) {
                    s.data.forEach(x => {
                        x.Name = x.Code + '_' + x.Name;
                    });
                    this.selectBoxOptions = {
                        items: s.data,
                        displayExpr: 'Name',
                        valueExpr: 'Id',
                        searchEnabled: true,
                        onValueChanged: this.onSupplierSelectionChanged.bind(this)
                    };
                }
            }
        );

        this.app.GetData('/Machines/GetMachines').subscribe(
            (s) => {
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
                    s.data.unshift({ Id: null, Name: '<無>' }); // 加入第一行
                    this.MachineEditorOptions = {
                        items: s.data,
                        displayExpr: 'Name',
                        valueExpr: 'Name',
                        searchEnabled: true,
                        readOnly: true,
                    };
                }
            }
        );

        // this.app.GetData('/PurchaseHeads/GetNotEndPurchaseHeads').subscribe(
        //     (s) => {
        //         if (s.success) {
        //             this.PurchaseHeadList = s.data;
        this.editorOptions = {
            dataSource: { paginate: true, store: { type: 'array', data: this.PurchaseHeadList, key: 'Id' } },
            searchEnabled: true,
            displayExpr: 'PurchaseNo',
            valueExpr: 'Id'
        };
        //         }
        //     }
        // );

        this.NgEditorOptions = {
            showSpinButtons: true,
            mode: 'number',
            // format: '#0',
            value: '0',
            min: '0',
            // onValueChanged: this.QuantityValueChanged.bind(this)
        };
        this.NcEditorOptions = {
            showSpinButtons: true,
            mode: 'number',
            // format: '#0',
            value: '0',
            min: '0',
            // onValueChanged: this.QuantityValueChanged.bind(this)
        };
        if (this.itemkeyval != null) {
            this.dataSourceDB = [];
            this.app.GetData('/WorkOrders/GetBomDataByWorkOrderId/' + this.itemkeyval).subscribe(
                (s) => {
                    if (s.success) {
                        let index = 1;
                        s.data.forEach(element => {
                            element.Serial = index++;
                        });
                        this.dataSourceDB = s.data;
                    }
                }
            );
            this.app.GetData('/Processes/GetProcessByWorkOrderId/' + this.itemkeyval).subscribe(
                (s) => {
                    // debugger;
                    if (s.success) {
                        this.itemval1 = '　　　　　工單號：　' + s.data.WorkOrderHead.WorkOrderNo;
                        this.itemval2 = '　　　　　　品號：　' + s.data.WorkOrderHead.DataNo;
                        this.itemval3 = '　　　　　　名稱：　' + s.data.WorkOrderHead.DataName;
                        this.itemval19 = '　　　　訂單數量：　' + (s.data.WorkOrderHead.OrderDetail?.Quantity ?? '0');
                        this.itemval4 = '　　　領料總數量：　' + s.data.ReceiveQuantity;
                        // this.itemval4 = '　　　　　　機號：　' + (s.data.WorkOrderHead?.MachineNo ?? '');
                        // this.itemval5 = '　　　　　預計／實際完工數量：　' + s.data.WorkOrderHead.Count + ' / ' + s.data.WorkOrderHead.ReCount;
                        this.itemval5 = '';
                        console.log("s.data",s.data);
                        let findProcess = false;
                        s.data.WorkOrderDetail.forEach(element => {
                            if (element.SerialNumber === this.serialkeyval && findProcess === false) {
                                this.itemval7 = '　　　　製程序號：　' + element.SerialNumber;
                                this.itemval8 = '　　　　製程名稱：　[' + element.ProcessNo + '] ' + element.ProcessName;
                                this.itemval9 = '　　　　　　圖號：　' + (element?.DrawNo ?? '');
                                this.itemval10 = '　　　　所需人力：　' + (element?.Manpower ?? '');
                                this.itemval11 = '　　　　　　備註：　' + (element?.Remarks ?? '');
                                this.itemval12 = '　前置時間（分）：　' + (element?.ProcessLeadTime ?? '');
                                this.itemval13 = '　標準工時（分）：　' + (element?.ProcessTime ?? '');
                                // this.itemval14 = '　　　預計開工日：　' + (element?.DueStartTime ?? '');
                                this.itemval14 = '';
                                this.itemval15 = '　　　預計完工日：　' + (element?.DueEndTime ?? '');
                                this.itemval16 = '　　　實際開工日：　' + (element?.ActualStartTime ?? '');
                                this.itemval17 = '　　　實際完工日：　' + (element?.ActualEndTime ?? '');
                                this.itemval18 = '　　　　工單數量：　' + (element?.Count ?? '0');
                                this.itemval20 = '　　　　完工數量：　' + (element?.ReCount ?? '0') + '　　( NG數量：' + element?.NgCount + ' )';
                                this.itemval6 = '';
                                this.itemval21 = '　　　可製造數量：　' + (element?.AvailableMCount ?? '0');
                                const reCount = (element?.Count ?? '0') - (element?.ReCount ?? '0');
                                this.QuantityEditorOptions = {
                                    showSpinButtons: true,
                                    mode: 'number',
                                    // format: '#0',
                                    value: reCount > 0 ? reCount : 0,
                                    min: '0',
                                    // onValueChanged: this.QuantityValueChanged.bind(this)
                                };

                                this.formData = element;
                                //console.log("this.formData",this.formData);
                                this.formData.ReCount = reCount > 0 ? reCount : 0;
                                this.formData.NgCount = 0;
                                this.formData.NcCount = 0;
                                this.formData.MCount = element?.MCount ?? '0';
                                this.setUserList(this.formData.CreateUser);
                                findProcess = true;
                                this.maxMCount = element?.AvailableMCount ?? 0;
                                //this.MCounteditorOptions = { max: element?.AvailableMCount ?? 0 , min:0};
                                // this.ReCount = this.formData.ReCount;
                                // this.NgCount = this.formData.NgCount;
                                // $('#numberBox').dxNumberBox('setValue', this.formData.ReCount);
                                // $('#numberBox2').dxNumberBox('setValue', this.formData.NgCount);
                                // if(this.formData.Count != 0 || this.formData.Count != null){
                                //     this.value = this.formData.Count;
                                // } else if(this.formData.Count == 0 || this.formData.Count == null) {
                                //     this.value = this.formData.MCount
                                // }
                                // 依照製程種類決定顯示報工畫面
                                if (element.ProcessType === 20) { // QC檢驗
                                    this.ShowQCReportView(element.Status, element.Type);
                                } else if (element.ProcessType === null || element.ProcessType === 10) {
                                    //debugger;
                                    this.ShowNCReportView(element.Status, element.Type);
                                }

                                //status =1 可填寫機台
                                if (element.Status == 1) {
                                    this.MachineEditorOptions.readOnly = false;
                                }
                            }
                        });

                    }
                }
            );
        }
    }

    //人員清單
    setUserList(id){
        //// 測試用暫時加入，可選人員
        this.app.GetData('/Users/GetUsers').subscribe(
            (s) => {
                // debugger;
                if (s.success) {
                    this.buttondisabled = false;
                    this.UserList = [];
                    // 過濾帳戶身分。(因此畫面是使用共用帳戶，但登記人員必須是個人身分)
                    s.data.forEach(element => {
                        if (element.Permission === 20 || element.Permission === 30 || element.Permission === 40 ||
                            element.Permission === 50 || element.Permission === 60 || element.Permission === 70 ||
                            element.Permission === 80) {
                            this.UserList.push(element);
                        }
                    });
                    this.SetUserEditorOptions(this.UserList, id);
                }
            }
        );
    }

    SetUserEditorOptions(List, IdVal) {
        // debugger;
        this.UserEditorOptions = {
            items: List,
            displayExpr: 'Realname',
            valueExpr: 'Id',
            value: IdVal,
            searchEnabled: true,
            disable: false
        };
    }
    ShowQCReportView(Status, Type) { // QC報工畫面
        if (Status === 1) { // 未開工
            this.startBtnVisible = true;
        } else if (Status === 2) { // 已開工
            this.endBtnVisible = true;
            this.HasQCVisible = true;
        } else if (Status === 3 || Status === 4 || Status === 6) {
            this.restartedBtnVisible = true;
            this.HasStockVisible = true;//新增入庫單按鈕
        }
    }
    ShowNCReportView(Status, Type) { // NC報工畫面
        this.HasCodeNoVisible = true;
        this.HasProducingMachineVisible = true;
        if (Status === 1) { // 未開工
            // 如工序為委外(有採購單)，則需選擇採購單or新建採購單
            if (Type === 1) {
                this.ReCountVisible = true;
                this.HasPurchaseVisible = true;
            } else {
                this.startBtnVisible = true;
            }
        } else if (Status === 2) { // 已開工
            this.ReCountVisible = true;
            this.NgCountVisible = true;
            this.RemarkVisible = true;
            this.endBtnVisible = true;
            this.NcCountVisible = true;

            // 可重複報開工 2020/09/09
            this.restartedBtnVisible = false;//再開工回報定義不清，功能隱藏

            // 如工序為委外(無採購單)，則需填入供應商、金額
            if (Type === 2) {
                this.NoPurchaseVisible = true;
            }
        } else if (Status === 3 || Status === 4 || Status === 6) {
            // 如工序為委外(有採購單)，則需選擇採購單or新建採購單
            if (Type === 1) {
                this.ReCountVisible = true;
                this.HasPurchaseVisible = true;
            } else {
                this.restartedBtnVisible = false;//再開工回報定義不清，功能隱藏
                this.HasStockVisible = true;//新增入庫單按鈕
                this.RemarkVisible = true;

                // 因重複報開工，所以重複完工 2020/09/09
                this.endBtnVisible = false;
                this.ReCountVisible = true;
                this.NgCountVisible = true;
                this.NcCountVisible = true;
            }
        }
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
    onEditorPreparing(e) {
    }
    onToolbarPreparing(e) {
        e.toolbarOptions.visible = false;
    }
    onValueChanged(e) {
    }
    PurchaseNoValueChanged(e) {

    }
    onStartClick(e) {
        this.modval = 'start';
    }
    onEndClick(e) {
        this.modval = 'end';
    }
    onRestartClick(e) {
        this.modval = 'restart';
    }
    onPurchaseClick(e) {
        this.modval = 'purchase';
    }
    onStockClick(e) {
        // debugger;
        this.modval = 'stock';
    }
    onNewPurchaseClick(e) {
        this.modval = 'newpurchase';
    }
    creatpopup_result(e) {
        this.creatpopupVisible = false;
        this.showMessage('success', '存檔完成', 3000);
    }
    stockpopup_result(e) {
        this.stockpopupVisible = false;
        this.showMessage('success', '存檔完成', 3000);
    }
    onSupplierSelectionChanged(e) {
        this.app.GetData('/PurchaseHeads/GetNotEndPurchaseHeadsSurfaceBySupplier/' + e.value).subscribe(
            (s) => {
                if (s.success) {
                    this.PurchaseHeadList = s.data;
                    this.editorOptions = {
                        dataSource: { paginate: true, store: { type: 'array', data: this.PurchaseHeadList, key: 'Id' } },
                        searchEnabled: true,
                        displayExpr: 'PurchaseNo',
                        valueExpr: 'Id'
                    };
                }
            }
        );
    }
    GetPurchaseDetailData() {
        this.dataSource = [];
        this.app.GetData('/Inventory/GetBasicsData').subscribe(
            (s) => {
                // debugger;
                if (s.success) {
                    const result = s.data.find(x =>
                        x.DataType === this.formData.WorkOrderHead.DataType &&
                        x.DataId === this.formData.WorkOrderHead.DataId
                    );
                    // debugger;
                    if (result) {
                        this.dataSource.push({
                            Serial: 1,
                            TempId: result.TempId,
                            DataType: result.DataType,
                            DataId: result.DataId,
                            WarehouseId: result.WarehouseId,
                            WarehouseIdA: result.WarehouseIdA,
                            WarehouseIdB: result.WarehouseIdB,
                            Quantity: this.postval.ReCount,
                            OriginPrice: result.Price,
                            Price: this.postval.ReCount * result.Price,
                            DeliveryTime: new Date()
                        });
                    }
                    this.mod = 'workorder';
                    this.creatpopupVisible = true;
                }
            }
        );
    }

    GetStockData() {
        this.dataSource = [];
        this.app.GetData('/Inventory/GetBasicsData').subscribe(
            (s) => {
                if (s.success) {
                    // debugger;
                    const result = s.data.find(x =>
                        x.DataType === this.formData.WorkOrderHead.DataType &&
                        x.DataId === this.formData.WorkOrderHead.DataId
                    );
                    if (result) {
                        // debugger;
                        this.itemkey = result.DataId;
                        this.workorderkey = this.formData.WorkOrderHead.Id;
                    }
                    this.mod = 'workorder';
                    this.stockpopupVisible = true;
                }
            }
        );
    }
    validate_before(): boolean {
        // 表單驗證
        if (this.myform.instance.validate().isValid === false) {
            this.showMessage('error', '請注意訂單內容必填的欄位或欲製造數量有誤', 3000);
            return false;
        }
        return true;
    }
    viewRefresh(e, result) {
        if (result) {
            this.myform.instance.resetValues();
            this.CustomerVal = null;
            e.preventDefault();
            this.childOuter.emit(true);
        }
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
    async onFormSubmit(e) {
        this.buttondisabled = true;
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }
        this.formData = this.myform.instance.option('formData');

        // this.postval = this.formData;
        // tslint:disable-next-line: new-parens
        this.postval = new workOrderReportData;
        this.postval.WorkOrderID = this.itemkeyval;
        this.postval.WorkOrderSerial = this.serialkeyval;
        this.postval.MCount = this.formData?.MCount ?? 0;
        this.postval.ReCount = this.formData.ReCount;
        this.postval.RePrice = this.formData?.RePrice ?? 0;
        this.postval.NgCount = this.formData.NgCount;
        this.postval.Message = this.formData.Message;
        this.postval.ProducingMachine = this.formData.ProducingMachine;
        this.postval.SupplierId = this.formData.SupplierId;
        this.postval.PurchaseId = this.formData.PurchaseId;
        this.postval.CreateUser = this.formData.CreateUser;
        this.postval.CodeNo = this.formData.CodeNo;

        this.postval.ReportType = this.formData.ReportType;
        this.postval.CkCount = this.formData.CkCount;
        this.postval.OkCount = this.formData.OkCount;
        this.postval.NgCount = this.formData.NgCount;
        this.postval.NcCount = this.formData.NcCount;
        this.postval.DrawNo = this.formData.DrawNo;
        this.postval.CheckResult = this.formData.CheckResult;
        this.postval.Message = this.formData.Message;

        try {
            if (this.modval === 'start') {
                // tslint:disable-next-line: max-line-length
                const sendRequest = await SendService.sendRequest(this.http, '/WorkOrders/WorkOrderReportStart', 'POST', { values: this.postval });
                this.viewRefresh(e, sendRequest);
            } else if (this.modval === 'end') {
                this.formData = this.myform.instance.option('formData');
                if ((this.formData.ReCount + this.formData.NgCount + this.formData.NcCount) === this.formData.Count) {

                    // tslint:disable-next-line: max-line-length
                    const sendRequest = await SendService.sendRequest(this.http, '/WorkOrders/WorkOrderReportEnd', 'POST', { values: this.postval });
                    this.viewRefresh(e, sendRequest);

                } else {
                    Swal.fire({
                        showCloseButton: true,
                        allowEnterKey: false,
                        allowOutsideClick: false,
                        title: '注意',
                        html: '回報數量與需求數量不符!',
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#D0D0D0',
                        cancelButtonText: '不回填',
                        confirmButtonText: '確認回填'

                    }).then(async (result) => {
                        if (result.value) {
                            // tslint:disable-next-line: max-line-length
                            const sendRequest = await SendService.sendRequest(this.http, '/WorkOrders/WorkOrderReportEnd', 'POST', { values: this.postval });
                            this.viewRefresh(e, sendRequest);
                        }
                    })
                }

            } else if (this.modval === 'restart') {
                // tslint:disable-next-line: max-line-length7
                const sendRequest = await SendService.sendRequest(this.http, '/WorkOrders/WorkOrderReportRestart', 'POST', { values: this.postval });
                this.viewRefresh(e, sendRequest);
            } else if (this.modval === 'purchase') {
                if (this.postval.SupplierId != null && this.postval.PurchaseId != null) {
                    const result = this.PurchaseHeadList.find(x => x.Id === this.postval.PurchaseId);
                    if (result) {
                        let tempCheck = false;
                        Swal.fire({
                            showCloseButton: true,
                            allowEnterKey: false,
                            allowOutsideClick: false,
                            title: '是否回填?',
                            html: '如確認回填採購單，將會進行轉倉!',
                            icon: 'question',
                            showCancelButton: true,
                            confirmButtonColor: '#3085d6',
                            cancelButtonColor: '#71c016',
                            cancelButtonText: '不回填',
                            confirmButtonText: '確認回填'
                        }).then(async (result2) => {
                            if (result2.value) {
                                tempCheck = true;
                                this.postval.CheckResult = 1;
                            } else if (result2.dismiss === Swal.DismissReason.cancel) {
                                tempCheck = true;
                                this.postval.CheckResult = 0;
                            } else if (result2.dismiss === Swal.DismissReason.close) {
                                tempCheck = false;
                            }
                            if (tempCheck) {
                                this.postval.PurchaseNo = result.PurchaseNo;
                                // tslint:disable-next-line: max-line-length
                                const sendRequest = await SendService.sendRequest(this.http, '/WorkOrders/ReportWorkOrderByPurchase', 'PUT', { key: this.postval.WorkOrderID, values: this.postval });
                                this.viewRefresh(e, sendRequest);
                            }
                        });
                    }
                } else {
                    this.showMessage('error', '請選擇[供應商]和[採購單號]!', 3000);
                }
            } else if (this.modval === 'newpurchase') {
                this.GetPurchaseDetailData();
            } else if (this.modval === 'stock') {
                this.GetStockData();
            }

        } catch (error) { }
        this.buttondisabled = false;
    }
}
