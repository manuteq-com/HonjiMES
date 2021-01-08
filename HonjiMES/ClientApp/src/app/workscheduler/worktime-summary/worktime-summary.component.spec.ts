import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorktimeSummaryComponent } from './worktime-summary.component';

describe('WorktimeSummaryComponent', () => {
  let component: WorktimeSummaryComponent;
  let fixture: ComponentFixture<WorktimeSummaryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorktimeSummaryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorktimeSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
