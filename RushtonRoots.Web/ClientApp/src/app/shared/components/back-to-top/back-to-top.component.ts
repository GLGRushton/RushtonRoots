import { Component, HostListener, OnInit } from '@angular/core';
import { fadeSlideIn } from '../../animations';

/**
 * BackToTopComponent - Scroll to top button
 * 
 * Displays a floating button that appears when user scrolls down
 * and smoothly scrolls to the top when clicked.
 * 
 * Usage:
 * <app-back-to-top></app-back-to-top>
 */
@Component({
  selector: 'app-back-to-top',
  standalone: false,
  templateUrl: './back-to-top.component.html',
  styleUrls: ['./back-to-top.component.scss'],
  animations: [fadeSlideIn]
})
export class BackToTopComponent implements OnInit {
  isVisible: boolean = false;
  private scrollThreshold: number = 300; // Show button after scrolling 300px

  ngOnInit(): void {
    this.checkScroll();
  }

  /**
   * Listen to scroll events
   */
  @HostListener('window:scroll', [])
  onWindowScroll(): void {
    this.checkScroll();
  }

  /**
   * Check if button should be visible based on scroll position
   */
  private checkScroll(): void {
    const scrollPosition = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0;
    this.isVisible = scrollPosition > this.scrollThreshold;
  }

  /**
   * Scroll to top of page
   */
  scrollToTop(): void {
    window.scrollTo({
      top: 0,
      behavior: 'smooth'
    });
  }
}
