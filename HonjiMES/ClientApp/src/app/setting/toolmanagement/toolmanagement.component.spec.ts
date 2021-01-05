import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ToolmanagementComponent } from './toolmanagement.component';

describe('ToolmanagementComponent', () => {
  let component: ToolmanagementComponent;
  let fixture: ComponentFixture<ToolmanagementComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ToolmanagementComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ToolmanagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
