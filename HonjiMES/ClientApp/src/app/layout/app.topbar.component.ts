import { Component, NgZone, OnInit } from '@angular/core';
import { AppComponent } from '../app.component';
import { HubMessage } from '../model/viewmodels';
import { SignalRService } from 'src/app/service/signal-r.service';
@Component({
    selector: 'app-topbar',
    templateUrl: 'app.topbar.component.html'

})
export class AppTopBarComponent implements OnInit {
    visible = false;
    message = [];
    constructor(public app: AppComponent, private SignalRService: SignalRService, private _ngZone: NgZone) { }
    ngOnInit() {

        this.subscribeToEvents();
    }
    private subscribeToEvents() {
        this.SignalRService.messageReceived.subscribe((message: HubMessage) => {
            this._ngZone.run(() => {
                if (message.type === 'AlertMessage') {
                    this.message.push(message.message);
                }

            });
        });
    }
    open() {
        debugger
        this.visible = true;
    }

    close() {
        this.visible = false;
    }
}
