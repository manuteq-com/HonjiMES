import notify from 'devextreme/ui/notify';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { APIResponse } from '../app.module';
import { environment } from 'src/environments/environment';

export class Guid {
  static newGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
      // tslint:disable-next-line: triple-equals
      let r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }
}

export class MyMsg {

  public static showSuccess(msg: string) {// 吐司訊息區
    const options = {
      message: msg,
      closeOnClick: true,
      visible: true,
      maxWidth: '600px',
      position: { at: 'center center', of: 'window', offset: '5 5' }
    };
    // notify(options,"success",2000);
    notify(options, 'success', 2000);
    // The message's type: "info", "warning", "error" or "success"
  }
  public static showErr(msg: string) {// 吐司訊息區
    const options = {
      message: msg,
      closeOnClick: true,
      visible: true,
      maxWidth: '600px',
      position: { at: 'center center', of: 'window', offset: '5 5' }
    };
    // notify(options,"success",2000);
    notify(options, 'error', 2000);
    // The message's type: "info", "warning", "error" or "success"
  }

  public static showToast(msg: string, MessageType: string, delaytime: number) {// 吐司訊息區
    const options = {
      message: msg,
      closeOnClick: true,
      visible: true,
      maxWidth: '600px',
      position: { at: 'center center', of: window, offset: '5 5' }
    };
    // notify(options,"success",2000);
    notify(options, MessageType, delaytime);
    // The message's type: "info", "warning", "error" or "success"
  }

}

export class DateTimeTool {
  public static formatDate(date) {
    const d = new Date(date);
    let month = '' + (d.getMonth() + 1);
    let day = '' + d.getDate();
    const year = d.getFullYear();

    if (month.length < 2) {
      month = '0' + month;
    }
    if (day.length < 2) {
      day = '0' + day;
    }

    return [year, month, day].join('-');
  }
}

// export class SendService {
//     constructor( ) { }
//     public  static sendRequest(http: HttpClient , url: string, method: string = 'GET', data: any = {}): Observable<APIResponse> {
//         debugger;
//         const apiurl = location.origin + '/api';
//         const body = JSON.stringify(data.values);
//         const keyurl = '/' + data.key;
//         const httpOptions = { withCredentials: true, body, headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
//         let result;
//         switch (method) {
//           case 'GET':
//             result = http.get<APIResponse>(apiurl + url, httpOptions);
//             break;
//           case 'PUT':
//             result = http.put<APIResponse>(apiurl + url + keyurl, body, httpOptions);
//             break;
//           case 'POST':
//             result = http.post<APIResponse>(apiurl + url + keyurl, body, httpOptions);
//             break;
//           case 'DELETE':
//             result = http.delete<APIResponse>(apiurl + url + keyurl, httpOptions);
//             break;
//         }
//         return result;
//       }
//     }


export class SendService {
    constructor( ) { }
    public  static sendRequest(http: HttpClient , url: string, method: string = 'GET', data: any = {}): any {
        // debugger;
        const apiurl = location.origin + '/api';
        const body = JSON.stringify(data.values);
        let keyurl = '' ;
        if (data.key) {
            keyurl = '/' + data.key;
        }
        const httpOptions = { withCredentials: true, body, headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
        let result;
        switch (method) {
          case 'GET':
            result = http.get(apiurl + url, httpOptions);
            break;
          case 'PUT':
            result = http.put(apiurl + url + keyurl, body, httpOptions);
            break;
          case 'POST':
            result = http.post(apiurl + url , body, httpOptions);
            break;
          case 'DELETE':
            result = http.delete(apiurl + url + keyurl, httpOptions);
            break;
        }
        return result.toPromise()
        .then((data: any) => {
          if (data.success) {
                  return (data.data);
          } else {
            //   debugger;
            notify({
                message: data.message,
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'error', 3000);
            throw data.message;
          }
        })
        .catch(e => {
            notify({
                message: e.error,
                position: {
                    my: 'center top',
                    at: 'center top'
                }
            }, 'error', 3000);
            throw e.error;
        });
      }
    }



// export class SendRequest {
//     apiurl = environment.apihost + '/api';
//     constructor(private http: HttpClient) { }
//     public   sendRequest(url: string, method: string = 'GET', data: any = {}): Observable<APIResponse> {
//         const body = JSON.stringify(data.values);
//         const keyurl = '/' + data.key;
//         const httpOptions = { withCredentials: true, body, headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
//         let result;
//         switch (method) {
//           case 'GET':
//             result = this.http.get<APIResponse>(this.apiurl + url, httpOptions);
//             break;
//           case 'PUT':
//             result = this.http.put<APIResponse>(this.apiurl + url + keyurl, body, httpOptions);
//             break;
//           case 'POST':
//             result = this.http.post<APIResponse>(this.apiurl + url, body, httpOptions);
//             break;
//           case 'DELETE':
//             result = this.http.delete<APIResponse>(this.apiurl + url + keyurl, httpOptions);
//             break;
//         }
//         return result;
//       }
//     }

