import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ShareService, ShareOptions } from '../../services/share.service';

/**
 * ShareDialogData interface
 */
export interface ShareDialogData {
  title: string;
  text: string;
  url: string;
  showSocialButtons?: boolean;
  showEmailButton?: boolean;
  showCopyButton?: boolean;
}

/**
 * ShareDialogComponent
 * 
 * Purpose: Comprehensive sharing dialog with multiple sharing options
 * 
 * Features:
 * - Native share button (mobile)
 * - Copy link button
 * - Email share button
 * - Social media buttons (Facebook, Twitter, LinkedIn, WhatsApp)
 * - QR code generation (optional)
 * - Shareable link preview
 * 
 * Usage:
 * ```typescript
 * constructor(private dialog: MatDialog) {}
 * 
 * openShareDialog() {
 *   this.dialog.open(ShareDialogComponent, {
 *     width: '500px',
 *     data: {
 *       title: 'John Doe - Family Profile',
 *       text: 'Check out this family member',
 *       url: window.location.href,
 *       showSocialButtons: true
 *     }
 *   });
 * }
 * ```
 */
@Component({
  selector: 'app-share-dialog',
  standalone: false,
  templateUrl: './share-dialog.component.html',
  styleUrls: ['./share-dialog.component.scss']
})
export class ShareDialogComponent {

  isNativeShareSupported: boolean;
  linkCopied = false;

  constructor(
    public dialogRef: MatDialogRef<ShareDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ShareDialogData,
    private shareService: ShareService
  ) {
    // Set defaults
    this.data.showSocialButtons = this.data.showSocialButtons !== false;
    this.data.showEmailButton = this.data.showEmailButton !== false;
    this.data.showCopyButton = this.data.showCopyButton !== false;

    // Check if native share is supported
    this.isNativeShareSupported = this.shareService.isNativeShareSupported();
  }

  /**
   * Share using native share API
   */
  async shareNative(): Promise<void> {
    const options: ShareOptions = {
      title: this.data.title,
      text: this.data.text,
      url: this.data.url
    };

    const result = await this.shareService.share(options);
    if (result.success) {
      this.dialogRef.close({ shared: true, method: result.method });
    }
  }

  /**
   * Copy link to clipboard
   */
  async copyLink(): Promise<void> {
    const result = await this.shareService.copyToClipboard(this.data.url);
    if (result.success) {
      this.linkCopied = true;
      setTimeout(() => {
        this.linkCopied = false;
      }, 3000);
    }
  }

  /**
   * Share via email
   */
  shareEmail(): void {
    const options: ShareOptions = {
      title: this.data.title,
      text: this.data.text,
      url: this.data.url
    };
    this.shareService.shareViaEmail(options);
    this.dialogRef.close({ shared: true, method: 'email' });
  }

  /**
   * Share on Facebook
   */
  shareFacebook(): void {
    this.shareService.shareOnFacebook(this.data.url);
    this.dialogRef.close({ shared: true, method: 'facebook' });
  }

  /**
   * Share on Twitter
   */
  shareTwitter(): void {
    const options: ShareOptions = {
      title: this.data.title,
      text: this.data.text,
      url: this.data.url
    };
    this.shareService.shareOnTwitter(options);
    this.dialogRef.close({ shared: true, method: 'twitter' });
  }

  /**
   * Share on LinkedIn
   */
  shareLinkedIn(): void {
    this.shareService.shareOnLinkedIn(this.data.url);
    this.dialogRef.close({ shared: true, method: 'linkedin' });
  }

  /**
   * Share on WhatsApp
   */
  shareWhatsApp(): void {
    const options: ShareOptions = {
      title: this.data.title,
      text: this.data.text,
      url: this.data.url
    };
    this.shareService.shareOnWhatsApp(options);
    this.dialogRef.close({ shared: true, method: 'whatsapp' });
  }

  /**
   * Close dialog
   */
  close(): void {
    this.dialogRef.close({ shared: false });
  }
}
