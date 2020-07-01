import { Component, Input, OnInit, EventEmitter, ViewChild } from '@angular/core';
import { trigger, state, style, transition, animate } from '@angular/animations';
import { MenuItem } from 'primeng/primeng';
import { AppComponent } from '../app.component';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { APIResponse } from '../app.module';
import { Observable } from 'rxjs';
import { AuthService } from '../service/auth.service';

@Component({
    selector: 'app-menu',
    templateUrl: 'app.menu.component.html',
    styleUrls: ['./app.menu.component.scss']
})
export class AppMenuComponent implements OnInit {

    @Input() reset: boolean;

    model: any[];
    breadcrumbitem: MenuItem[];
    theme = 'brown';

    layout = 'brown';

    version = 'v4';
    breadcrumbs: string[] = [];
    constructor(public app: AppComponent, route: ActivatedRoute, router: Router, private authenticationService: AuthService) {
        //  this.changeLayout('cyan');

        this.changeLayout('joomla', true);
        this.changeVersion('v4');
        console.log(route.snapshot.paramMap.get('name'));
        route.params.subscribe(params => console.log('side menu id parameter', params.id));
        const bc = this.breadcrumbs;
        router.events.subscribe((evt) => {
            if (evt instanceof NavigationEnd) {
                const url = evt.url;
                if (url === '' || url === '/') {
                    bc.length = 0;
                } else {
                    // debugger;
                    bc.push(evt.url.substr(1));
                    this.breadcrumbitem.push({ label: evt.url.substr(1) });
                }
            }
        });
    }
    ngOnInit() {
        debugger;
        this.breadcrumbitem = [];
        this.model = this.authenticationService.currentUserValue.Menu;
        // this.model = [

        //     // { label: 'Home', icon: 'fa fa-fw fa-home', routerLink: ['/'] },
        //     {
        //         label: '訂單管理', icon: 'fa fa-fw fa-bars',
        //         items: [
        //             { label: '客戶訂單', icon: 'fa fa-fw fa-genderless', routerLink: ['/orderlist'] },
        //             { label: '採購單', icon: 'fa fa-fw fa-genderless', routerLink: ['/purchaseorder'] },
        //             { label: '進貨單', icon: 'fa fa-fw fa-genderless', routerLink: ['/billpurchase'] },
        //             { label: '銷貨單', icon: 'fa fa-fw fa-genderless', routerLink: ['/salelist'] },
        //         ]

        //     },
        //     {
        //         label: '庫存管理', icon: 'fa fa-fw fa-bars',
        //         items: [
        //             { label: '原料庫存', icon: 'fa fa-fw fa-genderless', routerLink: ['/materialbasiclist'] },
        //             { label: '成品庫存', icon: 'fa fa-fw fa-genderless', routerLink: ['/productbasiclist'] },
        //             // { label: '半成品庫存', icon: 'fa fa-fw fa-genderless' },
        //             // { label: '倉庫資訊管理', icon: 'fa fa-fw fa-genderless', routerLink: ['/warehouselist'] },
        //         ]

        //     },
        //     // {
        //     //     label: '採購管理', icon: 'fa fa-fw fa-bars',
        //     //     items: [
        //     //         { label: '採購資料', icon: 'fa fa-fw fa-genderless', routerLink: ['/purchaseorder'] },
        //     //         { label: '進貨資料', icon: 'fa fa-fw fa-genderless', routerLink: ['/billpurchase'] },
        //     //     ]

        //     // },
        //     // {
        //     //     label: '銷貨管理', icon: 'fa fa-fw fa-bars',
        //     //     items: [
        //     //         { label: '銷貨資料', icon: 'fa fa-fw fa-genderless', routerLink: ['/salelist'] },
        //     //     ]

        //     // },
        //     {
        //         label: '組成管理', icon: 'fa fa-fw fa-bars',
        //         items: [
        //             { label: '組成資料', icon: 'fa fa-fw fa-genderless', routerLink: ['/billofmateriallist'] },
        //         ]

        //     },
        //     {
        //         label: '生產管理', icon: 'fa fa-fw fa-bars',
        //         items: [
        //             { label: '領料資料', icon: 'fa fa-fw fa-genderless', routerLink: ['/receiveList'] },
        //         ]

        //     },
        //     {
        //         label: '基本資訊管理', icon: 'fa fa-fw fa-bars',
        //         items: [
        //             { label: '客戶資料', icon: 'fa fa-fw fa-genderless', routerLink: ['/customerlist'] },
        //             { label: '供應商資料', icon: 'fa fa-fw fa-genderless', routerLink: ['/supplierlist'] },
        //             { label: '倉庫資訊管理', icon: 'fa fa-fw fa-genderless', routerLink: ['/warehouselist'] },
        //             { label: '使用者帳戶管理', icon: 'fa fa-fw fa-genderless', routerLink: ['/userlist'] },
        //         ]

        //     },
        //     // { label: '報工表單', icon: 'fa fa-fw fa-bars', routerLink: ['/taskform'] },
        //     // { label: '報工歷史', icon: 'fa fa-fw fa-bars', routerLink: ['/taskview'] },
        //     // { label: '報工歷史', icon: 'fa fa-exchange', routerLink: ['/taskviewdx'] },
        //     // { label: '測試區1', icon: 'fa fa-warning', routerLink: ['/test1'] },
        // ];


    }

