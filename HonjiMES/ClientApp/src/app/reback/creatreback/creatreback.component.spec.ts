import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatrebackComponent } from './creatreback.component';

describe('CreatrebackComponent', () => {
  let component: CreatrebackComponent;
  let fixture: ComponentFixture<CreatrebackComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreatrebackComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreatrebackComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
