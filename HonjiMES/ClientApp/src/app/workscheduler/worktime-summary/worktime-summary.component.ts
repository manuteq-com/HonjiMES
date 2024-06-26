import { Component, OnInit, ViewChild } from '@angular/core';
import notify from 'devextreme/ui/notify';
import { DxDataGridComponent, DxFormComponent } from 'devextreme-angular';
import { HttpClient } from '@angular/common/http';
import CustomStore from 'devextreme/data/custom_store';
import { SendService } from 'src/app/shared/mylib';
import { Myservice } from '../../service/myservice';
import { AppComponent } from 'src/app/app.component';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-worktime-summary',
  templateUrl: './worktime-summary.component.html',
  styleUrls: ['./worktime-summary.component.css']
})
export class WorktimeSummaryComponent implements OnInit {
    @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
    @ViewChild(DxFormComponent, { static: false }) myform: DxFormComponent;

    dataSourceDB: any;
    itemkey: number;
    mod: string;
    Controller = '/WorkScheduler';
    topurchase: any[] & Promise<any> & JQueryPromise<any>;
    remoteOperations: boolean;
    formData: any;
    editorOptions: any;
    detailfilter = [];
    DetailsDataSourceStorage: any;
    WorkOrderTypeList: any;
    selectedOperation: string = "between";

    constructor(private http: HttpClient, myservice: Myservice, public app: AppComponent, private titleService: Title) {
        this.WorkOrderTypeList = myservice.getWorkOrderStatus();
        this.remoteOperations = true;
        this.DetailsDataSourceStorage = [];
        this.editorOptions = { onValueChanged: this.onValueChanged.bind(this) };
        this.getdata();
    }
    ngOnInit() {
        this.titleService.setTitle('總工時統計');
    }
    getdata() {
        debugger;
        this.dataSourceDB = new CustomStore({
            key: 'Id',
            // load: () => SendService.sendRequest(this.http, this.Controller + '/GetBillofPurchaseHeads'),
            load: (loadOptions) => SendService.sendRequest(
                this.http,
                this.Controller + '/GetWorkOrderHeadSummarys',
                'GET', { loadOptions, remote: this.remoteOperations, detailfilter: this.detailfilter }),
        });
    }
    allowEdit(e) {
        if (e.row.data.Status === 0) {
            return true;
        } else {
            return false;
        }
    }
    updatepopup_result(e) {
        this.dataGrid.instance.refresh();
    }
    onEditorPreparing(e) {
    }
    selectionChanged(e) {
        // debugger;
        // 只開一筆Detail資料
        e.component.collapseAll(-1);
        e.component.expandRow(e.currentSelectedRowKeys[0]);
    }
    contentReady(e) {
        // 預設要打開的子表單
        if (!e.component.getSelectedRowKeys().length) {
            e.component.selectRowsByIndexes(0);
        }
    }
    onDataErrorOccurred(e) {
        notify({
            message: e.error.message,
            position: {
                my: 'center top',
                at: 'center top'
            }
        }, 'error', 3000);
    }
    onFocusedRowChanging(e) {
        const rowsCount = e.component.getVisibleRows().length;
        const pageCount = e.component.pageCount();
        const pageIndex = e.component.pageIndex();
        const key = e.event && e.event.key;

        if (key && e.prevRowIndex === e.newRowIndex) {
            if (e.newRowIndex === rowsCount - 1 && pageIndex < pageCount - 1) {
                // tslint:disable-next-line: only-arrow-functions
                e.component.pageIndex(pageIndex + 1).done(function () {
                    e.component.option('focusedRowIndex', 0);
                });
            } else if (e.newRowIndex === 0 && pageIndex > 0) {
                // tslint:disable-next-line: only-arrow-functions
                e.component.pageIndex(pageIndex - 1).done(function () {
                    e.component.option('focusedRowIndex', rowsCount - 1);
                });
            }
        }
    }
    onValueChanged(e) {
        //debugger;
        this.detailfilter = this.myform.instance.option('formData');
        this.dataGrid.instance.refresh();
    }
    onRowPrepared(e) {
        if (e.data !== undefined) {
            let hint = false;
            if (e.data.Status === 1) {
                e.rowElement.style.color = '#008800';
            } else {
                if (e.data !== undefined) {
                    const DeliverydateBefore = new Date(e.data.BillofPurchaseDate);
                    const DeliverydateAfter = new Date(new Date().setDate(new Date().getDate() - 1));
                    if (DeliverydateBefore <= DeliverydateAfter) {
                        hint = true;
                    }
                    if (hint) {
                        e.rowElement.style.color = '#d9534f';
                    }
                }
            }
        }
    }
    onEditingStart(e) {
    }
    onFocusedRowChanged(e) {
    }
    onCellPrepared(e) {
    }
}
