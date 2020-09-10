import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResourceWorkorderComponent } from './resource-workorder.component';

describe('ResourceWorkorderComponent', () => {
  let component: ResourceWorkorderComponent;
  let fixture: ComponentFixture<ResourceWorkorderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResourceWorkorderComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResourceWorkorderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
