import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LocationStrategy, HashLocationStrategy } from '@angular/common';
import { AppRoutes } from './app.routes';

import { MenuModule } from 'primeng/menu';
import { ScrollPanelModule } from 'primeng/scrollpanel';


import { AppComponent } from './app.component';
import { AppMenuComponent, AppSubMenuComponent } from './layout/app.menu.component';
import { AppTopBarComponent } from './layout/app.topbar.component';
import { AppFooterComponent } from './layout/app.footer.component';
import { AppProfileComponent } from './app.profile.component';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';


import {
    DxDataGridModule, DxButtonModule, DxTreeViewModule, DxDropDownBoxModule, DxSelectBoxModule,
    DxDateBoxModule, DxTextBoxModule, DxTextAreaModule, DxFormModule, DxMultiViewModule, DxTemplateModule,
    DxCheckBoxModule, DxFileUploaderModule, DxPopupModule, DxNumberBoxModule
} from 'devextreme-angular';

import { HomepageComponent } from './globalpage/homepage.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
// import { MytasklistComponent } from './mytasklist/mytasklist.component';
// import { MytaskformComponent } from './mytaskform/mytaskform.component';
// import { MytaskviewComponent } from './mytaskview/mytaskview.component';
// import { MytaskviewDxComponent } from './mytaskview-dx/mytaskview-dx.component';
// 多國語系
import { locale, loadMessages } from 'devextreme/localization';
import zhTW from '../assets/lang/zh-TW.json';
// import { Test1Component } from './test1/test1.component';
import { OrderListComponent } from './order-list/order-list.component';
import { CreatorderComponent } from './creatorder/creatorder.component';
import { BreadcrumbModule } from 'primeng';
import { OerdrdetailListComponent } from './oerdrdetail-list/oerdrdetail-list.component';
import { ProductListComponent } from './product-list/product-list.component';
import { MaterialListComponent } from './material-list/material-list.component';
import { CreatproductComponent } from './creatproduct/creatproduct.component';
import { CreatmaterialComponent } from './creatmaterial/creatmaterial.component';
import { InventoryChangeComponent } from './inventory-change/inventory-change.component';
import { OrdertosaleComponent } from './ordertosale/ordertosale.component';
@NgModule({
    imports: [
        BrowserModule,
        FormsModule,
        AppRoutes,
        NgbModule,
        HttpClientModule,
        BrowserAnimationsModule,
        MenuModule,
        ScrollPanelModule,
        DxButtonModule,
        DxTreeViewModule,
        DxDropDownBoxModule,
        DxDataGridModule,
        DxSelectBoxModule,
        DxDateBoxModule,
        DxTextBoxModule,
        DxTextAreaModule,
        DxFormModule,
        DxMultiViewModule,
        DxTemplateModule,
        DxFileUploaderModule,
        DxPopupModule,
        DxNumberBoxModule,
        DxCheckBoxModule,
        BreadcrumbModule,
        SweetAlert2Module.forRoot()
    ],
    declarations: [
        AppComponent,
        AppMenuComponent,
        AppSubMenuComponent,
        AppTopBarComponent,
        AppFooterComponent,
        AppProfileComponent,
        HomepageComponent,
        // MytasklistComponent,
        // MytaskformComponent,
        // MytaskviewComponent,
        // MytaskviewDxComponent,
        // Test1Component,
        OrderListComponent,
        CreatorderComponent,
        OerdrdetailListComponent,
        ProductListComponent,
        MaterialListComponent,
        CreatproductComponent,
        CreatmaterialComponent,
        InventoryChangeComponent,
        OrdertosaleComponent,
    ],
    providers: [
        { provide: LocationStrategy, useClass: HashLocationStrategy }
    ],
    bootstrap: [AppComponent]
})
export class AppModule {
    constructor() {
        loadMessages(zhTW);
        locale('zh-TW');
        // ??? tsconfig ? JSON ??
        // "resolveJsonModule": true,
        // "esModuleInterop": true,
        // ?? typings.d.ts
    }
}
export class APIResponse {
    success: boolean;

    timestamp: string;
    message: any;

    data: any;
  }
