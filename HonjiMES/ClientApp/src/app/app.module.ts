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
    DxCheckBoxModule, DxFileUploaderModule, DxPopupModule, DxNumberBoxModule, DxTreeListModule
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
import { OrderListComponent } from './order/order-list/order-list.component';
import { CreatorderComponent } from './order/creatorder/creatorder.component';
import { BreadcrumbModule } from 'primeng';
import { OerdrdetailListComponent } from './order/oerdrdetail-list/oerdrdetail-list.component';
import { ProductListComponent } from './product/product-list/product-list.component';
import { MaterialListComponent } from './material/material-list/material-list.component';
import { CreatproductComponent } from './product/creatproduct/creatproduct.component';
import { CreatmaterialComponent } from './material/creatmaterial/creatmaterial.component';
import { InventoryChangeComponent } from './inventory-change/inventory-change.component';
import { OrdertosaleComponent } from './ordertosale/ordertosale.component';
import { SaleListComponent } from './sale/sale-list/sale-list.component';
import { ReOrderSaleComponent } from './sale/re-order-sale/re-order-sale.component';
import { BillPurchaseComponent } from './billpurchase/bill-purchase/bill-purchase.component';
import { PurchaseOrderComponent } from './purchase/purchase-order/purchase-order.component';
import { CreatBillPurchaseComponent } from './billpurchase/creat-bill-purchase/creat-bill-purchase.component';
import { BillPurchaseDetailComponent } from './billpurchase/bill-purchase-detail/bill-purchase-detail.component';
import { PurchaseDetailComponent } from './purchase/purchase-detail/purchase-detail.component';
import { CreatPurchaseComponent } from './purchase/creat-purchase/creat-purchase.component';
import { BillPurchaseCheckinComponent } from './billpurchase/bill-purchase-checkin/bill-purchase-checkin.component';
import { CustomerListComponent } from './customer/customer-list/customer-list.component';
import { CreatcustomerComponent } from './customer/creatcustomer/creatcustomer.component';
import { BillofmateriallistComponent } from './billofmaterial/billofmateriallist/billofmateriallist.component';
import { SupplierListComponent } from './supplier/supplier-list/supplier-list.component';
import { CreatsupplierComponent } from './supplier/creatsupplier/creatsupplier.component';
import { WarehouseListComponent } from './warehouse/warehouse-list/warehouse-list.component';
import { CreatwarehouseComponent } from './warehouse/creatwarehouse/creatwarehouse.component';
import { UserListComponent } from './setting/user-list/user-list.component';
import { CreatuserComponent } from './setting/creatuser/creatuser.component';
import { BomlistComponent } from './billofmaterial/bomlist/bomlist.component';
import { ToOrderSaleComponent } from './sale/to-order-sale/to-order-sale.component';
import { EditbomComponent } from './billofmaterial/editbom/editbom.component';
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
        DxTreeListModule,
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
        SaleListComponent,
        ReOrderSaleComponent,
        BillPurchaseComponent,
        PurchaseOrderComponent,
        CreatBillPurchaseComponent,
        BillPurchaseDetailComponent,
        PurchaseDetailComponent,
        CreatPurchaseComponent,
        BillPurchaseCheckinComponent,
        CustomerListComponent,
        CreatcustomerComponent,
        BillofmateriallistComponent,
        SupplierListComponent,
        CreatsupplierComponent,
        WarehouseListComponent,
        CreatwarehouseComponent,
        UserListComponent,
        CreatuserComponent,
        BomlistComponent,
        ToOrderSaleComponent,
        EditbomComponent,
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
