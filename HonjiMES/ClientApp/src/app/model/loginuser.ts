
export class LoginUser {
    Username: string;
    Token: string;
    Menu: Menu[];
    Timeout: Date;
    Realname: string;
}
export class Menu {
    label: string;
    icon: string;
    routerLink?: string[];
    items: Menu[];
    Query: boolean;
    Creat: boolean;
    Edit: boolean;
    Delete: boolean;
}
// export class Item {
//     label: string;
//     icon: string;
//     routerLink?: string[];
//     items?: any;
// }
