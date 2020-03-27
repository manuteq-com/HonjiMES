import { Component, OnInit } from '@angular/core';
import { MyTask } from '../model/mytask';
import { MytaskService } from '../service/mytask.service';
import { taskimage } from '../model/taskimage';
import { environment } from 'src/environments/environment';

@Component({
    selector: 'app-mytaskview-dx',
    templateUrl: './mytaskview-dx.component.html',
    styleUrls: ['./mytaskview-dx.component.scss']
})
export class MytaskviewDxComponent implements OnInit {

    MyTasks: MyTask[] = [];
    MyTaskImage: taskimage[] = [];
    currentImage = '';
    constructor(private sv: MytaskService) { }

    ngOnInit() {
        this.sv.gettasks().subscribe(
            (r) => {
                if (r.success) {
                    this.MyTasks = r.data as MyTask[];
                }
            }
        );
    }
    onSelectionChanged(e) {
        this.currentImage = '';
        const items = e.addedItems as MyTask[];
        const item = items[0];
        this.sv.gettaskimage(item.gid).subscribe(
            (r) => {
                if (r.success) {
                    this.MyTaskImage = r.data as taskimage[];
                    if (this.MyTaskImage.length > 0) {
                        this.currentImage =
                            environment.apihost + '/Uploaded/Images/' +
                            this.MyTaskImage[0].targetfolder
                            + '/' + this.MyTaskImage[0].filename;
                    }
                }
            }
        );
    }

}
