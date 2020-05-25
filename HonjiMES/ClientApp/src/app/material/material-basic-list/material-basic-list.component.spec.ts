import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MaterialBasicListComponent } from './material-basic-list.component';

describe('MaterialBasicListComponent', () => {
  let component: MaterialBasicListComponent;
  let fixture: ComponentFixture<MaterialBasicListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MaterialBasicListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MaterialBasicListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
