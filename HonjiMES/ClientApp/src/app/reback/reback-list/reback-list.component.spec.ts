import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RebackListComponent } from './reback-list.component';

describe('RebackListComponent', () => {
  let component: RebackListComponent;
  let fixture: ComponentFixture<RebackListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RebackListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RebackListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
