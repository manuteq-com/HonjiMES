import { Component, OnInit, EventEmitter, Output, Input, ViewChild, OnChanges } from '@angular/core';
import { DxFormComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { HttpClient } from '@angular/common/http';
import { SendService } from 'src/app/shared/mylib';
import { Observable } from 'rxjs';
import { APIResponse } from 'src/app/app.module';
import { Myservice } from 'src/app/service/myservice';
import { AppComponent } from 'src/app/app.component';

@Component({
    selector: 'app-editbom',
    templateUrl: './editbom.component.html',
    styleUrls: ['./editbom.component.css']
})
export class EditbomComponent implements OnInit, OnChanges {
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() modval: any;
    @Input() randomkeyval: any;
    buttondisabled = false;
    labelLocation: string;
    formData: any;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    width: any;
    colCount: number;
    postval: any;
    editorOptions: any;
    ComponentList: any;
    MaterialList: any;
    ProductList: any;
    PostBom: { Id: number, TempId: number, BasicType: number, BasicId: number, Quantity: number, Name: string,
        NameType: string, NameNo: string, ReceiveQty: number, RQty: number, WarehouseId: number, WarehouseList: []
        MaterialBasicId: number, ProductBasicId: number};
    QuantityEditorOptions: { showSpinButtons: boolean; mode: string; format: string; value: number; min: number; };
    BasicDataList: any;
    NameVisible: boolean;

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent) {
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 300;
        this.colCount = 1;
        this.ComponentList = {
            items: myservice.getComponent(),
            displayExpr: 'Name',
            valueExpr: 'Id',
            onValueChanged: this.onComponentSelectionChanged.bind(this)
        };
    }

    ngOnInit() {
    }
    ngOnChanges() {
        if (this.modval === 'receive') {
            this.NameVisible = false;
        } else {
            this.NameVisible = true;
        }
        this.app.GetData('/Inventory/GetBasicsData').subscribe(
            (s) => {
                if (s.success) {
                    s.data.forEach(element => {
                        element.Name = element.DataNo + '_' + element.Name;
                    });
                    this.BasicDataList = s.data;
                    this.editorOptions = {
                        dataSource: { paginate: true, store: { type: 'array', data: this.BasicDataList, key: 'TempId' } },
                        searchEnabled: true,
                        displayExpr: 'Name',
                        valueExpr: 'TempId',
                    };
                }
            }
        );
        // this.app.GetData('/BillOfMaterials/GetMaterialBasicsDrowDown').subscribe(
        //     (s) => {
        //         if (s.success) {
        //             this.MaterialList = s.data;
        //         }
        //     }
        // );
        // this.app.GetData('/BillOfMaterials/GetProductBasicsDrowDown').subscribe(
        //     (s) => {
        //         if (s.success) {
        //             this.ProductList = s.data;
        //         }
        //     }
        // );
        this.QuantityEditorOptions = {
            showSpinButtons: true,
            mode: 'number',
            format: '#0.000',
            value: 1,
            min: 0.001
        };
        // const remote = true;
        // this.MaterialList = new CustomStore({
        //     key: 'Id',
        //     load: (loadOptions) => SendService.sendRequest
        //         (this.http,
        //             '/BillOfMaterials/GetMaterialBasicsDrowDown',
        //             'GET',
        //             { loadOptions, remote }),
        // });
    }
    onComponentSelectionChanged(e) {
        // if (e.value === 1) {
        //     this.editorOptions = {
        //         dataSource: { paginate: true, store: { type: 'array', data: this.MaterialList, key: 'Id' } },
        //         searchEnabled: true,
        //         // items: this.MaterialList,
        //         displayExpr: 'Name',
        //         valueExpr: 'Id',
        //     };
        // } else if (e.value === 2) {
        //     this.editorOptions = {
        //         dataSource: { paginate: true, store: { type: 'array', data: this.ProductList, key: 'Id' } },
        //         searchEnabled: true,
        //         // items: this.ProductList,
        //         displayExpr: 'Name',
        //         valueExpr: 'Id',
        //     };
        // } else {
        //     this.editorOptions.items = [];
        // }
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
    async onFormSubmit(e) {
        debugger;
        this.buttondisabled = true;
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }
        this.PostBom = this.myform.instance.option('formData');
        const basicData = this.BasicDataList.find(z => z.TempId === this.PostBom.TempId);
        this.PostBom.BasicType = basicData.DataType;
        this.PostBom.BasicId = basicData.DataId;
        this.buttondisabled = false;
        if (this.modval === 'receive') {
            try {
                const sendRequest = await SendService.sendRequest(
                    this.http,
                    '/Requisitions/GetWarehouseByPostBom',
                    'POST',
                    { values: this.PostBom });
                if (sendRequest) {
                    e.preventDefault();
                    this.PostBom.Id = basicData.TempId;
                    this.PostBom.NameType = basicData.DataType === 1 ? '元件' : basicData.DataType === 2 ? '成品' : '';
                    this.PostBom.MaterialBasicId = basicData.DataType === 1 ? basicData.DataId : null;
                    this.PostBom.ProductBasicId = basicData.DataType === 2 ? basicData.DataId : null;
                    this.PostBom.NameNo = basicData.DataNo;
                    this.PostBom.Quantity = 1;
                    this.PostBom.ReceiveQty = 0;
                    this.PostBom.RQty = 0;
                    this.PostBom.WarehouseId = basicData.WarehouseId;
                    this.PostBom.WarehouseList = sendRequest;
                    this.childOuter.emit(this.PostBom);
                    this.myform.instance.resetValues();
                }
            } catch (error) {

            }
        } else {
            try {
                const sendRequest = await SendService.sendRequest(
                    this.http,
                    '/BillOfMaterials/PostBomlist/' + this.itemkeyval,
                    'POST',
                    { values: this.PostBom });
                if (sendRequest) {
                    this.myform.instance.resetValues();
                    e.preventDefault();
                    this.childOuter.emit(true);
                }
            } catch (error) {

            }
        }

        this.buttondisabled = false;

    }
}
