import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatcustomerComponent } from './creatcustomer.component';

describe('CreatcustomerComponent', () => {
  let component: CreatcustomerComponent;
  let fixture: ComponentFixture<CreatcustomerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreatcustomerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreatcustomerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
