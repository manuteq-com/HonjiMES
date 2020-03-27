import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OerdrdetailListComponent } from './oerdrdetail-list.component';

describe('OerdrdetailListComponent', () => {
  let component: OerdrdetailListComponent;
  let fixture: ComponentFixture<OerdrdetailListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OerdrdetailListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OerdrdetailListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
