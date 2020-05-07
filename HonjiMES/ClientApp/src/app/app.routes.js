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
];
exports.AppRoutes = router_1.RouterModule.forRoot(exports.routes, { scrollPositionRestoration: 'enabled' });
//# sourceMappingURL=app.routes.js.map
