import { Warehouse } from './../../model/viewmodels';
import { Component, OnInit, ViewChild } from '@angular/core';
import { DxDataGridComponent } from 'devextreme-angular';
import { Observable } from 'rxjs';
import { APIResponse } from 'src/app/app.module';
import { HttpClient } from '@angular/common/http';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from 'src/app/shared/mylib';
import notify from 'devextreme/ui/notify';
import Swal from 'sweetalert2';
import { requisitionsDetailInfo } from 'src/app/model/viewmodels';
import { AppComponent } from 'src/app/app.component';

@Component({
    selector: 'app-receive-list',
    templateUrl: './receive-list.component.html',
    styleUrls: ['./receive-list.component.css']
})
export class ReceiveListComponent implements OnInit {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild('dataGridM') dataGridM: DxDataGridComponent;
    @ViewChild('dataGridP') dataGridP: DxDataGridComponent;
    dataSourceDB: any;
    Controller = '/Requisitions';
    remoteOperations = true;
    url: string;
    selectBoxOptions: {
        dataSource: { paginate: boolean; store: { type: string; data: any; key: string; }; }; searchEnabled: boolean;
        // items: this.MaterialList,
        displayExpr: string; valueExpr: string;
    };
    requisitionId: any;


    // 源料和成品 拆開
    dataSourceMaterialDB: CustomStore;
    dataSourceProductDB: CustomStore;
    popupVisible: boolean;
    WarehouseIDP: any;
    WarehouseIDM: any;
    Warehouselist: any;
    WarehouseListM: any;
    postval: any;
    // 目前用新的 源料和成品一起

