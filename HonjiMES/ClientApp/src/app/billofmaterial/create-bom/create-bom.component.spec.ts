import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateBomComponent } from './create-bom.component';

describe('CreateBomComponent', () => {
  let component: CreateBomComponent;
  let fixture: ComponentFixture<CreateBomComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateBomComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateBomComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
