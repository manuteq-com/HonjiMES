import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReceivedetailListComponent } from './receivedetail-list.component';

describe('ReceivedetailListComponent', () => {
  let component: ReceivedetailListComponent;
  let fixture: ComponentFixture<ReceivedetailListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReceivedetailListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReceivedetailListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
