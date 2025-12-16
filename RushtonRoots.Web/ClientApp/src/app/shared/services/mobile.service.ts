// Mobile Service - Detects mobile devices and provides mobile-specific utilities
import { Injectable } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';

export interface DeviceInfo {
  isMobile: boolean;
  isTablet: boolean;
  isDesktop: boolean;
  isPortrait: boolean;
  isLandscape: boolean;
  isTouchDevice: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class MobileService {
  
  // Observable for responsive breakpoint changes
  public isMobile$: Observable<boolean>;
  public isTablet$: Observable<boolean>;
  public isDesktop$: Observable<boolean>;
  
  constructor(private breakpointObserver: BreakpointObserver) {
    // Set up breakpoint observables
    this.isMobile$ = this.breakpointObserver
      .observe([Breakpoints.XSmall])
      .pipe(
        map(result => result.matches),
        shareReplay(1)
      );
    
    this.isTablet$ = this.breakpointObserver
      .observe([Breakpoints.Small, Breakpoints.Medium])
      .pipe(
        map(result => result.matches),
        shareReplay(1)
      );
    
    this.isDesktop$ = this.breakpointObserver
      .observe([Breakpoints.Large, Breakpoints.XLarge])
      .pipe(
        map(result => result.matches),
        shareReplay(1)
      );
  }
  
  /**
   * Get current device information synchronously
   */
  getDeviceInfo(): DeviceInfo {
    const width = window.innerWidth;
    const height = window.innerHeight;
    
    return {
      isMobile: width < 600,
      isTablet: width >= 600 && width < 960,
      isDesktop: width >= 960,
      isPortrait: height > width,
      isLandscape: width > height,
      isTouchDevice: this.isTouchDevice()
    };
  }
  
  /**
   * Check if current device is mobile
   */
  isMobile(): boolean {
    return this.getDeviceInfo().isMobile;
  }
  
  /**
   * Check if current device is tablet
   */
  isTablet(): boolean {
    return this.getDeviceInfo().isTablet;
  }
  
  /**
   * Check if current device is desktop
   */
  isDesktop(): boolean {
    return this.getDeviceInfo().isDesktop;
  }
  
  /**
   * Check if device supports touch
   */
  isTouchDevice(): boolean {
    return (
      'ontouchstart' in window ||
      navigator.maxTouchPoints > 0 ||
      (navigator as any).msMaxTouchPoints > 0
    );
  }
  
  /**
   * Check if device is in portrait orientation
   */
  isPortrait(): boolean {
    return window.innerHeight > window.innerWidth;
  }
  
  /**
   * Check if device is in landscape orientation
   */
  isLandscape(): boolean {
    return window.innerWidth > window.innerHeight;
  }
  
  /**
   * Get safe area insets for notched devices (iOS)
   */
  getSafeAreaInsets(): { top: number; bottom: number; left: number; right: number } {
    const style = getComputedStyle(document.documentElement);
    
    return {
      top: parseInt(style.getPropertyValue('env(safe-area-inset-top)') || '0'),
      bottom: parseInt(style.getPropertyValue('env(safe-area-inset-bottom)') || '0'),
      left: parseInt(style.getPropertyValue('env(safe-area-inset-left)') || '0'),
      right: parseInt(style.getPropertyValue('env(safe-area-inset-right)') || '0')
    };
  }
  
  /**
   * Vibrate device (if supported)
   */
  vibrate(pattern: number | number[] = 100): void {
    if ('vibrate' in navigator) {
      navigator.vibrate(pattern);
    }
  }
  
  /**
   * Check if device has PWA installed
   */
  isPWA(): boolean {
    return (
      window.matchMedia('(display-mode: standalone)').matches ||
      (window.navigator as any).standalone === true
    );
  }
  
  /**
   * Get viewport height accounting for mobile browser bars
   */
  getViewportHeight(): number {
    return window.innerHeight;
  }
  
  /**
   * Set viewport height CSS variable for mobile browsers
   */
  setViewportHeightVariable(): void {
    const vh = window.innerHeight * 0.01;
    document.documentElement.style.setProperty('--vh', `${vh}px`);
  }
  
  /**
   * Prevent body scroll (useful for modals on mobile)
   */
  disableBodyScroll(): void {
    document.body.style.overflow = 'hidden';
    document.body.style.position = 'fixed';
    document.body.style.width = '100%';
  }
  
  /**
   * Re-enable body scroll
   */
  enableBodyScroll(): void {
    document.body.style.overflow = '';
    document.body.style.position = '';
    document.body.style.width = '';
  }
}
