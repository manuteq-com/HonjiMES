import { Routes, RouterModule } from '@angular/router';
import { ModuleWithProviders } from '@angular/core';
import { HomepageComponent } from './globalpage/homepage.component';
import { MytasklistComponent } from './mytasklist/mytasklist.component';
import { MytaskformComponent } from './mytaskform/mytaskform.component';
import { MytaskviewComponent } from './mytaskview/mytaskview.component';
import { MytaskviewDxComponent } from './mytaskview-dx/mytaskview-dx.component';
import { Test1Component } from './test1/test1.component';
import {OrderListComponent} from './order-list/order-list.component';
import { OerdrdetailListComponent } from './oerdrdetail-list/oerdrdetail-list.component';

export const routes: Routes = [
    { path: '', component: HomepageComponent },
    { path: 'tasklist', component: MytasklistComponent },
    { path: 'taskform', component: MytaskformComponent },
    { path: 'taskview', component: MytaskviewComponent },
    { path: 'taskviewdx', component: MytaskviewDxComponent },
    { path: 'test1', component: Test1Component },
    { path: 'orderlist', component: OrderListComponent },
    { path: 'oerdrdetailList', component: OerdrdetailListComponent },
];

export const AppRoutes: ModuleWithProviders = RouterModule.forRoot(routes, { scrollPositionRestoration: 'enabled' });
