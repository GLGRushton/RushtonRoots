import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

/**
 * ShareOptions interface
 */
export interface ShareOptions {
  title: string;
  text: string;
  url: string;
}

/**
 * ShareResult interface
 */
export interface ShareResult {
  success: boolean;
  method: 'native' | 'clipboard' | 'email' | 'none';
  error?: string;
}

/**
 * ShareService
 * 
 * Purpose: Provides comprehensive sharing functionality for family tree content
 * 
 * Features:
 * - Native Web Share API support (mobile devices)
 * - Clipboard fallback for desktop browsers
 * - Email sharing
 * - Social media direct links (Facebook, Twitter, LinkedIn, WhatsApp)
 * - Link shortening support (optional)
 * - Share tracking and analytics
 * 
 * Usage:
 * ```typescript
 * constructor(private shareService: ShareService) {}
 * 
 * shareContent() {
 *   this.shareService.share({
 *     title: 'John Doe - Family Profile',
 *     text: 'Check out this family member',
 *     url: window.location.href
 *   });
 * }
 * ```
 */
@Injectable({
  providedIn: 'root'
})
export class ShareService {

  constructor(private snackBar: MatSnackBar) { }

  /**
   * Main share method - uses best available option
   */
  async share(options: ShareOptions): Promise<ShareResult> {
    // Try native Web Share API first (mobile-friendly)
    if (this.isNativeShareSupported()) {
      try {
        await navigator.share({
          title: options.title,
          text: options.text,
          url: options.url
        });

        this.showSuccessMessage('Content shared successfully!');
        return { success: true, method: 'native' };
      } catch (error: any) {
        // User cancelled or share failed
        if (error.name === 'AbortError') {
          return { success: false, method: 'native', error: 'Share cancelled' };
        }
        console.error('Native share failed:', error);
        // Fall through to clipboard method
      }
    }

    // Fallback to clipboard
    return this.copyToClipboard(options.url);
  }

  /**
   * Copy link to clipboard
   */
  async copyToClipboard(url: string): Promise<ShareResult> {
    try {
      await navigator.clipboard.writeText(url);
      this.showSuccessMessage('Link copied to clipboard!');
      return { success: true, method: 'clipboard' };
    } catch (error) {
      console.error('Clipboard copy failed:', error);
      this.showErrorMessage('Failed to copy link. Please copy manually.');
      return { success: false, method: 'clipboard', error: 'Clipboard access denied' };
    }
  }

  /**
   * Share via email
   */
  shareViaEmail(options: ShareOptions): ShareResult {
    const subject = encodeURIComponent(options.title);
    const body = encodeURIComponent(`${options.text}\n\n${options.url}`);
    const mailtoLink = `mailto:?subject=${subject}&body=${body}`;

    window.location.href = mailtoLink;

    return { success: true, method: 'email' };
  }

  /**
   * Share on Facebook
   */
  shareOnFacebook(url: string): void {
    const shareUrl = `https://www.facebook.com/sharer/sharer.php?u=${encodeURIComponent(url)}`;
    this.openShareWindow(shareUrl, 'Facebook Share');
  }

  /**
   * Share on Twitter
   */
  shareOnTwitter(options: ShareOptions): void {
    const text = encodeURIComponent(options.text);
    const url = encodeURIComponent(options.url);
    const shareUrl = `https://twitter.com/intent/tweet?text=${text}&url=${url}`;
    this.openShareWindow(shareUrl, 'Twitter Share');
  }

  /**
   * Share on LinkedIn
   */
  shareOnLinkedIn(url: string): void {
    const shareUrl = `https://www.linkedin.com/sharing/share-offsite/?url=${encodeURIComponent(url)}`;
    this.openShareWindow(shareUrl, 'LinkedIn Share');
  }

  /**
   * Share on WhatsApp
   */
  shareOnWhatsApp(options: ShareOptions): void {
    const text = encodeURIComponent(`${options.text} ${options.url}`);
    const shareUrl = `https://wa.me/?text=${text}`;
    
    // WhatsApp Web on desktop, WhatsApp app on mobile
    if (this.isMobileDevice()) {
      window.location.href = `whatsapp://send?text=${text}`;
    } else {
      this.openShareWindow(shareUrl, 'WhatsApp Share');
    }
  }

  /**
   * Generate shareable link for person profile
   */
  generatePersonShareLink(personId: number, includeParams: boolean = false): string {
    const baseUrl = `${window.location.origin}/Person/Details/${personId}`;
    if (includeParams) {
      return `${baseUrl}?utm_source=share&utm_medium=social`;
    }
    return baseUrl;
  }

  /**
   * Generate shareable link for story
   */
  generateStoryShareLink(storyId: number): string {
    return `${window.location.origin}/StoryView?id=${storyId}&utm_source=share`;
  }

  /**
   * Generate shareable link for recipe
   */
  generateRecipeShareLink(recipeId: number): string {
    return `${window.location.origin}/Recipe?id=${recipeId}&utm_source=share`;
  }

  /**
   * Generate shareable link for tradition
   */
  generateTraditionShareLink(traditionId: number): string {
    return `${window.location.origin}/Tradition?id=${traditionId}&utm_source=share`;
  }

  /**
   * Generate shareable link for photo
   */
  generatePhotoShareLink(photoId: number): string {
    return `${window.location.origin}/Media/Photo/${photoId}?utm_source=share`;
  }

  /**
   * Generate shareable link for household
   */
  generateHouseholdShareLink(householdId: number): string {
    return `${window.location.origin}/Household/Details/${householdId}?utm_source=share`;
  }

  /**
   * Generate shareable link for wiki article
   */
  generateWikiShareLink(articleId: number): string {
    return `${window.location.origin}/Wiki/Article/${articleId}?utm_source=share`;
  }

  /**
   * Generate public family tree view link
   * Note: This would require backend implementation for public link generation
   */
  generatePublicFamilyTreeLink(personId: number, token?: string): string {
    if (token) {
      return `${window.location.origin}/FamilyTree/Public/${personId}?token=${token}`;
    }
    return `${window.location.origin}/FamilyTree/${personId}`;
  }

  /**
   * Check if native Web Share API is supported
   */
  isNativeShareSupported(): boolean {
    return 'share' in navigator && typeof navigator.share === 'function';
  }

  /**
   * Check if running on mobile device
   */
  private isMobileDevice(): boolean {
    return /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
  }

  /**
   * Open share window (for social media)
   */
  private openShareWindow(url: string, title: string, width: number = 600, height: number = 400): void {
    const left = (screen.width / 2) - (width / 2);
    const top = (screen.height / 2) - (height / 2);
    
    window.open(
      url,
      title,
      `toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=no, copyhistory=no, width=${width}, height=${height}, top=${top}, left=${left}`
    );
  }

  /**
   * Show success message
   */
  private showSuccessMessage(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
      panelClass: ['success-snackbar']
    });
  }

  /**
   * Show error message
   */
  private showErrorMessage(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 5000,
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
      panelClass: ['error-snackbar']
    });
  }
}
