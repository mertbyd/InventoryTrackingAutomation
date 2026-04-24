import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { InventoryTrackingAutomationComponent } from './components/inventory-tracking-automation.component';
import { InventoryTrackingAutomationService } from '@inventory-tracking-automation';
import { of } from 'rxjs';

describe('InventoryTrackingAutomationComponent', () => {
  let component: InventoryTrackingAutomationComponent;
  let fixture: ComponentFixture<InventoryTrackingAutomationComponent>;
  const mockInventoryTrackingAutomationService = jasmine.createSpyObj('InventoryTrackingAutomationService', {
    sample: of([]),
  });
  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [InventoryTrackingAutomationComponent],
      providers: [
        {
          provide: InventoryTrackingAutomationService,
          useValue: mockInventoryTrackingAutomationService,
        },
      ],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InventoryTrackingAutomationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
