import { Component, AfterViewInit, ElementRef, Renderer2, ViewChild, OnDestroy, OnInit, OnChanges } from '@angular/core';
import { ScrollPanel } from 'primeng';
import { Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { AuthService } from './service/auth.service';
import { LoginUser } from './model/loginuser';
import { APIResponse } from './app.module';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { retry } from 'rxjs/operators';

enum MenuOrientation {
    STATIC,
    OVERLAY,
    SLIM,
    HORIZONTAL
}

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent implements AfterViewInit, OnInit, OnChanges {
    layoutMode: MenuOrientation = MenuOrientation.HORIZONTAL;
    darkMenu = true; // 黑色 Menu
    profileMode = 'top';
    rotateMenuButton: boolean;
    topbarMenuActive: boolean;
    overlayMenuActive: boolean;
    staticMenuDesktopInactive: boolean;
    staticMenuMobileActive: boolean;
    layoutMenuScroller: HTMLDivElement;
    menuClick: boolean;
    topbarItemClick: boolean;
    activeTopbarItem: any;
    resetMenu: boolean;
    menuHoverActive: boolean;
    @ViewChild('layoutMenuScroller', { static: true }) layoutMenuScrollerViewChild: ScrollPanel;
    UserName: string;
    menu: Array<any> = [];
    breadcrumbList: Array<any> = [];
    login$: LoginUser;
    constructor(public renderer: Renderer2, private _router: Router, private _authService: AuthService, private http: HttpClient) {
        this._authService.currentUser.subscribe(x => this.login$ = x);
        this._authService.currentUser.subscribe(x => this.UserName = x ? x.Username : '');
        this.checkRoles = this.checkRoles.bind(this);
    }
    CheckToken(httpOptions: { headers: HttpHeaders; }) {
        this.http.get<APIResponse>('/api/Home/CheckToken', httpOptions).toPromise().then(ReturnData => {
            if (!ReturnData.success) {
                if (ReturnData.message === 'ReLogin') {
                    this.logout(); // 如果是登出的狀態直接導到登入頁面
                }
            }
        });
    }
    public GetData(apiUrl: string): Observable<APIResponse> {
        const authenticationService = new AuthService(this.http);
        const currentUser = authenticationService.currentUserValue;
        let Token = '';
        if (currentUser != null) {
            Token = currentUser.Token;
        }
        const httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                Authorization: 'Bearer ' + Token,
                routerLink: location.pathname,
                apitype: 'GET'
            }),
        };
        this.CheckToken(httpOptions);
        return this.http.get<APIResponse>('/api' + apiUrl, httpOptions);
    }
    public PostData(apiUrl: string, data: any = {}): Observable<APIResponse> {
        const authenticationService = new AuthService(this.http);
        const currentUser = authenticationService.currentUserValue;
        let Token = '';
        if (currentUser != null) {
            Token = currentUser.Token;
        }
        const body = JSON.stringify(data);
        const httpOptions = {
            withCredentials: true, body,
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                Authorization: 'Bearer ' + Token,
                routerLink: location.pathname,
                apitype: 'POST'
            }),
            params: null
        };
        this.CheckToken(httpOptions);
        return this.http.post<APIResponse>('/api' + apiUrl, body, httpOptions);
    }
    ngOnInit() {
        this.listenRouting();
    }
    ngOnChanges() {

    }
    GetUserName() {
        this._authService.currentUser.subscribe(x => this.UserName = x ? x.Username : '');
        return this.UserName;
    }
    logout() {
        this._authService.logout();
        this._router.navigate(['/login']);
    }
    isLoggedIn() {
        const currentUser = localStorage.getItem('currentUser');
        if (currentUser) {
            return of(true);
        } else {
            return of(false);
        }

    }
    /* 監聽routing事件 */
    listenRouting() {
        let routerUrl: string;
        let routerList: Array<any>;
        let target: any;
        this._router.events.subscribe((router: any) => {
            routerUrl = router.urlAfterRedirects;
            if (routerUrl && typeof routerUrl === 'string') {
                // 初始化breadcrumb
                target = this.menu;
                this.breadcrumbList.length = 0;
                // 取得目前routing url用/區格, [0]=第一層, [1]=第二層 ...etc
                routerList = routerUrl.slice(1).split('/');
                routerList.forEach((r, i) => {
                    //debugger;
                    // 找到這一層在menu的路徑和目前routing相同的路徑
                    // target = target.find(page => page.path.slice(2) === r);
                    // // 存到breadcrumbList到時後直接loop這個list就是麵包屑了
                    // this.breadcrumbList.push({
                    //     name: target.name,
                    //     // 第二層開始路由要加上前一層的routing, 用相對位置會造成routing錯誤
                    //     path: (i === 0) ? target.path : `${this.breadcrumbList[i - 1].path}/${target.path.slice(2)}`
                    // });

                    // // 下一層要比對的目標是這一層指定的子頁面
                    // if (i + 1 !== routerList.length) {
                    //     target = target.children;
                    // }
                });

                // console.log(this.breadcrumbList);
            }
        });
    }
    ngAfterViewInit() {
        //   setTimeout(() => { this.layoutMenuScrollerViewChild.moveBar(); }, 100);
    }

    onLayoutClick() {
        if (!this.topbarItemClick) {
            this.activeTopbarItem = null;
            this.topbarMenuActive = false;
        }

        if (!this.menuClick) {
            if (this.isHorizontal() || this.isSlim()) {
                this.resetMenu = true;
            }

            if (this.overlayMenuActive || this.staticMenuMobileActive) {
                this.hideOverlayMenu();
            }

            this.menuHoverActive = false;
        }

        this.topbarItemClick = false;
        this.menuClick = false;
    }

    onMenuButtonClick(event) {
        this.menuClick = true;
        this.rotateMenuButton = !this.rotateMenuButton;
        this.topbarMenuActive = false;

        if (this.layoutMode === MenuOrientation.OVERLAY) {
            this.overlayMenuActive = !this.overlayMenuActive;
        } else {
            if (this.isDesktop()) {
                this.staticMenuDesktopInactive = !this.staticMenuDesktopInactive;
            } else {
                this.staticMenuMobileActive = !this.staticMenuMobileActive;
            }
        }

        event.preventDefault();
    }

    onMenuClick($event) {
        this.menuClick = true;
        this.resetMenu = false;

        if (!this.isHorizontal()) {
            setTimeout(() => { this.layoutMenuScrollerViewChild.moveBar(); }, 450);
        }
    }

    onTopbarMenuButtonClick(event) {
        this.topbarItemClick = true;
        this.topbarMenuActive = !this.topbarMenuActive;

        this.hideOverlayMenu();

        event.preventDefault();
    }

    onTopbarItemClick(event, item) {
        this.topbarItemClick = true;

        if (this.activeTopbarItem === item) {
            this.activeTopbarItem = null;
        } else {
            this.activeTopbarItem = item;
        }

        event.preventDefault();
    }

    onTopbarSubItemClick(event) {
        event.preventDefault();
    }

    hideOverlayMenu() {
        this.rotateMenuButton = false;
        this.overlayMenuActive = false;
        this.staticMenuMobileActive = false;
    }

    isTablet() {
        const width = window.innerWidth;
        return width <= 1024 && width > 640;
    }

    isDesktop() {
        return window.innerWidth > 1024;
    }

    isMobile() {
        return window.innerWidth <= 640;
    }

    isOverlay() {
        return this.layoutMode === MenuOrientation.OVERLAY;
    }

    isHorizontal() {
        return this.layoutMode === MenuOrientation.HORIZONTAL;
    }

    isSlim() {
        return this.layoutMode === MenuOrientation.SLIM;
    }

    changeToStaticMenu() {
        this.layoutMode = MenuOrientation.STATIC;
    }

    changeToOverlayMenu() {
        this.layoutMode = MenuOrientation.OVERLAY;
    }

    changeToHorizontalMenu() {
        this.layoutMode = MenuOrientation.HORIZONTAL;
    }

    changeToSlimMenu() {
        this.layoutMode = MenuOrientation.SLIM;
    }
    checkRoles(e) {
        let canuse = false;
        const authenticationService = new AuthService(this.http);
        const currentUser = authenticationService.currentUserValue;
        currentUser.Menu.forEach(x => {
            if (x.routerLink && x.routerLink.includes(location.pathname)) {
                canuse = this.switchloop(e, x);
            } else {
                if (!canuse && x.items) {
                    x.items.forEach(y => {
                        if (!canuse && y.routerLink && y.routerLink.includes(location.pathname)) {
                            canuse = this.switchloop(e, y);
                        } else {
                            if (!canuse && y.items) {
                                y.items.forEach(z => {
                                    if (z.routerLink && z.routerLink.includes(location.pathname)) {
                                        canuse = this.switchloop(e, z);
                                    }
                                });
                            }
                        }
                    });
                }
            }
        });
        // location.pathname
        return canuse;
    }
    checkAddRoles() {
        return this.checkRoles('Add');
    }
    checkUpdateRoles() {
        return this.checkRoles('Update');
    }
    checkDelRoles() {
        return this.checkRoles('Del');
    }
    switchloop(val, x) {
        let canuse = false;
        switch (val) {
            case 'Add':
                canuse = x.Creat;
                break;
            case 'Update':
                canuse = x.Edit;
                break;
            case 'Del':
                canuse = x.Delete;
                break;
            default:
                break;
        }
        return canuse;
    }
}
