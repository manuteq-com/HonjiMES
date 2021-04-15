import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MachineInformationComponent } from './machine-information.component';

describe('MachineInformationComponent', () => {
  let component: MachineInformationComponent;
  let fixture: ComponentFixture<MachineInformationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MachineInformationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MachineInformationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
