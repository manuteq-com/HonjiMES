import { Component, OnInit, OnChanges, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { DxFormComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { SendService } from 'src/app/shared/mylib';
import { HttpClient } from '@angular/common/http';
import { APIResponse } from 'src/app/app.module';
import { Observable } from 'rxjs';
import { AppComponent } from 'src/app/app.component';

@Component({
    selector: 'app-bill-purchase-return',
    templateUrl: './bill-purchase-return.component.html',
    styleUrls: ['./bill-purchase-return.component.css']
})
export class BillPurchaseReturnComponent implements OnInit, OnChanges {
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() modval: any;
    buttondisabled = false;
    CustomerVal: any;
    formData: any;
    postval: any;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    width: any;
    colCount: number;
    labelLocation: string;
    selectBoxOptions: any;
    QuantityEditorOptions: any;
    PriceEditorOptions: any;
    UnitCountEditorOptions: any;
    UnitPriceEditorOptions: any;
    WarehouseList: any[];

    constructor(private http: HttpClient, public app: AppComponent) {
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 300;
        this.colCount = 2;
        this.labelLocation = 'left';
    }
    ngOnInit() {
    }
    ngOnChanges() {
        this.app.GetData('/ToPurchase/GetBillofPurchaseReturnNo').subscribe(
            (s) => {
                if (s.success) {
                    this.formData = s.data;
                    this.QuantityEditorOptions = {
                        showSpinButtons: true,
                        mode: 'number',
                        format: '#0',
                        value: this.itemkeyval.CheckCountIn - this.itemkeyval.CheckCountOut,
                        min: 1,
                        max: this.itemkeyval.Quantity,
                        onValueChanged: this.QuantityValueChanged.bind(this)
                    };
                    this.PriceEditorOptions = { showSpinButtons: true, mode: 'number', onValueChanged: this.PriceValueChanged.bind(this) };
                    this.UnitCountEditorOptions = { showSpinButtons: true, mode: 'number', onValueChanged: this.UnitCountValueChanged.bind(this) };
                    this.UnitPriceEditorOptions = { showSpinButtons: true, mode: 'number', onValueChanged: this.UnitPriceValueChanged.bind(this) };
                }
            }
        );

        if (this.itemkeyval.DataType === 1) {
            this.app.GetData('/Warehouses/GetWarehouseByMaterial/' + this.itemkeyval.DataId).subscribe(
                (s) => {
                    if (s.success) {
                        this.WarehousesDataFormat(s.data);
                    }
                }
            );
        } else if (this.itemkeyval.DataType === 2) {
            this.app.GetData('/Warehouses/GetWarehouseByProduct/' + this.itemkeyval.DataId).subscribe(
                (s) => {
                    if (s.success) {
                        this.WarehousesDataFormat(s.data);
                    }
                }
            );
        } else if (this.itemkeyval.DataType === 3) {
            this.app.GetData('/Warehouses/GetWarehouseByWiproduct/' + this.itemkeyval.DataId).subscribe(
                (s) => {
                    if (s.success) {
                        this.WarehousesDataFormat(s.data);
                    }
                }
            );
        }
    }
    WarehousesDataFormat(data) {
        this.WarehouseList = [];
        data.forEach((element, index) => {
            element.Warehouse.Name = element.Warehouse.Code + element.Warehouse.Name + ' (庫存 ' + element.Quantity + ')';
            this.WarehouseList[index] = element.Warehouse;
        });
        this.selectBoxOptions = {
            items: this.WarehouseList,
            displayExpr: 'Name',
            valueExpr: 'Id',
        };
    }
    QuantityValueChanged(e) {
        this.formData.PriceAll = this.formData.Price * e.value;
        // this.formData.UnitPrice = this.formData.UnitCount * e.value;
    }
    PriceValueChanged(e) {
        this.formData.PriceAll = this.formData.Quantity * e.value;
    }
    UnitCountValueChanged(e) {
        this.formData.UnitPriceAll = this.formData.UnitPrice * e.value;
    }
    UnitPriceValueChanged(e) {
        this.formData.UnitPriceAll = this.formData.UnitCount * e.value;
    }
    validate_before(): boolean {
        // 表單驗證
        if (this.myform.instance.validate().isValid === false) {
            notify({
                message: '請注意訂單內容必填的欄位',
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'error', 3000);
            return false;
        }
        return true;
    }
    refreshReturnNo() {
        this.app.GetData('/ToPurchase/GetBillofPurchaseReturnNo').subscribe(
            (s) => {
                if (s.success) {
                    this.formData.ReturnNo = s.data.ReturnNo;
                }
            }
        );
    }
    async onFormSubmit(e) {
        this.buttondisabled = true;
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }
        this.formData = this.myform.instance.option('formData');
        this.postval = this.formData;
        this.postval.BillofPurchaseDetailId = this.itemkeyval.Id;
        debugger;
        try {
            // tslint:disable-next-line: max-line-length
            const sendRequest = await SendService.sendRequest(this.http, '/ToPurchase/PostPurchaseCheckReturn', 'POST', { values: this.postval });
            // let data = this.client.POST( this.url + '/OrderHeads/PostOrderMaster_Detail').toPromise();
            if (sendRequest) {
                this.myform.instance.resetValues();
                this.CustomerVal = null;
                e.preventDefault();
                this.childOuter.emit(true);
            }
        } catch (error) {

        }
        this.buttondisabled = false;

    }
}
