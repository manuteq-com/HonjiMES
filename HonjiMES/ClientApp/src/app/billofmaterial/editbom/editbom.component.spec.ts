import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditbomComponent } from './editbom.component';

describe('EditbomComponent', () => {
  let component: EditbomComponent;
  let fixture: ComponentFixture<EditbomComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditbomComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditbomComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
