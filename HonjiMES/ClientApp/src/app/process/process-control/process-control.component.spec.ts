import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProcessControlComponent } from './process-control.component';

describe('ProcessControlComponent', () => {
  let component: ProcessControlComponent;
  let fixture: ComponentFixture<ProcessControlComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProcessControlComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProcessControlComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
