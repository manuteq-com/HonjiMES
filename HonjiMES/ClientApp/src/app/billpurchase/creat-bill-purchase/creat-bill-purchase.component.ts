import { Component, OnInit, OnChanges, EventEmitter, Output, Input, ViewChild } from '@angular/core';
import { DxFormComponent, DxDataGridComponent } from 'devextreme-angular';
import { HttpClient } from '@angular/common/http';
import notify from 'devextreme/ui/notify';
import { Observable } from 'rxjs';
import { APIResponse } from 'src/app/app.module';
import { SendService } from 'src/app/shared/mylib';

@Component({
    selector: 'app-creat-bill-purchase',
    templateUrl: './creat-bill-purchase.component.html',
    styleUrls: ['./creat-bill-purchase.component.css']
})
export class CreatBillPurchaseComponent implements OnInit, OnChanges {

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
    selectBoxOptions: { items: any; displayExpr: string; valueExpr: string; onValueChanged: any; };
    SupplierList: any;
    MaterialList: any;

    constructor(private http: HttpClient) {
        this.CustomerVal = null;
        this.formData = null;
        this.editOnkeyPress = true;
        this.enterKeyAction = 'moveFocus';
        this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 300;
        this.colCount = 2;
        this.url = location.origin + '/api';
        this.dataSourceDB = [];
        this.controller = '/OrderDetails';
        this.GetData('/Suppliers/GetSuppliers').subscribe(
            (s) => {
                if (s.success) {
                    this.SupplierList = s.data;
                }
            }
        );
        this.GetData('/Materials/GetMaterials').subscribe(
            (s) => {
                if (s.success) {
                    this.MaterialList = s.data;
                }
            }
        );
    }
    public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>(apiUrl);
    }
    ngOnInit() {
    }
    ngOnChanges() {

    }
    onInitNewRow(e) {

    }
    onFocusedCellChanging(e) {
    }
    onCustomerSelectionChanged(e) {

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
        const hnull = this.dataSourceDB.find(item => item.ProductId == null);
        if (hnull || (this.SerialNo > 0 && this.dataSourceDB.length < 1)) {
            notify({
                message: '請注意訂單內容必填的欄位',
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'error', 3000);
            return false;
        } else {
            this.dataGrid.instance.saveEditData();
            this.formData = this.myform.instance.option('formData');
            this.postval = {
                PurchaseHead: this.formData,
                PurchaseDetails: this.dataSourceDB
            };
            this.buttondisabled = false;
            // tslint:disable-next-line: max-line-length
            const sendRequest = await SendService.sendRequest(this.http, '/OrderHeads/PostOrderMaster_Detail', 'POST', { values: this.postval });
            // let data = this.client.POST( this.url + '/OrderHeads/PostOrderMaster_Detail').toPromise();
            if (sendRequest) {
                this.SerialNo = 0;
                this.dataSourceDB = null;
                this.dataGrid.instance.refresh();
                this.myform.instance.resetValues();
                this.CustomerVal = null;
                e.preventDefault();
                this.childOuter.emit(true);
            }
            this.buttondisabled = false;
        }
    };
}
