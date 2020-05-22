import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductBasicListComponent } from './product-basic-list.component';

describe('ProductBasicListComponent', () => {
  let component: ProductBasicListComponent;
  let fixture: ComponentFixture<ProductBasicListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductBasicListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductBasicListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
