import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MachineorderReportComponent } from './machineorder-report.component';

describe('MachineorderReportComponent', () => {
  let component: MachineorderReportComponent;
  let fixture: ComponentFixture<MachineorderReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MachineorderReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MachineorderReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
