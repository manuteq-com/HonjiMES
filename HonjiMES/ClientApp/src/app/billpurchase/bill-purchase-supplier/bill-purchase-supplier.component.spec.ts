import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BillPurchaseSupplierComponent } from './bill-purchase-supplier.component';

describe('BillPurchaseSupplierComponent', () => {
  let component: BillPurchaseSupplierComponent;
  let fixture: ComponentFixture<BillPurchaseSupplierComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BillPurchaseSupplierComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BillPurchaseSupplierComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
