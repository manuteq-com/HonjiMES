import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DealLogComponent } from './deal-log.component';

describe('DealLogComponent', () => {
  let component: DealLogComponent;
  let fixture: ComponentFixture<DealLogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DealLogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DealLogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
