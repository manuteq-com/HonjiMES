import { Routes, RouterModule } from '@angular/router';
import { ModuleWithProviders } from '@angular/core';
import { HomepageComponent } from './globalpage/homepage.component';
import {OrderListComponent} from './order/order-list/order-list.component';
import { MaterialListComponent } from './material-list/material-list.component';
import { ProductListComponent } from './product/product-list/product-list.component';
import { SaleListComponent } from './sale-list/sale-list.component';
import { PurchaseOrderComponent } from './purchase/purchase-order/purchase-order.component';
import { BillPurchaseComponent } from './billpurchase/bill-purchase/bill-purchase.component';


export const routes: Routes = [
    { path: '', component: HomepageComponent },
    { path: 'orderlist', component: OrderListComponent },
    { path: 'materiallist', component: MaterialListComponent },
    { path: 'productlist', component: ProductListComponent },
    { path: 'salelist', component: SaleListComponent },
    { path: 'billpurchase', component: BillPurchaseComponent },
    { path: 'purchaseorder', component: PurchaseOrderComponent }
];

export const AppRoutes: ModuleWithProviders = RouterModule.forRoot(routes, { scrollPositionRestoration: 'enabled' });
