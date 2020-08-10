import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditworkorderComponent } from './editworkorder.component';

describe('EditworkorderComponent', () => {
  let component: EditworkorderComponent;
  let fixture: ComponentFixture<EditworkorderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditworkorderComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditworkorderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
