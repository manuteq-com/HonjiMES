import { Injectable } from '@angular/core';
import { Selectitem } from '../model/viewmodels';
@Injectable({
    providedIn: 'root'
})
export class Myservice {
    getpurchasetypes(): Selectitem[] {
        return purchasetypes;
    }
    getPermission(): Selectitem[] {
        return permissiontypes;
    }
    getDepartment(): Selectitem[] {
        return departmenttypes;
    }
    getComponent(): Selectitem[] {
        return componenttypes;
    }
}
const purchasetypes: Selectitem[] = [
    { Id: 10, Name: '採購' },
    { Id: 20, Name: '外包' }
];
const permissiontypes: Selectitem[] = [
    { Id: 1, Name: '系統管理員' },
    { Id: 2, Name: '主管' },
    { Id: 3, Name: '一般人員' },
    { Id: 4, Name: '採購人員' },
    { Id: 5, Name: '設計人員' },
    { Id: 6, Name: '生館人員' }
];
const departmenttypes: Selectitem[] = [
    { Id: 1, Name: '預設' }
];
const componenttypes: Selectitem[] = [
    { Id: 1, Name: '原料' },
    { Id: 2, Name: '成品' },
];
