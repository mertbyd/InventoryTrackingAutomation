import { TestBed } from '@angular/core/testing';
import { InventoryTrackingAutomationService } from './services/inventory-tracking-automation.service';
import { RestService } from '@abp/ng.core';

describe('InventoryTrackingAutomationService', () => {
  let service: InventoryTrackingAutomationService;
  const mockRestService = jasmine.createSpyObj('RestService', ['request']);
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        {
          provide: RestService,
          useValue: mockRestService,
        },
      ],
    });
    service = TestBed.inject(InventoryTrackingAutomationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
