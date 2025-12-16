import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ConnectedAccount, CONNECTED_ACCOUNT_PROVIDERS } from '../../models/user-profile.model';

/**
 * ConnectedAccountsComponent - Manage connected social accounts
 */
@Component({
  selector: 'app-connected-accounts',
  templateUrl: './connected-accounts.component.html',
  styleUrls: ['./connected-accounts.component.scss'],
  standalone: false
})
export class ConnectedAccountsComponent {
  @Input() connectedAccounts: ConnectedAccount[] = [];
  @Output() accountConnect = new EventEmitter<string>();
  @Output() accountDisconnect = new EventEmitter<string>();

  availableProviders = CONNECTED_ACCOUNT_PROVIDERS;

  isConnected(providerId: string): boolean {
    return this.connectedAccounts.some(acc => acc.provider === providerId && acc.status === 'active');
  }

  getConnectedAccount(providerId: string): ConnectedAccount | undefined {
    return this.connectedAccounts.find(acc => acc.provider === providerId && acc.status === 'active');
  }

  connect(providerId: string): void {
    this.accountConnect.emit(providerId);
  }

  disconnect(providerId: string): void {
    this.accountDisconnect.emit(providerId);
  }

  formatDate(date: Date | undefined): string {
    if (!date) return 'Never';
    return new Date(date).toLocaleDateString();
  }
}
