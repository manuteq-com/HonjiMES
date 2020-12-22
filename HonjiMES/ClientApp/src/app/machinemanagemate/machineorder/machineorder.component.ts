import { Component, Input, OnInit } from '@angular/core';
import { AppComponent } from 'src/app/app.component';
import notify from 'devextreme/ui/notify';

@Component({
    selector: 'app-machineorder',
    templateUrl: './machineorder.component.html',
    styleUrls: ['./machineorder.component.css']
})
export class MachineorderComponent implements OnInit {
    dataSourceDB: any;
    creatpopupVisible: any;
    popupVisibleWorkorderList: any;
    editVisible: boolean;
    btnDisabled: boolean;
    orderbtnDisabled: boolean;
    checkVisible: boolean;
    itemkey: any;
    itemtdkey: any;
    serialkey: number;
    randomkey: number;
    mod: string;
    ReportHeight:number;
    loadingVisible = false;


    constructor(public app: AppComponent) {
        this.editVisible = true;
        this.btnDisabled = false;
        this.orderbtnDisabled = true;
        this.loadingVisible = true;
        this.ReportHeight = 750;
    }

    ngOnInit() {
        this.app.GetData('/MachineManagement/GetMachineData').subscribe(
            (s) => {
                this.dataSourceDB = s.data;
                debugger;
                // this.dataSourceDB.forEach(x => {
                //     x.RemainingTime = x.RemainingTime * 60;
                //     x.TotalTime = x.TotalTime * 60;
                //     x.DelayTime = x.DelayTime * 60;
                // });
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
                }, 60000)
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

    //製程頁面
    viewWorkorderList(data) {
        debugger;
        this.popupVisibleWorkorderList = true;
        this.itemtdkey = data.Id;
        this.serialkey = data.SerialNumber;
        // this.getWorkOrderData();
    }

    // getWorkOrderData() {
    //     this.app.GetData('/Processes/GetWorkOrderByMode/1').subscribe(
    //         (s) => {
    //             this.dataSourceDB = s.data;
    //             this.loadingVisible = false;
    //         }
    //     );
    // }

    //機台詳情、製程頁面關閉後
    creatpopup_result(e) {
        this.creatpopupVisible = false;
        this.popupVisibleWorkorderList = false;
        this.itemkey = null;
        this.checkVisible = false;
        this.loadingVisible = true;
        this.dataSourceDB.instance.refresh();
        // this.getWorkOrderData();
        this.showMessage('success', '更新完成', 3000);
        // if (this.workOrderHeadId !== undefined) {
        //     this.readProcess(null, this.workOrderHeadId);
    }

    formatsecondTime(secondTime) {
        var minuteTime = 0;// 分
        var hourTime = 0;// 小時
        var days = 0;// 天
        if (secondTime > 60) {//如果秒數大於60，將秒數轉換成整數
            //獲取分鐘，除以60取整數，得到整數分鐘
            minuteTime = Math.floor(secondTime / 60);
            //獲取秒數，秒數取佘，得到整數秒數
            secondTime = Math.floor(secondTime % 60);
        }
        //如果分鐘大於60，將分鐘轉換成小時
        if (minuteTime > 60) {
            //獲取小時，獲取分鐘除以60，得到整數小時
            hourTime = Math.floor(minuteTime / 60);
            //獲取小時後取佘的分，獲取分鐘除以60取佘的分
            minuteTime = Math.floor(minuteTime % 60);
        }
        //如果小時大於24，將小時轉成天數
        if (hourTime > 24) {
            days = Math.floor(hourTime / 24);
            hourTime = Math.floor(hourTime % 24);
        }
        var resultTime = "" + Math.floor(secondTime) + "秒";
        if (minuteTime > 0) {
            resultTime = "" + Math.floor(minuteTime) + "分" + resultTime;
        }
        if (hourTime > 0) {
            resultTime = "" + Math.floor(hourTime) + "時" + resultTime;
        }
        if (days > 0) {
            resultTime = "" + Math.floor(days) + "天" + resultTime;
        }
        return resultTime;
    }
    formattime(minuteTime) {// 分鐘轉小時和天
        var workhoue = 11; // 不用24的原因是因為他們的工作時間只有11小時
        var hourTime = 0;// 小時
        var days = 0;// 天
        //如果分鐘大於60，將分鐘轉換成小時
        if (minuteTime > 60) {
            //獲取小時，獲取分鐘除以60，得到整數小時
            hourTime = Math.floor(minuteTime / 60);
            //獲取小時後取佘的分，獲取分鐘除以60取佘的分
            minuteTime = Math.floor(minuteTime % 60);
        }
        //如果小時大於11，將小時轉成天數
        // if (hourTime > workhoue) {
        //     days = Math.floor(hourTime / workhoue);
        //     hourTime = Math.floor(hourTime % workhoue);
        // }
        var resultTime = "";
        if (minuteTime > 0) {
            resultTime = "" + Math.floor(minuteTime) + "分" + resultTime;
        }
        if (hourTime > 0) {
            resultTime = "" + Math.floor(hourTime) + "時" + resultTime;
        }
        // if (days > 0) {
        //     resultTime = "" + Math.floor(days) + "天" + resultTime;
        // }
        return resultTime;
    }

    showMessage(type, data, val) {
        notify({
            message: data,
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, type, val);
    }

}
