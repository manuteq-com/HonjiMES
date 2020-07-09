
export class LoginUser {
    Username: string;
    Token: string;
    Menu: Menu[];
    Timeout: Date;
}
export class Menu {
    label: string;
    icon: string;
    routerLink?: any;
    items: Item[];
}
export class Item {
    label: string;
    icon: string;
    routerLink?: string[];
    items?: any;
}
