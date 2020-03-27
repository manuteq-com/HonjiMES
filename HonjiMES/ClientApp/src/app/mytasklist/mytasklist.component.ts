import { Component, OnInit, ViewChild } from '@angular/core';
import { MytaskService } from '../service/mytask.service';
import { MyTask } from '../model/mytask';
import { SwalComponent } from '@sweetalert2/ngx-sweetalert2';

@Component({
    selector: 'app-mytasklist',
    templateUrl: './mytasklist.component.html',
    styleUrls: ['./mytasklist.component.css']
})
export class MytasklistComponent implements OnInit {
    @ViewChild('okSwal') okSwal: SwalComponent;

    newTask: MyTask = new MyTask();
    BsTask: MyTask[] = [];
    constructor(private sv: MytaskService) { }

    ngOnInit() {
        this.DefaultFill();
        this.newTask.titles = null;
        this.newTask.taskcontent = null;
    }
    DefaultFill() {
        this.sv.gettasks().subscribe(
            (s) => {
                if (s.success) {
                    this.BsTask = s.data as MyTask[];
                }
            }
        );
    }
    AddTask() {
        this.newTask.closed = 0;
        //  this.newTask.closetimes = new Date();
        this.sv.insert_tasks(this.newTask).subscribe(
            (r) => {
                this.DefaultFill();
                this.okSwal.fire();
            }
        );
        this.newTask = new MyTask();

    }
    onRowUpdated(e) {
        const o = e.data as MyTask;
        this.sv.updatetasks(o).subscribe(
            (r) => {
                console.log(r);
            }
        );
    }
    onRowRemoved(e) {
        const o = e.data as MyTask;
        this.sv.delete_task(o).subscribe(
            (r) => {
                console.log(r);
                this.DefaultFill();
            }
        );
    }
}
