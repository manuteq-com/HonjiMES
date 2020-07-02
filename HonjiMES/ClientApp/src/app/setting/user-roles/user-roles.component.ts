import { Component, OnInit, Input } from '@angular/core';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from 'src/app/shared/mylib';
import { HttpClient } from '@angular/common/http';

@Component({
    selector: 'app-user-roles',
    templateUrl: './user-roles.component.html',
    styleUrls: ['./user-roles.component.css']
})
export class UserRolesComponent implements OnInit {
    @Input() itemkeyval: any;
    dataSourceDB: any;
    editOnkeyPress: boolean;
    enterKeyAction: string;
    enterKeyDirection: string;
    labelLocation: string;
    Controller = '/Users';

    constructor(private http: HttpClient) {
        this.editOnkeyPress = true;
        this.enterKeyAction = 'moveFocus';
        this.enterKeyDirection = 'row';
        this.labelLocation = 'left';
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            load: (loadOptions) =>
                SendService.sendRequest(this.http, this.Controller + '/GetUsersMenuRoles/' + this.itemkeyval),
            update: (key, values) =>
                SendService.sendRequest(this.http, this.Controller + '/PutUsersMenuRoles', 'PUT', { key, values }),
        });
    }
    ngOnInit() {
    }
    onValueChanged(e, data) {
        data.setValue(e.value);
    }
    onInitNewRow(e) {

    }
    onFocusedCellChanging(e) {
    }
    onCellPrepared(e) {
        if (e.column.command === 'edit') {

        }
    }
    onEditingStart(e) {

    }
}
