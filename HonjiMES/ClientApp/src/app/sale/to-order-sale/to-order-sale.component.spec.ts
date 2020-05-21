import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ToOrderSaleComponent } from './to-order-sale.component';

describe('ToOrderSaleComponent', () => {
  let component: ToOrderSaleComponent;
  let fixture: ComponentFixture<ToOrderSaleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ToOrderSaleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ToOrderSaleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
