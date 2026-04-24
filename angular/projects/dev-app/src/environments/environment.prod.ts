import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

export const environment = {
  production: true,
  application: {
    baseUrl: 'http://localhost:4200/',
    name: 'InventoryTrackingAutomation',
    logoUrl: '',
  },
  oAuthConfig: {
    issuer: 'https://localhost:44307/',
    redirectUri: baseUrl,
    clientId: 'InventoryTrackingAutomation_App',
    responseType: 'code',
    scope: 'offline_access InventoryTrackingAutomation',
    requireHttps: true
  },
  apis: {
    default: {
      url: 'https://localhost:44307',
      rootNamespace: 'InventoryTrackingAutomation',
    },
    InventoryTrackingAutomation: {
      url: 'https://localhost:44372',
      rootNamespace: 'InventoryTrackingAutomation',
    },
  },
} as Environment;
