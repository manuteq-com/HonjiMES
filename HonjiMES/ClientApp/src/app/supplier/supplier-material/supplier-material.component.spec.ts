import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SupplierMaterialComponent } from './supplier-material.component';

describe('SupplierMaterialComponent', () => {
  let component: SupplierMaterialComponent;
  let fixture: ComponentFixture<SupplierMaterialComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SupplierMaterialComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SupplierMaterialComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
