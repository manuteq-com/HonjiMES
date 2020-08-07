import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MbillofmaterialModelComponent } from './mbillofmaterial-model.component';

describe('MbillofmaterialModelComponent', () => {
  let component: MbillofmaterialModelComponent;
  let fixture: ComponentFixture<MbillofmaterialModelComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MbillofmaterialModelComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MbillofmaterialModelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