    changeTheme(theme: string) {
        const themeLink: HTMLLinkElement = document.getElementById('theme-css') as HTMLLinkElement;

        if (this.version === 'v3') {
            themeLink.href = 'assets/theme/theme-' + theme + '.css';
        } else {
            themeLink.href = 'assets/theme/theme-' + theme + '-v4' + '.css';
        }

        this.theme = theme;

    }

    changeLayout(layout: string, special?: boolean) {
        const layoutLink: HTMLLinkElement = document.getElementById('layout-css') as HTMLLinkElement;

        if (this.version === 'v3') {
            layoutLink.href = 'assets/layout/css/layout-' + layout + '.css';
        } else {
            layoutLink.href = 'assets/layout/css/layout-' + layout + '-v4' + '.css';
        }

        this.layout = layout;

        if (special) {
            this.app.darkMenu = true;
        }
    }

    changeVersion(version: string) {
        const themeLink: HTMLLinkElement = document.getElementById('theme-css') as HTMLLinkElement;
        const layoutLink: HTMLLinkElement = document.getElementById('layout-css') as HTMLLinkElement;

        if (version === 'v3') {
            this.version = 'v3';
            themeLink.href = 'assets/theme/theme-' + this.theme + '.css';
            layoutLink.href = 'assets/layout/css/layout-' + this.layout + '.css';
        } else {
            themeLink.href = 'assets/theme/theme-' + this.theme + '-v4' + '.css';
            layoutLink.href = 'assets/layout/css/layout-' + this.layout + '-v4' + '.css';
            this.version = '-v4';
        }

    }
}

