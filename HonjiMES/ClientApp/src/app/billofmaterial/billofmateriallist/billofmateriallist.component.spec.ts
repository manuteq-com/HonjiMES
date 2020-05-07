import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BillofmateriallistComponent } from './billofmateriallist.component';

describe('BillofmateriallistComponent', () => {
  let component: BillofmateriallistComponent;
  let fixture: ComponentFixture<BillofmateriallistComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BillofmateriallistComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BillofmateriallistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
