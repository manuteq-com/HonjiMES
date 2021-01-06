import { Component, OnInit } from '@angular/core';
import { AppComponent } from '../app.component';

@Component({
    selector: 'app-homepage',
    templateUrl: './homepage.component.html',
    styleUrls: ['./homepage.component.css']
})
export class HomepageComponent implements OnInit {
    message = [];
    constructor(public app: AppComponent) {

        this.app.GetData('/Home/GetAlertMsgList').subscribe(
            (s) => {
                //debugger;
                this.message = s.data;
            }
        );
    }

    ngOnInit() {

    }

}
