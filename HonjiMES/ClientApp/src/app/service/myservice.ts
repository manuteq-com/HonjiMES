import { Injectable } from '@angular/core';
import { Selectitem } from '../model/viewmodels';
@Injectable({
    providedIn: 'root'
  })
export class Myservice {
    getpurchasetypes(): Selectitem[] {
        return purchasetypes;
    }
}
const purchasetypes: Selectitem[] = [
    {
        Id: 10,
        Name: '採購'
    },
    {
        Id: 20,
        Name: '外包'
    }
];
