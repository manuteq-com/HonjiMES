import { Component, OnInit, OnChanges, EventEmitter, Output, Input, ViewChild } from '@angular/core';
import { DxFormComponent, DxDataGridComponent, DxTreeListComponent } from 'devextreme-angular';
import { HttpClient } from '@angular/common/http';
import notify from 'devextreme/ui/notify';
import { Observable } from 'rxjs';
import { APIResponse } from 'src/app/app.module';
import { SendService } from 'src/app/shared/mylib';
import CustomStore from 'devextreme/data/custom_store';

@Component({
  selector: 'app-bomverlist',
  templateUrl: './bomverlist.component.html',
  styleUrls: ['./bomverlist.component.css']
})
export class BomverlistComponent implements OnInit, OnChanges {
    @Input() bomverdata: any;
    Controller = '/BillOfMaterials';
    dataSourceDB: any;

    constructor(private http: HttpClient) {

    }
    ngOnInit() {
    }
    ngOnChanges() {
        debugger;
        this.dataSourceDB = this.bomverdata;
        // this.GetData('/BillOfMaterials/GetBomVerByProductId/' + this.itemkeyval).subscribe(
        //     (s) => {
        //         this.dataSourceDB = s.data;
        //     }
        // );
        // this.dataSourceDB = new CustomStore({
        //     key: 'Id',
        //     load: (loadOptions) =>
        //         SendService.sendRequest(this.http, this.Controller + '/GetBomVerByProductId/' + this.itemkeyval),
        //     insert: (values) =>
        //         SendService.sendRequest(this.http, this.Controller + '/PostBomlist/' + this.itemkeyval, 'POST', { values }),
        //     update: (key, values) =>
        //         SendService.sendRequest(this.http, this.Controller + '/PutBomlist', 'PUT', { key, values }),
        //     remove: (key) =>
        //         SendService.sendRequest(this.http, this.Controller + '/DeleteBomlist/' + key, 'DELETE')
        // });
    }

}
