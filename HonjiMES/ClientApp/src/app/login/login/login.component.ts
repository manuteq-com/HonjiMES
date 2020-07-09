import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/service/auth.service';
import { first } from 'rxjs/operators';
import { DxFormComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { AppComponent } from 'src/app/app.component';
import { LoginUser } from 'src/app/model/loginuser';
import { APIResponse } from 'src/app/app.module';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    buttondisabled = false;
    formData: any;
    labelLocation: string;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    colCount: number;
    width: any;
    returnUrl: any;
    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private authService: AuthService,
        public app: AppComponent
    ) {
        // 已登入的狀態下直接到首頁
        if (this.authService.currentUserValue) {
            this.router.navigate(['/']);
        }
        this.formData = null;
        // this.editOnkeyPress = true;
        // this.enterKeyAction = 'moveFocus';
        // this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 100;
        this.colCount = 1;
    }


    ngOnInit() {
        // get return url from route parameters or default to '/'
        this.returnUrl = this.route.snapshot.queryParams.returnUrl || '/';
    }
    validate_before(): boolean {
        // 表單驗證
        if (this.myform.instance.validate().isValid === false) {
            notify({
                message: '請注必填的欄位',
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
        // return this.http.post<any>(`/users/authenticate`, { loginUser })
        //     .pipe(map(user => {
        //         // store user details and jwt token in local storage to keep user logged in between page refreshes
        //         localStorage.setItem('currentUser', JSON.stringify(user));
        //         this.currentUserSubject.next(user);
        //         return user;
        //     }));
        this.formData = this.myform.instance.option('formData');
        this.buttondisabled = true;
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }
        this.app.PostData('/api/Home/SingIn/', this.formData).toPromise()
            .then((ReturnData: any) => {
                debugger;
                if (ReturnData.success) {
                    this.app.UserName = ReturnData.data.Username;
                    localStorage.setItem('currentUser', JSON.stringify(ReturnData.data));
                    this.authService.currentUserSubject.next(ReturnData.data);
                    this.router.navigate([this.returnUrl]);
                    this.app.isLoggedIn();
                } else {
                    notify({
                        message: '帳號密碼錯誤',
                        position: {
                            my: 'center top',
                            at: 'center top'
                        }
                    }, 'error');
                    this.buttondisabled = false;
                }
            })
            .catch((e) => {
                notify({
                    message: e,
                    position: {
                        my: 'center top',
                        at: 'center top'
                    }
                }, 'error');

            });
    };

}
