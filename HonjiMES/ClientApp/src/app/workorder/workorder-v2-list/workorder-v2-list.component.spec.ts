import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkorderV2ListComponent } from './workorder-v2-list.component';

describe('WorkorderV2ListComponent', () => {
  let component: WorkorderV2ListComponent;
  let fixture: ComponentFixture<WorkorderV2ListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkorderV2ListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkorderV2ListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
