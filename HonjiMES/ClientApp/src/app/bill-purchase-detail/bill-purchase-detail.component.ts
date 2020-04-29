import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { DxDataGridComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { HttpClient } from '@angular/common/http';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from '../shared/mylib';
@Component({
    selector: 'app-bill-purchase-detail',
    templateUrl: './bill-purchase-detail.component.html',
    styleUrls: ['./bill-purchase-detail.component.css']
})
export class BillPurchaseDetailComponent implements OnInit {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @Input() itemkey: number;
    @Input() SupplierList: any;
    @Input() MaterialList: any;
    allMode: string;
    checkBoxesMode: string;
    topurchase: any[] & Promise<any> & JQueryPromise<any>;
    dataSourceDB: CustomStore;
    Controller = '/BillofPurchaseDetails';
    Quantityval: number;
    OriginPriceval: number;
    Priceval: number;
    Quantityvalmax: number;
    changeMode: boolean;
    constructor(private http: HttpClient) {
        this.onContentReady = this.onContentReady.bind(this);
        this.allMode = 'allPages';
        this.checkBoxesMode = 'always'; // 'onClick';
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: () => SendService.sendRequest(this.http, this.Controller + '/GetBillofPurchaseDetailByPId?PId=' + this.itemkey),
            byKey: (key) => SendService.sendRequest(this.http, this.Controller + '/GetBillofPurchaseDetail', 'GET', { key }),
            insert: (values) => SendService.sendRequest(this.http, this.Controller + '/PostBillofPurchaseDetail', 'POST', { values }),
            // tslint:disable-next-line: max-line-length
            update: (key, values) => SendService.sendRequest(this.http, this.Controller + '/PutBillofPurchaseDetail', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(this.http, this.Controller + '/DeleteBillofPurchaseDetail', 'DELETE')
        });
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
        let _dataGrid = e.element;
        let dataGrid = e.component;
        if (this.changeMode && ! _dataGrid.find('.dx-row-inserted').length) {
            debugger;
            dataGrid.beginUpdate();
            e.component.option('editing.mode', 'row');
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
    }
    onRowInserting(e) {

    }
    onRowInserted(e) {

    }
    to_purchaseClick(e) {
        debugger;
        this.topurchase = this.dataGrid.instance.getSelectedRowsData();
    }
    selectvalueChanged(e, data) {
        debugger;
        data.setValue(e.value);
        const today = new Date();
        this.MaterialList.forEach(x => {
            if (x.Id === e.value) {
                this.Quantityvalmax = x.Quantity;
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
    onDataErrorOccurred(e) {
        notify({
            message: e.error.message,
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'error', 3000);
    }
}
