import { Component, OnInit, OnChanges, Output, Input, EventEmitter, ViewChild } from '@angular/core';
import { DxFormComponent, DxDataGridComponent, DxButtonComponent } from 'devextreme-angular';
import { HttpClient } from '@angular/common/http';
import { APIResponse } from '../../app.module';
import { Observable } from 'rxjs';
import notify from 'devextreme/ui/notify';
import { SendService } from '../../shared/mylib';
import { Myservice } from '../../service/myservice';
import { Button } from 'primeng';
import { CreateNumberInfo } from 'src/app/model/viewmodels';
import Swal from 'sweetalert2';
import Buttons from 'devextreme/ui/button';
import { AppComponent } from 'src/app/app.component';
import CustomStore from 'devextreme/data/custom_store';

@Component({
    selector: 'app-supplier-material',
    templateUrl: './supplier-material.component.html',
    styleUrls: ['./supplier-material.component.css']
})
export class SupplierMaterialComponent implements OnInit {
    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() modval: any;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild('dataGrid2') dataGrid2: DxDataGridComponent;
    @ViewChild('myButton') myButton: DxButtonComponent;

    controller: string;
    formData: any;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    width: any;
    colCount: number;
    dataSourceDB: any;
    labelLocation: string;
    editOnkeyPress: boolean;
    enterKeyAction: string;
    enterKeyDirection: string;
    buttondisabled = false;


    CreateTimeDateBoxOptions: any;
    ProductBasicSelectBoxOptions: any;
    ProductBasicList: any;
    MaterialBasicList: any;
    NumberBoxOptions: any;
    SerialNo: any;
    saveDisabled: boolean;
    runVisible: boolean;
    modVisible: boolean;
    modCheck: boolean;
    modName: any;
    MaterialNo: any;
    Name: any;
    Specification: any;
    Quantity: any;
    Property: any;
    Price: any;
    Unit: any;