@Component({
    /* tslint:disable:component-selector */
    selector: '[app-submenu]',
    /* tslint:enable:component-selector */
    template: `
        <ng-template ngFor let-child let-i="index" [ngForOf]="(root ? item : item.items)">
            <li [ngClass]="{'active-menuitem': isActive(i)}" [class]="child.badgeStyleClass" *ngIf="child.visible === false ? false : true">
                <a [href]="child.url||'#'" (click)="itemClick($event,child,i)" (mouseenter)="onMouseEnter(i)"
                   class="ripplelink" *ngIf="!child.routerLink"
                    [attr.tabindex]="!visible ? '-1' : null" [attr.target]="child.target">
                    <i [ngClass]="child.icon"></i><span>{{child.label}}</span>
                    <i class="fa fa-fw fa-angle-down menuitem-toggle-icon" *ngIf="child.items"></i>
                    <span class="menuitem-badge" *ngIf="child.badge">{{child.badge}}</span>
                </a>

                <a (click)="itemClick($event,child,i)" (mouseenter)="onMouseEnter(i)" class="ripplelink" *ngIf="child.routerLink"
                    [routerLink]="child.routerLink" routerLinkActive="active-menuitem-routerlink"
                   [routerLinkActiveOptions]="{exact: true}" [attr.tabindex]="!visible ? '-1' : null" [attr.target]="child.target">
                    <i [ngClass]="child.icon"></i><span>{{child.label}}</span>
                    <i class="fa fa-fw fa-angle-down menuitem-toggle-icon" *ngIf="child.items"></i>
                    <span class="menuitem-badge" *ngIf="child.badge">{{child.badge}}</span>
                </a>
                <div class="layout-menu-tooltip">
                    <div class="layout-menu-tooltip-arrow"></div>
                    <div class="layout-menu-tooltip-text">{{child.label}}</div>
                </div>
                <div class="submenu-arrow" *ngIf="child.items"></div>
                <ul app-submenu [item]="child" *ngIf="child.items" [visible]="isActive(i)" [reset]="reset" [parentActive]="isActive(i)"
                    [@children]="(app.isSlim()||app.isHorizontal())&&root ? isActive(i) ?
                     'visible' : 'hidden' : isActive(i) ? 'visibleAnimated' : 'hiddenAnimated'"></ul>
            </li>
        </ng-template>
    `,
    animations: [
        trigger('children', [
            state('hiddenAnimated', style({
                height: '0px'
            })),
            state('visibleAnimated', style({
                height: '*'
            })),
            state('visible', style({
                display: 'block'
            })),
            state('hidden', style({
                display: 'none'
            })),
            transition('visibleAnimated => hiddenAnimated', animate('400ms cubic-bezier(0.86, 0, 0.07, 1)')),
            transition('hiddenAnimated => visibleAnimated', animate('400ms cubic-bezier(0.86, 0, 0.07, 1)'))
        ])
    ]
})
export class AppSubMenuComponent {

    @Input() item: MenuItem;

    @Input() root: boolean;

    @Input() visible: boolean;

    _reset: boolean;

    _parentActive: boolean;

    activeIndex: number;

    constructor(public app: AppComponent) { }

    itemClick(event: Event, item: MenuItem, index: number) {
        if (this.root) {
            this.app.menuHoverActive = !this.app.menuHoverActive;
        }

        // avoid processing disabled items
        if (item.disabled) {
            event.preventDefault();
            return true;
        }

        // activate current item and deactivate active sibling if any
        this.activeIndex = (this.activeIndex === index) ? null : index;

        // execute command
        if (item.command) {
            item.command({ originalEvent: event, item });
        }

        // prevent hash change
        if (item.items || (!item.url && !item.routerLink)) {
            setTimeout(() => {
                this.app.layoutMenuScrollerViewChild.moveBar();
            }, 450);
            event.preventDefault();
        }

        // hide menu
        if (!item.items) {
            if (this.app.isHorizontal() || this.app.isSlim()) {
                this.app.resetMenu = true;
            } else {
                this.app.resetMenu = false;
            }

            this.app.overlayMenuActive = false;
            this.app.staticMenuMobileActive = false;
            this.app.menuHoverActive = !this.app.menuHoverActive;
        }
    }

    onMouseEnter(index: number) {
        if (this.root && this.app.menuHoverActive && (this.app.isHorizontal() || this.app.isSlim())
            && !this.app.isMobile() && !this.app.isTablet()) {
            this.activeIndex = index;
        }
    }

    isActive(index: number): boolean {
        return this.activeIndex === index;
    }

    @Input() get reset(): boolean {
        return this._reset;
    }

    set reset(val: boolean) {
        this._reset = val;

        if (this._reset && (this.app.isHorizontal() || this.app.isSlim())) {
            this.activeIndex = null;
        }
    }

    @Input() get parentActive(): boolean {
        return this._parentActive;
    }

    set parentActive(val: boolean) {
        this._parentActive = val;

        if (!this._parentActive) {
            this.activeIndex = null;
        }
    }
}
