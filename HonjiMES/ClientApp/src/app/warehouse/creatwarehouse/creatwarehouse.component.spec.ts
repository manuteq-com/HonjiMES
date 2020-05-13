import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatwarehouseComponent } from './creatwarehouse.component';

describe('CreatwarehouseComponent', () => {
  let component: CreatwarehouseComponent;
  let fixture: ComponentFixture<CreatwarehouseComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreatwarehouseComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreatwarehouseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
