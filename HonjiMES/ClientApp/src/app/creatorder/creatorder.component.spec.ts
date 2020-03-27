import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatorderComponent } from './creatorder.component';

describe('CreatorderComponent', () => {
  let component: CreatorderComponent;
  let fixture: ComponentFixture<CreatorderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreatorderComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreatorderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
