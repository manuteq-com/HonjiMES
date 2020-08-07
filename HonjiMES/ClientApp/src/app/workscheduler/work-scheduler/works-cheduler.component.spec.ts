import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorksChedulerComponent } from './works-cheduler.component';

describe('WorksChedulerComponent', () => {
  let component: WorksChedulerComponent;
  let fixture: ComponentFixture<WorksChedulerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorksChedulerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorksChedulerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
