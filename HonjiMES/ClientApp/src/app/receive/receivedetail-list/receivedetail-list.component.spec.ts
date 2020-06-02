import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { ReceiveDetailListComponent } from './receivedetail-list.component';
describe('ReceiveListComponent', () => {
    let component: ReceiveDetailListComponent;
    let fixture: ComponentFixture<ReceiveDetailListComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ReceiveDetailListComponent]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(ReceiveDetailListComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
