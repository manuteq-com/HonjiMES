import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DealPriceComponent } from './deal-price.component';

describe('DealPriceComponent', () => {
  let component: DealPriceComponent;
  let fixture: ComponentFixture<DealPriceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DealPriceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DealPriceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
