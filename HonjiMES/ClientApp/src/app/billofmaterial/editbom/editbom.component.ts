import { Component, OnInit, EventEmitter, Output, Input, ViewChild, OnChanges } from '@angular/core';
import { DxFormComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { HttpClient } from '@angular/common/http';
import { SendService } from 'src/app/shared/mylib';
import { Observable } from 'rxjs';
import { APIResponse } from 'src/app/app.module';
import { Myservice } from 'src/app/service/myservice';

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
    PostBom: { BasicType: number, BasicId: number, Quantity: number, Name: string };
    QuantityEditorOptions: { showSpinButtons: boolean; mode: string; format: string; value: number; min: number; };

    public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>('/api' + apiUrl);
    }
    constructor(private http: HttpClient, myservice: Myservice) {
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
        this.GetData('/BillOfMaterials/GetMaterialBasicsDrowDown').subscribe(
            (s) => {
                if (s.success) {
                    this.MaterialList = s.data;
                }
            }
        );
        this.GetData('/BillOfMaterials/GetProductBasicsDrowDown').subscribe(
            (s) => {
                if (s.success) {
                    this.ProductList = s.data;
                }
            }
        );
        this.QuantityEditorOptions = {
            showSpinButtons: true,
            mode: 'number',
            format: '#0',
            value: 1,
            min: 1
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
        if (e.value === 1) {
            this.editorOptions = {
                dataSource: { paginate: true, store: { type: 'array', data: this.MaterialList, key: 'Id' } },
                searchEnabled: true,
                // items: this.MaterialList,
                displayExpr: 'Name',
                valueExpr: 'Id',

            };
        } else if (e.value === 2) {
            this.editorOptions = {
                dataSource: { paginate: true, store: { type: 'array', data: this.ProductList, key: 'Id' } },
                searchEnabled: true,
                // items: this.ProductList,
                displayExpr: 'Name',
                valueExpr: 'Id',
            };
        } else {
            this.editorOptions.items = [];
        }

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
        this.buttondisabled = false;
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

        this.buttondisabled = false;

    }
}
