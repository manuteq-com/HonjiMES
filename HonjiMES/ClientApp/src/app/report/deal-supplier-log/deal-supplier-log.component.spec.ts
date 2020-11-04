import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DealSupplierLogComponent } from './deal-supplier-log.component';

describe('DealSupplierLogComponent', () => {
  let component: DealSupplierLogComponent;
  let fixture: ComponentFixture<DealSupplierLogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DealSupplierLogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DealSupplierLogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
