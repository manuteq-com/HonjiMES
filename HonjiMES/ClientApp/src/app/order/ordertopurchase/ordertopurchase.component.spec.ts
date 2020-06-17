import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrdertopurchaseComponent } from './ordertopurchase.component';

describe('OrdertopurchaseComponent', () => {
  let component: OrdertopurchaseComponent;
  let fixture: ComponentFixture<OrdertopurchaseComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrdertopurchaseComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrdertopurchaseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
