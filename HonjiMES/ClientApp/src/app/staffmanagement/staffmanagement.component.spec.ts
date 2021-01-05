import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffmanagementComponent } from './staffmanagement.component';

describe('StaffmanagementComponent', () => {
  let component: StaffmanagementComponent;
  let fixture: ComponentFixture<StaffmanagementComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StaffmanagementComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StaffmanagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
