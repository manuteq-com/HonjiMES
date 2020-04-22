import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BillPurchaseComponent } from './bill-purchase.component';

describe('BillPurchaseComponent', () => {
  let component: BillPurchaseComponent;
  let fixture: ComponentFixture<BillPurchaseComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BillPurchaseComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BillPurchaseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
