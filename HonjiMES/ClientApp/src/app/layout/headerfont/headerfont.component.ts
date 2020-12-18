import { Component, OnInit, Input } from '@angular/core';
import { AppComponent } from 'src/app/app.component';

@Component({
  selector: 'app-headerfont',
  templateUrl: './headerfont.component.html',
  styleUrls: ['./headerfont.component.css']
})
export class HeaderfontComponent implements OnInit {
    headerfont: any;

    constructor(public app: AppComponent) {
        this.headerfont = this.app.headerfont();
  }

  ngOnInit() {
  }

}
