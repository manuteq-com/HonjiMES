import { Component, OnInit, ViewChild, Output, EventEmitter } from '@angular/core';
import { APIResponse } from '../app.module';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import notify from 'devextreme/ui/notify';
import { DxFormComponent, DxDataGridComponent, DxPopupComponent } from 'devextreme-angular';
import { SendService } from '../shared/mylib';
import { OrderHead, OrderDetail, PostOrderMaster_Detail } from '../model/orders';
import CustomStore from 'devextreme/data/custom_store';
import { NgbPaginationNumber } from '@ng-bootstrap/ng-bootstrap';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-creatorder',
  templateUrl: './creatorder.component.html',
  styleUrls: ['./creatorder.component.css']
})
export class CreatorderComponent  {
    @Output() event = new EventEmitter();
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    formData: OrderHead;
    SerialNo = 0;
    dataSourceDB: any;
    postval: any;
    labelLocation: string;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    colCount: number;
    width: any;
    ProductList: any;
    url: string;
    editOnkeyPress: boolean;
    enterKeyAction: string;
    enterKeyDirection: string;
    buttonOptions: any = {
        text: '存檔',
        type: 'success',
        useSubmitBehavior: true,
        icon: 'save'
    };
    Customerlist: any;
  constructor( private http: HttpClient) {
    this.editOnkeyPress = true;
    this.enterKeyAction = 'moveFocus';
    this.enterKeyDirection = 'row';
    this.labelLocation = 'left';
    this.readOnly = false;
    this.showColon = true;
    this.minColWidth = 300;
    this.colCount = 4;
    this.url = location.origin + '/api';
    this.dataSourceDB = [];
    // this.Customerlist = SendRequest.sendRequest(this.http, this.url + '/Customers/GetCustomers' );
    this.GetData(this.url + '/Products/GetProducts').subscribe(
        (s) => {
          console.log(s);
          this.ProductList = s.data;
          if (s.success) {

          }
        }
      );
    this.GetData(this.url + '/Customers/GetCustomers').subscribe(
        (s) => {
          console.log(s);
          this.Customerlist = s.data;
          if (s.success) {

          }
        }
      );
  }
  public GetData(apiUrl: string): Observable<APIResponse> {
      return this.http.get<APIResponse>(apiUrl);
    }
    onInitNewRow(e) {
        this.SerialNo++;
        e.data.Serial = this.SerialNo;
    }
    onFocusedCellChanging(e) {
        e.isHighlighted = true;
    }

    onFormSubmit = async function(e) {

      this.dataGrid.instance.saveEditData();
      this.formData =  this.myform.instance.option('formData');
      if (this.SerialNo > 0 && this.dataSourceDB.length < 1) {
        notify({
            message: '請注意訂單內容必填的欄位',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'error', 3000);
        return false;
            } else {
                this.postval = new PostOrderMaster_Detail();
                this.postval.OrderHead = this.formData as OrderHead;
                this.postval.orderDetail = this.dataSourceDB as OrderDetail[];
                // tslint:disable-next-line: max-line-length
                const sendRequest = await  this.sr.sendRequest(this.http, this.url + '/OrderHeads/PostOrderMaster_Detail', 'POST', { values: this.postval} );
                // let data = this.client.POST( this.url + '/OrderHeads/PostOrderMaster_Detail').toPromise();
                if (sendRequest) {
                    this.SerialNo = 0;
                    this.dataSourceDB = [];
                    this.dataGrid.instance.refresh();
                    this.form.instance.resetValues();
                    e.preventDefault();
                    this.event.emit(true);
                }
            }
      };
}