    saveCheck: boolean;
    onCellPreparedLevel: any;

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent) {
        this.onReorder = this.onReorder.bind(this);
        this.onRowRemoved = this.onRowRemoved.bind(this);
        // this.CustomerVal = null;
        // this.formData = null;
        this.editOnkeyPress = true;
        this.enterKeyAction = 'moveFocus';
        this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 300;
        this.colCount = 4;
        this.controller = '/SupplierOfMaterials';
        this.saveDisabled = true;
        this.modCheck = false;
        this.modName = 'new';
        this.saveCheck = true;

        this.MaterialNo = null;
        this.Name = null;
        this.Specification = null;
        this.Quantity = null;
        this.Property = null;
        this.Price = null;
        this.Unit = null;
    }
    ngOnInit() {
    }
    ngOnChanges() {
        debugger;
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: () => SendService.sendRequest(this.http, this.controller + '/GetSupplierOfMaterials/' + this.itemkeyval.Id),
            byKey: (key) => SendService.sendRequest(this.http, this.controller + '/GetSupplier', 'GET', { key }),
            insert: (values) => SendService.sendRequest(this.http,
                this.controller + '/PostSupplierOfMaterial/' + this.itemkeyval.Id, 'POST', { values }),
            update: (key, values) => SendService.sendRequest(this.http, this.controller + '/PutSupplierofMaterial', 'PUT', {key, values}),
            remove: (key) => SendService.sendRequest(this.http, this.controller + '/DeleteSupplierOfMaterial/' + key, 'DELETE')
        });
        this.formData = this.itemkeyval;
        this.onCellPreparedLevel = 0;
        this.app.GetData('/MaterialBasics/GetMaterialBasicsAsc').subscribe(
            (s) => {
                if (s.success) {
                    if (s.success) {
                        // debugger;
                        this.MaterialBasicList = s.data;
                    }
                }
            }
        );
    }
    onFormSubmit = async function (e) {
        // debugger;
        this.buttondisabled = true;
        if (this.validate_before() === false) {
            this.buttondisabled = false;
            return;
        }
        if (!this.saveCheck) {
            return;
        }
        this.dataGrid2.instance.saveEditData();
        if (this.dataSourceDB.length === 0) {
            notify({
                message: '原料內容不能為空!',
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'error', 3000);
            return false;
        }

    };
    allowEdit(e) {
        if (e.row.data.Status > 1) {
            return false;
        } else {
            return true;
        }
    }
    onInitialized(value, data) {
        data.setValue(value);
    }
    onRowRemoved(e) {
        this.dataSourceDB.forEach((element, index) => {
            element.SerialNumber = index + 1;
        });
    }
    onReorder(e) {
        debugger;
        const visibleRows = e.component.getVisibleRows();
        const toIndex = this.dataSourceDB.indexOf(visibleRows[e.toIndex].data);
        const fromIndex = this.dataSourceDB.indexOf(e.itemData);

        this.dataSourceDB.splice(fromIndex, 1);
        this.dataSourceDB.splice(toIndex, 0, e.itemData);
        this.dataSourceDB.forEach((element, index) => {
            element.SerialNumber = index + 1;
        });
    }
    onFocusedCellChanging(e) {
    }
    CreateTimeValueChange = async function (e) {
        // this.formData = this.myform.instance.option('formData');
        if (this.formData.CreateTime != null) {
            this.CreateNumberInfoVal = new CreateNumberInfo();
            this.CreateNumberInfoVal.CreateNumber = this.formData.WorkOrderNo;
            this.CreateNumberInfoVal.CreateTime = this.formData.CreateTime;
            // tslint:disable-next-line: max-line-length
            const sendRequest = await SendService.sendRequest(this.http, '/Processes/GetWorkOrderNumberByInfo', 'POST', { values: this.CreateNumberInfoVal });
            if (sendRequest) {
                this.formData.WorkOrderNo = sendRequest.CreateNumber;
                this.formData.CreateTime = sendRequest.CreateTime;
            }
        }
    };
    onProductBasicSelectionChanged(e) {
        // debugger;
        if (this.modCheck) {
            this.modCheck = false;
        } else {
            this.saveDisabled = false;
            if (e.value !== 0 && e.value !== null && e.value !== undefined) {
                this.app.GetData('/BillOfMaterials/GetProcessByProductBasicId/' + e.value).subscribe(
                    (s) => {
                        if (s.success) {
                            s.data.forEach(e => {
                                e.DueStartTime = new Date();
                                e.DueEndTime = new Date();
                            });
                            this.dataSourceDB = s.data;
                        }
                    }
                );
            }
        }
    }
    selectvalueChanged(e, data) {
        // debugger;
        data.setValue(e.value);
        const today = new Date();
        this.MaterialBasicList.forEach(x => {
            if (x.Id === e.value) {
                this.MaterialNo = x.MaterialNo;
                this.Name = x.Name;
                this.Specification = x.Specification;
                this.Property = x.ProducingMachine;
                this.Price = x.Remark;
                this.Unit = x?.Unit ?? null;
            }
        });
    }
    onInitNewRow(e) {
        // debugger;
        this.saveCheck = false;
        this.onCellPreparedLevel = 1;

        this.SerialNo = this.dataSourceDB.length;
        this.SerialNo++;
        e.data.SerialNumber = this.SerialNo;
        e.data.MaterialNo = '';
        e.data.Name = '';
        e.data.Specification = '';
        e.data.Property = '';
        e.data.Price = '0';
        e.data.Unit = '';

        this.MaterialNo = '';
        this.Name = '';
        this.Specification = '';
        this.Property = '';
        this.Price = 0;
        this.Unit = '';
    }

    onEditingStart(e) {
        debugger;
        this.saveCheck = false;
        this.onCellPreparedLevel = 1;

        this.MaterialNo = e.data.MaterialNo;
        this.Name = e.data.Name;
        this.Quantity = e.data.Quantity;
        this.Specification = e.data.Specification;
        this.Property = e.data.Property;
        this.Price = e.data.Price;
        this.Unit = e.data.Unit;
    }
    onCellPrepared(e) {
        if (e.column.command === 'edit') {
            if (this.onCellPreparedLevel === 1) {
                this.onCellPreparedLevel = 2;
            } else if (this.onCellPreparedLevel === 2) {
                this.onCellPreparedLevel = 3;
                this.saveCheck = true;
            } else if (this.onCellPreparedLevel === 3) {
                this.myButton.instance.focus();
            }
        }
    }

    viewRefresh(e, result) {
        if (result) {
            this.dataSourceDB = [];
            this.dataGrid2.instance.refresh();
            this.formData.CreateTime = new Date();
            this.formData.ProductBasicId = null;
            this.formData.Count = 1;
            this.formData.ProducingMachine = '';
            this.formData.Remarks = '';
            this.dataSourceDB = [];
            e.preventDefault();
            this.childOuter.emit(true);
        }
    }
}
