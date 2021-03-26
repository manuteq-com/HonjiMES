import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SurfaceDetailComponent } from './surface-detail.component';

describe('SurfaceDetailComponent', () => {
  let component: SurfaceDetailComponent;
  let fixture: ComponentFixture<SurfaceDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SurfaceDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SurfaceDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
