import { HttpClient } from '@angular/common/http';
import { Component, Input, OnInit, NgZone } from '@angular/core';
import notify from 'devextreme/ui/notify';
import { AppComponent } from 'src/app/app.component';
import { HubMessage } from 'src/app/model/viewmodels';
import { SignalRService } from 'src/app/service/signal-r.service';

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
    checkVisible: boolean;
    itemkey: any;
    itemtdkey: any;
    serialkey: number;
    randomkey: number;
    mod: string;
    ReportHeight: number;
    loadingVisible = false;
    messages = new Array<HubMessage>();
    message = new HubMessage();
    uniqueID: string = new Date().getTime().toString();
    constructor(private SignalRService: SignalRService, private _ngZone: NgZone, private http: HttpClient, public app: AppComponent) {
        this.editVisible = true;
        this.btnDisabled = false;
        this.loadingVisible = true;
        this.ReportHeight = 800;
        this.subscribeToEvents();
        // this.startHttpRequest();
    }

    ngOnInit() {
        this.getdata();
        this.startInterval()
    }
    startInterval() {
        setInterval(() => {
            if (this.dataSourceDB) {
                this.dataSourceDB.forEach(x => {
                    if (x.RemainingTime > 0) {
                        x.RemainingTime--;
                        x.TotalTime--;
                    }
                    if ((x.DelayTime > 0) || (x.RemainingTime <= 0 && x.No)) {
                        x.DelayTime++;
                    }
                });
            }
        }, 60000)
    }
    getdata() {
        this.app.GetData('/MachineManagement/GetMachineData').subscribe(
            (s) => {
                this.dataSourceDB = s.data;
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
        this.mod = 'report';
    }
    //機台詳情、回報完工頁面關閉後
    creatpopup_result(e) {
        this.creatpopupVisible = false;
        this.popupVisibleWorkorderList = false;
        this.itemkey = null;
        this.checkVisible = false;
        this.loadingVisible = true;
        this.getdata();
        this.showMessage('success', '更新完成', 3000);
        // if (this.workOrderHeadId !== undefined) {
        //     this.readProcess(null, this.workOrderHeadId);
    }
    formattime(minuteTime) {
        //var minuteTime = 0;// 分
        var hourTime = 0;// 小時

        //如果分鐘大於60，將分鐘轉換成小時
        if (minuteTime > 60) {
            //獲取小時，獲取分鐘除以60，得到整數小時
            hourTime = Math.floor(minuteTime / 60);
            //獲取小時後取佘的分，獲取分鐘除以60取佘的分
            minuteTime = Math.floor(minuteTime % 60);
        }

        var resultTime = "";
        if (minuteTime > 0) {
            resultTime = "" + Math.floor(minuteTime) + "分" + resultTime;
        }
        if (hourTime > 0) {
            resultTime = "" + Math.floor(hourTime) + "時" + resultTime;
        }
        return resultTime;
    }
    formatsecondTime(secondTime) {
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
    private subscribeToEvents(): void {

        this.SignalRService.messageReceived.subscribe((message: HubMessage) => {
            this._ngZone.run(() => {
                if (message.clientuniqueid !== this.uniqueID) {
                    message.type = "received";
                    this.messages.push(message);
                    notify({
                        message: message.message,
                        position: {
                            my: 'center top',
                            at: 'center top'
                        }
                    }, 'warning', 6000);
                }
            });
        });
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
    //判斷完工按鈕是否顯示
    orderbtnDisabled(data) {
        debugger;
        if (data.No) {
            return true;
        } else {
            return false;
        }
    }
}
