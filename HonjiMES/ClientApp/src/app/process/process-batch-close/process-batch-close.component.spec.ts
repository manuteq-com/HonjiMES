import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProcessBatchCloseComponent } from './process-batch-close.component';

describe('ProcessBatchCloseComponent', () => {
  let component: ProcessBatchCloseComponent;
  let fixture: ComponentFixture<ProcessBatchCloseComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProcessBatchCloseComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProcessBatchCloseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
