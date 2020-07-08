import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatprocessComponent } from './creatprocess.component';

describe('CreatprocessComponent', () => {
  let component: CreatprocessComponent;
  let fixture: ComponentFixture<CreatprocessComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreatprocessComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreatprocessComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
