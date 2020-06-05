import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReceiveListComponent } from './receive-list.component';

describe('ReceiveListComponent', () => {
  let component: ReceiveListComponent;
  let fixture: ComponentFixture<ReceiveListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReceiveListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReceiveListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
