import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BomverlistComponent } from './bomverlist.component';

describe('BomverlistComponent', () => {
  let component: BomverlistComponent;
  let fixture: ComponentFixture<BomverlistComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BomverlistComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BomverlistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
