import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkorderStockComponent } from './workorder-stock.component';

describe('WorkorderStockComponent', () => {
  let component: WorkorderStockComponent;
  let fixture: ComponentFixture<WorkorderStockComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkorderStockComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkorderStockComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
