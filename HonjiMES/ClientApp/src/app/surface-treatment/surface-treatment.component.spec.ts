import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SurfaceTreatmentComponent } from './surface-treatment.component';

describe('SurfaceTreatmentComponent', () => {
  let component: SurfaceTreatmentComponent;
  let fixture: ComponentFixture<SurfaceTreatmentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SurfaceTreatmentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SurfaceTreatmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
