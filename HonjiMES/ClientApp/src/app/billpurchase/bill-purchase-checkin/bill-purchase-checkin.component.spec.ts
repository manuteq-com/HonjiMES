import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BillPurchaseCheckinComponent } from './bill-purchase-checkin.component';

describe('BillPurchaseCheckinComponent', () => {
  let component: BillPurchaseCheckinComponent;
  let fixture: ComponentFixture<BillPurchaseCheckinComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BillPurchaseCheckinComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BillPurchaseCheckinComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
