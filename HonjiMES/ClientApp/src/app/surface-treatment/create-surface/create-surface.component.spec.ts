import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateSurfaceComponent } from './create-surface.component';

describe('CreateSurfaceComponent', () => {
  let component: CreateSurfaceComponent;
  let fixture: ComponentFixture<CreateSurfaceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateSurfaceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateSurfaceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
