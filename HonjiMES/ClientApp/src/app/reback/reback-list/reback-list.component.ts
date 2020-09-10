import { Component, OnInit, ViewChild } from '@angular/core';
import { DxDataGridComponent } from 'devextreme-angular';
import { HttpClient } from '@angular/common/http';
import { AppComponent } from 'src/app/app.component';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from 'src/app/shared/mylib';
import Swal from 'sweetalert2';
import notify from 'devextreme/ui/notify';
import { Title } from '@angular/platform-browser';

@Component({
    selector: 'app-reback-list',
    templateUrl: './reback-list.component.html',
    styleUrls: ['./reback-list.component.css']
})
export class RebackListComponent implements OnInit {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    dataSourceDB: any;
    Controller = '/Rebacks';
    remoteOperations = true;
    url: string;
    selectBoxOptions: {
        dataSource: { paginate: boolean; store: { type: string; data: any; key: string; }; }; searchEnabled: boolean;
        // items: this.MaterialList,
        displayExpr: string; valueExpr: string;
    };
    requisitionId: any;


    creatpopupVisible: boolean;
    WarehouseIDP: any;
    WarehouseIDM: any;
    Warehouselist: any;
    WarehouseListM: any;
    postval: any;
    // 目前用新的 源料和成品一起

    dataSourceAllDB: any;
    WarehouseIDAll: any;
    WarehouselistAll: any;
    infopopupVisible: boolean;
    randomkey: number;
    itemkey: any;
    constructor(private http: HttpClient, public app: AppComponent, private titleService: Title) {
        this.RQtyValidation = this.RQtyValidation.bind(this);
        const remote = this.remoteOperations;
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) =>
                SendService.sendRequest(this.http, this.Controller + '/GetRebacks', 'GET', { loadOptions, remote }),
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
        this.app.GetData('/Warehouses/GetWarehouses').subscribe(
            (s) => {
                s.data.forEach(e => {
                    e.Name = e.Code + e.Name;
                });
                this.Warehouselist = s.data;
            }
        );
    }
    ngOnInit() {
        this.titleService.setTitle('退料資料');
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
    searchRequisitionData(e, data) {
        this.itemkey = data.data.WorkOrderHeadId;
        this.infopopupVisible = true;
        this.randomkey = new Date().getTime();
    }
    readRequisitionData(e, data) {
        this.requisitionId = data.data.Id;
        this.dataSourceAllDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) =>
                SendService.sendRequest(this.http, this.Controller + '/GetRebacksDetailMaterialByAllShow/' + data.data.Id),
        });
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
        });
    }

    creatdata() {
        // tslint:disable-next-line: deprecation
        // this.dataGrid.instance.insertRow();
        this.randomkey = new Date().getTime();
        this.creatpopupVisible = true;
    }
    infodata() {
        this.itemkey = null;
        this.infopopupVisible = true;
        this.randomkey = new Date().getTime();
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
        data.setValue(e.value);
    }
    GetWarehouselistbyNo(data) {
        return data.data.WarehouseList;
    }
    GetWarehouseStockQty(data) {
        if (data.value) {
            return data.data.WarehouseList.find(x => x.ID === data.value).StockQty ?? 0;

        } else {
            return data.value;
        }
    }
    RQtyValidation(e) {
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
            } else {
                const StockQty = e.data.WarehouseList.find(x => x.ID === e.data.WarehouseId).StockQty;
                if (e.data.RQty > StockQty) {
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
        }
        return true;
    }
    creatpopup_result(e) {
        this.creatpopupVisible = false;
        this.dataGrid.instance.refresh();
        notify({
            message: '更新完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
    }
}
