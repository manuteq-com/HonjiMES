import { Component, OnInit, Output, EventEmitter, ViewChild, OnChanges, Input } from '@angular/core';
import { DxFormComponent, DxDataGridComponent } from 'devextreme-angular';
import { Customer } from 'src/app/model/viewmodels';
import { HttpClient } from '@angular/common/http';
import { AppComponent } from 'src/app/app.component';
import notify from 'devextreme/ui/notify';
import { SendService } from 'src/app/shared/mylib';
import CustomStore from 'devextreme/data/custom_store';

@Component({
    selector: 'app-inventory-search',
    templateUrl: './inventory-search.component.html',
    styleUrls: ['./inventory-search.component.css']
})
export class InventorySearchComponent implements OnInit, OnChanges {
    @Output() childOuter = new EventEmitter();
    @Input() randomkeyval: any;
    @Input() itemkeyval: any;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    formData: any;
    postval: Customer;
    labelLocation: string;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    colCount: number;
    width: any;
    Controller = '/Adjusts';
    scrollValue = { x: '800', y: '600px' };
    NumberBoxOptions: { showSpinButtons: boolean; mode: string; min: number; value: number; };
    editorOptions: any;
    dataSourceDB: any = { AdjustInfoData: [], ColumnOptionlist: [] };
    dataSourceDB1: any;
    Warehouselist: CustomStore;
    BasicDataList: any;
    constructor(private http: HttpClient, public app: AppComponent) {
        this.formData = {};
        // this.editOnkeyPress = true;
        // this.enterKeyAction = 'moveFocus';
        // this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 100;
        this.colCount = 1;
        this.NumberBoxOptions = { showSpinButtons: true, mode: 'number', min: 0, value: 0 };
        this.BasicDataList = [];
        this.app.GetData('/Inventory/GetBasicsData').subscribe(
            (s) => {
                if (s.success) {
                    s.data.forEach(element => {
                        element.Name = element.DataNo + '_' + element.Name;
                    });
                    this.BasicDataList = s.data;
                    this.editorOptions = {
                        dataSource: { paginate: true, store: { type: 'array', data: this.BasicDataList, key: 'TempId' } },
                        searchEnabled: true,
                        displayExpr: 'Name',
                        valueExpr: 'TempId',
                        onValueChanged: this.onValueChanged.bind(this)
                    };
                }
            }
        );
    }
    ngOnInit() {
        this.scrollValue.y = (window.innerHeight - 300) + 'px';
        this.scrollValue.x = (window.innerWidth - 0) + 'px';
    }
    ngOnChanges() {
        this.formData = {};
        if (this.itemkeyval !== null) {
            // this.formData.WorkOrderNo = this.itemkeyval;
            // this.GetInventoryData(this.itemkeyval);
        } else {
            this.dataSourceDB1 = [];
        }
    }
    GetInventoryData(type, key) {
        if (type === 1) { // 原料
            this.app.GetData(this.Controller + '/GetAdjustLogByMaterialBasicID/' + key).subscribe(
                (s) => {
                    if (s.success) {
                        this.dataSourceDB = s.data;
                    }
                }
            );
        } else if (type === 2) { // 成品
            this.app.GetData(this.Controller + '/GetAdjustLogByProductBasicID/' + key).subscribe(
                (s) => {
                    if (s.success) {
                        this.dataSourceDB = s.data;
                    }
                }
            );
        } else if (type === 3) { // 半成品
            this.app.GetData(this.Controller + '/GetAdjustLogByWiproductBasicID/' + key).subscribe(
                (s) => {
                    if (s.success) {
                        this.dataSourceDB = s.data;
                    }
                }
            );
        }
    }
    onValueChanged(e) {
        this.dataSourceDB = [];
        const BasicData = this.BasicDataList.find(x => x.TempId === e.value);
        this.GetInventoryData(BasicData.DataType, BasicData.DataId);
    }
}
