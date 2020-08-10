import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkorderReportLogComponent } from './workorder-report-log.component';

describe('WorkorderReportLogComponent', () => {
  let component: WorkorderReportLogComponent;
  let fixture: ComponentFixture<WorkorderReportLogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkorderReportLogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkorderReportLogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
