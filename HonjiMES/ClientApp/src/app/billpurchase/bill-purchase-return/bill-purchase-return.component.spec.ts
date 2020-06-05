import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BillPurchaseReturnComponent } from './bill-purchase-return.component';

describe('BillPurchaseReturnComponent', () => {
  let component: BillPurchaseReturnComponent;
  let fixture: ComponentFixture<BillPurchaseReturnComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BillPurchaseReturnComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BillPurchaseReturnComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
