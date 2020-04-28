import { Component, OnInit, OnChanges, Output, Input, EventEmitter, ViewChild } from '@angular/core';
import { DxFormComponent, DxDataGridComponent } from 'devextreme-angular';
import { HttpClient } from '@angular/common/http';
import { APIResponse } from '../app.module';
import { Observable } from 'rxjs';
import notify from 'devextreme/ui/notify';
import { SendService } from '../shared/mylib';
import { Myservice } from '../service/myservice';

@Component({
    selector: 'app-creat-purchase',
    templateUrl: './creat-purchase.component.html',
    styleUrls: ['./creat-purchase.component.css']
})
export class CreatPurchaseComponent implements OnInit, OnChanges {

    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() exceldata: any;
    @Input() modval: any;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    buttondisabled: false;
    CustomerVal: any;
    formData: any;
    editOnkeyPress: boolean;
    enterKeyAction: string;
    enterKeyDirection: string;
    labelLocation: string;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    width: any;
    colCount: number;
    url: string;
    dataSourceDB: any[];
    controller: string;
    SupplierselectBoxOptions: any;
    MaterialselectBoxOptions: any;
    SupplierList: any;
    MaterialList: any;
    PurchasetypeList: any;
    TypeselectBoxOptions: any;
    Quantityval: number;
    OriginPriceval: number;
    Priceval: number;
    addRow = true;
    eformData: any;
    Quantityvalmax: number;
    constructor(private http: HttpClient, myservice: Myservice) {
        this.CustomerVal = null;
        this.formData = null;
        this.editOnkeyPress = true;
        this.enterKeyAction = 'moveFocus';
        this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 300;
        this.colCount = 3;
        this.url = location.origin + '/api';
        this.dataSourceDB = [];
        this.controller = '/OrderDetails';
        this.GetData('/Suppliers/GetSuppliers').subscribe(
            (s) => {
                if (s.success) {
                    this.SupplierList = s.data;
                    this.SupplierList.forEach(x => {
                        x.Name = x.Code + '_' + x.Name;
                    });
                    this.SupplierselectBoxOptions = {
                        items: this.SupplierList,
                        displayExpr: 'Name',
                        valueExpr: 'Id',
                        searchEnabled: true,
                        onValueChanged: this.onSupplierSelectionChanged.bind(this)
                    };
                }
            }
        );
        this.PurchasetypeList = myservice.getpurchasetypes();
        this.TypeselectBoxOptions = {
            items: this.PurchasetypeList,
            displayExpr: 'Name',
            valueExpr: 'Id',
            searchEnabled: true
        };

    }
    public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>(location.origin + '/api' + apiUrl);
    }
    ngOnInit() {

    }
    ngOnChanges() {
    }
    onInitNewRow(e) {

    }
    onFocusedCellChanging(e) {
    }
    onSupplierSelectionChanged(e) {
        this.GetData('/ToPurchase/GetCanPurchase/' + e.value).subscribe(
            (s) => {
                if (s.success) {
                    this.MaterialList = s.data;
                    this.Quantityvalmax = null;
                    this.Quantityval = null;
                    this.OriginPriceval = null;
                    this.Priceval = null;
                    if (this.addRow) {
                        this.dataGrid.instance.addRow();
                        this.addRow = false;
                    }
                }
            }
        );
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
    onFormSubmit = async function (e) {
        // debugger;
        this.buttondisabled = true;
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }
        this.dataGrid.instance.saveEditData();
        this.formData = this.myform.instance.option('formData');
        this.postval = {
            PurchaseHead: this.formData,
            PurchaseDetails: this.dataSourceDB
        };
        this.buttondisabled = false;
        try {
            // tslint:disable-next-line: max-line-length
            const sendRequest = await SendService.sendRequest(this.http, '/ToPurchase/PostPurchaseMaster_Detail', 'POST', { values: this.postval });
            if (sendRequest) {
                this.dataSourceDB = null;
                this.dataGrid.instance.refresh();
                this.myform.instance.resetValues();
                e.preventDefault();
                this.childOuter.emit(true);
            }
        } catch (error) {
        }
        this.buttondisabled = false;
    };
}
