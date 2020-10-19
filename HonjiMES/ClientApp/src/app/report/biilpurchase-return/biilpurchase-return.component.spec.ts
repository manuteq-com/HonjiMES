import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BiilpurchaseReturnComponent } from './biilpurchase-return.component';

describe('BiilpurchaseReturnComponent', () => {
  let component: BiilpurchaseReturnComponent;
  let fixture: ComponentFixture<BiilpurchaseReturnComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BiilpurchaseReturnComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BiilpurchaseReturnComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
