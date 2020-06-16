import { Component, OnInit, ViewChild, Input, OnChanges } from '@angular/core';
import { DxDataGridComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { HttpClient } from '@angular/common/http';
import CustomStore from 'devextreme/data/custom_store';
import $ from 'jquery';
import { SendService } from 'src/app/shared/mylib';
import { APIResponse } from 'src/app/app.module';
import { Observable } from 'rxjs';

@Component({
    selector: 'app-bill-purchase-detail',
    templateUrl: './bill-purchase-detail.component.html',
    styleUrls: ['./bill-purchase-detail.component.css']
})
export class BillPurchaseDetailComponent implements OnInit {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @Input() itemkey: number;
    @Input() SupplierList: any;
    @Input() MaterialBasicList: any;
    allMode: string;
    checkBoxesMode: string;
    topurchase: any[] & Promise<any> & JQueryPromise<any>;
    dataSourceDB: CustomStore;
    Controller = '/BillofPurchaseDetails';
    Quantityval: number;
    OriginPriceval: number;
    Priceval: number;
    changeMode: boolean;
    popupVisibleTo: boolean;
    popupVisibleRe: boolean;
    mod: string;
    keyID: any;
    bopData: any;
    WarehouseList: any;

    constructor(private http: HttpClient) {
        this.checkInOnClick = this.checkInOnClick.bind(this);
        this.checkOutOnClick = this.checkOutOnClick.bind(this);
        this.allMode = 'allPages';
        this.checkBoxesMode = 'always'; // 'onClick';
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: () => SendService.sendRequest(this.http, this.Controller + '/GetBillofPurchaseDetailByPId?PId=' + this.itemkey),
            byKey: (key) => SendService.sendRequest(this.http, this.Controller + '/GetBillofPurchaseDetail', 'GET', { key }),
            // tslint:disable-next-line: max-line-length
            insert: (values) => SendService.sendRequest(this.http, this.Controller + '/PostBillofPurchaseDetail?PId=' + this.itemkey, 'POST', { values }),
            // tslint:disable-next-line: max-line-length
            update: (key, values) => SendService.sendRequest(this.http, this.Controller + '/PutBillofPurchaseDetail', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(this.http, this.Controller + '/DeleteBillofPurchaseDetail', 'DELETE')
        });
        this.GetData('/Warehouses/GetWarehouses').subscribe(
            (s) => {
                this.WarehouseList = s.data;
            }
        );
    }
    public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>(location.origin + '/api' + apiUrl);
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
        this.OriginPriceval = null;
        this.Priceval = null;
    }
    onRowInserting(e) {

    }
    onRowInserted(e) {

    }
    to_purchaseClick(e) {
        this.topurchase = this.dataGrid.instance.getSelectedRowsData();
    }
    allowEdit(e) {
        if (e.row.data.CheckStatus === 1) {
            return false;
        } else {
            return true;
        }
    }
    selectvalueChanged(e, data) {
        data.setValue(e.value);
        const today = new Date();
        this.MaterialBasicList.forEach(x => {
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
        notify({
            message: '存檔完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
    }
}
