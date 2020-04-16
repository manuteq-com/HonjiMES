import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrdertosaleComponent } from './ordertosale.component';

describe('OrdertosaleComponent', () => {
  let component: OrdertosaleComponent;
  let fixture: ComponentFixture<OrdertosaleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrdertosaleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrdertosaleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
