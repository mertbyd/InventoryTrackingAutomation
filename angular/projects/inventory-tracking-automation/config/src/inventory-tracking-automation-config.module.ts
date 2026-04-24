import { ModuleWithProviders, NgModule } from '@angular/core';
import { INVENTORY_TRACKİNG_AUTOMATİON_ROUTE_PROVIDERS } from './providers/route.provider';

@NgModule()
export class InventoryTrackingAutomationConfigModule {
  static forRoot(): ModuleWithProviders<InventoryTrackingAutomationConfigModule> {
    return {
      ngModule: InventoryTrackingAutomationConfigModule,
      providers: [INVENTORY_TRACKİNG_AUTOMATİON_ROUTE_PROVIDERS],
    };
  }
}
