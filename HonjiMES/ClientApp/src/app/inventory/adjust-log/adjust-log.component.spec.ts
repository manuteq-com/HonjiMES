import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AdjustLogComponent } from './adjust-log.component';

describe('AdjustLogComponent', () => {
  let component: AdjustLogComponent;
  let fixture: ComponentFixture<AdjustLogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AdjustLogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdjustLogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
