import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReOrderSaleComponent } from './re-order-sale.component';

describe('ReOrderSaleComponent', () => {
  let component: ReOrderSaleComponent;
  let fixture: ComponentFixture<ReOrderSaleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReOrderSaleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReOrderSaleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
