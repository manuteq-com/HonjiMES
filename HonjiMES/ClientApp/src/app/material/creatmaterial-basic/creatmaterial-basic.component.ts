import { Component, OnInit, Output, EventEmitter, Input, ViewChild, OnChanges } from '@angular/core';
import { DxFormComponent, DxDataGridComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Material } from 'src/app/model/viewmodels';
import { APIResponse } from 'src/app/app.module';
import { SendService } from 'src/app/shared/mylib';
import { AppComponent } from 'src/app/app.component';

@Component({
    selector: 'app-creatmaterial-basic',
    templateUrl: './creatmaterial-basic.component.html',
    styleUrls: ['./creatmaterial-basic.component.css']
})
export class CreatmaterialBasicComponent implements OnInit, OnChanges {
    @Output() childOuter = new EventEmitter();
    @Input() masterkey: any;
    @Input() btnVisibled: boolean;
    @Input() editStatus: boolean;
    @Input() itemkeyval: any;
    @Input() exceldata: any;
    @Input() modval: any;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    buttondisabled = false;
    formData: any = {};
    postval: Material;
    labelLocation: string;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    colCount: number;
    width: any;
    selectBoxOptions: any;
    warehousesOptions: any;
    NumberBoxOptions: any;
    gridBoxValue: number[] = [1];
    buttonOptions: any = {
        text: '存檔',
        type: 'success',
        useSubmitBehavior: true,
        icon: 'save'
    };
    supplierList: any;
    WarehouseList: any;
    selectSupplier: { items: any; displayExpr: string; valueExpr: string; searchEnabled: boolean; };

    constructor(private http: HttpClient, private app: AppComponent) {
        this.formData = null;
        // this.editOnkeyPress = true;
        // this.enterKeyAction = 'moveFocus';
        // this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.showColon = true;
        this.minColWidth = 100;
        this.colCount = 2;
        this.btnVisibled = false;
        this.asyncValidation = this.asyncValidation.bind(this);
        this.app.GetData('/Suppliers/GetSuppliers').subscribe(
            (s) => {
                console.log(s);
                if (s.success) {
                    this.selectBoxOptions = {
                        items: s.data,
                        displayExpr: 'Name',
                        valueExpr: 'Id',
                    };
                }
            }
        );
    }
    ngOnChanges() {
        debugger;
        this.readOnly = true;
        if (this.editStatus !== null) {
            this.readOnly = this.editStatus;
        }
        this.gridBoxValue = [1];
        this.NumberBoxOptions = { showSpinButtons: true, mode: 'number', min: 0, value: 0 };
        this.formData = {
            MaterialNo: '',
            Name: '',
            // Quantity: '',
            Specification: '',
            Property: '採購件',
            Price: 0,
            Unit: ''
        };
        if (this.masterkey !== null) {
            this.app.GetData('/MaterialBasics/GetMaterialBasic/' + this.masterkey).subscribe(
                (s) => {
                    if (s.success) {
                        this.formData = s.data;
                    }
                }
            );
            this.app.GetData('/Warehouses/GetWarehouseListByMaterialBasic/' + this.masterkey).subscribe(
                (s) => {
                    if (s.success) {
                        this.WarehouseList = s.data;
                        if (!this.btnVisibled) {
                            this.gridBoxValue = [];
                            this.WarehouseList.forEach(element => {
                                if (element.HasWarehouse) {
                                    this.gridBoxValue.push(element.Id);
                                }
                            });
                        }
                    }
                }
            );
        }
        this.app.GetData('/Warehouses/GetWarehouses').subscribe(
            (s) => {
                console.log(s);
                if (s.success) {
                    // debugger;
                    s.data.forEach(e => {
                        e.Name = e.Code + e.Name;
                    });
                    this.warehousesOptions = s.data;
                }
            }
        );
        this.app.GetData('/Suppliers/GetSuppliers').subscribe(
            (s) => {
                if (s.success) {
                    this.supplierList = s.data;
                    this.selectSupplier = {
                        items: this.supplierList,
                        displayExpr: 'Name',
                        valueExpr: 'Id',
                        searchEnabled: true
                    };
                }
            }
        );
    }
    ngOnInit() {
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
        // this.buttondisabled = true;
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }
        this.formData = this.myform.instance.option('formData');
        // this.postval = new Material();
        // this.postval = this.formData as Material;
        this.formData.wid = this.gridBoxValue;
        this.formData.warehouseData = this.warehousesOptions;
        // tslint:disable-next-line: max-line-length
        const sendRequest = await SendService.sendRequest(this.http, '/MaterialBasics/PostMaterial', 'POST', { values: this.formData });
        // let data = this.client.POST( this.url + '/OrderHeads/PostOrderMaster_Detail').toPromise();
        if (sendRequest) {
            this.myform.instance.resetValues();
            e.preventDefault();
            this.childOuter.emit(true);
        }
        this.buttondisabled = false;
    };

    asyncValidation(e) {
        const promise = new Promise((resolve, reject) => {
            this.app.GetData('/MaterialBasics/CheckMaterialNumber?DataNo=' + e.value).toPromise().then((res: APIResponse) => {
                resolve(res.success);
            },
                err => {
                    // Error
                    reject(err);
                }
            );
        });
        return promise;
    }
}
