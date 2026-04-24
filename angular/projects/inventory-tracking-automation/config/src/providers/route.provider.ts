import { eLayoutType, RoutesService } from '@abp/ng.core';
import { APP_INITIALIZER } from '@angular/core';
import { eInventoryTrackingAutomationRouteNames } from '../enums/route-names';

export const INVENTORY_TRACKİNG_AUTOMATİON_ROUTE_PROVIDERS = [
  {
    provide: APP_INITIALIZER,
    useFactory: configureRoutes,
    deps: [RoutesService],
    multi: true,
  },
];

export function configureRoutes(routesService: RoutesService) {
  return () => {
    routesService.add([
      {
        path: '/inventory-tracking-automation',
        name: eInventoryTrackingAutomationRouteNames.InventoryTrackingAutomation,
        iconClass: 'fas fa-book',
        layout: eLayoutType.application,
        order: 3,
      },
    ]);
  };
}
