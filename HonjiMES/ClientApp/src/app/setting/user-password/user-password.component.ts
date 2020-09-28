import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/service/auth.service';
import { first } from 'rxjs/operators';
import { DxFormComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { LoginUser } from 'src/app/model/loginuser';
import { APIResponse } from 'src/app/app.module';
import { SendService } from 'src/app/shared/mylib';
import Swal from 'sweetalert2';
import { AppComponent } from 'src/app/app.component';
import { HttpClient } from '@angular/common/http';

@Component({
    selector: 'app-user-password',
    templateUrl: './user-password.component.html',
    styleUrls: ['./user-password.component.css']
})
export class UserPasswordComponent implements OnInit {

    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    buttondisabled = false;
    labelLocation: string;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    colCount: number;
    width: any;
    returnUrl: any;
    Controller = '/Home';
    formData: { Password: string, CheckPassword: string };
    constructor(private http: HttpClient, public app: AppComponent) {
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 100;
        this.colCount = 1;
    }
    ngOnInit() {
    }
    onFormSubmit = async function(e) {
        debugger;
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }
        this.formData = this.myform.instance.option('formData');
        if (this.formData.Password !== this.formData.CheckPassword) {
            Swal.fire({
                allowEnterKey: false,
                allowOutsideClick: false,
                title: '密碼輸入不一致',
                icon: 'warning',
                timer: 3000
            });
            return;
        } else {
            // const Id = this.app.GetUserID();
            const sendRequest = await SendService.sendRequest(this.http, this.Controller + '/PutPassword', 'PUT',
            { key: 1 , values: this.formData});
            if (sendRequest) {
                Swal.fire({
                    allowEnterKey: false,
                    allowOutsideClick: false,
                    title: '密碼更新成功',
                    icon: 'success',
                    timer: 3000
                });
            }
        }
    };
    validate_before(): boolean {
        // 表單驗證
        if (this.myform.instance.validate().isValid === false) {
            notify({
                message: '請注意內容必填的欄位',
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'error', 3000);
            return false;
        }
        return true;
    }

}
