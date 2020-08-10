import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AppComponent } from 'src/app/app.component';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from 'src/app/shared/mylib';
import { Myservice } from 'src/app/service/myservice';

@Component({
    selector: 'app-work-scheduler',
    templateUrl: './work-scheduler.component.html',
    styleUrls: ['./work-scheduler.component.css']
})
export class WorksChedulerComponent implements OnInit {
    dataSourceDB: CustomStore;
    Controller = '/WorkScheduler';
    WorkSchedulerStatus: any;

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent) {
        this.WorkSchedulerStatus = myservice.getWorkSchedulerStatus();
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: () => SendService.sendRequest(http, this.Controller + '/GetWorkScheduler/'),
            // byKey: (key) => SendService.sendRequest(http, this.Controller + '/GetWiproduct', 'GET', { key }),
            // insert: (values) => SendService.sendRequest(http, this.Controller + '/PostWiproduct', 'POST', { values }),
            // update: (key, values) => SendService.sendRequest(http, this.Controller + '/PutWiproduct', 'PUT', { key, values }),
            // remove: (key) => SendService.sendRequest(http, this.Controller + '/DeleteWiproduct/' + key, 'DELETE')
        });
    }

    ngOnInit() {
    }

}
