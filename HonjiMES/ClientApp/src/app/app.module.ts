import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LocationStrategy, HashLocationStrategy, PathLocationStrategy } from '@angular/common';
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
    DxCheckBoxModule, DxFileUploaderModule, DxPopupModule, DxNumberBoxModule, DxTreeListModule, DxLoadPanelModule,
    DxSchedulerModule, DxTabPanelModule, DxGalleryModule, DxTabsModule, DxScrollViewModule, DxLookupModule
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
import { OrderdetailListComponent } from './order/orderdetail-list/orderdetail-list.component';
import { ProductListComponent } from './product/product-list/product-list.component';
import { MaterialListComponent } from './material/material-list/material-list.component';
import { CreatproductComponent } from './product/creatproduct/creatproduct.component';
import { CreatmaterialComponent } from './material/creatmaterial/creatmaterial.component';
import { InventoryChangeComponent } from './inventory/inventory-change/inventory-change.component';
import { OrdertosaleComponent } from './order/ordertosale/ordertosale.component';
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
import { ProductBasicListComponent } from './product/product-basic-list/product-basic-list.component';
import { MaterialBasicListComponent } from './material/material-basic-list/material-basic-list.component';
import { CreatproductBasicComponent } from './product/creatproduct-basic/creatproduct-basic.component';
import { CreatmaterialBasicComponent } from './material/creatmaterial-basic/creatmaterial-basic.component';
import { ReceiveListComponent } from './receive/receive-list/receive-list.component';
import { ReceiveDetailListComponent } from './receive/receivedetail-list/receivedetail-list.component';
import { BillPurchaseReturnComponent } from './billpurchase/bill-purchase-return/bill-purchase-return.component';
import { InventoryListComponent } from './inventory/inventory-list/inventory-list.component';
import { JwtModule } from '@auth0/angular-jwt';
import { LoginComponent } from './login/login/login.component';
import { WiproductListComponent } from './wiproduct/wiproduct-list/wiproduct-list.component';
import { WiproductBasicListComponent } from './wiproduct/wiproduct-basic-list/wiproduct-basic-list.component';
import { CreatwiproductComponent } from './wiproduct/creatwiproduct/creatwiproduct.component';
import { CreatwiproductBasicComponent } from './wiproduct/creatwiproduct-basic/creatwiproduct-basic.component';
import { ProcessListComponent } from './process/process-list/process-list.component';
import { CreatprocessComponent } from './process/creatprocess/creatprocess.component';
import { UserRolesComponent } from './setting/user-roles/user-roles.component';
import { AdjustListComponent } from './inventory/adjust-list/adjust-list.component';
import { ProcessControlComponent } from './process/process-control/process-control.component';
import { MbillofmateriallistComponent } from './mbillofmaterial/mbillofmateriallist/mbillofmateriallist.component';
import { NgZorroAntdModule, NzDrawerModule, NzPaginationModule } from 'ng-zorro-antd';
import { NZ_I18N, zh_TW } from 'ng-zorro-antd/i18n';
import { BomverlistComponent } from './billofmaterial/bomverlist/bomverlist.component';
import { CreatprocessControlComponent } from './process/creatprocess-control/creatprocess-control.component';
import { WorkorderListComponent } from './workorder/workorder-list/workorder-list.component';
import { WorkorderReportComponent } from './workorder/workorder-report/workorder-report.component';
import { AdjustLogComponent } from './inventory/adjust-log/adjust-log.component';
import { AdjustdetailListComponent } from './inventory/adjustdetail-list/adjustdetail-list.component';
import { WorkorderLogComponent } from './workorder/workorder-log/workorder-log.component';
import { SupplierMaterialComponent } from './supplier/supplier-material/supplier-material.component';
import { QrcodeComponent } from './process/qrcode/qrcode.component';
import { MbillofmaterialModelComponent } from './mbillofmaterial/mbillofmaterial-model/mbillofmaterial-model.component';
import { WorkorderQaComponent } from './workorder/workorder-qa/workorder-qa.component';
import { WorkorderReportLogComponent } from './workorder/workorder-report-log/workorder-report-log.component';
import { ProcessControlViewComponent } from './process/process-control-view/process-control-view.component';
import { EditworkorderComponent } from './workorder/editworkorder/editworkorder.component';
import { WorksChedulerComponent } from './workscheduler/work-scheduler/work-scheduler.component';
import { ResourceAllocationComponent } from './workorder/resource-allocation/resource-allocation.component';
import { InventoryLogComponent } from './inventory/inventory-log/inventory-log.component';
import { CreateSaleComponent } from './sale/create-sale/create-sale.component';
import { BillPurchaseSupplierComponent } from './billpurchase/bill-purchase-supplier/bill-purchase-supplier.component';
import { CreatreceiveComponent } from './receive/creatreceive/creatreceive.component';
import { RebackListComponent } from './reback/reback-list/reback-list.component';
import { CreatrebackComponent } from './reback/creatreback/creatreback.component';
import { ReceiveInfoComponent } from './receive/receive-info/receive-info.component';
import { OrdertoworkComponent } from './order/ordertowork/ordertowork.component';
import { WorkorderStockComponent } from './workorder/workorder-stock/workorder-stock.component';
import { ResourceWorkorderComponent } from './workorder/resource-workorder/resource-workorder.component';
import { ResourceProcessComponent } from './workorder/resource-process/resource-process.component';
import { InventorySearchComponent } from './inventory/inventory-search/inventory-search.component';
import { WorkorderCloseComponent } from './workorder/workorder-close/workorder-close.component';
import { UserQrcodeComponent } from './setting/user-qrcode/user-qrcode.component';
import { UserPasswordComponent } from './setting/user-password/user-password.component';
import { OrderOverviewComponent } from './order/order-overview/order-overview.component';
import { SaleOverviewComponent } from './sale/sale-overview/sale-overview.component';
import { SaleReturnComponent } from './report/sale-return/sale-return.component';
import { BiilpurchaseReturnComponent } from './report/biilpurchase-return/biilpurchase-return.component';
import { WorkorderCreatStockComponent } from './workorder/workorder-creat-stock/workorder-creat-stock.component';
import { QualityRecordComponent } from './report/quality-record/quality-record.component';
import { DealPriceComponent } from './report/deal-price/deal-price.component';
import { DealLogComponent } from './report/deal-log/deal-log.component';
import { DealSupplierComponent } from './report/deal-supplier/deal-supplier.component';
import { DealSupplierLogComponent } from './report/deal-supplier-log/deal-supplier-log.component';
import { PurchaseTotalComponent } from './report/purchase-total/purchase-total.component';
import { InventoryShortComponent } from './report/inventory-short/inventory-short.component';
import { WorkorderEstimateComponent } from './workorder/workorder-estimate/workorder-estimate.component';
import { WorkorderEstimateSettingComponent } from './workorder/workorder-estimate-setting/workorder-estimate-setting.component';
import { MachineorderComponent } from './machinemanagemate/machineorder/machineorder.component';
import { MachinedetailComponent } from './machinemanagemate/machinedetail/machinedetail.component';
import { CreateBomComponent } from './billofmaterial/create-bom/create-bom.component';
import { HeaderfontComponent } from './layout/headerfont/headerfont.component';
import { MachineorderReportComponent } from './machinemanagemate/machineorder-report/machineorder-report.component';
import { ChatHubComponent } from './chatHub/chat-hub.component';
import { ToolmanagementComponent } from './setting/toolmanagement/toolmanagement.component';
import { MachineorderBoardComponent } from './machinemanagemate/machineorder-board/machineorder-board.component';
import { MaintenanceComponent } from './maintenance/maintenance.component';
import { MaintenanceDetailComponent } from './maintenance/maintenance-detail/maintenance-detail.component';
import { CreatemaintenanceComponent } from './maintenance/createmaintenance/createmaintenance.component';
import { StaffmanagementComponent } from './staffmanagement/staffmanagement.component';
import { ToolsetComponent } from './process/toolset/toolset.component';
import { WorktimeSummaryComponent } from './workscheduler/worktime-summary/worktime-summary.component';
import { WorktimeListComponent } from './workscheduler/worktime-list/worktime-list.component';
import { MachineLogsComponent } from './machine-logs/machine-logs-list/machine-logs.component';
import { MachineLogsDetailsComponent } from './machine-logs/machine-logs-details/machine-logs-details.component';
import { OrderSaleunfinishedComponent } from './order/order-saleunfinished/order-saleunfinished.component';
import { SurfaceTreatmentComponent } from './surface-treatment/surface-treatment.component';
import { SurfaceDetailComponent } from './surface-treatment/surface-detail/surface-detail.component';
import { CreateSurfaceComponent } from './surface-treatment/create-surface/create-surface.component';
import { SufacetoworkComponent } from './surface-treatment/sufacetowork/sufacetowork.component';
import { MachineInformationComponent } from './machinemanagemate/machine-information/machine-information.component';
import { CreatmachineComponent } from './machinemanagemate/creatmachine/creatmachine.component';
import { MachineProcessTimeComponent } from './machine-process-time/machine-process-time.component';
import { WorkorderV2ListComponent } from './workorder/workorder-v2-list/workorder-v2-list.component';
import { ProcessBatchCloseComponent } from './process/process-batch-close/process-batch-close.component';



