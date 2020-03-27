import { Component, OnInit, ViewChild, Input } from '@angular/core';
import DataSource from 'devextreme/data/data_source';
import ArrayStore from 'devextreme/data/array_store';
import { DxDataGridComponent } from 'devextreme-angular';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from '../shared/mylib';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-oerdrdetail-list',
  templateUrl: './oerdrdetail-list.component.html',
  styleUrls: ['./oerdrdetail-list.component.css']
})
export class OerdrdetailListComponent implements OnInit {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @Input() itemkey: number;
    @Input() ProductList: any;
    dataSourceDB: any;
    controller: string;
  constructor(private http: HttpClient) {
    this.controller = '/OrderDetails';
    this.dataSourceDB = new CustomStore({
        key: 'Id',
        load: () => SendService.sendRequest( http, this.controller + '/GetOrderDetailsByOrderId?OrderId=' + this.itemkey),
        byKey: () => SendService.sendRequest( http, this.controller +  '/GetOrderDetail'),
        insert: (values) => SendService.sendRequest( http, this.controller +  '/PostOrderDetail', 'POST', { values }),
        update: (key, values) => SendService.sendRequest( http, this.controller +  '/PutOrderDetail', 'PUT', { key, values}),
        remove: (key) => SendService.sendRequest( http, this.controller +  '/DeleteOrderDetail', 'DELETE')
    });
   }
  ngOnInit() {
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
}
