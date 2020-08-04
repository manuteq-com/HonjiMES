import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkorderLogComponent } from './workorder-log.component';

describe('WorkorderLogComponent', () => {
  let component: WorkorderLogComponent;
  let fixture: ComponentFixture<WorkorderLogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkorderLogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkorderLogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
