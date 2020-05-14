import { Component, OnInit, Input, ViewChild } from '@angular/core';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from 'src/app/shared/mylib';
import { HttpClient } from '@angular/common/http';

@Component({
    selector: 'app-bomlist',
    templateUrl: './bomlist.component.html',
    styleUrls: ['./bomlist.component.css']
})
export class BomlistComponent implements OnInit {
    @Input() itemkeyval: any;
    Controller = '/BillOfMaterials';
    dataSourceDB: CustomStore;

    constructor(private http: HttpClient) {
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) =>
                SendService.sendRequest(this.http, this.Controller + '/GetBomlist/' + this.itemkeyval),
            byKey: (key) =>
                SendService.sendRequest(this.http, this.Controller + '/GetBillofPurchaseDetail', 'GET', { key }),
            insert: (values) =>
                SendService.sendRequest(this.http, this.Controller + '/PostBillofPurchaseDetail', 'POST', { values }),
            update: (key, values) =>
                SendService.sendRequest(this.http, this.Controller + '/PutProduct', 'PUT', { key, values }),
            remove: (key) =>
                SendService.sendRequest(this.http, this.Controller + '/DeleteBillofPurchaseDetail', 'DELETE')
        });

    }

    ngOnInit() {
    }

}
