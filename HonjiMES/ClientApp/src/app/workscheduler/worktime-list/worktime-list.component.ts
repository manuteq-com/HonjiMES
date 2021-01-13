import { Component, OnInit, ViewChild, Input, OnChanges, EventEmitter, Output } from '@angular/core';
import { DatePipe } from '@angular/common';
import { DxDataGridComponent } from 'devextreme-angular';
import CustomStore from 'devextreme/data/custom_store';
import { HttpClient } from '@angular/common/http';
import { SendService } from '../../shared/mylib';
import notify from 'devextreme/ui/notify';
import { AppComponent } from 'src/app/app.component';
import { Myservice } from 'src/app/service/myservice';

@Component({
  selector: 'app-worktime-list',
  templateUrl: './worktime-list.component.html',
  styleUrls: ['./worktime-list.component.css']
})
export class WorktimeListComponent implements OnInit {
    @Output() childOuter = new EventEmitter();
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @Input() status: boolean;
    @Input() itemkey: number;
    @Input() SupplierList: any;
    @Input() MaterialBasicList: any;
    allMode: string;
    checkBoxesMode: string;
    dataSourceDB: CustomStore;
    Controller = '/WorkScheduler';
    StatusList: any;
    ProcessBasicList: any;

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent) {
        this.allMode = 'allPages';
        this.checkBoxesMode = 'always'; // 'onClick';
        this.StatusList = myservice.getWorkOrderStatus();
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: () => SendService.sendRequest(this.http, this.Controller + '/GetWorkOrderDetailSummary?Id=' + this.itemkey),
        });
    }
    ngOnInit() {
    }
    ngOnChanges() {
        this.app.GetData('/Processes/GetProcesses').subscribe(
            (s) => {
                if (s.success) {
                    if (s.success) {
                        this.ProcessBasicList = s.data;
                        this.ProcessBasicList.forEach(x => {
                            x.Name = x.Code + '_' + x.Name;
                        });
                    }
                }
            }
        );
    }
    onDataErrorOccurred(e) {
        notify({
            message: e.error.message,
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'error', 3000);
    }
    customizeText(e) {
        return '總數：' + e.value + '筆';
    }
    onRowUpdated(e) {
        this.childOuter.emit(true);
    }
}
