import { Component, OnInit, Output, EventEmitter, Input, ViewChild } from '@angular/core';
import { DxFormComponent, DxDataGridComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Material } from 'src/app/model/viewmodels';
import { APIResponse } from 'src/app/app.module';
import { SendService } from 'src/app/shared/mylib';

@Component({
    selector: 'app-creatmaterial',
    templateUrl: './creatmaterial.component.html',
    styleUrls: ['./creatmaterial.component.css']
})
export class CreatmaterialComponent implements OnInit {
    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() exceldata: any;
    @Input() modval: any;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    buttondisabled: false;
    formData: any;
    postval: Material;
    labelLocation: string;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    colCount: number;
    width: any;
    selectBoxOptions: any;
    url: string;
    buttonOptions: any = {
        text: '存檔',
        type: 'success',
        useSubmitBehavior: true,
        icon: 'save'
    };
    constructor(private http: HttpClient) {
        this.formData = null;
        // this.editOnkeyPress = true;
        // this.enterKeyAction = 'moveFocus';
        // this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 100;
        this.colCount = 2;
        this.url = location.origin + '/api';
        this.GetData(this.url + '/Suppliers/GetSuppliers').subscribe(
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
     public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>(apiUrl);
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
    onFormSubmit = async function(e) {
        this.buttondisabled = true;
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }
        this.formData = this.myform.instance.option('formData');
        this.postval = new Material();
        this.postval = this.formData as Material;
        // tslint:disable-next-line: max-line-length
        const sendRequest = await SendService.sendRequest(this.http, '/Materials/PostMaterial', 'POST', { values:  this.postval });
        // let data = this.client.POST( this.url + '/OrderHeads/PostOrderMaster_Detail').toPromise();
        if (sendRequest) {
            this.myform.instance.resetValues();
            e.preventDefault();
            this.childOuter.emit(true);
        }
        this.buttondisabled = false;

    };
}
