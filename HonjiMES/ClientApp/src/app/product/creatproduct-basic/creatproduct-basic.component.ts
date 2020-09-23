import { Component, OnInit, EventEmitter, Output, Input, ViewChild, OnChanges } from '@angular/core';
import { DxFormComponent, DxDataGridComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { APIResponse } from '../../app.module';
import { SendService } from '../../shared/mylib';
import { Product } from '../../model/viewmodels';
import { AppComponent } from 'src/app/app.component';

@Component({
    selector: 'app-creatproduct-basic',
    templateUrl: './creatproduct-basic.component.html',
    styleUrls: ['./creatproduct-basic.component.css']
})
export class CreatproductBasicComponent implements OnInit, OnChanges {
    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() exceldata: any;
    @Input() modval: any;
    @Input() masterkey: any;
    @Input() editStatus: any;
    @Input() btnVisibled: boolean;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    buttondisabled = false;
    formData: any;
    labelLocation: string;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    colCount: number;
    width: any;
    selectBoxOptions: any;
    warehousesOptions: any;
    NumberBoxOptions: any;
    gridBoxValue: number[] = [2];
    WarehouseList: any;
    selectType: { items: any; displayExpr: string; valueExpr: string; searchEnabled: boolean; };

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
        this.app.GetData('/MaterialBasics/GetMaterialBasicsAsc').subscribe(
            (s) => {
                console.log(s);
                if (s.success) {
                    this.selectBoxOptions = {
                        items: s.data,
                        displayExpr: 'MaterialNo',
                        valueExpr: 'Id',
                        searchEnabled: true
                    };
                }
            }
        );
    }
    ngOnInit() {
    }
    ngOnChanges() {
        this.readOnly = true;
        if (this.editStatus !== null) {
            this.readOnly = this.editStatus;
        }
        this.gridBoxValue = [2];

        this.NumberBoxOptions = { showSpinButtons: true, mode: 'number', min: 0, value: 0 };
        this.formData = {
            ProductNo: '',
            Name: '',
            // Quantity: '',
            Specification: '',
            Property: '',
            Price: 0,
            Unit: '',
            ProductNumber: ''
        };
        if (this.masterkey !== null) {
            debugger;
            this.app.GetData('/ProductBasics/GetProductBasic/' + this.masterkey).subscribe(
                (s) => {
                    if (s.success) {
                        this.formData = s.data;
                    }
                }
            );
            this.app.GetData('/Warehouses/GetWarehouseListByProductBasic/' + this.masterkey).subscribe(
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
        // this.buttondisabled = true;
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }
        this.formData = this.myform.instance.option('formData');
        this.formData.wid = this.gridBoxValue;
        this.formData.warehouseData = this.warehousesOptions;
        // tslint:disable-next-line: max-line-length
        const sendRequest = await SendService.sendRequest(this.http, '/ProductBasics/PostProduct', 'POST', { values: this.formData });
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
            this.app.GetData('/ProductBasics/CheckProductNumber?DataNo=' + e.value).toPromise().then((res: APIResponse) => {
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
