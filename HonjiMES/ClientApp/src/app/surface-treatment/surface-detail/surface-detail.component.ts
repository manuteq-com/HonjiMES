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
    topurchase: any[] & Promise<any> & JQueryPromise<any>;
    dataSourceDB: CustomStore;
    Controller = '/PurchaseDetails';
    Quantityval: number;
    OriginPriceval: number;
    Priceval: number;
    WarehouseList: any;
    ItemTypeList: any;
    MaterialList: any;
    toworkorderkey: any;
    toworkkey : any;
    mod: any;
    popupVisibleWork : boolean;
    WO: any;
    checkBoxValue:any;
    checkBoxarray: any = [];
    Deliveredval: number;
    Undeliveredval: number;
    Okval: number;
    NotOkval: number;
    Repairval: number;
    Unrepairval: number;
    InNGval: number;
    OutNGval: number;

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent, public datepipe: DatePipe) {
        this.allMode = 'allPages';
        this.checkBoxValue = false;
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

    onEditingStart(e) {
        debugger;
        this.Quantityval = e.data.PurchaseDetails.Quantity;
        this.Deliveredval = e.data.PurchaseDetails.Delivered;
        this.Undeliveredval = e.data.PurchaseDetails.Undelivered;
        this.Okval = e.data.PurchaseDetails.Ok;
        this.NotOkval = e.data.PurchaseDetails.NotOk;
        this.Repairval = e.data.PurchaseDetails.Repair;
        this.Unrepairval = e.data.PurchaseDetails.Unrepair;
        this.InNGval = e.data.PurchaseDetails.InNg;
        this.OutNGval = e.data.PurchaseDetails.OutNg;
    }
    QuantityValueChanged(e, data) {
        data.setValue(e.value);
    }
    DeliveredValueChanged(e, data) {
        data.setValue(e.value);
        this.Deliveredval = e.value;
        this.Undeliveredval = this.Quantityval - this.Deliveredval;
    }
    UndeliveredValueChanged(e, data) {
        data.setValue(e.value);
        this.Undeliveredval = e.value;
        this.Deliveredval = this.Quantityval - this.Undeliveredval;
    }
    OkValueChanged(e, data) {
        data.setValue(e.value);
        this.Okval = e.value;
        this.NotOkval = this.Quantityval - this.Okval;
    }
    NotOkValueChanged(e, data) {
        data.setValue(e.value);
        this.NotOkval = e.value;
        this.Okval = this.Quantityval - this.NotOkval;
    }
    RepairValueChanged(e, data) {
        data.setValue(e.value);
        this.Repairval = e.value;
        this.Unrepairval = this.NotOkval - this.Repairval;
    }
    UnRepairValueChanged(e, data) {
        data.setValue(e.value);
        this.Unrepairval = e.value;
        this.Repairval = this.NotOkval - this.Unrepairval;
    }
    InNGValueChanged(e, data) {
        data.setValue(e.value);
        this.InNGval = e.value;
        this.OutNGval = this.Unrepairval - this.InNGval;
    }
    OutNGValueChanged(e, data) {
        data.setValue(e.value);
        this.OutNGval = e.value;
        this.InNGval = this.Unrepairval - this.OutNGval;
    }
    OriginValueChanged(e, data) {
        data.setValue(e.value);
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
        if(value.value){
            this.WO = value.value.split(",");
            return this.WO;
        }else{
            return null;
        }
    }

    popup_result(e) {
        this.popupVisibleWork = false;
        this.childOuter.emit(true);
        this.dataGrid.instance.refresh();
        this.dataGrid.instance.clearSelection();
        notify({
            message: '存檔完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
    }

    async to_workClick(e) {
        debugger;
        this.toworkorderkey = null;
        this.toworkorderkey = this.checkBoxarray;
        if (this.toworkorderkey.length === 0) {
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
                SurfaceData.surfaceDetail = this.toworkorderkey;
                //const sendRequest = await SendService.sendRequest(this.http, '/WorkOrders/OrderToWorkOrderCheck', 'POST',
                // { values: OrderData });
                //if (sendRequest.length !== 0) {
                    this.mod = "surfacetreat";
                    this.popupVisibleWork = true;
                //}
                this.toworkorderkey = [];
            } catch (error) {

            }
        }
    }

    handleValueChange(e) {
        debugger;
        const previousValue = e.previousValue;
        const newValue = e.value;
        if (newValue == true){
            this.checkBoxarray.push(e.element.innerText);
        }else{
            this.checkBoxarray.forEach(function (item, index, arr) {
                if (newValue == false) {
                    arr.splice(index, 1);
                }
            });
        }
        console.log(this.checkBoxarray)
    }

    QuantitysetCellValue(newData, value, currentRowData) {
        newData.Quantity = value;
        newData.Price = value * currentRowData.OriginPrice;
        if (isNaN(newData.Price)) {
            newData.Price = null;
        }
    }
    // handleValueChange(e) {
    //     debugger;
    //     const previousValue = e.previousValue;
    //     const newValue = e.value;
    //     if (newValue == true) {
    //         this.checkBoxarray.push(e.element.innerText);
    //     } else {
    //         this.checkBoxarray.splice(0, 1);
    //     }
    //     console.log(this.checkBoxarray)
    // }
}

