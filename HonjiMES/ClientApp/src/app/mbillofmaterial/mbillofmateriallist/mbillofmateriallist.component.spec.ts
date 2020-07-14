import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MbillofmateriallistComponent } from './mbillofmateriallist.component';

describe('MbillofmateriallistComponent', () => {
  let component: MbillofmateriallistComponent;
  let fixture: ComponentFixture<MbillofmateriallistComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MbillofmateriallistComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MbillofmateriallistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
