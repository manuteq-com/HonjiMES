import { Component, OnInit } from '@angular/core';
import { MytaskService } from '../service/mytask.service';
import { MyTask } from '../model/mytask';

@Component({
    selector: 'app-mytaskview',
    templateUrl: './mytaskview.component.html',
    styleUrls: ['./mytaskview.component.css']
})
export class MytaskviewComponent implements OnInit {

    pagetitle = '報工歷史';
    mytask: MyTask[] = [];
    constructor(private sv: MytaskService) { }

    ngOnInit() {
        this.sv.gettasks().subscribe(
            (r) => {
                if (r.success) {
                    this.mytask = r.data as MyTask[];
                }
            }
        );
    }

}
