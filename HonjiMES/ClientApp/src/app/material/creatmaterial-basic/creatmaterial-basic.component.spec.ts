import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatmaterialBasicComponent } from './creatmaterial-basic.component';

describe('CreatmaterialBasicComponent', () => {
  let component: CreatmaterialBasicComponent;
  let fixture: ComponentFixture<CreatmaterialBasicComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreatmaterialBasicComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreatmaterialBasicComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
