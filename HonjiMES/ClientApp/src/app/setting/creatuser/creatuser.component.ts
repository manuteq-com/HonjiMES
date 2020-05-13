import { Component, OnInit, Output, EventEmitter, Input, ViewChild } from '@angular/core';
import { DxFormComponent, DxDataGridComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { User } from 'src/app/model/viewmodels';
import { APIResponse } from 'src/app/app.module';
import { SendService } from 'src/app/shared/mylib';
import { Myservice } from 'src/app/service/myservice';

@Component({
  selector: 'app-creatuser',
  templateUrl: './creatuser.component.html',
  styleUrls: ['./creatuser.component.css']
})
export class CreatuserComponent implements OnInit {
    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() exceldata: any;
    @Input() modval: any;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    buttondisabled = false;
    formData: any;
    postval: User;
    labelLocation: string;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    colCount: number;
    width: any;
    url: string;
    listPermission: any;
    listDepartment: any;
    selectBoxOptionsPermission: any;
    selectBoxOptionsDepartment: any;
    buttonOptions: any = {
        text: '存檔',
        type: 'success',
        useSubmitBehavior: true,
        icon: 'save'
    };
    // passwordComparison = () => {
    //     return this.myform.instance.option('formData').Password;
    // }
    constructor(private http: HttpClient, myservice: Myservice) {
        this.formData = null;
        // this.editOnkeyPress = true;
        // this.enterKeyAction = 'moveFocus';
        // this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 100;
        this.colCount = 1;
        this.url = location.origin + '/api';
        this.listPermission = myservice.getPermission();
        this.listDepartment = myservice.getDepartment();
    }
    public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>(apiUrl);
    }
    // tslint:disable-next-line: use-lifecycle-interface
    ngOnChanges() {
        this.selectBoxOptionsPermission = {
            items: this.listPermission,
            displayExpr: 'Name',
            valueExpr: 'Id',
        };
        this.selectBoxOptionsDepartment = {
            items: this.listDepartment,
            displayExpr: 'Name',
            valueExpr: 'Id',
        };
    }
    ngOnInit() {
    }
    passwordComparison(data): boolean {
        if (data.Password !== data.PasswordConfirm) {
            notify({
                message: '輸入密碼不相符',
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'error', 3000);
            return false;
        }
        return true;
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
        // debugger;
        // this.buttondisabled = true;
        this.formData = this.myform.instance.option('formData');
        if (this.passwordComparison(this.formData) === false) {
            return;
        }
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }
        this.formData = this.myform.instance.option('formData');
        // this.postval = new User();
        // this.postval = this.formData as User;
        // tslint:disable-next-line: max-line-length
        const sendRequest = await SendService.sendRequest(this.http, '/Users/PostUser', 'POST', { values:  this.formData });
        // let data = this.client.POST( this.url + '/OrderHeads/PostOrderMaster_Detail').toPromise();
        if (sendRequest) {
            this.myform.instance.resetValues();
            e.preventDefault();
            this.childOuter.emit(true);
        }
        this.buttondisabled = false;

    };

}
