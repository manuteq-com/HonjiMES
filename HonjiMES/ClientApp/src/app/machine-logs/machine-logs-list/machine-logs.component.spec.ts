import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MachineLogsComponent } from './machine-logs.component';

describe('MachineLogsComponent', () => {
  let component: MachineLogsComponent;
  let fixture: ComponentFixture<MachineLogsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MachineLogsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MachineLogsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
