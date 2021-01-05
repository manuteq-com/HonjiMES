import { Component, OnInit, Output, EventEmitter, Input, ViewChild, OnChanges } from '@angular/core';

@Component({
  selector: 'app-createmaintenance',
  templateUrl: './createmaintenance.component.html',
  styleUrls: ['./createmaintenance.component.css']
})
export class CreatemaintenanceComponent implements OnInit {
  @Input() itemdata: any;
  formData;
  labelLocation = 'left';
  minColWidth = 100;
  colCount = 2;
  width: any;
  buttondisabled = false;
  constructor() { }

  ngOnInit(): void {
  }

  ngOnChanges() {
    // debugger;
    this.formData = this.itemdata;
  }

  onFormSubmit = async function(e) {
    // this.buttondisabled = true;
    if (this.validate_before() === false) {
        this.buttondisabled = false;
        return;
    }
    this.formData = this.myform.instance.option('formData');
    // this.postval = new Material();
    // this.postval = this.formData as Material;
    this.formData.wid = this.gridBoxValue;
    this.formData.warehouseData = this.warehousesOptions;
    // tslint:disable-next-line: max-line-length
    //++ const sendRequest = await SendService.sendRequest(this.http, '/Materials/PostMaterial', 'POST', { values: this.formData });
    // let data = this.client.POST( this.url + '/OrderHeads/PostOrderMaster_Detail').toPromise();
    //++ if (sendRequest) {
    //     // this.myform.instance.resetValues();
    //     e.preventDefault();
    //     this.childOuter.emit(true);
    // }
    this.buttondisabled = false;

};

}
