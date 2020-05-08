import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatsupplierComponent } from './creatsupplier.component';

describe('CreatsupplierComponent', () => {
  let component: CreatsupplierComponent;
  let fixture: ComponentFixture<CreatsupplierComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreatsupplierComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreatsupplierComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
