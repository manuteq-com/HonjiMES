import { Component, OnInit, ViewChild, Input } from '@angular/core';
import DataSource from 'devextreme/data/data_source';
import ArrayStore from 'devextreme/data/array_store';
import { DxDataGridComponent } from 'devextreme-angular';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from '../shared/mylib';
import { HttpClient } from '@angular/common/http';
import notify from 'devextreme/ui/notify';
import Swal from 'sweetalert2';
import { APIResponse } from '../app.module';
import { Observable } from 'rxjs';

@Component({
    selector: 'app-oerdrdetail-list',
    templateUrl: './oerdrdetail-list.component.html',
    styleUrls: ['./oerdrdetail-list.component.css']
})
export class OerdrdetailListComponent implements OnInit {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @Input() itemkey: number;
    @Input() ProductList: any;
    url = location.origin + '/api';
    popupVisible = false;
    tosealkey: any;
    mod: string;
    dataSourceDB: any;
    controller: string;
    allMode: string;
    checkBoxesMode: string;
    constructor(private http: HttpClient) {
        this.allMode = 'allPages';
        this.checkBoxesMode = 'onClick'
        this.controller = '/OrderDetails';
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: () => SendService.sendRequest(http, this.controller + '/GetOrderDetailsByOrderId?OrderId=' + this.itemkey),
            byKey: () => SendService.sendRequest(http, this.controller + '/GetOrderDetail'),
            insert: (values) => SendService.sendRequest(http, this.controller + '/PostOrderDetail', 'POST', { values }),
            update: (key, values) => SendService.sendRequest(http, this.controller + '/PutOrderDetail', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(http, this.controller + '/DeleteOrderDetail', 'DELETE')
        });
    }
    ngOnInit() {
    }
    to_saleClick(e) {
        this.tosealkey = null;
        this.tosealkey = this.dataGrid.instance.getSelectedRowKeys();
        if (this.tosealkey.length === 0) {
            Swal.fire({
                allowEnterKey: false,
                allowOutsideClick: false,
                title: '沒有勾選任何訂單項目',
                html: '請勾選要轉銷貨的訂單項目',
                icon: 'warning',
                timer: 1000
            });
        } else {
            Swal.fire({
                showCloseButton: true,
                allowEnterKey: false,
                allowOutsideClick: false,
                title: '轉銷貨',
                html: '如需合併銷貨單，請點選[輸入銷貨單]!',
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#71c016',
                cancelButtonText: '輸入銷貨單',
                confirmButtonText: '新建銷貨單'
            }).then(async (result) => {
                if (result.value) {
                    this.mod = 'add';
                } else {
                    this.mod = 'merge';
                }
                this.popupVisible = true;
            });
        }

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
    onCellPrepared(e: any) {
        if (e.rowType === 'data' && e.column.command === 'select') {
            const instance = e.cellElement.find('.dx-select-checkbox').dxCheckBox('instance');
            instance.option('disabled', true);
            e.cellElement.off();
        }
    }
    popup_result(e) {
        this.popupVisible = false;
        this.dataGrid.instance.refresh();
        notify({
            message: '存檔完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
    }
    distinct(value) {
        debugger;
        const Saleslist = [];
        const list = value.filter(
            (v, i, arr) => arr.findIndex(t => t.SaleId === v.SaleId) === i
        );
        list.forEach(element => {
            Saleslist.push(SendService.sendRequest(this.http, '/Sales/GetSaleNo/' + element.SaleId));
        });
        return Saleslist;
    }
    // getid(value) {
    //     debugger;
    //     <span></span>
    //     value.array.forEach(element => {

    //     });
    //     return
    // }
}
