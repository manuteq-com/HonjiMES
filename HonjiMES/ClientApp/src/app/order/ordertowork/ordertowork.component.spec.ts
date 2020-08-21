import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrdertoworkComponent } from './ordertowork.component';

describe('OrdertoworkComponent', () => {
  let component: OrdertoworkComponent;
  let fixture: ComponentFixture<OrdertoworkComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrdertoworkComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrdertoworkComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
