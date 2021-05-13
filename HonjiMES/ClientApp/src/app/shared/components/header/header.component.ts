import { Component, NgModule, Input, Output, EventEmitter, OnInit,NgZone } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../service/auth.service';

import { UserPanelModule } from '../user-panel/user-panel.component';
import { DxButtonModule } from 'devextreme-angular/ui/button';
import { DxToolbarModule } from 'devextreme-angular/ui/toolbar';
import { AppComponent } from '../../../app.component';
import { HubMessage } from '../../../model/viewmodels';
import { SignalRService } from 'src/app/service/signal-r.service';

import { Router } from '@angular/router';
@Component({
  selector: 'app-header',
  templateUrl: 'header.component.html',
  styleUrls: ['./header.component.scss']
})

export class HeaderComponent implements OnInit {
    visible = false;
    message = [];
  @Output()
  menuToggle = new EventEmitter<boolean>();

  @Input()
  menuToggleEnabled = false;

  @Input()
  title: string;

  user = { email: '' };

//   userMenuItems = [{
//     text: 'Profile',
//     icon: 'user',
//     onClick: () => {
//       this.router.navigate(['/profile']);
//     }
//   },
//   {
//     text: 'Logout',
//     icon: 'runner',
//     onClick: () => {
//       this.authService.logout();
//       this.router.navigate(['/login']);
//     }
//   }];

userMenuItems = [
      {
        text: 'Logout',
        icon: 'runner',
        onClick: () => {
          this.authService.logout();
          this.router.navigate(['/login']);
        }
      }];

  constructor(public app: AppComponent, private authService: AuthService, private router: Router, private SignalRService: SignalRService, private _ngZone: NgZone) { }

  ngOnInit() {
    //this.authService.getUser().then((e) => this.user = e.data);
    this.subscribeToEvents();
  }

  toggleMenu = () => {
    this.menuToggle.emit();
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

@NgModule({
  imports: [
    CommonModule,
    DxButtonModule,
    UserPanelModule,
    DxToolbarModule
  ],
  declarations: [ HeaderComponent ],
  exports: [ HeaderComponent ]
})
export class HeaderModule { }
