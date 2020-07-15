import { Routes, RouterModule } from '@angular/router';
import { ModuleWithProviders } from '@angular/core';
import { HomepageComponent } from './globalpage/homepage.component';
import { MaterialListComponent } from './material/material-list/material-list.component';
import { OrderListComponent } from './order/order-list/order-list.component';
import { SaleListComponent } from './sale/sale-list/sale-list.component';
import { PurchaseOrderComponent } from './purchase/purchase-order/purchase-order.component';
import { BillPurchaseComponent } from './billpurchase/bill-purchase/bill-purchase.component';
import { CustomerListComponent } from './customer/customer-list/customer-list.component';
import { SupplierListComponent } from './supplier/supplier-list/supplier-list.component';
import { WarehouseListComponent } from './warehouse/warehouse-list/warehouse-list.component';
import { UserListComponent } from './setting/user-list/user-list.component';
import { BillofmateriallistComponent } from './billofmaterial/billofmateriallist/billofmateriallist.component';
import { ProductBasicListComponent } from './product/product-basic-list/product-basic-list.component';
import { WiproductBasicListComponent } from './wiproduct/wiproduct-basic-list/wiproduct-basic-list.component';
import { MaterialBasicListComponent } from './material/material-basic-list/material-basic-list.component';
import { ReceiveListComponent } from './receive/receive-list/receive-list.component';
import { ProcessListComponent } from './process/process-list/process-list.component';
import { AuthGuard } from './helpers/auth.guard';
import { LoginComponent } from './login/login/login.component';
import { ProcessControlComponent } from './process/process-control/process-control.component';



export const routes: Routes = [
    { path: '', component: HomepageComponent, canActivate: [AuthGuard] },
    { path: 'login', component: LoginComponent },
    { path: 'orderlist', component: OrderListComponent, canActivate: [AuthGuard] },
    { path: 'materiallist', component: MaterialListComponent, canActivate: [AuthGuard] },
    { path: 'productbasiclist', component: ProductBasicListComponent, canActivate: [AuthGuard] },
    { path: 'materialbasiclist', component: MaterialBasicListComponent, canActivate: [AuthGuard] },
    { path: 'salelist', component: SaleListComponent, canActivate: [AuthGuard] },
    { path: 'billpurchase', component: BillPurchaseComponent, canActivate: [AuthGuard] },
    { path: 'purchaseorder', component: PurchaseOrderComponent, canActivate: [AuthGuard] },
    { path: 'customerlist', component: CustomerListComponent, canActivate: [AuthGuard] },
    { path: 'supplierlist', component: SupplierListComponent, canActivate: [AuthGuard] },
    { path: 'warehouselist', component: WarehouseListComponent, canActivate: [AuthGuard] },
    { path: 'userlist', component: UserListComponent, canActivate: [AuthGuard] },
    { path: 'billofmateriallist', component: BillofmateriallistComponent, canActivate: [AuthGuard] },
    { path: 'wiproductbasiclist', component: WiproductBasicListComponent, canActivate: [AuthGuard] },
    { path: 'processlist', component: ProcessListComponent, canActivate: [AuthGuard] },
    { path: 'receiveList', component: ReceiveListComponent, canActivate: [AuthGuard] },
    { path: 'processControl', component: ProcessControlComponent }
];

export const AppRoutes: ModuleWithProviders = RouterModule.forRoot(routes, { scrollPositionRestoration: 'enabled' });
