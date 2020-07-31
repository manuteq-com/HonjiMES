import { Component, OnInit, EventEmitter, Output, Input, ViewChild, OnChanges } from '@angular/core';
import { DxFormComponent, DxDataGridComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { APIResponse } from '../../app.module';
import { SendService } from '../../shared/mylib';
import { AppComponent } from 'src/app/app.component';

@Component({
    selector: 'app-creatwiproduct-basic',
    templateUrl: './creatwiproduct-basic.component.html',
    styleUrls: ['./creatwiproduct-basic.component.css']
})
export class CreatwiproductBasicComponent implements OnInit, OnChanges {
    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() exceldata: any;
    @Input() modval: any;
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

    constructor(private http: HttpClient, private app: AppComponent) {
        this.formData = null;
        // this.editOnkeyPress = true;
        // this.enterKeyAction = 'moveFocus';
        // this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 100;
        this.colCount = 2;
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
        this.app.GetData('/Warehouses/GetWarehouses').subscribe(
            (s) => {
                console.log(s);
                if (s.success) {
                    this.warehousesOptions = s.data;
                }
            }
        );

    }

    ngOnInit() {
    }
    ngOnChanges() {
        // debugger;
        this.NumberBoxOptions = { showSpinButtons: true, mode: 'number', min: 0, value: 0 };
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
        const sendRequest = await SendService.sendRequest(this.http, '/WiproductBasics/PostWiproduct', 'POST', { values: this.formData });
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
            this.app.GetData('/WiproductBasics/CheckWiproductNumber?DataNo=' + e.value).toPromise().then((res: APIResponse) => {
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
