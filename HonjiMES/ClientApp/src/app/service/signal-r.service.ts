import { EventEmitter, Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { HubMessage } from '../model/viewmodels';
@Injectable({
    providedIn: 'root'
})
export class SignalRService {

    messageReceived = new EventEmitter<HubMessage>();
    connectionEstablished = new EventEmitter<Boolean>();
    private connectionIsEstablished = false;
    private hubConnection: HubConnection;

    public data: any;
    constructor() {
        this.createConnection();
        this.registerOnServerEvents();
        this.startConnection();
    }
    sendMessage(message: HubMessage) {
        this.hubConnection.invoke('NewMessage', message);
    }

    private createConnection() {
        this.hubConnection = new HubConnectionBuilder()
            .withUrl('/chart')
            .build();
    }
    private registerOnServerEvents(): void {
        this.hubConnection.on('MessageReceived', (data: any) => {
            this.messageReceived.emit(data);
        });
    }
    private startConnection(): void {
        this.hubConnection
            .start()
            .then(() => {
                this.connectionIsEstablished = true;
                // console.log('廣播連接開始');
                this.connectionEstablished.emit(true);
            })
            .catch(err => {
                // console.log('建立連接時發生錯誤，正在重試...');
                setTimeout(function () { this.startConnection(); }, 5000);
            });
    }
}
