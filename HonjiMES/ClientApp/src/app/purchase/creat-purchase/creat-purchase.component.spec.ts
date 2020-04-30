import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatPurchaseComponent } from './creat-purchase.component';

describe('CreatPurchaseComponent', () => {
  let component: CreatPurchaseComponent;
  let fixture: ComponentFixture<CreatPurchaseComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreatPurchaseComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreatPurchaseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
