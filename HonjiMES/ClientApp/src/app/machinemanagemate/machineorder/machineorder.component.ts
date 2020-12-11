import { Component, OnInit } from '@angular/core';
import { AppComponent } from 'src/app/app.component';

@Component({
    selector: 'app-machineorder',
    templateUrl: './machineorder.component.html',
    styleUrls: ['./machineorder.component.css']
})
export class MachineorderComponent implements OnInit {
    dataSourceDB: any;

    constructor(public app: AppComponent) { }

    ngOnInit() {
        this.app.GetData('/MachineManagement/GetMachineData').subscribe(
            (s) => {
                debugger;
                this.dataSourceDB = s.data;
            }
        );
    }

    getClass(data){
        if (data <= 5) { // 開工
            return 'Alert';
        }
    }



}
