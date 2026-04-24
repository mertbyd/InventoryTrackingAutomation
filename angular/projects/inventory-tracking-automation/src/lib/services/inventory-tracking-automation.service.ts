import { Injectable } from '@angular/core';
import { RestService } from '@abp/ng.core';

@Injectable({
  providedIn: 'root',
})
export class InventoryTrackingAutomationService {
  apiName = 'InventoryTrackingAutomation';

  constructor(private restService: RestService) {}

  sample() {
    return this.restService.request<void, any>(
      { method: 'GET', url: '/api/InventoryTrackingAutomation/sample' },
      { apiName: this.apiName }
    );
  }
}
