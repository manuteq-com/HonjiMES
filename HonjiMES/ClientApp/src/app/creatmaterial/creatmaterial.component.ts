import { Component, OnInit, Output, EventEmitter, Input, ViewChild } from '@angular/core';
import { DxFormComponent, DxDataGridComponent } from 'devextreme-angular';

@Component({
  selector: 'app-creatmaterial',
  templateUrl: './creatmaterial.component.html',
  styleUrls: ['./creatmaterial.component.css']
})
export class CreatmaterialComponent implements OnInit {
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
