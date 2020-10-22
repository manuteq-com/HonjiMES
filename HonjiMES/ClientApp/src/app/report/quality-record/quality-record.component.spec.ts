import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { QualityRecordComponent } from './quality-record.component';

describe('QualityRecordComponent', () => {
  let component: QualityRecordComponent;
  let fixture: ComponentFixture<QualityRecordComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ QualityRecordComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(QualityRecordComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
