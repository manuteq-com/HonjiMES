import { Component, OnInit, ViewChild, Output, Input, OnChanges, EventEmitter } from '@angular/core';
import { DxDataGridComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { HttpClient } from '@angular/common/http';
import CustomStore from 'devextreme/data/custom_store';
import $ from 'jquery';
import { SendService } from 'src/app/shared/mylib';
import { APIResponse } from 'src/app/app.module';
import { Observable } from 'rxjs';
import Swal from 'sweetalert2';
import { AppComponent } from 'src/app/app.component';
import { Myservice } from 'src/app/service/myservice';
@Component({
    selector: 'app-bill-purchase-detail',
    templateUrl: './bill-purchase-detail.component.html',
    styleUrls: ['./bill-purchase-detail.component.css']
})
export class BillPurchaseDetailComponent implements OnInit {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @Output() childOuter = new EventEmitter();
    @Input() itemkey: number;
    @Input() SupplierList: any;
    @Input() MaterialBasicList: any;
    allMode: string;
    checkBoxesMode: string;
    topurchase: any[] & Promise<any> & JQueryPromise<any>;
    dataSourceDB: CustomStore;
    Controller = '/BillofPurchaseDetails';
    Quantityval: number;
    Priceval: number;
    PriceAllval: number;
    changeMode: boolean;
    popupVisibleTo: boolean;
    popupVisibleRe: boolean;
    mod: string;
    keyID: any;
    bopData: any;
    WarehouseList: any;
    CheckInBtnVisible: boolean;
    postval: any;
    ItemTypeList: any;
    UnitQuantityval: any;
    UnitPriceAllval: number;
    UnitPriceval: any;
    WorkPriceval: any;

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent) {
        this.checkInOnClick = this.checkInOnClick.bind(this);
        this.checkOutOnClick = this.checkOutOnClick.bind(this);
        this.onCellPrepared = this.onCellPrepared.bind(this);
        this.ItemTypeList = myservice.getlistAdjustStatus();
        this.allMode = 'allPages';
        this.checkBoxesMode = 'always'; // 'onClick';
        this.CheckInBtnVisible = false;
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: () => SendService.sendRequest(this.http, this.Controller + '/GetBillofPurchaseDetailByPId?PId=' + this.itemkey),
            byKey: (key) => SendService.sendRequest(this.http, this.Controller + '/GetBillofPurchaseDetail', 'GET', { key }),
            // tslint:disable-next-line: max-line-length
            insert: (values) => SendService.sendRequest(this.http, this.Controller + '/PostBillofPurchaseDetail?PId=' + this.itemkey, 'POST', { values }),
            // tslint:disable-next-line: max-line-length
            update: (key, values) => SendService.sendRequest(this.http, this.Controller + '/PutBillofPurchaseDetail', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(this.http, this.Controller + '/DeleteBillofPurchaseDetail', 'DELETE', { key })
        });
        this.app.GetData('/Warehouses/GetWarehouses').subscribe(
            (s) => {
                s.data.forEach(e => {
                    e.Name = e.Code + e.Name;
                });
                this.WarehouseList = s.data;
            }
        );
    }
    ngOnInit() {
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
    onCellPrepared(e: any) {
        if (e.rowType === 'data') {
            if (e.data.CheckStatus === 0) {
                this.CheckInBtnVisible = true;
            }
        }
    }
    onContentReady(e) {
        const _dataGrid = $(e.element);
        const dataGrid = e.component;
        if (this.changeMode && !_dataGrid.find('.dx-row-inserted').length) {
            dataGrid.beginUpdate();
            e.component.option('editing.mode', 'form');
            this.changeMode = false;
            dataGrid.columnOption('SupplierId', { allowEditing: false });
            dataGrid.columnOption('DataId', { allowEditing: false });
            dataGrid.endUpdate();
        }
    }
    onInitNewRow(e) {
        const dataGrid = e.component;
        dataGrid.beginUpdate();
        dataGrid.option('editing.mode', 'form');
        dataGrid.columnOption('SupplierId', { allowEditing: true });
        dataGrid.columnOption('DataId', { allowEditing: true });
        dataGrid.endUpdate();
        this.changeMode = true;
        this.Quantityval = null;
        this.Priceval = null;
        this.PriceAllval = null;
    }
    onRowInserting(e) {

    }
    onRowInserted(e) {

    }
    async to_CheckInClick(e) {
        Swal.fire({
            showCloseButton: true,
            allowEnterKey: false,
            allowOutsideClick: false,
            title: '整批驗收',
            html: '如確認全數驗收，請點選[確認]!',
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#CE312C',
            cancelButtonText: '取消',
            confirmButtonText: '確認'
        }).then(async (result) => {
            if (result.value) {
                try {
                    this.postval = {
                        Id: this.itemkey
                    };
                    // tslint:disable-next-line: max-line-length
                    const sendRequest = await SendService.sendRequest(this.http, '/ToPurchase/PostPurchaseCheckInArray', 'POST', { values: this.postval });
                    if (sendRequest) {
                        this.CheckInBtnVisible = false;
                        // e.preventDefault();
                        this.dataGrid.instance.refresh();
                        this.childOuter.emit(true);
                        notify({
                            message: sendRequest.message,
                            position: {
                                my: 'center top',
                                at: 'center top'
                            }
                        }, 'success', 3000);
                    }
                } catch (error) {

                }
            }
        });
    }
    allowEdit(e) {
        if (e.row.data.CheckStatus === 1) {
            return false;
        } else {
            return true;
        }
    }
    onEditorPreparing(e) {
        debugger;

    }
    selectvalueChanged(e, data) {
        data.setValue(e.value);
        const today = new Date();
        this.MaterialBasicList.forEach(x => {
            if (x.Id === e.value) {
                this.Quantityval = x.Quantity;
                this.Priceval = x.OriginPrice;
                this.PriceAllval = x.Quantity * x.OriginPrice;
            }
        });
    }
    QuantityValueChanged(e, data) {
        data.setValue(e.value);
        // this.Quantityval = e.value;
        // this.PriceAllval = this.Quantityval * this.Priceval;
    }
    PriceValueChanged(e, data) {
        data.setValue(e.value);
        // debugger;
        // e.component.cellValue(data.rowIndex, 'PriceAll' , e.value * data.data.Quantity);

    }

    updateCell(rowIndex, dataField, value) {
        this.dataGrid.instance.cellValue(rowIndex, dataField, value);
        this.dataGrid.instance.saveEditData();
    }
    UnitQuantityValueChanged(e, data) {
        data.setValue(e.value);
        this.UnitQuantityval = e.value;
        this.UnitPriceAllval = this.UnitQuantityval * this.UnitPriceval;
    }
    UnitPriceValueChanged(e, data) {
        data.setValue(e.value);
        this.UnitPriceval = e.value;
        this.UnitPriceAllval = this.UnitQuantityval * this.UnitPriceval;
    }
    onEditingStart(e) {
        this.Quantityval = e.data.Quantity;
        this.Priceval = e.data.OriginPrice;
        this.PriceAllval = e.data.Price;
        this.UnitQuantityval = e.data.UnitQuantityval;
        this.UnitPriceval = e.data.UnitPriceval;
        this.UnitPriceAllval = e.data.UnitPriceAllval;
        this.WorkPriceval = e.data.WorkPrice;
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
    checkInOnClick(e, item) {
        debugger;
        this.keyID = item.key;
        this.popupVisibleTo = true;
    }
    checkOutOnClick(e, item) {
        this.bopData = item.data;
        this.popupVisibleRe = true;
    }
    popup_result(e) {
        this.popupVisibleTo = false;
        this.popupVisibleRe = false;
        this.dataGrid.instance.refresh();
        this.childOuter.emit(true);
        notify({
            message: '存檔完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
    }

    customizeText(e) {
        return '總數：' + e.value + '筆';
    }
}
