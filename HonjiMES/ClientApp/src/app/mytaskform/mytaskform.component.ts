import { Component, OnInit, ViewChild } from '@angular/core';
import { MyTask } from '../model/mytask';
import { FormControl } from '@angular/forms';
import { SwalComponent } from '@sweetalert2/ngx-sweetalert2';
import { MytaskService } from '../service/mytask.service';
import { Router } from '@angular/router';
import { Guid } from '../shared/mylib';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { APIResponse } from '../app.module';

@Component({
    selector: 'app-mytaskform',
    templateUrl: './mytaskform.component.html',
    styleUrls: ['./mytaskform.component.css']
})
export class MytaskformComponent implements OnInit {
    @ViewChild('taskform') taskform: FormControl;
    @ViewChild('okSwal') okSwal: SwalComponent;
    @ViewChild('failSwal') failSwal: SwalComponent;
    currentTask: MyTask = new MyTask();
    currentGuid = '';
    Upload_API_Url = '/';

    fvalue: any[] = [];
    constructor(private sv: MytaskService, private router: Router, private http: HttpClient) {
        this.currentTask.sn = -1;
        this.currentGuid = Guid.newGuid();
        this.currentTask.gid = this.currentGuid;
    }

    ngOnInit() {

    }
    submit() {
        if (this.taskform.valid === false) {

        }

        this.currentTask.closed = 0;
        //  this.newTask.closetimes = new Date();
        this.sv.insert_tasks(this.currentTask).subscribe(
            (r) => {

                if (r.success) {

                    if (this.fvalue.length === 0) {
                        this.okSwal.fire();
                        this.router.navigate(['/taskview']);
                        return;
                    }
                    this.uploadImage();

                } else {
                    this.failSwal.fire();

                }

            }
        );
    }
    onValueChanged(e) {
        if (this.fvalue.length === 0) { return; } // 沒檔離開
        const url = URL.createObjectURL(this.fvalue[0]);
        const img = new Image();
        img.onload = () => {
            // const w = img.width;
            // const h = img.height;
            // this.imagesize = img.width + ' * ' + img.height;
            // if (w > 800 || w < 760 || h > 800 || h < 760) {
            //     this.alertmsg = '檔案不符合尺寸, 僅接受 800 * 800 的圖面';
            // } else {
            //     this.alertmsg = '';
            // }

        };
        img.src = url;

    }


    testupload() {
        this.uploadImage();
    }
    uploadImage() {
        if (this.fvalue.length === 0) { return; } // 沒檔離開
        const formData: FormData = new FormData();
        formData.append('Image', this.fvalue[0], this.fvalue[0].name);
        formData.append('GID', this.currentGuid);
        const apihost = environment.apihost;
        this.http.post<APIResponse>(apihost + 'api/UpdateImage/Goo', formData).subscribe(
            (x) => {
                console.log(JSON.stringify(x));
                if (x.success === true) {
                    this.okSwal.fire();
                    this.router.navigate(['/taskview']);
                } else {
                    // console.log(x.message);
                }
            }
        );

    }
}
