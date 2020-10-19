import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkorderCreatStockComponent } from './workorder-creat-stock.component';

describe('WorkorderCreatStockComponent', () => {
  let component: WorkorderCreatStockComponent;
  let fixture: ComponentFixture<WorkorderCreatStockComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkorderCreatStockComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkorderCreatStockComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
