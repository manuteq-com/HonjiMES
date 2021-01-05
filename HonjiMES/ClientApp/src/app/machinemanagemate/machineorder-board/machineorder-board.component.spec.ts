import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MachineorderBoardComponent } from './machineorder-board.component';

describe('MachineorderBoardComponent', () => {
  let component: MachineorderBoardComponent;
  let fixture: ComponentFixture<MachineorderBoardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MachineorderBoardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MachineorderBoardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
