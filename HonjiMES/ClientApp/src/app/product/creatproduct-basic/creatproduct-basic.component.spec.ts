import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatproductBasicComponent } from './creatproduct-basic.component';

describe('CreatproductBasicComponent', () => {
  let component: CreatproductBasicComponent;
  let fixture: ComponentFixture<CreatproductBasicComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreatproductBasicComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreatproductBasicComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
