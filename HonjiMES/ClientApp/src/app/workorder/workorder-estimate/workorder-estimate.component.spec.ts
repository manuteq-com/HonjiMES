import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkorderEstimateComponent } from './workorder-estimate.component';

describe('WorkorderEstimateComponent', () => {
  let component: WorkorderEstimateComponent;
  let fixture: ComponentFixture<WorkorderEstimateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkorderEstimateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkorderEstimateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
