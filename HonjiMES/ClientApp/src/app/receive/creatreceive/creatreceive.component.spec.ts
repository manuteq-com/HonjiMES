import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatreceiveComponent } from './creatreceive.component';

describe('CreatreceiveComponent', () => {
  let component: CreatreceiveComponent;
  let fixture: ComponentFixture<CreatreceiveComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreatreceiveComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreatreceiveComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
