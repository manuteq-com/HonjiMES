import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserQrcodeComponent } from './user-qrcode.component';

describe('UserQrcodeComponent', () => {
  let component: UserQrcodeComponent;
  let fixture: ComponentFixture<UserQrcodeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserQrcodeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserQrcodeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
