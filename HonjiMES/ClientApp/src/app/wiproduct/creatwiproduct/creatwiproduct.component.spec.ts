import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatwiproductComponent } from './creatwiproduct.component';

describe('CreatwiproductComponent', () => {
  let component: CreatwiproductComponent;
  let fixture: ComponentFixture<CreatwiproductComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreatwiproductComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreatwiproductComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
