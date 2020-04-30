import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BillPurchaseDetailComponent } from './bill-purchase-detail.component';

describe('BillPurchaseDetailComponent', () => {
  let component: BillPurchaseDetailComponent;
  let fixture: ComponentFixture<BillPurchaseDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BillPurchaseDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BillPurchaseDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
