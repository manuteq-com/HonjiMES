import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MachineProcessTimeComponent } from './machine-process-time.component';

describe('MachineProcessTimeComponent', () => {
  let component: MachineProcessTimeComponent;
  let fixture: ComponentFixture<MachineProcessTimeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MachineProcessTimeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MachineProcessTimeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
