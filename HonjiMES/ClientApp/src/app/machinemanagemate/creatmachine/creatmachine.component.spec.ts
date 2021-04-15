import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatmachineComponent } from './creatmachine.component';

describe('CreatmachineComponent', () => {
  let component: CreatmachineComponent;
  let fixture: ComponentFixture<CreatmachineComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreatmachineComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreatmachineComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
