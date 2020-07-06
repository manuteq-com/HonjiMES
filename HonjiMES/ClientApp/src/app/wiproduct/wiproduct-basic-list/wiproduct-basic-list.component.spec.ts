import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WiproductBasicListComponent } from './wiproduct-basic-list.component';

describe('WiproductBasicListComponent', () => {
  let component: WiproductBasicListComponent;
  let fixture: ComponentFixture<WiproductBasicListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WiproductBasicListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WiproductBasicListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
