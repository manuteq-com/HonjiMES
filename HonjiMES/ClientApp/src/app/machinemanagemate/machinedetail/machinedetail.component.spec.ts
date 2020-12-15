import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MachinedetailComponent } from './machinedetail.component';

describe('MachinedetailComponent', () => {
  let component: MachinedetailComponent;
  let fixture: ComponentFixture<MachinedetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MachinedetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MachinedetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
