import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatBillPurchaseComponent } from './creat-bill-purchase.component';

describe('CreatBillPurchaseComponent', () => {
  let component: CreatBillPurchaseComponent;
  let fixture: ComponentFixture<CreatBillPurchaseComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreatBillPurchaseComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreatBillPurchaseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
