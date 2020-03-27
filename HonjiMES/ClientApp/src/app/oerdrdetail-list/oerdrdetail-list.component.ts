import { Component, OnInit, ViewChild } from '@angular/core';
import DataSource from 'devextreme/data/data_source';
import ArrayStore from 'devextreme/data/array_store';
import { DxDataGridComponent } from 'devextreme-angular';

@Component({
  selector: 'app-oerdrdetail-list',
  templateUrl: './oerdrdetail-list.component.html',
  styleUrls: ['./oerdrdetail-list.component.css']
})
export class OerdrdetailListComponent implements OnInit {
    ProductList: any;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
  DetailsDataSourceStorage: any[];
  url: string;
  constructor() {
    this.url = location.origin + '/api';
    this.DetailsDataSourceStorage = [];
    //   this.GetData(this.url + '/Products/GetProducts').subscribe(
    //     (s) => {
    //         console.log(s);
    //         this.ProductList = s.data;
    //         if (s.success) {

    //         }
    //     }
    // );
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
        // this.GetData(this.url + '/Products/GetProducts').subscribe(
        //     (s) => {
        //         console.log(s);
        //         this.ProductList = s.data;
        //         if (s.success) {

        //         }
        //     }
        // );
//   getDetails(key) {
//       let item = this.DetailsDataSourceStorage.find((i) => i.key === key);
//       if (!item) {
//           item = {
//               key,
//               dataSourceInstance: new DataSource({
//                   store: new ArrayStore({
//                       data: this.dataSourceDBDetails,
//                       key: 'Id'
//                     }),
//                     filter: ['OrderId', '=', key]
//                 })
//             };
//             this.DetailsDataSourceStorage.push(item);
//         }
//         return item.dataSourceInstance;
//     }
}
