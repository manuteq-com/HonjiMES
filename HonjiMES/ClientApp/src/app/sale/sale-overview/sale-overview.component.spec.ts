import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOverviewComponent } from './sale-overview.component';

describe('SaleOverviewComponent', () => {
  let component: SaleOverviewComponent;
  let fixture: ComponentFixture<SaleOverviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOverviewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
