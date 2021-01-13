import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorktimeListComponent } from './worktime-list.component';

describe('WorktimeListComponent', () => {
  let component: WorktimeListComponent;
  let fixture: ComponentFixture<WorktimeListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorktimeListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorktimeListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
