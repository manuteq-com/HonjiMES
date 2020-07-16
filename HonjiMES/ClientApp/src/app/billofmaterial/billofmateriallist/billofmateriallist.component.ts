import { Component, OnInit } from '@angular/core';
import { APIResponse } from 'src/app/app.module';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { createStore } from 'devextreme-aspnet-data-nojquery';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from 'src/app/shared/mylib';
import DataSource from 'devextreme/data/data_source';
@Component({
    selector: 'app-billofmateriallist',
    templateUrl: './billofmateriallist.component.html',
    styleUrls: ['./billofmateriallist.component.css']
})
export class BillofmateriallistComponent implements OnInit {
    dataSourceDB: any;
    bomMod: any;
    apiurl = location.origin + '/api';
    Controller = '/BillOfMaterials';
    remoteOperations = true;
    verpopupVisible: boolean;
    itemkey: number;
    bomverdata: any;

    constructor(private http: HttpClient) {
        this.bomMod = 'PBOM';
        const remote = this.remoteOperations;
        this.readBomVer = this.readBomVer.bind(this);
        // this.dataSourceDB = createStore({
        //     key: 'Id',
        //     loadUrl: this.apiurl + this.Controller + '/GetProducts',
        //     insertUrl: this.apiurl + this.Controller + '/PostBillofPurchaseDetail',
        //     updateUrl: this.apiurl + this.Controller + '/PutProduct',
        //     deleteUrl: this.apiurl + this.Controller + '/DeleteBillofPurchaseDetail',
        // });
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) =>
                SendService.sendRequest(this.http, this.Controller + '/GetProductBasics', 'GET', { loadOptions, remote }),
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
    public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>(location.origin + '/api' + apiUrl);
    }
    ngOnInit() {
    }
    readBomVer(e, data) {
        this.GetData('/BillOfMaterials/GetBomVerByProductId/' + data.key).subscribe(
            (s) => {
                debugger;
                s.data.forEach(element => {
                    if (element.ShowPLV === 0) {
                        element.Lv = null;
                        element.Quantity = null;
                    }
                });
                this.bomverdata = s.data;
                this.verpopupVisible = true;
            }
        );
        // this.onChangeVar.emit(data.data);
    }
}
