import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InventoryChangeComponent } from './inventory-change.component';

describe('InventoryChangeComponent', () => {
  let component: InventoryChangeComponent;
  let fixture: ComponentFixture<InventoryChangeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InventoryChangeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InventoryChangeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
