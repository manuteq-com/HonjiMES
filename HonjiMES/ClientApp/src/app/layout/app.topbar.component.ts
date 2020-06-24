import { Component, OnInit } from '@angular/core';
import { AppComponent } from '../app.component';

@Component({
    selector: 'app-topbar',
    templateUrl: 'app.topbar.component.html'

})
export class AppTopBarComponent implements OnInit {
    constructor(public app: AppComponent) { }
    ngOnInit() {
    }
}
