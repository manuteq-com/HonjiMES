import { Component, OnInit } from '@angular/core';
import { AppComponent } from 'src/app/app.component';

@Component({
    selector: 'app-machineorder',
    templateUrl: './machineorder.component.html',
    styleUrls: ['./machineorder.component.css']
})
export class MachineorderComponent implements OnInit {
    dataSourceDB: any;
    creatpopupVisible: any;
    editVisible: boolean;
    btnDisabled: boolean;
    itemkey: number;
    randomkey: number;
    mod: string;


    constructor(public app: AppComponent) {
        this.editVisible = true;
        this.btnDisabled = false;
     }

    ngOnInit() {
        this.app.GetData('/MachineManagement/GetMachineData').subscribe(
            (s) => {
                debugger;
                if(s.data[0].MachineName == null){
                    this.dataSourceDB = s.data;
                    this.dataSourceDB.splice(0,1);
                }else {
                    this.dataSourceDB = s.data;
                    this.dataSourceDB.machineOrderList.splice(0,1);
                }

            }
        );
    }

    // 製程數小於5顯示紅色
    getClass(data){
        if (data < 2) {
            return 'Alert';
        }
    }

    //機台詳情
    viewProcess() {
        this.creatpopupVisible = true;
        // this.itemkey = this.workOrderHeadId;
        this.mod = 'view';
        this.randomkey = new Date().getTime();
    }

    //機台詳情頁面關閉後
    creatpopup_result(e) {
        this.creatpopupVisible = false;
        this.itemkey = null;
        this.dataSourceDB.instance.refresh();
        // if (this.workOrderHeadId !== undefined) {
        //     this.readProcess(null, this.workOrderHeadId);
    }



}
