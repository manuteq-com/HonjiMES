import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WiproductListComponent } from './wiproduct-list.component';

describe('WiproductListComponent', () => {
  let component: WiproductListComponent;
  let fixture: ComponentFixture<WiproductListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WiproductListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WiproductListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
