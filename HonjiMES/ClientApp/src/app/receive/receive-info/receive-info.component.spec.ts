import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReceiveInfoComponent } from './receive-info.component';

describe('ReceiveInfoComponent', () => {
  let component: ReceiveInfoComponent;
  let fixture: ComponentFixture<ReceiveInfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReceiveInfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReceiveInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
