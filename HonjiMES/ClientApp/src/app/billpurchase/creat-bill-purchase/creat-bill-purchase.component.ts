import { Component, OnInit, OnChanges, EventEmitter, Output, Input, ViewChild } from '@angular/core';
import { DxFormComponent, DxDataGridComponent } from 'devextreme-angular';
import { HttpClient } from '@angular/common/http';
import notify from 'devextreme/ui/notify';
import { Observable } from 'rxjs';
import { APIResponse } from 'src/app/app.module';
import { SendService } from 'src/app/shared/mylib';
import $ from 'jquery';
import { BillofPurchaseDetail } from 'src/app/model/viewmodels';
@Component({
    selector: 'app-creat-bill-purchase',
    templateUrl: './creat-bill-purchase.component.html',
    styleUrls: ['./creat-bill-purchase.component.css']
})
export class CreatBillPurchaseComponent implements OnInit, OnChanges {

    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() exceldata: any;
    @Input() modval: any;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    buttondisabled = false;
    CustomerVal: any;
    formData: any;
    editOnkeyPress: boolean;
    enterKeyAction: string;
    enterKeyDirection: string;
    labelLocation: string;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    width: any;
    colCount: number;
    url: string;
    dataSourceDB: {
        Id: number, SupplierId: number, PurchaseId: number,
        DataId: number, Quantity: number, OriginPrice: number, Price: number
    }[] = [];
    controller: string;
    selectBoxOptions: { items: any; displayExpr: string; valueExpr: string; onValueChanged: any; };
    SupplierList: any;
    PurchaseList: any;
    MaterialList: any;
    PurchaseselectBoxOptions: any;
    changeMode: boolean;
    topurchase: any[] & Promise<any> & JQueryPromise<any>;
    Quantityval: any;
    OriginPriceval: any;
    Priceval: number;
    allMode: string;
    checkBoxesMode: string;
    postval: any;

    constructor(private http: HttpClient) {
        this.allMode = 'allPages';
        this.checkBoxesMode = 'always'; // 'onClick';
        this.CustomerVal = null;
        this.formData = null;
        this.editOnkeyPress = true;
        this.enterKeyAction = 'moveFocus';
        this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 300;
        this.colCount = 3;
        this.url = location.origin + '/api';
        this.controller = '/OrderDetails';
        this.dataSourceDB = [];

    }
    public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>('/api' + apiUrl);
    }
    ngOnInit() {
    }
    ngOnChanges() {
        this.GetData('/Suppliers/GetSuppliers').subscribe(
            (s) => {
                if (s.success) {
                    this.SupplierList = s.data;
                    this.SupplierList.forEach(x => {
                        x.Name = x.Code + '_' + x.Name;
                    });
                }
            }
        );
        // this.GetData('/Materials/GetMaterials').subscribe(
        //     (s) => {
        //         if (s.success) {
        //             this.MaterialList = s.data;
        //         }
        //     }
        // );
    }
    cellClick(e) {
        if (e.rowType === 'header') {
            if (e.column.type === 'buttons') {
                if (e.column.cssClass === 'addmod') {
                    // tslint:disable-next-line: deprecation
                    this.dataGrid.instance.insertRow();

                }

            }
        }
    }
    onContentReady(e) {
        debugger;
        // const _dataGrid = $(e.element);
        // const dataGrid = e.component;
        // if (this.changeMode && !_dataGrid.find('.dx-row-inserted').length) {
        //     // debugger;
        //     dataGrid.beginUpdate();
        //     e.component.option('editing.mode', 'form');
        //     this.changeMode = false;
        //     dataGrid.columnOption('SupplierId', { allowEditing: false });
        //     dataGrid.columnOption('DataId', { allowEditing: false });
        //     dataGrid.endUpdate();
        // }
    }
    onInitNewRow(e) {
        debugger;
        // const dataGrid = e.component;
        // dataGrid.beginUpdate();
        // dataGrid.option('editing.mode', 'form');
        // dataGrid.columnOption('SupplierId', { allowEditing: true });
        // dataGrid.columnOption('DataId', { allowEditing: true });
        // dataGrid.endUpdate();
        // this.changeMode = true;
    }
    onRowInserting(e) {
        debugger;
        const SupplierId = e.data.SupplierId;
        const PurchaseId = e.data.PurchaseId;
        const DataId = e.data.DataId;
        const datas = this.dataSourceDB;
        datas.forEach(element => {// 阻擋重複新增
            if (element.SupplierId === SupplierId &&
                element.PurchaseId === PurchaseId &&
                element.DataId === DataId) {
                this.showMessage('新增項目已存在!!');
                e.cancel = true;
            }
        });
        if (e.cancel === false) {
            this.Quantityval = 0;
            this.OriginPriceval = 0;
            this.Priceval = 0;
        }
    }
    onRowInserted(e) {
        debugger;
    }
    to_purchaseClick(e) {
        debugger;
        this.topurchase = this.dataGrid.instance.getSelectedRowsData();
    }
    selectSupplierValueChanged(e, data) {
        data.setValue(e.value);
        this.GetData('/PurchaseHeads/GetPurchasesBySupplier/' + e.value).subscribe(
            (s) => {
                if (s.success) {
                    this.PurchaseList = s.data;
                }
            }
        );
    }
    selectPurchaseValueChanged(e, data) {
        data.setValue(e.value);
        this.GetData('/PurchaseDetails/GetMaterialsByPurchase/' + e.value).subscribe(
            (s) => {
                if (s.success) {
                    this.MaterialList = s.data;
                }
            }
        );
    }
    selectMateriaValueChanged(e, data) {
        data.setValue(e.value);
        this.MaterialList.find(x => {
            if (x.Id === e.value) {
                this.Quantityval = x.Quantity;
                this.OriginPriceval = x.OriginPrice;
                this.Priceval = x.Quantity * x.OriginPrice;
            }
        });
    }
    QuantityValueChanged(e, data) {
        data.setValue(e.value);
        this.Quantityval = e.value;
        this.Priceval = this.Quantityval * this.OriginPriceval;
    }
    OriginValueChanged(e, data) {
        data.setValue(e.value);
        this.OriginPriceval = e.value;
        this.Priceval = this.Quantityval * this.OriginPriceval;
    }
    onEditingStart(e) {
        debugger;
        this.Quantityval = e.data.Quantity;
        this.OriginPriceval = e.data.OriginPrice;
        this.Priceval = e.data.Price;
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
    showMessage(data) {
        notify({
            message: data,
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'error', 3000);
    }
    async onFormSubmit(e) {
        debugger;
        this.buttondisabled = true;
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }
        this.dataGrid.instance.saveEditData();
        this.formData = this.myform.instance.option('formData');
        this.postval = {
            BillofPurchaseHead: this.formData,
            BillofPurchaseDetail: this.dataSourceDB as BillofPurchaseDetail[]
        };
        this.buttondisabled = false;
        // tslint:disable-next-line: max-line-length
        const sendRequest = await SendService.sendRequest(this.http, '/BillofPurchaseHeads/PostBillofPurchaseHead_Detail', 'POST', { values: this.postval });
        // let data = this.client.POST( this.url + '/OrderHeads/PostOrderMaster_Detail').toPromise();
        if (sendRequest) {
            this.dataSourceDB = null;
            this.dataGrid.instance.refresh();
            this.myform.instance.resetValues();
            this.CustomerVal = null;
            e.preventDefault();
            this.childOuter.emit(true);
        }
        this.buttondisabled = false;

    }
}
