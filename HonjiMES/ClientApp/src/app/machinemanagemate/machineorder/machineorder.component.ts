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
                debugger;
                setInterval(() => {
                    this.dataSourceDB.forEach(x => {
                        if (x.RemainingTime > 0) {
                            x.RemainingTime--;
                            x.TotalTime--;
                        }
                        if ((x.DelayTime > 0) || (x.RemainingTime <= 0 && x.No)) {
                            x.DelayTime++;
                        }
                    });
                }, 1000)
            }
        );
    }

    // 已安排剩餘時間小於100min 畫面顯示紅色
    getClass(data) {
        if (data < 100) {
            return 'Alert';
        }
    }

    //已安排製程數等於0 字體顯示紅色
    sendAlert(data) {
        if (data == 0) {
            return 'Alert-text'
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
    formattime(secondTime) {
        var minuteTime = 0;// 分
        var hourTime = 0;// 小時
        if (secondTime > 60) {//如果秒數大於60，將秒數轉換成整數
            //獲取分鐘，除以60取整數，得到整數分鐘
            minuteTime = Math.floor(secondTime / 60);
            //獲取秒數，秒數取佘，得到整數秒數
            secondTime = Math.floor(secondTime % 60);
            //如果分鐘大於60，將分鐘轉換成小時
            if (minuteTime > 60) {
                //獲取小時，獲取分鐘除以60，得到整數小時
                hourTime = Math.floor(minuteTime / 60);
                //獲取小時後取佘的分，獲取分鐘除以60取佘的分
                minuteTime = Math.floor(minuteTime % 60);
            }
        }
        var resultTime = "" + Math.floor(secondTime) + "秒";
        if (minuteTime > 0) {
            resultTime = "" + Math.floor(minuteTime) + "分" + resultTime;
        }
        if (hourTime > 0) {
            resultTime = "" + Math.floor(hourTime) + "時" + resultTime;
        }
        return resultTime;
    }


}
