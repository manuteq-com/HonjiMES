import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MachineLogsDetailsComponent } from './machine-logs-details.component';

describe('MachineLogsDetailsComponent', () => {
  let component: MachineLogsDetailsComponent;
  let fixture: ComponentFixture<MachineLogsDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MachineLogsDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MachineLogsDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
