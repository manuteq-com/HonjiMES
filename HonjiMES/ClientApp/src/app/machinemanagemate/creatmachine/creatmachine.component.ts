import { Component, OnInit, Output, EventEmitter, Input, ViewChild, OnChanges } from '@angular/core';
import { DxFormComponent, DxDataGridComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Supplier } from 'src/app/model/viewmodels';
import { APIResponse } from 'src/app/app.module';
import { SendService } from 'src/app/shared/mylib';
import { AppComponent } from 'src/app/app.component';

@Component({
    selector: 'app-creatmachine',
    templateUrl: './creatmachine.component.html',
    styleUrls: ['./creatmachine.component.css']
})
export class CreatmachineComponent implements OnInit {
    @Output() childOuter = new EventEmitter();
    @Input() itemkeyval: any;
    @Input() exceldata: any;
    @Input() modval: any;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    buttondisabled = false;
    formData: any;
    postval: Supplier;
    labelLocation: string;
    readOnly: boolean;
    showColon: boolean;
    minColWidth: number;
    colCount: number;
    width: any;
    btnMod: any;
    buttonOptions: any = {
        text: '存檔',
        type: 'success',
        useSubmitBehavior: true,
        icon: 'save'
    };

    constructor(private http: HttpClient, public app: AppComponent) {
        this.formData = null;
        this.labelLocation = 'left';
        this.readOnly = false;
        this.showColon = true;
        this.minColWidth = 100;
        this.colCount = 1;
    }

    ngOnInit() {
    }
    ngOnChanges() {
    }
    saveBtn(e) {
        this.btnMod = 'save';
    }
    removeBtn(e) {
        this.btnMod = 'remove';
    }
    validate_before(): boolean {
        // 表單驗證
        if (this.myform.instance.validate().isValid === false) {
            notify({
                message: '請注意訂單內容必填的欄位',
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'error', 3000);
            return false;
        }
        return true;
    }
    onFormSubmit = async function (e) {
        try {
            if (this.btnMod === 'save') {
                if (this.validate_before() === false) {
                    this.buttondisabled = false;
                    return;
                }
                this.formData = this.myform.instance.option('formData');
                const sendRequest = await SendService.sendRequest(this.http, '/MachineInformation/PostMachineInformation', 'POST', { values: this.formData });

                if (sendRequest) {
                    this.myform.instance.resetValues();
                    e.preventDefault();
                    this.childOuter.emit(true);
                }
            } else if (this.btnMod === 'remove') {
                this.formData = [];
            }
        } catch (error) {
        }
        this.buttondisabled = false;

    };

}
