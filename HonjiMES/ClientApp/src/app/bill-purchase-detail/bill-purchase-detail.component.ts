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
    constructor(private http: HttpClient) {
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
    to_purchaseClick(e) {
        debugger;
        this.topurchase = this.dataGrid.instance.getSelectedRowsData();
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
