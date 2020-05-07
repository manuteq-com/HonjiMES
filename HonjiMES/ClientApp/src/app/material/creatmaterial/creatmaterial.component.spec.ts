import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatmaterialComponent } from './creatmaterial.component';

describe('CreatmaterialComponent', () => {
  let component: CreatmaterialComponent;
  let fixture: ComponentFixture<CreatmaterialComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreatmaterialComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreatmaterialComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
