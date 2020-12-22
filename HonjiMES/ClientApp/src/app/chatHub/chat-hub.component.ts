import { Component, NgZone, OnInit } from '@angular/core';
import { HubMessage } from 'src/app/model/viewmodels';
import { SignalRService } from 'src/app/service/signal-r.service';

@Component({
  selector: 'app-chat-hub',
  templateUrl: './chat-hub.component.html',
  styleUrls: ['./chat-hub.component.css']
})
export class ChatHubComponent implements OnInit {
  uniqueID: string = new Date().getTime().toString();
  messages = new Array<HubMessage>();
  message = new HubMessage();
  txtMessage: string = '';
  constructor(
    private SignalRService: SignalRService,
    private _ngZone: NgZone
  ) {
    this.subscribeToEvents();
  }

  ngOnInit() {
  }
  sendMessage(): void {
    if (this.txtMessage) {
      this.message = new HubMessage();
      this.message.clientuniqueid = this.uniqueID;
      this.message.type = "sent";
      this.message.message = this.txtMessage;
      this.message.date = new Date();
      this.messages.push(this.message);
      this.SignalRService.sendMessage(this.message);
      this.txtMessage = '';
    }
  }
  private subscribeToEvents(): void {

    this.SignalRService.messageReceived.subscribe((message: HubMessage) => {
      this._ngZone.run(() => {
        if (message.clientuniqueid !== this.uniqueID) {
          message.type = "received";
          this.messages.push(message);
        }
      });
    });
  }
}
