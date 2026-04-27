import { Injectable, NgZone, OnDestroy } from '@angular/core';
import { EnvironmentService } from '@abp/ng.core';
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { OAuthService } from 'angular-oauth2-oidc';
import { BehaviorSubject, Subject } from 'rxjs';

export interface InventoryNotificationMessage {
  type: string;
  title: string;
  message: string;
  entityType?: string;
  entityId?: string;
  workflowInstanceId?: string;
  workflowInstanceStepId?: string;
  createdAt?: string;
}

@Injectable({
  providedIn: 'root',
})
export class InventoryNotificationSignalrService implements OnDestroy {
  private readonly apiName = 'InventoryTrackingAutomation';
  private readonly hubPath = '/signalr-hubs/inventory-notification';
  private connection?: HubConnection;

  private readonly connectedSubject = new BehaviorSubject<boolean>(false);
  private readonly notificationsSubject = new Subject<InventoryNotificationMessage>();

  connected$ = this.connectedSubject.asObservable();
  notifications$ = this.notificationsSubject.asObservable();

  constructor(
    private environmentService: EnvironmentService,
    private oAuthService: OAuthService,
    private ngZone: NgZone
  ) {}

  async start(): Promise<void> {
    if (
      this.connection?.state === HubConnectionState.Connected ||
      this.connection?.state === HubConnectionState.Connecting
    ) {
      return;
    }

    this.connection = this.createConnection();
    this.registerHandlers(this.connection);

    await this.connection.start();
    this.connectedSubject.next(true);
  }

  async stop(): Promise<void> {
    if (!this.connection || this.connection.state === HubConnectionState.Disconnected) {
      return;
    }

    await this.connection.stop();
    this.connectedSubject.next(false);
  }

  ping(): Promise<string> {
    if (!this.connection || this.connection.state !== HubConnectionState.Connected) {
      return Promise.reject(new Error('Inventory notification hub is not connected.'));
    }

    return this.connection.invoke<string>('PingAsync');
  }

  ngOnDestroy(): void {
    void this.stop();
  }

  private createConnection(): HubConnection {
    const apiUrl = this.environmentService.getApiUrl(this.apiName);

    return new HubConnectionBuilder()
      .withUrl(`${apiUrl}${this.hubPath}`, {
        accessTokenFactory: () => this.oAuthService.getAccessToken(),
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build();
  }

  private registerHandlers(connection: HubConnection): void {
    connection.on('ReceiveInventoryNotification', (message: InventoryNotificationMessage) => {
      this.ngZone.run(() => this.notificationsSubject.next(message));
    });

    connection.onreconnecting(() => {
      this.ngZone.run(() => this.connectedSubject.next(false));
    });

    connection.onreconnected(() => {
      this.ngZone.run(() => this.connectedSubject.next(true));
    });

    connection.onclose(() => {
      this.ngZone.run(() => this.connectedSubject.next(false));
    });
  }
}
