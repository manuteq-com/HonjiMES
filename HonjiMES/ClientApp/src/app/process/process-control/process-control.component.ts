import { Component, OnInit } from '@angular/core';
import { AppComponent } from 'src/app/app.component';
import notify from 'devextreme/ui/notify';

@Component({
    selector: 'app-process-control',
    templateUrl: './process-control.component.html',
    styleUrls: ['./process-control.component.css']
})
export class ProcessControlComponent implements OnInit {
    dataSourceDB: any = {};
    loadingVisible = false;
    constructor(public app: AppComponent) {
<<<<<<< HEAD
        this.loadingVisible = true;
        this.app.GetData('/Processes/GetProcessesStatus/1').subscribe(
=======
        this.app.GetData('/Processes/GetWorkOrderByStatus/1').subscribe(
>>>>>>> 18f59a52ea635609e4e8a9893dfe9a5cc389e60f
            (s) => {
                this.dataSourceDB = s.data;
                this.loadingVisible = false;
            }
        );
        // this.app.GetData('/Processes/GetProcessesStatus/1').subscribe(
        //     (s) => {
        //         debugger;
        //         this.dataSourceDB = s.data;
        //     }
        // );

        // this.dataSourceDB.listOfData = [
        //     {
        //         key: '1',
        //         name: 'test1',
        //         age: 32,
        //         address: 'New York No. 1 Lake Park'
        //     },
        //     {
        //         key: '2',
        //         name: 'test2',
        //         age: 42,
        //         address: 'London No. 1 Lake Park'
        //     },
        //     {
        //         key: '3',
        //         name: 'test3',
        //         age: 32,
        //         address: 'Sidney No. 1 Lake Park'
        //     }
        // ];

        // this.dataSourceDB.columnOptions = [
        //     { key: 'name', title: '名稱', span: true },
        //     { key: 'No', title: '品號', span: false },
        //     { key: 'count', title: '數量', span: false },
        // ];
    }
    ngOnInit() {
    }
    tdclick(e) {
        notify({
            message: e.ProductNo,
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'error', 3000);
    }
    creatdata() {

    }
}
