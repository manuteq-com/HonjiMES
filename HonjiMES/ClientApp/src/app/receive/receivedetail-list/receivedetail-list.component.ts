import { Component, OnInit, Input, ViewChild, OnChanges } from '@angular/core';
import CustomStore from 'devextreme/data/custom_store';
import { APIResponse } from 'src/app/app.module';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { SendService } from 'src/app/shared/mylib';
import notify from 'devextreme/ui/notify';
import { DxDataGridComponent } from 'devextreme-angular';

@Component({
    selector: 'app-receivedetail-list',
    templateUrl: './receivedetail-list.component.html',
    styleUrls: ['./receivedetail-list.component.css']
})
export class ReceiveDetailListComponent implements OnInit, OnChanges {

    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @Input() itemkeyval: any;
    Controller = '/Requisitions';
    dataSourceMaterialDB: CustomStore;
    dataSourceProductDB: CustomStore;
    popupVisible: boolean;
    WarehouseIDP: any;
    WarehouseIDM: any;
    Warehouselist: any;
    public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>('/api' + apiUrl);
    }
    constructor(private http: HttpClient) {
        this.dataSourceMaterialDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) =>
                SendService.sendRequest(this.http, this.Controller + '/GetRequisitionsDetailMaterial/' + this.itemkeyval),
            insert: (values) =>
                SendService.sendRequest(this.http, this.Controller + '/PostBomlist/' + this.itemkeyval, 'POST', { values }),
            update: (key, values) =>
                SendService.sendRequest(this.http, this.Controller + '/PutRequisitionsDetail', 'PUT',
                    { key, values: this.setWarehouse(values, this.WarehouseIDM) }),
            remove: (key) =>
                SendService.sendRequest(this.http, this.Controller + '/DeleteBomlist', 'DELETE')
        });
        this.dataSourceProductDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) =>
                SendService.sendRequest(this.http, this.Controller + '/GetRequisitionsDetailProduct/' + this.itemkeyval),
            insert: (values) =>
                SendService.sendRequest(this.http, this.Controller + '/PostBomlist/' + this.itemkeyval, 'POST', { values }),
            update: (key, values) =>
                SendService.sendRequest(this.http, this.Controller + '/PutRequisitionsDetail', 'PUT',
                    // { key, values, WarehouseID: this.WarehouseIDP }),
                    { key, values: this.setWarehouse(values, this.WarehouseIDP) }),
            remove: (key) =>
                SendService.sendRequest(this.http, this.Controller + '/DeleteBomlist', 'DELETE')
        });
        this.Warehouselist = new CustomStore({
            key: 'Id',
            load: () =>
                SendService.sendRequest(this.http, '/Warehouses/GetWarehouses')
        });
    }
    setWarehouse(values: any, WarehouseIDM: number) {
        values.WarehouseID = WarehouseIDM;
        debugger;
        return values;
    }
    ngOnInit() {
    }
    ngOnChanges() {
    }
    onToolbarPreparingP(e) {
        const toolbarItems = e.toolbarOptions.items;
        toolbarItems.forEach(item => {
            if (item.name === 'saveButton') {
                item.options.icon = '';
                item.options.text = '領料';
                item.showText = 'always';
            } else if (item.name === 'revertButton') {
                item.options.icon = '';
                item.options.text = '取消';
                item.showText = 'always';
            }
        });
        e.toolbarOptions.items.unshift(
            {
                text: '成品領料',
                location: 'before'
            },
            {
                location: 'after',
                widget: 'dxSelectBox',
                options: {
                    dataSource: this.Warehouselist,
                    displayExpr: 'Name',
                    valueExpr: 'Id',
                    onValueChanged: this.WarehouseChangedP.bind(this)
                }
            });
    }
    onToolbarPreparingM(e) {
        const toolbarItems = e.toolbarOptions.items;
        toolbarItems.forEach(item => {
            if (item.name === 'saveButton') {
                item.options.icon = '';
                item.options.text = '領料';
                item.showText = 'always';
            } else if (item.name === 'revertButton') {
                item.options.icon = '';
                item.options.text = '取消';
                item.showText = 'always';
            }
        });
        e.toolbarOptions.items.unshift(
            {
                text: '原料領料',
                location: 'before'
            },
            {
                location: 'after',
                widget: 'dxSelectBox',
                options: {
                    dataSource: this.Warehouselist,
                    displayExpr: 'Name',
                    valueExpr: 'Id',
                    onValueChanged: this.WarehouseChangedM.bind(this)
                }
            });
    }
    WarehouseChangedP(e) {
        this.WarehouseIDP = e.value;
    }
    WarehouseChangedM(e) {
        this.WarehouseIDM = e.value;
    }
    cellClick(e) {
        if (e.rowType === 'header') {
            if (e.column.type === 'buttons') {
                if (e.column.cssClass === 'addmod') {
                    this.popupVisible = true;
                    // this.TreeList.instance.addRow();
                }
            }
        }
    }
    popup_result(e) {
        this.popupVisible = false;
        notify({
            message: '存檔完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
    }
}
