import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AppComponent } from 'src/app/app.component';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from 'src/app/shared/mylib';
import { Myservice } from 'src/app/service/myservice';
import { Title } from '@angular/platform-browser';
import notify from 'devextreme/ui/notify';

@Component({
  selector: 'app-workorder-estimate',
  templateUrl: './workorder-estimate.component.html',
  styleUrls: ['./workorder-estimate.component.css']
})
export class WorkorderEstimateComponent implements OnInit {
    dataSourceDB: CustomStore;
    Controller = '/WorkScheduler';
    WorkEstimateStatus: any;
    views = ['day', 'week', 'month'];
    MaxMachineCount: number;
    Height: number;
    creatpopupVisible: boolean;
    randomkey: number;

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent, private titleService: Title) {
        this.MaxMachineCount = 5;
        this.app.GetData('/Machines/GetMachines').subscribe(
            (s) => {
                if (s.success) {
                    if (s.success) {
                        this.MaxMachineCount = s.data.length + 1;
                        this.Height = this.MaxMachineCount * 110 + 200;
                    }
                }
            }
        );
        this.views = ['month'];
        this.WorkEstimateStatus = myservice.getWorkEstimateStatus();
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: () => SendService.sendRequest(http, this.Controller + '/GetWorkOrderEstimate/'),
            // byKey: (key) => SendService.sendRequest(http, this.Controller + '/GetWiproduct', 'GET', { key }),
            // insert: (values) => SendService.sendRequest(http, this.Controller + '/PostWiproduct', 'POST', { values }),
            // update: (key, values) => SendService.sendRequest(http, this.Controller + '/PutWiproduct', 'PUT', { key, values }),
            // remove: (key) => SendService.sendRequest(http, this.Controller + '/DeleteWiproduct/' + key, 'DELETE')
        });
    }
    ngOnInit() {
        this.titleService.setTitle('工單交期預估');
    }
    creatdata() {
        this.randomkey = new Date().getTime();
        this.creatpopupVisible = true;
    }
    creatpopup_result(e) {
        this.creatpopupVisible = false;
        // this.dataGrid.instance.refresh();
        notify({
            message: '存檔完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
    }
}
