import { PurchaseOrderComponent } from './../purchase-order/purchase-order.component';
import { Component, OnInit, ViewChild, Input, OnChanges, EventEmitter, Output } from '@angular/core';
import { DatePipe } from '@angular/common';
import { DxDataGridComponent } from 'devextreme-angular';
import CustomStore from 'devextreme/data/custom_store';
import { HttpClient } from '@angular/common/http';
import { SendService } from '../../shared/mylib';
import notify from 'devextreme/ui/notify';
import { APIResponse } from 'src/app/app.module';
import { Observable } from 'rxjs';
import { AppComponent } from 'src/app/app.component';
import { Myservice } from 'src/app/service/myservice';

@Component({
    selector: 'app-purchase-detail',
    templateUrl: './purchase-detail.component.html',
    styleUrls: ['./purchase-detail.component.css'],
    providers: [DatePipe]
})
export class PurchaseDetailComponent implements OnInit, OnChanges {
    @Output() childOuter = new EventEmitter();
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @Input() itemkey: number;
    @Input() SupplierList: any;
    @Input() MaterialBasicList: any;
    allMode: string;
    checkBoxesMode: string;
    topurchase: any[] & Promise<any> & JQueryPromise<any>;
    dataSourceDB: CustomStore;
    Controller = '/PurchaseDetails';
    Quantityval: number;
    OriginPriceval: number;
    Priceval: number;
    WarehouseList: any;
    ItemTypeList: any;

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent, public datepipe: DatePipe) {
        this.allMode = 'allPages';
        this.checkBoxesMode = 'always'; // 'onClick';
        this.ItemTypeList = myservice.getlistAdjustStatus();
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: () => SendService.sendRequest(this.http, this.Controller + '/GetPurchaseDetailByPId?PId=' + this.itemkey),
            byKey: (key) => SendService.sendRequest(this.http, this.Controller + '/GetPurchaseDetail', 'GET', { key }),
            insert: (values) => SendService.sendRequest(this.http, this.Controller + '/PostPurchaseDetail', 'POST', { values }),
            update: (key, values) => SendService.sendRequest(this.http, this.Controller + '/PutPurchaseDetail', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(this.http, this.Controller + '/DeletePurchaseDetail/' + key, 'DELETE')
        });
    }
    ngOnInit() {
    }
    ngOnChanges() {
        this.app.GetData('/Warehouses/GetWarehouses').subscribe(
            (s) => {
                s.data.forEach(e => {
                    e.Name = e.Code + e.Name;
                });
                this.WarehouseList = s.data;
            }
        );
    }
    to_purchaseClick(e) {
        this.topurchase = this.dataGrid.instance.getSelectedRowsData();
    }
    QuantityValueChanged(e, data) {
        data.setValue(e.value);
        // this.Quantityval = e.value;
        // this.Priceval = this.Quantityval * this.OriginPriceval;
    }
    OriginValueChanged(e, data) {
        data.setValue(e.value);
        // this.OriginPriceval = e.value;
        // this.Priceval = this.Quantityval * this.OriginPriceval;
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
    customizeText(e) {
        return '總數：' + e.value + '筆';
    }
    onRowUpdated(e) {
        this.childOuter.emit(true);
    }
    onRowPrepared(e) {
        // debugger;
        let hint = false;
        if (e.data !== undefined) {
            const DeliverydateBefore = new Date(e.data.DeliveryTime);
            const DeliverydateAfter = new Date(new Date().setDate(new Date().getDate() - 1));
            if (DeliverydateBefore <= DeliverydateAfter) {
                hint = true;
            }
            if (hint) {
                e.rowElement.style.color = '#d9534f';
            }
        }
    }
}
