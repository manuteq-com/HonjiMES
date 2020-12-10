import { Component, OnInit, ViewChild } from '@angular/core';
import { APIResponse } from 'src/app/app.module';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { createStore } from 'devextreme-aspnet-data-nojquery';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from 'src/app/shared/mylib';
import DataSource from 'devextreme/data/data_source';
import { AppComponent } from 'src/app/app.component';
import { Title } from '@angular/platform-browser';
import notify from 'devextreme/ui/notify';
import { DxDataGridComponent, DxFormComponent, DxPopupComponent, DxFileUploaderComponent } from 'devextreme-angular';

@Component({
    selector: 'app-billofmateriallist',
    templateUrl: './billofmateriallist.component.html',
    styleUrls: ['./billofmateriallist.component.css']
})
export class BillofmateriallistComponent implements OnInit {
    dataSourceDB: any;
    bomMod: any;
    Controller = '/BillOfMaterials';
    remoteOperations = true;
    verpopupVisible: boolean;
    itemkey: number;
    bomverdata: any;
    //dataGrid: any;
    creatpopupVisible: boolean;
    dataSourceDB2: any;
    @ViewChild('dsDB', { static: false }) dataGrid: DxDataGridComponent;
    @ViewChild('dsDB2', { static: false }) dataGridnobom: DxDataGridComponent;

    constructor(private http: HttpClient, public app: AppComponent, private titleService: Title) {
        this.bomMod = 'PBOM';
        const remote = this.remoteOperations;
        this.readBomVer = this.readBomVer.bind(this);
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) =>
                SendService.sendRequest(this.http, this.Controller + '/GetMaterialBasicsHaveBom', 'GET', { loadOptions, remote }),
            // byKey: (key) =>
            //     SendService.sendRequest(this.http, this.Controller + '/GetBillofPurchaseDetail', 'GET', { key }),
            insert: (values) =>
                SendService.sendRequest(this.http, this.Controller + '/PostBillofPurchaseDetail', 'POST', { values }),
            update: (key, values) =>
                SendService.sendRequest(this.http, '/MaterialBasics/PutActualSpecification', 'PUT', { key, values }),
            remove: (key) =>
                SendService.sendRequest(this.http, this.Controller + '/DeleteBillofPurchaseDetail', 'DELETE')
        });
        this.dataSourceDB2 = new CustomStore({
            key: 'Id',
            load: (loadOptions) =>
                SendService.sendRequest(this.http, this.Controller + '/GetMaterialBasicsHaveAny', 'GET', { loadOptions, remote }),
            // byKey: (key) =>
            //     SendService.sendRequest(this.http, this.Controller + '/GetBillofPurchaseDetail', 'GET', { key }),
            insert: (values) =>
                SendService.sendRequest(this.http, this.Controller + '/PostBillofPurchaseDetail', 'POST', { values }),
            update: (key, values) =>
                SendService.sendRequest(this.http, '/MaterialBasics/PutActualSpecification', 'PUT', { key, values }),
            remove: (key) =>
                SendService.sendRequest(this.http, this.Controller + '/DeleteBillofPurchaseDetail', 'DELETE')
        });
    }
    ngOnInit() {
        this.titleService.setTitle('物料清單管理');
    }
    creatdata() {
        this.creatpopupVisible = true;
    }
    creatpopup_result(e) {
        this.creatpopupVisible = false;
        this.dataGridnobom.instance.refresh();
        if (e.message !== undefined) {
            notify({
                message: '注意!! 客戶單號已存在!! ' + e.message,
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'warning', 6000);
        } else {
            notify({
                message: '存檔完成!',
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'success', 3000);
        }
    }
    readBomVer(e, data) {
        this.app.GetData('/BillOfMaterials/GetBomVerByProductId/' + data.key).subscribe(
            (s) => {
                //debugger;
                s.data.forEach(element => {
                    if (element.ShowPLV === 0) {
                        element.Lv = null;
                        element.Quantity = null;
                        element.Version = 'ver.' + element.Version;
                    } else {
                        element.Version = '';
                    }
                });
                this.bomverdata = s.data;
                this.verpopupVisible = true;
            }
        );
        // this.onChangeVar.emit(data.data);
    }

    cancelClickHandler(e) {
        this.dataGridnobom.instance.cancelEditData();
    }
    saveClickHandler(e) {
        this.dataGridnobom.instance.saveEditData();
    }

    //#region 重新載入
    refreshBM() {
        console.log('its reload');
        this.dataGrid.instance.refresh();
        this.dataGridnobom.instance.refresh();
    }
    //#endregion
}
