"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var router_1 = require("@angular/router");
var homepage_component_1 = require("./globalpage/homepage.component");
var mytasklist_component_1 = require("./mytasklist/mytasklist.component");
var mytaskform_component_1 = require("./mytaskform/mytaskform.component");
var mytaskview_component_1 = require("./mytaskview/mytaskview.component");
var mytaskview_dx_component_1 = require("./mytaskview-dx/mytaskview-dx.component");
var test1_component_1 = require("./test1/test1.component");
var order_list_component_1 = require("./order-list/order-list.component");
var material_list_component_1 = require("./material/material-list/material-list.component");
var product_list_component_1 = require("./product-list/product-list.component");
var customer_list_component_1 = require("./customer/customer-list/customer-list.component");
var supplier_list_component_1 = require("./supplier/supplier-list/supplier-list.component");
var adjust_list_component_1 = require("./inventory/adjust-list/adjust-list.component");
var adjust_log_component_1 = require("./inventory/adjust-log/adjust-log.component");
var inventory_log_component_1 = require("./inventory/inventory-log/inventory-log.component");
var warehouse_list_component_1 = require("./warehouse/warehouse-list/warehouse-list.component");
var user_list_component_1 = require("./setting/user-list/user-list.component");
var work_order_log_component_1 = require("./workorder/workorder-log/workorder-log.component");
var resource_allocation_component_1 = require("./workorder/resource_allocation/resource_allocation.component");
var user_password_1 = require("./setting/user_password/user_password.component");
var sale_return_1 = require("./report/sale_return/sale_return.component");
var billpurchase_return_1 = require("./report/billpurchase_return/billpurchase_return.component");
var quality_record_1 = require("./report/quality_record/quality_record.component");

exports.routes = [
    { path: '', component: homepage_component_1.HomepageComponent },
    { path: 'tasklist', component: mytasklist_component_1.MytasklistComponent },
    { path: 'taskform', component: mytaskform_component_1.MytaskformComponent },
    { path: 'taskview', component: mytaskview_component_1.MytaskviewComponent },
    { path: 'taskviewdx', component: mytaskview_dx_component_1.MytaskviewDxComponent },
    { path: 'test1', component: test1_component_1.Test1Component },
    { path: 'orderlist', component: order_list_component_1.OrderListComponent },
    { path: 'materiallist', component: material_list_component_1.MaterialListComponent },
    { path: 'productlist', component: product_list_component_1.ProductListComponent },
    { path: 'customerlist', component: customer_list_component_1.CustomerListComponent },
    { path: 'supplierlist', component: supplier_list_component_1.SupplierListComponent },
    { path: 'adjustlist', component: adjust_list_component_1.AdjustListComponent },
    { path: 'adjustlog', component: adjust_log_component_1.AdjustLogComponent },
    { path: 'inventorylog', component: inventory_log_component_1.InventoryLogComponent },
    { path: 'warehouselist', component: warehouse_list_component_1.WarehouseListComponent },
    { path: 'userlist', component: user_list_component_1.UserListComponent },
    { path: 'workorderlog', component: work_order_log_component_1.WorkorderLogComponent },
    { path: 'resourceallocation', component: resource_allocation_component_1.ResourceAllocationComponent },
    { path: 'userpassword', component: user_password_1.UserPasswordComponent },
    { path: 'salereturn', component: sale_return_1.SaleReturnComponent },
    { path: 'billpurchasereturn', component: billpurchase_return_1.BiilpurchaseReturnComponent },
    { path: 'qualityrecord', component: quality_record_1.QualityRecordComponent },
];
exports.AppRoutes = router_1.RouterModule.forRoot(exports.routes, { scrollPositionRestoration: 'enabled' });
//# sourceMappingURL=app.routes.js.map
