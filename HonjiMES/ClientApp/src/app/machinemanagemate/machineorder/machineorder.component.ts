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
                    this.dataSourceDB = s.data;
            }
        );
    }

    // 已安排剩餘時間小於100min 畫面顯示紅色
    getClass(data){
        if (data < 100) {
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
