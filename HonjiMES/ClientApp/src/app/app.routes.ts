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
import { AdjustListComponent } from './inventory/adjust-list/adjust-list.component';
import { AdjustLogComponent } from './inventory/adjust-log/adjust-log.component';
import { WarehouseListComponent } from './warehouse/warehouse-list/warehouse-list.component';
import { UserListComponent } from './setting/user-list/user-list.component';
import { BillofmateriallistComponent } from './billofmaterial/billofmateriallist/billofmateriallist.component';
import { MbillofmateriallistComponent } from './mbillofmaterial/mbillofmateriallist/mbillofmateriallist.component';
import { ProductBasicListComponent } from './product/product-basic-list/product-basic-list.component';
import { WiproductBasicListComponent } from './wiproduct/wiproduct-basic-list/wiproduct-basic-list.component';
import { MaterialBasicListComponent } from './material/material-basic-list/material-basic-list.component';
import { ReceiveListComponent } from './receive/receive-list/receive-list.component';
import { ProcessListComponent } from './process/process-list/process-list.component';
import { AuthGuard } from './helpers/auth.guard';
import { LoginComponent } from './login/login/login.component';
import { ProcessControlComponent } from './process/process-control/process-control.component';
import { WorkorderListComponent } from './workorder/workorder-list/workorder-list.component';
import { WorkorderLogComponent } from './workorder/workorder-log/workorder-log.component';
import { WorkorderQaComponent } from './workorder/workorder-qa/workorder-qa.component';
import { WorkorderStockComponent } from './workorder/workorder-stock/workorder-stock.component';
import { WorksChedulerComponent } from './workscheduler/work-scheduler/work-scheduler.component';
import { ResourceAllocationComponent } from './workorder/resource-allocation/resource-allocation.component';
import { InventoryLogComponent } from './inventory/inventory-log/inventory-log.component';
import { RebackListComponent } from './reback/reback-list/reback-list.component';
import { UserPasswordComponent } from './setting/user-password/user-password.component';
import { SaleReturnComponent } from './report/sale-return/sale-return.component';
import { BiilpurchaseReturnComponent } from './report/biilpurchase-return/biilpurchase-return.component';
import { QualityRecordComponent } from './report/quality-record/quality-record.component';
import { DealPriceComponent } from './report/deal-price/deal-price.component';
import { DealSupplierComponent } from './report/deal-supplier/deal-supplier.component';
import { PurchaseTotalComponent } from './report/purchase-total/purchase-total.component';
import { WorkorderEstimateComponent } from './workorder/workorder-estimate/workorder-estimate.component';
import { MachineorderComponent } from './machinemanagemate/machineorder/machineorder.component';
import { CreateBomComponent } from './billofmaterial/create-bom/create-bom.component';
import { ChatHubComponent } from './chatHub/chat-hub.component';
import { MachineorderBoardComponent } from './machinemanagemate/machineorder-board/machineorder-board.component';
import { ToolmanagementComponent } from './setting/toolmanagement/toolmanagement.component';
import { MaintenanceComponent } from './maintenance/maintenance.component';
import { StaffmanagementComponent } from './staffmanagement/staffmanagement.component';
import { WorktimeSummaryComponent } from './workscheduler/worktime-summary/worktime-summary.component';
import { MachineLogsComponent } from './machine-logs/machine-logs-list/machine-logs.component';
import { SurfaceTreatmentComponent } from './surface-treatment/surface-treatment.component';
import { MachineInformationComponent } from './machinemanagemate/machine-information/machine-information.component';
import { MachineProcessTimeComponent } from './machine-process-time/machine-process-time.component';
import { ReceiveInfoComponent } from './receive/receive-info/receive-info.component';

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
    { path: 'adjustlist', component: AdjustListComponent, canActivate: [AuthGuard] },
    { path: 'adjustlog', component: AdjustLogComponent, canActivate: [AuthGuard] },
    { path: 'inventorylog', component: InventoryLogComponent, canActivate: [AuthGuard] },
    { path: 'warehouselist', component: WarehouseListComponent, canActivate: [AuthGuard] },
    { path: 'userlist', component: UserListComponent, canActivate: [AuthGuard] },
    { path: 'billofmateriallist', component: BillofmateriallistComponent, canActivate: [AuthGuard] },
    { path: 'mbillofmateriallist', component: MbillofmateriallistComponent, canActivate: [AuthGuard] },
    { path: 'wiproductbasiclist', component: WiproductBasicListComponent, canActivate: [AuthGuard] },
    { path: 'processlist', component: ProcessListComponent, canActivate: [AuthGuard] },
    { path: 'receiveList', component: ReceiveListComponent, canActivate: [AuthGuard] },
    { path: 'processcontrol', component: ProcessControlComponent, canActivate: [AuthGuard] },
    { path: 'workorderlist', component: WorkorderListComponent, canActivate: [AuthGuard] },
    { path: 'workorderlog', component: WorkorderLogComponent, canActivate: [AuthGuard] },
    { path: 'workorderqa', component: WorkorderQaComponent, canActivate: [AuthGuard] },
    { path: 'workorderstock', component: WorkorderStockComponent, canActivate: [AuthGuard] },
    { path: 'resourceallocation', component: ResourceAllocationComponent, canActivate: [AuthGuard] },
    { path: 'workscheduler', component: WorksChedulerComponent, canActivate: [AuthGuard] },
    { path: 'rebackList', component: RebackListComponent, canActivate: [AuthGuard] },
    { path: 'userpassword', component: UserPasswordComponent, canActivate: [AuthGuard] },
    { path: 'salereturn', component: SaleReturnComponent, canActivate: [AuthGuard] },
    { path: 'billpurchasereturn', component: BiilpurchaseReturnComponent, canActivate: [AuthGuard] },
    { path: 'qualityrecord', component: QualityRecordComponent, canActivate: [AuthGuard] },
    { path: 'dealprice', component: DealPriceComponent, canActivate: [AuthGuard] },
    { path: 'dealsupplier', component: DealSupplierComponent, canActivate: [AuthGuard] },
    { path: 'purchasetotal', component: PurchaseTotalComponent, canActivate: [AuthGuard] },
    { path: 'workestimate', component: WorkorderEstimateComponent, canActivate: [AuthGuard] },
    { path: 'machineorder', component: MachineorderComponent },
    { path: 'createbom', component: CreateBomComponent },
    { path: 'chatHub', component: ChatHubComponent },
    { path: 'board', component: MachineorderBoardComponent },
    { path: 'toolmanagement', component: ToolmanagementComponent },
    { path: 'maintenance', component: MaintenanceComponent },
    { path: 'staffmanagement', component: StaffmanagementComponent },
    { path: 'worktimesummary', component: WorktimeSummaryComponent },
    { path: 'machinelogs', component: MachineLogsComponent },
    { path: 'surfacetreat', component: SurfaceTreatmentComponent },
    { path: 'machineinformation', component: MachineInformationComponent },
    { path: 'machineprocesstime', component: MachineProcessTimeComponent },
    { path: 'receiveInfo', component: ReceiveInfoComponent},
];

export const AppRoutes: ModuleWithProviders = RouterModule.forRoot(routes, { scrollPositionRestoration: 'enabled' });
