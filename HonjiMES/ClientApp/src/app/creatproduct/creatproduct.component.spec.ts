import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatproductComponent } from './creatproduct.component';

describe('CreatproductComponent', () => {
  let component: CreatproductComponent;
  let fixture: ComponentFixture<CreatproductComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreatproductComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreatproductComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
