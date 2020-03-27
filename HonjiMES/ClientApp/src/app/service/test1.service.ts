import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Orders } from '../model/orders';
import { environment } from 'src/environments/environment';
import { APIResponse } from '../app.module';

@Injectable({
    providedIn: 'root'
})
export class Test1Service {

    apiurl = environment.apihost;
    constructor(private http: HttpClient) { }
    public getOrders(): Observable<APIResponse> {

        const apiUrl = this.apiurl + 'orders';
        return this.http.get<APIResponse>(apiUrl);
    }

    public update_tasks(o: Orders): Observable<any> {
        const headers = new HttpHeaders({
            'Content-Type': 'application/json'
        }).append('Authorization', 'a');
        const myOption = {
            headers
        };
        const apiUrl = this.apiurl + 'orders/update/' + o.id;
        return this.http.post<any>(apiUrl, o);
    }

    public insert_tasks(o: Orders): Observable<any> {
        const headers = new HttpHeaders({
            'Content-Type': 'application/json'
        }).append('Authorization', 'a');
        const myOption = {
            headers
        };
        const apiUrl = this.apiurl + 'orders/add';
        return this.http.post<APIResponse>(apiUrl, o);
    }

    public delete_task(o: Orders): Observable<APIResponse> {
        const headers = new HttpHeaders({
            'Content-Type': 'application/json'
        }).append('Authorization', 'a');
        const myOption = {
            headers
        };
        const apiUrl = this.apiurl + 'orders/delete/' + o.id;
        return this.http.post<APIResponse>(apiUrl, o);
    }
}