export function tokenGetter() {
    return localStorage.getItem('token');
}
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
        DxScrollViewModule,
        DxNumberBoxModule,
        DxCheckBoxModule,
        BreadcrumbModule,
        DxLoadPanelModule,
        DxSchedulerModule,
        DxTabPanelModule,
        DxTabsModule,
        DxLookupModule,
        SweetAlert2Module.forRoot(),
        JwtModule.forRoot({
            config: {
                tokenGetter
            }
        }),
        NgZorroAntdModule,
        DxGalleryModule,
        NzPaginationModule
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
        OrderdetailListComponent,
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
        ProductBasicListComponent,
        MaterialBasicListComponent,
        CreatproductBasicComponent,
        CreatmaterialBasicComponent,
        ReceiveListComponent,
        ReceiveDetailListComponent,
        BillPurchaseReturnComponent,
        InventoryListComponent,
        LoginComponent,
        WiproductListComponent,
        WiproductBasicListComponent,
        CreatwiproductComponent,
        CreatwiproductBasicComponent,
        ProcessListComponent,
        CreatprocessComponent,
        UserRolesComponent,
        AdjustListComponent,
        ProcessControlComponent,
        MbillofmateriallistComponent,
        BomverlistComponent,
        CreatprocessControlComponent,
        WorkorderListComponent,
        WorkorderReportComponent,
        AdjustLogComponent,
        AdjustdetailListComponent,
        WorkorderLogComponent,
        SupplierMaterialComponent,
        QrcodeComponent,
        MbillofmaterialModelComponent,
        WorkorderQaComponent,
        WorkorderReportLogComponent,
        ProcessControlViewComponent,
        EditworkorderComponent,
        WorksChedulerComponent,
        ResourceAllocationComponent,
        InventoryLogComponent,
        CreateSaleComponent,
        BillPurchaseSupplierComponent,
        CreatreceiveComponent,
        RebackListComponent,
        CreatrebackComponent,
        ReceiveInfoComponent,
        OrdertoworkComponent,
        WorkorderStockComponent,
        ResourceWorkorderComponent,
        ResourceProcessComponent,
        InventorySearchComponent,
        WorkorderCloseComponent,
        UserQrcodeComponent,
        UserPasswordComponent,
        OrderOverviewComponent,
        SaleOverviewComponent,
        SaleReturnComponent,
        BiilpurchaseReturnComponent,
        WorkorderCreatStockComponent,
        QualityRecordComponent,
        DealPriceComponent,
        DealLogComponent,
        DealSupplierComponent,
        DealSupplierLogComponent,
        PurchaseTotalComponent,
        InventoryShortComponent,
        WorkorderEstimateComponent,
        WorkorderEstimateSettingComponent,
        MachineorderComponent,
        MachinedetailComponent,
        HeaderfontComponent,
        CreateBomComponent,
        MachineorderReportComponent,
        ChatHubComponent,
        ToolmanagementComponent,
        MachineorderBoardComponent,
        MaintenanceComponent,
        MaintenanceDetailComponent,
        CreatemaintenanceComponent,
        StaffmanagementComponent,
        ToolsetComponent,
        WorktimeSummaryComponent,
        WorktimeListComponent,
        MachineLogsComponent,
        MachineLogsDetailsComponent,
        OrderSaleunfinishedComponent,
        SurfaceTreatmentComponent,
        SurfaceDetailComponent,
        CreateSurfaceComponent,
        SufacetoworkComponent,
        MachineInformationComponent,
        CreatmachineComponent,
        MachineProcessTimeComponent,
        WorkorderV2ListComponent,
        ProcessBatchCloseComponent,
    ],
    providers: [
        { provide: LocationStrategy, useClass: PathLocationStrategy },
        { provide: NZ_I18N, useValue: zh_TW }
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
