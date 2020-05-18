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
            byKey: (key) =>
                SendService.sendRequest(this.http, this.Controller + '/GetBillofPurchaseDetail', 'GET', { key }),
            insert: (values) =>
                SendService.sendRequest(this.http, this.Controller + '/PostBillofPurchaseDetail', 'POST', { values }),
            update: (key, values) =>
                SendService.sendRequest(this.http, this.Controller + '/PutBomlist', 'PUT', { key, values }),
            remove: (key) =>
                SendService.sendRequest(this.http, this.Controller + '/DeleteBillofPurchaseDetail', 'DELETE')
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
            this.dataSourceDB.update(e.itemData.Id, { Pid: targetData.Pid }).then(() => {
                e.component.refresh();
            });
        } else { }

        // const visibleRows = e.component.getVisibleRows();
        // const sourceData = e.itemData;
        // const targetData = visibleRows[e.toIndex].data;

        // if (e.dropInsideItem) {
        //     e.itemData.Head_ID = targetData.ID;
        //     e.component.refresh();
        // } else {
        //     const sourceIndex = this.dataSourceDB.indexOf(sourceData);
        //     let targetIndex = this.dataSourceDB.indexOf(targetData);

        //     if (sourceData.Head_ID !== targetData.Head_ID) {
        //         sourceData.Head_ID = targetData.Head_ID;
        //         if (e.toIndex > e.fromIndex) {
        //             targetIndex++;
        //         }
        //     }

        //     this.dataSourceDB.splice(sourceIndex, 1);
        //     this.dataSourceDB.splice(targetIndex, 0, sourceData);
        // }
    }
}
