import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderdetailListComponent } from './orderdetail-list.component';

describe('OrderdetailListComponent', () => {
  let component: OrderdetailListComponent;
  let fixture: ComponentFixture<OrderdetailListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrderdetailListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrderdetailListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
