import { Component, AfterViewInit, ElementRef, Renderer2, ViewChild, OnDestroy, OnInit } from '@angular/core';
import { ScrollPanel } from 'primeng';
import { Router } from '@angular/router';

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
export class AppComponent implements AfterViewInit, OnInit {

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
    menu: Array<any> = [];
    breadcrumbList: Array<any> = [];

    constructor(public renderer: Renderer2, private _router: Router) { }
    ngOnInit() {

        this.listenRouting();
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
        setTimeout(() => { this.layoutMenuScrollerViewChild.moveBar(); }, 100);
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

}
