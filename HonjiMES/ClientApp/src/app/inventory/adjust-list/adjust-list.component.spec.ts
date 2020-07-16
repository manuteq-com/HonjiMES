import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AdjustListComponent } from './adjust-list.component';

describe('AdjustListComponent', () => {
  let component: AdjustListComponent;
  let fixture: ComponentFixture<AdjustListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AdjustListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdjustListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
