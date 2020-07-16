import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatprocessControlComponent } from './creatprocess-control.component';

describe('CreatprocessControlComponent', () => {
  let component: CreatprocessControlComponent;
  let fixture: ComponentFixture<CreatprocessControlComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreatprocessControlComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreatprocessControlComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
