import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AdjustdetailListComponent } from './adjustdetail-list.component';

describe('AdjustdetailListComponent', () => {
  let component: AdjustdetailListComponent;
  let fixture: ComponentFixture<AdjustdetailListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AdjustdetailListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdjustdetailListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
