import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkorderCloseComponent } from './workorder-close.component';

describe('WorkorderCloseComponent', () => {
  let component: WorkorderCloseComponent;
  let fixture: ComponentFixture<WorkorderCloseComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkorderCloseComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkorderCloseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
