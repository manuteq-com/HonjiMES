import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-process-control',
    templateUrl: './process-control.component.html',
    styleUrls: ['./process-control.component.css']
})
export class ProcessControlComponent implements OnInit {
    dataSourceDB: any;
    listOfData = [
        {
            key: '1',
            name: 'John Brown',
            age: 32,
            address: 'New York No. 1 Lake Park'
        },
        {
            key: '2',
            name: 'Jim Green',
            age: 42,
            address: 'London No. 1 Lake Park'
        },
        {
            key: '3',
            name: 'Joe Black',
            age: 32,
            address: 'Sidney No. 1 Lake Park'
        }
    ];

    columnOptions = [
        { key: 'name', title: '名稱', span: true },
        { key: 'No', title: '品號', span: false },
        { key: 'count', title: '數量', span: false },
    ]
    constructor() {

    }

    ngOnInit() {
    }

}
