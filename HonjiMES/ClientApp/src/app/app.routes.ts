import { Routes, RouterModule } from '@angular/router';
import { ModuleWithProviders } from '@angular/core';
import { HomepageComponent } from './globalpage/homepage.component';
import {OrderListComponent} from './order-list/order-list.component';
import { MaterialListComponent } from './material-list/material-list.component';
import { ProductListComponent } from './product-list/product-list.component';


export const routes: Routes = [
    { path: '', component: HomepageComponent },
    { path: 'orderlist', component: OrderListComponent },
    { path: 'materiallist', component: MaterialListComponent },
    { path: 'productlist', component: ProductListComponent },
];

export const AppRoutes: ModuleWithProviders = RouterModule.forRoot(routes, { scrollPositionRestoration: 'enabled' });
