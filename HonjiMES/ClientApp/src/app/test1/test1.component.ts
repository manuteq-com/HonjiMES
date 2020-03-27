import { Component, OnInit, ViewChild } from '@angular/core';
import { Test1Service } from '../service/test1.service';
import { Orders } from '../model/orders';
import { SwalComponent } from '@sweetalert2/ngx-sweetalert2';
import { DxDataGridComponent } from 'devextreme-angular';

@Component({
    selector: 'app-test1',
    templateUrl: './test1.component.html',
    styleUrls: ['./test1.component.css']
})
export class Test1Component implements OnInit {
    @ViewChild('okSwal') okSwal: SwalComponent;
    @ViewChild(DxDataGridComponent, { static: false }) dataGrid: DxDataGridComponent;
    saleAmountHeaderFilter: any;
    applyFilterTypes: any;
    currentFilter: any;
    showFilterRow: boolean;
    showHeaderFilter: boolean;

    newOrders: Orders = new Orders();
    BsOrders: Orders[] = [];

    constructor(private sv: Test1Service) {
        this.showFilterRow = true;
        this.showHeaderFilter = true;
    }

    ngOnInit() {
        this.DefaultFill();
    }

    DefaultFill() {
        this.sv.getOrders().subscribe(
            (s) => {
                console.log(s);
                if (s.success) {
                    this.BsOrders = s.data as Orders[];
                }
            }
        );
    }

    onRowUpdated(e) {
        const o = e.data as Orders;
        this.sv.update_tasks(o).subscribe(
            (r) => {
                console.log(r);
            }
        );
    }

    AddTask(e) {
        const o = e.data as Orders;
        this.sv.insert_tasks(o).subscribe(
            (r) => {
                this.DefaultFill();
                this.okSwal.fire();
            }
        );
    }

    onRowRemoved(e) {
        const o = e.data as Orders;
        this.sv.delete_task(o).subscribe(
            (r) => {
                console.log(r);
                this.DefaultFill();
            }
        );
    }

    private getOrderDay(rowData) {
        return (new Date(rowData.OrderDate)).getDay();
    }

    calculateFilterExpression(value, selectedFilterOperations, target) {
        const column = this as any;
        if (target === 'headerFilter' && value === 'weekends') {
            return [[this.getOrderDay, '=', 0], 'or', [this.getOrderDay, '=', 6]];
        }
        return column.defaultCalculateFilterExpression.apply(this, arguments);
    }

    orderHeaderFilter(data) {
        data.dataSource.postProcess = (results) => {
            results.push({
                text: 'Weekends',
                value: 'weekends'
            });
            return results;
        };
    }

    clearFilter() {
        this.dataGrid.instance.clearFilter();
    }
}
