import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-maintenance-detail',
  templateUrl: './maintenance-detail.component.html',
  styleUrls: ['./maintenance-detail.component.css']
})
export class MaintenanceDetailComponent implements OnInit {
    dataSourceDB
  constructor() {

    this.dataSourceDB = [{"id":1,"time":"2020-12-15T00:30:00","pcharge":"小明"},{"id":2,"time":"2020-12-15T00:32:00","pcharge":"小明2"}]
   }

  ngOnInit(): void {
  }
  cancelClickHandler(){}

}
