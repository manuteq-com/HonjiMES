import { Component, OnInit, ViewChild, Input, OnChanges } from '@angular/core';
import { DxDataGridComponent } from 'devextreme-angular';
import CustomStore from 'devextreme/data/custom_store';
import { HttpClient } from '@angular/common/http';
import { SendService } from '../../shared/mylib';
import notify from 'devextreme/ui/notify';
import { APIResponse } from 'src/app/app.module';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-purchase-detail',
  templateUrl: './purchase-detail.component.html',
  styleUrls: ['./purchase-detail.component.css']
})
export class PurchaseDetailComponent implements OnInit, OnChanges {
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

    constructor(private http: HttpClient) {
        this.allMode = 'allPages';
        this.checkBoxesMode = 'always'; // 'onClick';
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: () => SendService.sendRequest(this.http, this.Controller + '/GetPurchaseDetailByPId?PId=' + this.itemkey),
            byKey: (key) => SendService.sendRequest(this.http, this.Controller + '/GetPurchaseDetail', 'GET', { key }),
            insert: (values) => SendService.sendRequest(this.http, this.Controller + '/PostPurchaseDetail', 'POST', { values }),
            update: (key, values) => SendService.sendRequest(this.http, this.Controller + '/PutPurchaseDetail', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(this.http, this.Controller + '/DeletePurchaseDetail/' + key, 'DELETE')
        });
    }
    public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>(location.origin + '/api' + apiUrl);
    }
    ngOnInit() {
    }
    ngOnChanges() {
        this.GetData('/Warehouses/GetWarehouses').subscribe(
            (s) => {
                this.WarehouseList = s.data;
            }
        );
    }
    to_purchaseClick(e) {
        debugger;
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
}
