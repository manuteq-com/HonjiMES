import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkorderQaComponent } from './workorder-qa.component';

describe('WorkorderQaComponent', () => {
  let component: WorkorderQaComponent;
  let fixture: ComponentFixture<WorkorderQaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkorderQaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkorderQaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
