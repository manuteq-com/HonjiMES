import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatwiproductBasicComponent } from './creatwiproduct-basic.component';

describe('CreatwiproductBasicComponent', () => {
  let component: CreatwiproductBasicComponent;
  let fixture: ComponentFixture<CreatwiproductBasicComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreatwiproductBasicComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreatwiproductBasicComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
