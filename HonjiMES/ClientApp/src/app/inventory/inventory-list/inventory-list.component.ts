import { Component, OnInit, OnChanges, Output, Input, EventEmitter, ViewChild } from '@angular/core';
import { DxFormComponent, DxDataGridComponent } from 'devextreme-angular';
import { HttpClient } from '@angular/common/http';
import { APIResponse } from '../../app.module';
import { Observable } from 'rxjs';
import notify from 'devextreme/ui/notify';
import { SendService } from '../../shared/mylib';
import { Myservice } from '../../service/myservice';
import { Button } from 'primeng';
import { CreateNumberInfo } from 'src/app/model/viewmodels';

@Component({
  selector: 'app-inventory-list',
  templateUrl: './inventory-list.component.html',
  styleUrls: ['./inventory-list.component.css']
})
export class InventoryListComponent implements OnInit, OnChanges {

    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() exceldata: any;
    @Input() modval: any;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    buttondisabled = false;
    CustomerVal: any;
    formData: any;
    editOnkeyPress: boolean;
    enterKeyAction: string;
    enterKeyDirection: string;
    labelLocation: string;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    width: any;
    colCount: number;
    url: string;
    dataSourceDB: any[];
    addRow = true;
    eformData: any;
    saveCheck = true;
    showdisabled: boolean;

    BasicDataList: any;
    WarehouseList: any;
    DataType: any;
    DataId: any;

    constructor(private http: HttpClient) {
        this.CustomerVal = null;
        this.formData = null;
        this.editOnkeyPress = true;
        this.enterKeyAction = 'moveFocus';
        this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 200;
        this.colCount = 3;
        this.url = location.origin + '/api';
        this.dataSourceDB = [];

    }
    public GetData(apiUrl: string): Observable<APIResponse> {
        return this.http.get<APIResponse>(location.origin + '/api' + apiUrl);
    }
    ngOnInit() {
    }
    ngOnChanges() {
        this.dataSourceDB = [];
        this.GetData('/Inventory/GetAdjustNo').subscribe(
            (s) => {
                if (s.success) {
                    this.formData = s.data;
                }
            }
        );
        this.GetData('/Inventory/GetBasicsData').subscribe(
            (s) => {
                if (s.success) {
                    this.BasicDataList = s.data;
                }
            }
        );
        this.GetData('/Warehouses/GetWarehouses').subscribe(
            (s) => {
                this.WarehouseList = s.data;
            }
        );
    }
    refreshAdjustNo() {
        this.GetData('/Inventory/GetAdjustNo').subscribe(
            (s) => {
                if (s.success) {
                    this.formData = s.data;
                }
            }
        );
    }
    onFocusedCellChanging(e) {
    }
    TempIdValueChanged(e, data) {
        data.setValue(e.value);
        // const basicData = this.BasicDataList.find(z => z.TempId === e.value);
        // this.DataType = basicData.DataType;
        // this.DataId = basicData.DataId;
    }
    onRowInserting(e) {
        const datas = this.dataSourceDB;
        this.dataSourceDB.forEach(element => {// 阻擋重複新增
            if (element.TempId === e.data.TempId &&
                element.WarehouseId === e.data.WarehouseId) {
                this.showMessage('新增項目已存在!!');
                e.cancel = true;
            }
        });
        if (e.cancel === false) {
            this.DataType = 0;
            this.DataId = 0;
        }
    }
    onInitNewRow(e) {
        this.saveCheck = false;
    }
    onEditingStart(e) {
        this.saveCheck = false;
    }
    onCellPrepared(e) {
        if (e.column.command === 'edit') {
            this.saveCheck = true;
        }
    }
    showMessage(data) {
        notify({
            message: data,
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'error', 3000);
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
    onFormSubmit = async function (e) {
        // debugger;
        this.buttondisabled = true;
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }
        if (!this.saveCheck) {
            this.buttondisabled = false;
            return;
        }
        this.dataGrid.instance.saveEditData();
        this.formData = this.myform.instance.option('formData');
        this.dataSourceDB.forEach(element => {
            const basicData = this.BasicDataList.find(z => z.TempId === element.TempId);
            element.DataType = basicData.DataType;
            element.DataId = basicData.DataId;
        });
        this.postval = {
            AdjustNo: this.formData.AdjustNo,
            LinkOrder: this.formData.LinkOrder,
            AdjustDataDetail: this.dataSourceDB
        };
        try {
            // tslint:disable-next-line: max-line-length
            const sendRequest = await SendService.sendRequest(this.http, '/Inventory/inventoryListChange', 'POST', { values: this.postval });
            if (sendRequest) {
                this.dataSourceDB = [];
                this.dataGrid.instance.refresh();
                // this.myform.instance.resetValues();
                e.preventDefault();
                this.childOuter.emit(true);
                this.refreshAdjustNo();
            }
        } catch (error) {
        }
        this.buttondisabled = false;
    };
}