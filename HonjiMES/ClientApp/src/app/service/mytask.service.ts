import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MyTask } from '../model/mytask';
import { environment } from 'src/environments/environment';
import { APIResponse } from '../app.module';

@Injectable({
    providedIn: 'root'
})
export class MytaskService {

    apiurl = environment.apihost;
    constructor(private http: HttpClient) { }
    public gettasks(): Observable<APIResponse> {

        const apiUrl = this.apiurl + 'api/gettask/';
        return this.http.get<APIResponse>(apiUrl);
    }
    public gettaskimage(gid: string): Observable<APIResponse> {

        const apiUrl = this.apiurl + 'api/gettaskimage/' + gid;
        return this.http.get<APIResponse>(apiUrl);
    }
    public updatetasks(o: MyTask): Observable<any> {
        const headers = new HttpHeaders({
            'Content-Type': 'application/json'
        }).append('Authorization', 'a');
        const myOption = {
            headers
        };
        const apiUrl = this.apiurl + 'api/updatetask/';
        return this.http.post<any>(apiUrl, o);
    }
    public insert_tasks(o: MyTask): Observable<APIResponse> {
        const headers = new HttpHeaders({
            'Content-Type': 'application/json'
        }).append('Authorization', 'a');
        const myOption = {
            headers
        };
        const apiUrl = this.apiurl + 'api/insert_task/';
        return this.http.post<APIResponse>(apiUrl, o);
    }
    public delete_task(o: MyTask): Observable<APIResponse> {
        const headers = new HttpHeaders({
            'Content-Type': 'application/json'
        }).append('Authorization', 'a');
        const myOption = {
            headers
        };
        const apiUrl = this.apiurl + 'api/delete_task/';
        return this.http.post<APIResponse>(apiUrl, o);
    }
}
