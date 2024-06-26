import { Component, OnInit, EventEmitter, Output, Input, ViewChild, OnChanges } from '@angular/core';
import { DxFormComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { HttpClient } from '@angular/common/http';
import { SendService } from 'src/app/shared/mylib';
import { Observable } from 'rxjs';
import { APIResponse } from 'src/app/app.module';
import { Myservice } from 'src/app/service/myservice';
import { AppComponent } from 'src/app/app.component';
import { Title } from '@angular/platform-browser';


@Component({
  selector: 'app-create-bom',
  templateUrl: './create-bom.component.html',
  styleUrls: ['./create-bom.component.css']
})
export class CreateBomComponent implements OnInit {

    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
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
    MaterialList: any;
    ProductList: any;
    PostBom: { Id: number, TempId: number, BasicType: number, BasicId: number, Quantity: number, Name: string,
        NameType: string, NameNo: string, ReceiveQty: number, RQty: number, WarehouseId: number, WarehouseList: []
        MaterialBasicId: number, ProductBasicId: number};
    QuantityEditorOptions: { showSpinButtons: boolean; mode: string; format: string; value: number; min: number; };
    BasicDataList: any;
    NameVisible: boolean;
    selectData: any;

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent, private titleService: Title) {
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 100;
        this.colCount = 1;
        this.app.GetData('/BillOfMaterials/GetMaterialBasicsHaveAny').subscribe(
            (s) => {
                debugger;
                s.data.data.forEach(element => {
                    element.Name = element.MaterialNo + '_' + element.Name;
                });
                this.selectData = {
                    dataSource: { paginate: true, store: { type: 'array', data: s.data.data, key: 'Id' } },
                    searchEnabled: true,
                    displayExpr: 'Name',
                    valueExpr: 'Id',
                };
            }
        );
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
                        onValueChanged: this.onSelectionChanged.bind(this)
                    };
                }
            }
        );
        this.QuantityEditorOptions = {
            showSpinButtons: true,
            mode: 'number',
            format: '#0.000',
            value: 1,
            min: 0.001
        };
    }
    onSelectionChanged(e){
        debugger;
        this.formData = this.myform.instance.option('formData');
        this.BasicDataList.forEach(element => {
            debugger;
            if(element.TempId == e.value){
                this.formData.Name = element.Name;
            }
        });
    }
    ngOnInit() {
        this.titleService.setTitle('快速建BOM');
    }
    ngOnChanges() {
    }
    onComponentSelectionChanged(e) {
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
        const sendRequest = await SendService.sendRequest(
            this.http,
            '/BillOfMaterials/PostBomlist/' + this.PostBom.Id,'POST',{ values: this.PostBom });
        if (sendRequest) {
            this.myform.instance.resetValues();
            e.preventDefault();
        }
        this.buttondisabled = false;
        notify({
            message: '更新完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
    }
}
