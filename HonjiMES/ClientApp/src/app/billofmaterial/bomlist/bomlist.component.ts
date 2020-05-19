import { Component, OnInit, Input, ViewChild } from '@angular/core';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from 'src/app/shared/mylib';
import { HttpClient } from '@angular/common/http';

@Component({
    selector: 'app-bomlist',
    templateUrl: './bomlist.component.html',
    styleUrls: ['./bomlist.component.css']
})
export class BomlistComponent implements OnInit {
    @Input() itemkeyval: any;
    Controller = '/BillOfMaterials';
    dataSourceDB: CustomStore;

    constructor(private http: HttpClient) {
        this.onReorder = this.onReorder.bind(this);
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) =>
                SendService.sendRequest(this.http, this.Controller + '/GetBomlist/' + this.itemkeyval),
            insert: (values) =>
                SendService.sendRequest(this.http, this.Controller + '/PostBomlist', 'POST', { values }),
            update: (key, values) =>
                SendService.sendRequest(this.http, this.Controller + '/PutBomlist', 'PUT', { key, values }),
            remove: (key) =>
                SendService.sendRequest(this.http, this.Controller + '/DeleteBomlist', 'DELETE')
        });

    }

    ngOnInit() {
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
}
