import { Component, OnInit, Output, EventEmitter, Input, ViewChild, OnChanges } from '@angular/core';
import { DxFormComponent, DxDataGridComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { User } from 'src/app/model/viewmodels';
import { APIResponse } from 'src/app/app.module';
import { SendService } from 'src/app/shared/mylib';
import { Myservice } from 'src/app/service/myservice';
import CustomStore from 'devextreme/data/custom_store';
import { AppComponent } from 'src/app/app.component';

@Component({
    selector: 'app-creatuser',
    templateUrl: './creatuser.component.html',
    styleUrls: ['./creatuser.component.css']
})
export class CreatuserComponent implements OnInit, OnChanges {
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
    // MENU
    Controller = '/Users';
    dataSourceDB: any;
    editOnkeyPress: boolean;
    enterKeyAction: string;
    enterKeyDirection: string;
    creatuser: any = {};
    // passwordComparison = () => {
    //     return this.myform.instance.option('formData').Password;
    // }
    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent) {
        this.formData = null;
        // this.editOnkeyPress = true;
        // this.enterKeyAction = 'moveFocus';
        // this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 100;
        this.colCount = 2;
        this.listPermission = myservice.getPermission();
        this.listDepartment = myservice.getDepartment();
        // MENU
        this.editOnkeyPress = true;
        this.enterKeyAction = 'moveFocus';
        this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.app.GetData('/Users/GetUsersMenu').subscribe(
            (s) => {
                debugger;
                this.dataSourceDB = s.data;
            }
        );
        //this.dataSourceDB =  SendService.sendRequest(this.http, this.Controller + '/GetUsersMenu');
    }
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
    onValueChanged(e, data) {
        data.setValue(e.value);
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
    onFormSubmit = async function (e) {
        debugger;
        // this.buttondisabled = true;
        this.formData = this.myform.instance.option('formData');
        this.dataGrid.instance.saveEditData();
        const db = this.dataSourceDB;
        if (this.passwordComparison(this.formData) === false) {
            return;
        }
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }
        // this.formData = this.myform.instance.option('formData');
        // this.postval = new User();
        // this.postval = this.formData as User;
        // tslint:disable-next-line: max-line-length

        this.creatuser.user = this.formData;
        this.creatuser.MenuList = this.dataSourceDB;
        const sendRequest = await SendService.sendRequest(this.http, '/Users/PostUser', 'POST', { values: this.creatuser });
        // const sendRequest = true;
        // let data = this.client.POST( this.url + '/OrderHeads/PostOrderMaster_Detail').toPromise();
        if (sendRequest) {
            this.myform.instance.resetValues();
            e.preventDefault();
            this.childOuter.emit(true);
        }
        this.buttondisabled = false;

    };
    onInitNewRow(e) {

    }
    onFocusedCellChanging(e) {
    }
    onCellPrepared(e) {
        if (e.column.command === 'edit') {

        }
    }
    onEditingStart(e) {

    }
}
