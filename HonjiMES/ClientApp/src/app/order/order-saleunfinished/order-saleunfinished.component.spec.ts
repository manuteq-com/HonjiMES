import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderSaleunfinishedComponent } from './order-saleunfinished.component';

describe('OrderSaleunfinishedComponent', () => {
  let component: OrderSaleunfinishedComponent;
  let fixture: ComponentFixture<OrderSaleunfinishedComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrderSaleunfinishedComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrderSaleunfinishedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
