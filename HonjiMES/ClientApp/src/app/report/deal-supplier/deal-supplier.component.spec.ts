import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DealSupplierComponent } from './deal-supplier.component';

describe('DealSupplierComponent', () => {
  let component: DealSupplierComponent;
  let fixture: ComponentFixture<DealSupplierComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DealSupplierComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DealSupplierComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
