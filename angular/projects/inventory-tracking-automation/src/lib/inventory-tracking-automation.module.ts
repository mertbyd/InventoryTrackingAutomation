import { NgModule, NgModuleFactory, ModuleWithProviders } from '@angular/core';
import { CoreModule, LazyModuleFactory } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { InventoryTrackingAutomationComponent } from './components/inventory-tracking-automation.component';
import { InventoryTrackingAutomationRoutingModule } from './inventory-tracking-automation-routing.module';

@NgModule({
  declarations: [InventoryTrackingAutomationComponent],
  imports: [CoreModule, ThemeSharedModule, InventoryTrackingAutomationRoutingModule],
  exports: [InventoryTrackingAutomationComponent],
})
export class InventoryTrackingAutomationModule {
  static forChild(): ModuleWithProviders<InventoryTrackingAutomationModule> {
    return {
      ngModule: InventoryTrackingAutomationModule,
      providers: [],
    };
  }

  static forLazy(): NgModuleFactory<InventoryTrackingAutomationModule> {
    return new LazyModuleFactory(InventoryTrackingAutomationModule.forChild());
  }
}
