import { Component, OnInit, Input, ViewChild, Output, EventEmitter, OnChanges } from '@angular/core';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from 'src/app/shared/mylib';
import dxTreeList from 'devextreme/ui/tree_list';
import { DxTreeListComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { Observable } from 'rxjs';
import { APIResponse } from 'src/app/app.module';
import { AppComponent } from 'src/app/app.component';
import { HttpClient } from '@angular/common/http';
import { Myservice } from 'src/app/service/myservice';

@Component({
    selector: 'app-bomlist',
    templateUrl: './bomlist.component.html',
    styleUrls: ['./bomlist.component.css']
})
export class BomlistComponent implements OnInit, OnChanges {
    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() bomMod: any;
    @ViewChild(DxTreeListComponent) TreeList: DxTreeListComponent;
    Controller = '/BillOfMaterials';
    dataSourceDB: CustomStore;
    popupVisible: boolean;
    MaterialList: any;
    ProductList: any;
    btnVisible: boolean;
    randomkey: number;
    materialpopupVisible: boolean;
    productpopupVisible: boolean;
    masterkey: any;
    WeightVisible: boolean;
    MaterialKey: any;
    ProductKey: any;
    MasterList: any;

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent) {
        this.btnVisible = true;
        this.MasterList = myservice.getMasterType();
        this.onReorder = this.onReorder.bind(this);
        this.isEditVisible = this.isEditVisible.bind(this);
        this.isDeleteVisible = this.isDeleteVisible.bind(this);
        this.isUploadVisible = this.isUploadVisible.bind(this);
        this.readBomProcess = this.readBomProcess.bind(this);
        this.uploadIconClick = this.uploadIconClick.bind(this);
        this.app.GetData('/MaterialBasics/GetMaterialBasics').subscribe(
            (s) => {
                if (s.success) {
                    this.MaterialList = s.data.data;
                }
            }
        );
        this.app.GetData('/ProductBasics/GetProductBasics').subscribe(
            (s) => {
                if (s.success) {
                    this.ProductList = s.data.data;
                }
            }
        );
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) => SendService.sendRequest(http, this.Controller + '/GetBomlist/' + this.itemkeyval),
            insert: (values) => SendService.sendRequest(http, this.Controller + '/PostBomlist/' + this.itemkeyval, 'POST', { values }),
            update: (key, values) => SendService.sendRequest(http, this.Controller + '/PutBomlist', 'PUT', { key, values }),
            remove: (key) => SendService.sendRequest(http, this.Controller + '/DeleteBomlist/' + key, 'DELETE')
        });

    }
    ngOnInit() {
    }
    ngOnChanges() {
        if (this.bomMod === 'MBOM') {
            this.btnVisible = false;
        }
    }
    cellClick(e) {
        if (e.rowType === 'header') {
            if (e.column.type === 'buttons') {
                if (e.column.cssClass === 'addmod') {
                    this.popupVisible = true;
                    this.randomkey = new Date().getTime();
                    // this.TreeList.instance.addRow();
                }
            }
        }
    }
    onDragChange(e) {
        const visibleRows = e.component.getVisibleRows();
        const sourceNode = e.component.getNodeByKey(e.itemData.Id);
        let targetNode = visibleRows[e.toIndex].node;

        while (targetNode && targetNode.data) {
            if (targetNode.data.Id === sourceNode.data.Id) {
                e.cancel = true;
                break;
            }
            targetNode = targetNode.parent;
        }
    }
    readData(e, data) {
        // debugger;
        if (data.data.Ismaterial) {
            this.materialpopupVisible = true;
            this.MaterialKey = data.data.MaterialBasicId;
        } else {
            this.productpopupVisible = true;
            this.ProductKey = data.data.ProductBasicId;
        }
    }
    async uploadIconClick(e) {
        try {
            // tslint:disable-next-line: max-line-length
            const sendRequest = await SendService.sendRequest(this.http, '/BillOfMaterials/PutBomlistMaster', 'PUT', { key: e.row.key, values: {Master: 1} });
            if (sendRequest) {
                notify({
                    message: '存檔完成',
                    position: {
                        my: 'center top',
                        at: 'center top'
                    }
                }, 'success', 3000);
                this.TreeList.instance.refresh();
            }
        } catch (error) {

        }
    }
    onReorder(e) {
        debugger;
        const visibleRows = e.component.getVisibleRows();
        const sourceData = e.itemData;
        const targetData = visibleRows[e.toIndex].data;
        if (e.dropInsideItem) {
            this.dataSourceDB.update(e.itemData.Id, { Pid: targetData.Id }).then(() => {
                e.component.refresh();
            });
        } else {
            if (targetData.Pid) {
                this.dataSourceDB.update(e.itemData.Id, { Pid: targetData.Pid }).then(() => {
                    e.component.refresh();
                });
            } else {
                this.dataSourceDB.update(e.itemData.Id, { ProductBasicId: targetData.ProductBasicId }).then(() => {
                    e.component.refresh();
                });
            }
        }
    }
    popup_result(e) {
        this.popupVisible = false;
        this.TreeList.instance.refresh();
        notify({
            message: '存檔完成',
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'success', 3000);
    }
    isEditVisible(e) {
        // if (e.row.data.Lv === 1) {
        //     return true;
        // } else {
        return false;
        // }
    }
    isDeleteVisible(e) {
        if (e.row.data.Lv === 1) {
            return true;
        } else {
            return false;
        }
    }
    isUploadVisible(e) {
        if (e.row.data.Master === 0 && e.row.data.Lv === 1) {
            return true;
        } else {
            return false;
        }
    }
    readBomProcess(e, data) {
        this.childOuter.emit(data.data);
    }
}