    dataSourceAllDB: any;
    WarehouseIDAll: any;
    WarehouselistAll: any;
    constructor(private http: HttpClient, public app: AppComponent) {
        const remote = this.remoteOperations;
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) =>
                SendService.sendRequest(this.http, this.Controller + '/GetRequisitions', 'GET', { loadOptions, remote }),
            byKey: (key) =>
                SendService.sendRequest(this.http, this.Controller + '/GetRequisition', 'GET', { key }),
            insert: (values) =>
                SendService.sendRequest(this.http, this.Controller + '/PostRequisitionByWorkOrderNo', 'POST', { values }),
            update: (key, values) =>
                SendService.sendRequest(this.http, this.Controller + '/PutRequisition', 'PUT', { key, values }),
            remove: (key) =>
                SendService.sendRequest(this.http, this.Controller + '/DeleteRequisition/' + key, 'DELETE')
        });
        this.app.GetData('/BillOfMaterials/GetProductBasicsDrowDown').subscribe(
            (s) => {
                if (s.success) {
                    this.selectBoxOptions = {
                        dataSource: { paginate: true, store: { type: 'array', data: s.data, key: 'Id' } },
                        searchEnabled: true,
                        // items: this.MaterialList,
                        displayExpr: 'Name',
                        valueExpr: 'Id',

                    };
                }
            }
        );
        this.Warehouselist = new CustomStore({
            key: 'Id',
            load: () =>
                SendService.sendRequest(this.http, '/Warehouses/GetWarehouses')
        });
    }
    ngOnInit() {
    }
    async deleteReceiveList(e, data) {
        this.requisitionId = data.data.Id;
        Swal.fire({
            allowEnterKey: false,
            allowOutsideClick: false,
            width: 400,
            title: '確認刪除 ?',
            html: '刪除後將無法復原!',
            icon: 'warning',
            showCancelButton: true,
            // confirmButtonColor: '#3085d6',
            // cancelButtonColor: '#d33',
            cancelButtonText: '取消',
            confirmButtonText: '確認'
        }).then(async (result) => {
            if (result.value) {
                // tslint:disable-next-line: max-line-length
                const sendRequest = await SendService.sendRequest(this.http, this.Controller + '/DeleteRequisition/' + this.requisitionId, 'DELETE');
                if (sendRequest) {
                    this.dataGrid.instance.refresh();
                    notify({
                        message: '刪除成功 !',
                        position: {
                            my: 'center top',
                            at: 'center top'
                        }
                    }, 'success', 3000);
                }
            } else {
                this.dataGrid.instance.refresh();
            }
        });
    }
    readRequisitionData(e, data) {
        debugger;
        this.requisitionId = data.data.Id;
        this.dataSourceAllDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) =>
                SendService.sendRequest(this.http, this.Controller + '/GetRequisitionsDetailMaterialByAll', 'POST',
                    { values: { RequisitionId: this.requisitionId } }),
            insert: (values) =>
                SendService.sendRequest(this.http, this.Controller + '/PostBomlist/' + this.requisitionId, 'POST', { values }),
            update: (key, values) =>
                SendService.sendRequest(this.http, this.Controller + '/PutRequisitionsDetailAll', 'PUT',
                    { key, values }),
            remove: (key) =>
                SendService.sendRequest(this.http, this.Controller + '/DeleteBomlist', 'DELETE')
        });


        // this.dataSourceMaterialDB = new CustomStore({
        //     key: 'Id',
        //     load: (loadOptions) =>
        //         SendService.sendRequest(this.http, this.Controller + '/GetRequisitionsDetailMaterialByWarehouse', 'POST',
        //             { values: { RequisitionId: this.requisitionId, WarehouseId: this.WarehouseIDM } }),
        //     insert: (values) =>
        //         SendService.sendRequest(this.http, this.Controller + '/PostBomlist/' + this.requisitionId, 'POST', { values }),
        //     update: (key, values) =>
        //         SendService.sendRequest(this.http, this.Controller + '/PutRequisitionsDetail', 'PUT',
        //             { key, values: this.setWarehouse(values, this.WarehouseIDM) }),
        //     remove: (key) =>
        //         SendService.sendRequest(this.http, this.Controller + '/DeleteBomlist', 'DELETE')
        // });
        // this.dataSourceProductDB = new CustomStore({
        //     key: 'Id',
        //     load: (loadOptions) =>
        //         SendService.sendRequest(this.http, this.Controller + '/GetRequisitionsDetailProductByWarehouse', 'POST',
        //             { values: { RequisitionId: this.requisitionId, WarehouseId: this.WarehouseIDP } }),
        //     insert: (values) =>
        //         SendService.sendRequest(this.http, this.Controller + '/PostBomlist/' + this.requisitionId, 'POST', { values }),
        //     update: (key, values) =>
        //         SendService.sendRequest(this.http, this.Controller + '/PutRequisitionsDetail', 'PUT',
        //             // { key, values, WarehouseID: this.WarehouseIDP }),
        //             { key, values: this.setWarehouse(values, this.WarehouseIDP) }),
        //     remove: (key) =>
        //         SendService.sendRequest(this.http, this.Controller + '/DeleteBomlist', 'DELETE')
        // });
    }
    setWarehouse(values: any, WarehouseID: number) {
        values.WarehouseID = WarehouseID;
        return values;
    }
    onToolbarPreparingAll(e) {
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
        });;
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
    async WarehouseChangedM(e) {
        this.WarehouseIDM = e.value;
        this.dataGridM.instance.refresh();
    }
    async WarehouseChangedP(e) {
        this.WarehouseIDP = e.value;
        this.dataGridP.instance.refresh();
    }
    creatdata() {
        // tslint:disable-next-line: deprecation
        this.dataGrid.instance.insertRow();
    }
    cellClick(e) {
        if (e.rowType === 'header') {
            if (e.column.type === 'buttons') {
                if (e.column.cssClass === 'addmod') {
                    // tslint:disable-next-line: deprecation
                    this.dataGrid.instance.insertRow();
                }
            }
        }
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
    onContentReady(e) {

    }
    onInitNewRow(e) {

    }
    onRowInserting(e) {

    }
    onRowInserted(e) {

    }
    onEditingStart(e) {

    }
    WarehouseselectvalueChanged(e, data) {

        data.data.StockQty = data.data.WarehouseList.find(x => x.ID === e.value).StockQty;
        data.setValue(e.value);
    }
    GetWarehouselistbyNo(data) {
        return data.data.WarehouseList;
    }
    RQtyValidation(e) {
        debugger;
        let msg = '';
        if (e.data.WarehouseId > 0) {
            if (e.data.RQty == null || e.data.RQty < 1) {
                msg = e.data.NameNo + '領取數量 必須大於0';
                e.rule.message = msg;
                notify({
                    message: msg,
                    position: {
                        my: 'center top',
                        at: 'center top'
                    }
                }, 'error', 3000);
                return false;
            } else if (e.data.RQty > e.data.StockQty) {
                msg = e.data.NameNo + ' 庫存數不足';
                e.rule.message = msg;
                notify({
                    message: msg,
                    position: {
                        my: 'center top',
                        at: 'center top'
                    }
                }, 'error', 3000);
                return false;
            }
        }
        return true;
    }
}
