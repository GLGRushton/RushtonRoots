// Pull-to-Refresh Directive for mobile list views
import { 
  Directive, 
  ElementRef, 
  EventEmitter, 
  OnDestroy, 
  OnInit, 
  Output,
  Renderer2,
  Input
} from '@angular/core';
import { MobileService } from '../services/mobile.service';

@Directive({
  selector: '[appPullToRefresh]',
  standalone: true
})
export class PullToRefreshDirective implements OnInit, OnDestroy {
  
  @Output() refresh = new EventEmitter<void>();
  @Input() refreshThreshold = 80; // Pixels to pull before triggering refresh
  @Input() refreshEnabled = true;
  
  private startY = 0;
  private currentY = 0;
  private pulling = false;
  private indicator: HTMLElement | null = null;
  private unlisteners: (() => void)[] = [];
  
  constructor(
    private el: ElementRef,
    private renderer: Renderer2,
    private mobileService: MobileService
  ) {}
  
  ngOnInit(): void {
    // Only enable on mobile touch devices
    if (!this.mobileService.isTouchDevice()) {
      return;
    }
    
    this.createIndicator();
    this.attachListeners();
  }
  
  ngOnDestroy(): void {
    this.removeIndicator();
    this.detachListeners();
  }
  
  private createIndicator(): void {
    this.indicator = this.renderer.createElement('div');
    this.renderer.addClass(this.indicator, 'ptr-indicator');
    this.renderer.setStyle(this.indicator, 'position', 'absolute');
    this.renderer.setStyle(this.indicator, 'top', '-60px');
    this.renderer.setStyle(this.indicator, 'left', '50%');
    this.renderer.setStyle(this.indicator, 'transform', 'translateX(-50%)');
    this.renderer.setStyle(this.indicator, 'padding', '12px');
    this.renderer.setStyle(this.indicator, 'background', '#fff');
    this.renderer.setStyle(this.indicator, 'border-radius', '50%');
    this.renderer.setStyle(this.indicator, 'box-shadow', '0 2px 8px rgba(0,0,0,0.15)');
    this.renderer.setStyle(this.indicator, 'opacity', '0');
    this.renderer.setStyle(this.indicator, 'transition', 'opacity 0.3s, transform 0.3s');
    this.renderer.setStyle(this.indicator, 'z-index', '1000');
    
    // Add loading spinner icon
    this.indicator.innerHTML = `
      <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
        <circle cx="12" cy="12" r="10" stroke="#2e7d32" stroke-width="2" stroke-dasharray="15 35" stroke-linecap="round">
          <animateTransform attributeName="transform" type="rotate" from="0 12 12" to="360 12 12" dur="1s" repeatCount="indefinite"/>
        </circle>
      </svg>
    `;
    
    this.renderer.setStyle(this.el.nativeElement, 'position', 'relative');
    this.renderer.appendChild(this.el.nativeElement, this.indicator);
  }
  
  private removeIndicator(): void {
    if (this.indicator && this.indicator.parentNode) {
      this.renderer.removeChild(this.el.nativeElement, this.indicator);
    }
  }
  
  private attachListeners(): void {
    const element = this.el.nativeElement;
    
    // Touch start
    const touchStartListener = this.renderer.listen(element, 'touchstart', (e: TouchEvent) => {
      if (!this.refreshEnabled) return;
      
      // Only trigger if scrolled to top
      if (element.scrollTop === 0) {
        this.startY = e.touches[0].clientY;
        this.pulling = true;
      }
    });
    
    // Touch move
    const touchMoveListener = this.renderer.listen(element, 'touchmove', (e: TouchEvent) => {
      if (!this.pulling || !this.refreshEnabled) return;
      
      this.currentY = e.touches[0].clientY;
      const pullDistance = this.currentY - this.startY;
      
      if (pullDistance > 0 && element.scrollTop === 0) {
        // Prevent default scrolling
        e.preventDefault();
        
        // Update indicator position and opacity
        if (this.indicator) {
          const progress = Math.min(pullDistance / this.refreshThreshold, 1);
          const translateY = Math.min(pullDistance / 2, this.refreshThreshold / 2);
          
          this.renderer.setStyle(this.indicator, 'opacity', progress.toString());
          this.renderer.setStyle(
            this.indicator, 
            'transform', 
            `translateX(-50%) translateY(${translateY}px)`
          );
          
          // Add visual feedback when threshold is reached
          if (pullDistance >= this.refreshThreshold) {
            this.renderer.setStyle(this.indicator, 'transform', 
              `translateX(-50%) translateY(${translateY}px) scale(1.1)`);
          }
        }
      }
    });
    
    // Touch end
    const touchEndListener = this.renderer.listen(element, 'touchend', (e: TouchEvent) => {
      if (!this.pulling || !this.refreshEnabled) return;
      
      const pullDistance = this.currentY - this.startY;
      
      if (pullDistance >= this.refreshThreshold) {
        this.triggerRefresh();
      } else {
        this.resetIndicator();
      }
      
      this.pulling = false;
      this.startY = 0;
      this.currentY = 0;
    });
    
    this.unlisteners.push(touchStartListener, touchMoveListener, touchEndListener);
  }
  
  private detachListeners(): void {
    this.unlisteners.forEach(unlisten => unlisten());
    this.unlisteners = [];
  }
  
  private triggerRefresh(): void {
    if (this.indicator) {
      this.renderer.addClass(this.indicator, 'active');
    }
    
    // Emit refresh event
    this.refresh.emit();
    
    // Add haptic feedback
    this.mobileService.vibrate(50);
    
    // Auto-hide after 2 seconds
    setTimeout(() => {
      this.resetIndicator();
    }, 2000);
  }
  
  private resetIndicator(): void {
    if (this.indicator) {
      this.renderer.setStyle(this.indicator, 'opacity', '0');
      this.renderer.setStyle(this.indicator, 'transform', 'translateX(-50%) translateY(0)');
      this.renderer.removeClass(this.indicator, 'active');
    }
  }
  
  /**
   * Public method to manually reset the indicator (call this after refresh completes)
   */
  public completeRefresh(): void {
    this.resetIndicator();
  }
}
