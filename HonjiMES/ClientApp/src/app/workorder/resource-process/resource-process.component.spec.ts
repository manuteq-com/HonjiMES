import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResourceProcessComponent } from './resource-process.component';

describe('ResourceProcessComponent', () => {
  let component: ResourceProcessComponent;
  let fixture: ComponentFixture<ResourceProcessComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResourceProcessComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResourceProcessComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
