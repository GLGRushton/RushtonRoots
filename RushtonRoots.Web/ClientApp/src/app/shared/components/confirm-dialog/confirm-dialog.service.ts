import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { ConfirmDialogComponent, ConfirmDialogData } from './confirm-dialog.component';

/**
 * ConfirmDialogService - Service to easily show confirmation dialogs
 * 
 * Usage:
 * constructor(private confirmDialog: ConfirmDialogService) {}
 * 
 * this.confirmDialog.confirm({
 *   title: 'Delete Item',
 *   message: 'Are you sure?',
 *   confirmText: 'Delete',
 *   confirmColor: 'warn'
 * }).subscribe(confirmed => {
 *   if (confirmed) {
 *     // Perform delete
 *   }
 * });
 */
@Injectable({
  providedIn: 'root'
})
export class ConfirmDialogService {
  constructor(private dialog: MatDialog) {}

  confirm(data: ConfirmDialogData): Observable<boolean> {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: data,
      disableClose: false,
      autoFocus: true
    });

    return dialogRef.afterClosed();
  }

  confirmDelete(itemName: string, itemType: string = 'item'): Observable<boolean> {
    return this.confirm({
      title: `Delete ${itemType}`,
      message: `Are you sure you want to delete "${itemName}"? This action cannot be undone.`,
      confirmText: 'Delete',
      cancelText: 'Cancel',
      confirmColor: 'warn'
    });
  }
}
