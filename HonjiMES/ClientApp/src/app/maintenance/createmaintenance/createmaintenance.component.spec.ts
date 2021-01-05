import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatemaintenanceComponent } from './createmaintenance.component';

describe('CreatemaintenanceComponent', () => {
  let component: CreatemaintenanceComponent;
  let fixture: ComponentFixture<CreatemaintenanceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreatemaintenanceComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreatemaintenanceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
