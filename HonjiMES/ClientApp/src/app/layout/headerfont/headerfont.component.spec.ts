import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HeaderfontComponent } from './headerfont.component';

describe('HeaderfontComponent', () => {
  let component: HeaderfontComponent;
  let fixture: ComponentFixture<HeaderfontComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HeaderfontComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HeaderfontComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
