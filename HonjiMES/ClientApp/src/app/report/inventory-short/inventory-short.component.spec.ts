import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InventoryShortComponent } from './inventory-short.component';

describe('InventoryShortComponent', () => {
  let component: InventoryShortComponent;
  let fixture: ComponentFixture<InventoryShortComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InventoryShortComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InventoryShortComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
