import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { DxFormComponent } from 'devextreme-angular';
import moment from 'moment';
import { AppComponent } from 'src/app/app.component';
moment.locale('zh-tw');
@Component({
    selector: 'app-workorder-v2-list',
    templateUrl: './workorder-v2-list.component.html',
    styleUrls: ['./workorder-v2-list.component.css']
})
export class WorkorderV2ListComponent implements OnInit, OnDestroy {
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    stDate: Date;
    endDate: Date;
    fake: any;
    machines: string[];
    dataMax: number;
    dataSourceDB: any;
    refreshIntervalId: any;
    StaffList: any;
    ReportHeight: number;
    creatpopupVisible: boolean;
    itemtdkey: any;
    serialkey: any;
    mod: string;
    constructor(public app: AppComponent) { }
    ngOnDestroy(): void {
        clearInterval(this.refreshIntervalId);
    }

    ngOnInit() {
        this.ReportHeight = 800;
        this.stDate = moment().startOf('isoWeek').toDate();
        this.endDate = moment().endOf('isoWeek').toDate();
        this.app.GetData('/Users/GetUsers').subscribe(
            (s) => {
                if (s.success) {
                    this.StaffList = s.data;
                }
            }
        );
        this.getdata();
    }
    query() {
        this.getdata();
    }

    getdata() {
        let st = moment(this.stDate).format('YYYY-MM-DD');
        let ed = moment(this.endDate).format('YYYY-MM-DD');
        this.app.GetData('/MachineManagement/GetMachineKanban?StartTime=' + st + '&EndTime=' + ed).subscribe(
            (s) => {
                console.log("data=>", s.data);
                let rawData = s.data;
                // rawData.forEach((v) => {
                //     //空欄位需版面占位
                //     // if (!v.MachineProcessList.length) {
                //     //     v.MachineProcessList.push("null");
                //     // }
                // });
                let result = rawData.filter(v => v.MachineProcessList.length > 0);
                this.dataSourceDB = result;
                this.machines = this.getMachineList(this.dataSourceDB);
                this.dataMax = this.getMaxDataCount(this.dataSourceDB);
                this.startInterval();
            }
        );
    }

    toName(e) {
        if (e) {
            let result = this.StaffList.filter(v => v.Id == e);
            return result[0] ? result[0]["Realname"] : undefined;
        } else {
            return undefined;
        }
    }

    startInterval() {
        clearInterval(this.refreshIntervalId);
        this.refreshIntervalId = setInterval(() => {
            if (this.dataSourceDB) {
                this.dataSourceDB.forEach(ele => {
                    ele.MachineProcessList.forEach(e => {
                        if (e.PredictTime) {
                            e.PredictTime = Number(e.PredictTime) - 1;
                        }
                    });
                });
            }

        }, 60000);
    }

    tdclick(data) {
        console.log("mytest2", data);

        this.itemtdkey = data.WorkOrderHeadId;
        this.serialkey = data.SerialNumber;
        switch (data.Status) {
            case 1:
                this.mod = "start";
                break;
            case 2:
                this.mod = "report";
                break;
            default:
                break;
        }
        this.creatpopupVisible = true;

    }
    isNull(data) {
        if (data) {
            return false;
        } else {
            return true;
        }
    }

    //取機台清單
    getMachineList(ary) {
        let list = [];
        ary.forEach((v) => {
            if (v.MachineProcessList.length > 0) {
                list.push(v.MachineName);
            }

        })
        return list;
    }

    getMaxDataCount(ary) {
        let num = 1;
        ary.forEach((v) => {
            let dataLength = v.MachineProcessList.length;
            if (dataLength > num) {
                num = dataLength;
            }
        })
        return num;
    }

    creatpopup_result(e){

    }

}
