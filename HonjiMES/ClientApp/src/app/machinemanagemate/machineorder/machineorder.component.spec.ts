import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MachineorderComponent } from './machineorder.component';

describe('MachineorderComponent', () => {
  let component: MachineorderComponent;
  let fixture: ComponentFixture<MachineorderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MachineorderComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MachineorderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
