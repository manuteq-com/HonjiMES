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
import CheckBox from 'devextreme/ui/check_box';
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
    disabledValues: number[];
    constructor(private http: HttpClient) {
        this.disabledValues = [];
        this.onCellPrepared = this.onCellPrepared.bind(this);
        this.onSelectionChanged = this.onSelectionChanged.bind(this);
        this.allMode = 'allPages';
        this.checkBoxesMode = 'always'; // 'onClick';
        this.controller = '/OrderDetails';
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: () => SendService.sendRequest(http, this.controller + '/GetOrderDetailsByOrderId?OrderId=' + this.itemkey),
            byKey: () => SendService.sendRequest(http, this.controller + '/GetOrderDetail'),
            insert: (values) => SendService.sendRequest(http, this.controller + '/PostOrderDetail?PID=' + this.itemkey, 'POST', { values }),
            update: (key, values) => SendService.sendRequest(http, this.controller + '/PutOrderDetail', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(http, this.controller + '/DeleteOrderDetail', 'DELETE')
        });
    }
    ngOnInit() {
    }
    to_saleClick(e) {
        this.tosealkey = null;
        this.tosealkey = this.dataGrid.instance.getSelectedRowsData();
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
                    this.popupVisible = true;
                } else if (result.dismiss === Swal.DismissReason.cancel)  {
                    this.mod = 'merge';
                    this.popupVisible = true;
                } else if (result.dismiss === Swal.DismissReason.close)  {
                    this.popupVisible = false;
                }
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
            if (e.data.Quantity === e.data.SaleCount) {
                const instance = CheckBox.getInstance(e.cellElement.querySelector('.dx-select-checkbox'));
                instance.option('disabled', true);
                this.disabledValues.push(e.data.Id);
            }
        }
    }
    onSelectionChanged(e) {// CheckBox disabled還是會勾選，必須清掉，這是官方寫法
        const disabledKeys = e.currentSelectedRowKeys.filter(i => this.disabledValues.indexOf(i) > -1);
        if (disabledKeys.length > 0) {
          e.component.deselectRows(disabledKeys);
        }
      }
    popup_result(e) {
        this.popupVisible = false;
        this.dataGrid.instance.refresh();
        this.dataGrid.instance.clearSelection();
        notify({
            message: '存檔完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
    }
    distinct(value) {
        const list = value.filter(
            (v, i, arr) => arr.findIndex(t => t.SaleId === v.SaleId) === i
        );
        return list;
    }
    // getid(value) {
    //     debugger;
    //     <span></span>
    //     value.array.forEach(element => {

    //     });
    //     return
    // }
}
