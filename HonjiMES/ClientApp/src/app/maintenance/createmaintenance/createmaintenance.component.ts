import { NgModule, Component, OnInit, ViewChild, Input, OnChanges, EventEmitter, Output } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import CustomStore from 'devextreme/data/custom_store';
import DataSource from 'devextreme/data/data_source';
import ArrayStore from 'devextreme/data/array_store';
import { DxDataGridComponent, DxFormComponent, DxPopupComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { APIResponse } from 'src/app/app.module';
import { SendService } from 'src/app/shared/mylib';
import { AppComponent } from 'src/app/app.component';
@Component({
    selector: 'app-createmaintenance',
    templateUrl: './createmaintenance.component.html',
    styleUrls: ['./createmaintenance.component.css']
})
export class CreatemaintenanceComponent implements OnInit {
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @Input() itemdata: any;
    formData;
    labelLocation = 'left';
    minColWidth = 100;
    colCount = 2;
    width: any;
    buttondisabled = false;
    Controller = '/MachineMaintenance';
    MachineList: any;
    editorOptions: any;
    Postdata: any;
    constructor(private http: HttpClient, public app: AppComponent) {
        this.getdata();
        this.app.GetData('/Machines/GetMachines').subscribe(
            (s) => {
                if (s.success) {
                    // s.data.forEach(element => {
                    //     element.Name = element.DataNo + '_' + element.Name;
                    // });
                    this.MachineList = s.data;
                    console.log('this.MachineList', this.MachineList);
                    this.editorOptions = {
                        dataSource: { paginate: true, store: { type: 'array', data: this.MachineList, key: 'Id' } },
                        searchEnabled: true,
                        displayExpr: 'Name',
                        valueExpr: 'Id',
                        onValueChanged: this.onSelectionChanged.bind(this)
                    };
                }
            }
        );
    }

    ngOnInit(): void {


    }

    getdata() {
        // this.dataSourceDB = new CustomStore({
        //     key: 'Id',
        //     load: (loadOptions) => {
        //         // loadOptions.sort = [{ selector: 'WorkOrderNo', desc: true }];
        //         // if (loadOptions.searchValue) {
        //         // loadOptions.filter = [
        //         //     ['CreateTime', '>=', oldDay],
        //         //     'and',
        //         //     ['CreateTime', '<=', toDay],
        //         // ];
        //         // }
        //         return SendService.sendRequest(
        //             this.http,
        //             this.Controller + '/GetOrderHeads',
        //             'GET', { loadOptions, remote: true, detailfilter: this.detailfilter });
        //     },
        //     byKey: (key) => SendService.sendRequest(this.http, this.Controller + '/GetOrderHead', 'GET', { key }),
        //     insert: (values) => SendService.sendRequest(this.http, this.Controller + '/PostOrderHead', 'POST', { values }),
        //     update: (key, values) => SendService.sendRequest(this.http, this.Controller + '/PutOrderHead', 'PUT', { key, values }),
        //     remove: (key) => SendService.sendRequest(this.http, this.Controller + '/DeleteOrderHead/' + key, 'DELETE')
        // });
    }

    ngOnChanges() {
        // debugger;
        this.formData = this.itemdata;
    }

    onSelectionChanged(e) {
        //debugger;
        this.formData = this.myform.instance.option('formData');
        this.MachineList.forEach(element => {
            if (element.TempId == e.value) {
                this.formData.Name = element.Name;
            }
        });
    }
    validate_before(): boolean {
        // 表單驗證
        if (this.myform.instance.validate().isValid === false) {
            notify({
                message: '請注意訂單內容必填的欄位',
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'error', 3000);
            return false;
        }
        return true;
    }
    async onFormSubmit(e) {
        debugger;
        this.buttondisabled = true;
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }
        this.Postdata = this.myform.instance.option('formData');
        // const basicData = this.MachineList.find(z => z.TempId === this.Postdata.TempId);
        // this.Postdata.BasicType = basicData.DataType;
        // this.Postdata.TempIdId = basicData.TempId;
        const sendRequest = await SendService.sendRequest(
            this.http,
            '/MachineMaintenance/PostMachineMaintenance/', 'POST', { values: this.Postdata });
        if (sendRequest) {
            this.myform.instance.resetValues();
            e.preventDefault();
        }
        this.buttondisabled = false;
        notify({
            message: '更新完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
    }

}
