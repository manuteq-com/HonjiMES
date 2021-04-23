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
import Swal from 'sweetalert2';
import { PostSufaceMaster_Detail } from 'src/app/model/viewmodels';


@Component({
  selector: 'app-surface-detail',
  templateUrl: './surface-detail.component.html',
  styleUrls: ['./surface-detail.component.css'],
  providers: [DatePipe]
})
export class SurfaceDetailComponent implements OnInit {
    @Output() childOuter = new EventEmitter();
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @Input() status: boolean;
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
    MaterialList: any;
    topurchasekey: any;
    toworkkey : any;
    Otoworkkey : any;
    popupVisibleWork : boolean;

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
        this.app.GetData('/MaterialBasics/GetMaterialBasics').subscribe(
            (s) => {
                s.data.data.forEach(e => {
                    e.MaterialNo = e.MaterialNo + ' / ' +  e.Specification;
                });
                this.MaterialList = s.data.data;
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

    }

    distinct(value) {
        //debugger;
        if(value.value){
            return value.value.split(",");
        }else{
            return null;
        }
    }

    async to_workClick(e) {
        this.topurchasekey = null;
        this.topurchasekey = this.dataGrid.instance.getSelectedRowsData();
        if (this.topurchasekey.length === 0) {
            Swal.fire({
                allowEnterKey: false,
                allowOutsideClick: false,
                title: '沒有勾選任何訂單項目',
                html: '請勾選要轉工單的訂單項目',
                icon: 'warning',
                timer: 3000
            });
        } else {
            try {
                const SurfaceData = new PostSufaceMaster_Detail();
                SurfaceData.surfaceDetail = this.topurchasekey;

                // tslint:disable-next-line: max-line-length
                //const sendRequest = await SendService.sendRequest(this.http, '/WorkOrders/OrderToWorkOrderCheck', 'POST', { values: OrderData });
                //if (sendRequest.length !== 0) {
                    //this.toworkkey = sendRequest;
                    this.Otoworkkey = this.topurchasekey;
                    this.popupVisibleWork = true;
                //}
                this.topurchasekey = [];
            } catch (error) {

            }
        }
    }
}

