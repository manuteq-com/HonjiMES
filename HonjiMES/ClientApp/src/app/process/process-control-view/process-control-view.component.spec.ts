import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProcessControlViewComponent } from './process-control-view.component';

describe('ProcessControlViewComponent', () => {
  let component: ProcessControlViewComponent;
  let fixture: ComponentFixture<ProcessControlViewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProcessControlViewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProcessControlViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
