import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SufacetoworkComponent } from './sufacetowork.component';

describe('SufacetoworkComponent', () => {
  let component: SufacetoworkComponent;
  let fixture: ComponentFixture<SufacetoworkComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SufacetoworkComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SufacetoworkComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
