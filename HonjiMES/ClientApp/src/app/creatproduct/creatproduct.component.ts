import { Component, OnInit, EventEmitter, Output, Input, ViewChild } from '@angular/core';
import { DxFormComponent, DxDataGridComponent } from 'devextreme-angular';

@Component({
  selector: 'app-creatproduct',
  templateUrl: './creatproduct.component.html',
  styleUrls: ['./creatproduct.component.css']
})
export class CreatproductComponent implements OnInit {
    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() exceldata: any;
    @Input() modval: any;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
  constructor() { }

  ngOnInit() {
  }

}
