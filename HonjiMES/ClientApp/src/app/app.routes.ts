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
import { MaterialBasicListComponent } from './material/material-basic-list/material-basic-list.component';


export const routes: Routes = [
    { path: '', component: HomepageComponent },
    { path: 'orderlist', component: OrderListComponent },
    { path: 'materiallist', component: MaterialListComponent },
    { path: 'productbasiclist', component: ProductBasicListComponent },
    { path: 'materialbasiclist', component: MaterialBasicListComponent },
    { path: 'salelist', component: SaleListComponent },
    { path: 'billpurchase', component: BillPurchaseComponent },
    { path: 'purchaseorder', component: PurchaseOrderComponent },
    { path: 'customerlist', component: CustomerListComponent },
    { path: 'supplierlist', component: SupplierListComponent },
    { path: 'warehouselist', component: WarehouseListComponent },
    { path: 'userlist', component: UserListComponent },
    { path: 'Billofmateriallist', component: BillofmateriallistComponent }
];

export const AppRoutes: ModuleWithProviders = RouterModule.forRoot(routes, { scrollPositionRestoration: 'enabled' });
