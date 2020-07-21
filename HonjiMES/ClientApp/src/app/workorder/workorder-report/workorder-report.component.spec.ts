import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkorderReportComponent } from './workorder-report.component';

describe('WorkorderReportComponent', () => {
  let component: WorkorderReportComponent;
  let fixture: ComponentFixture<WorkorderReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkorderReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkorderReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
