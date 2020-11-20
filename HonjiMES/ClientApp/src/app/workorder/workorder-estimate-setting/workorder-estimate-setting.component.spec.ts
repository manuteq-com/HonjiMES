import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkorderEstimateSettingComponent } from './workorder-estimate-setting.component';

describe('WorkorderEstimateSettingComponent', () => {
  let component: WorkorderEstimateSettingComponent;
  let fixture: ComponentFixture<WorkorderEstimateSettingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkorderEstimateSettingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkorderEstimateSettingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
