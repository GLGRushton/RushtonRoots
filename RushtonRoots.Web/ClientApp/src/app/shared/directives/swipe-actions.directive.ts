// Swipe Actions Directive - Enables swipe gestures for actions like delete/archive
import {
  Directive,
  ElementRef,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
  Renderer2
} from '@angular/core';
import { MobileService } from '../services/mobile.service';

export interface SwipeAction {
  icon: string;
  label: string;
  color: string;
  action: string;
}

export interface SwipeEvent {
  action: string;
  direction: 'left' | 'right';
}

@Directive({
  selector: '[appSwipeActions]',
  standalone: true
})
export class SwipeActionsDirective implements OnInit, OnDestroy {
  
  @Input() leftAction?: SwipeAction;
  @Input() rightAction?: SwipeAction;
  @Input() swipeThreshold = 80; // Pixels to swipe before triggering action
  @Input() swipeEnabled = true;
  
  @Output() swipe = new EventEmitter<SwipeEvent>();
  
  private startX = 0;
  private currentX = 0;
  private swiping = false;
  private leftActionEl: HTMLElement | null = null;
  private rightActionEl: HTMLElement | null = null;
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
    
    this.createActionElements();
    this.attachListeners();
  }
  
  ngOnDestroy(): void {
    this.removeActionElements();
    this.detachListeners();
  }
  
  private createActionElements(): void {
    const element = this.el.nativeElement;
    
    // Set up container styles
    this.renderer.setStyle(element, 'position', 'relative');
    this.renderer.setStyle(element, 'overflow', 'hidden');
    this.renderer.setStyle(element, 'transition', 'transform 0.3s ease');
    
    // Create left action element
    if (this.leftAction) {
      this.leftActionEl = this.renderer.createElement('div');
      this.renderer.addClass(this.leftActionEl, 'swipe-action-left');
      this.renderer.setStyle(this.leftActionEl, 'position', 'absolute');
      this.renderer.setStyle(this.leftActionEl, 'left', '0');
      this.renderer.setStyle(this.leftActionEl, 'top', '0');
      this.renderer.setStyle(this.leftActionEl, 'bottom', '0');
      this.renderer.setStyle(this.leftActionEl, 'width', '80px');
      this.renderer.setStyle(this.leftActionEl, 'display', 'flex');
      this.renderer.setStyle(this.leftActionEl, 'align-items', 'center');
      this.renderer.setStyle(this.leftActionEl, 'justify-content', 'center');
      this.renderer.setStyle(this.leftActionEl, 'flex-direction', 'column');
      this.renderer.setStyle(this.leftActionEl, 'color', 'white');
      this.renderer.setStyle(this.leftActionEl, 'background-color', this.leftAction.color);
      this.renderer.setStyle(this.leftActionEl, 'gap', '4px');
      
      this.leftActionEl.innerHTML = `
        <mat-icon>${this.leftAction.icon}</mat-icon>
        <span style="font-size: 12px;">${this.leftAction.label}</span>
      `;
      
      this.renderer.insertBefore(element.parentNode, this.leftActionEl, element);
    }
    
    // Create right action element
    if (this.rightAction) {
      this.rightActionEl = this.renderer.createElement('div');
      this.renderer.addClass(this.rightActionEl, 'swipe-action-right');
      this.renderer.setStyle(this.rightActionEl, 'position', 'absolute');
      this.renderer.setStyle(this.rightActionEl, 'right', '0');
      this.renderer.setStyle(this.rightActionEl, 'top', '0');
      this.renderer.setStyle(this.rightActionEl, 'bottom', '0');
      this.renderer.setStyle(this.rightActionEl, 'width', '80px');
      this.renderer.setStyle(this.rightActionEl, 'display', 'flex');
      this.renderer.setStyle(this.rightActionEl, 'align-items', 'center');
      this.renderer.setStyle(this.rightActionEl, 'justify-content', 'center');
      this.renderer.setStyle(this.rightActionEl, 'flex-direction', 'column');
      this.renderer.setStyle(this.rightActionEl, 'color', 'white');
      this.renderer.setStyle(this.rightActionEl, 'background-color', this.rightAction.color);
      this.renderer.setStyle(this.rightActionEl, 'gap', '4px');
      
      this.rightActionEl.innerHTML = `
        <mat-icon>${this.rightAction.icon}</mat-icon>
        <span style="font-size: 12px;">${this.rightAction.label}</span>
      `;
      
      this.renderer.insertBefore(element.parentNode, this.rightActionEl, element);
    }
  }
  
  private removeActionElements(): void {
    if (this.leftActionEl && this.leftActionEl.parentNode) {
      this.renderer.removeChild(this.leftActionEl.parentNode, this.leftActionEl);
    }
    if (this.rightActionEl && this.rightActionEl.parentNode) {
      this.renderer.removeChild(this.rightActionEl.parentNode, this.rightActionEl);
    }
  }
  
  private attachListeners(): void {
    const element = this.el.nativeElement;
    
    // Touch start
    const touchStartListener = this.renderer.listen(element, 'touchstart', (e: TouchEvent) => {
      if (!this.swipeEnabled) return;
      
      this.startX = e.touches[0].clientX;
      this.swiping = true;
      this.renderer.setStyle(element, 'transition', 'none');
    });
    
    // Touch move
    const touchMoveListener = this.renderer.listen(element, 'touchmove', (e: TouchEvent) => {
      if (!this.swiping || !this.swipeEnabled) return;
      
      this.currentX = e.touches[0].clientX;
      const swipeDistance = this.currentX - this.startX;
      
      // Limit swipe distance
      const maxSwipe = 80;
      const limitedSwipe = Math.max(-maxSwipe, Math.min(maxSwipe, swipeDistance));
      
      // Only allow swipe if action is defined for that direction
      if ((swipeDistance > 0 && this.leftAction) || (swipeDistance < 0 && this.rightAction)) {
        this.renderer.setStyle(element, 'transform', `translateX(${limitedSwipe}px)`);
        
        // Show/hide action elements
        if (swipeDistance > 0 && this.leftActionEl) {
          this.renderer.setStyle(this.leftActionEl, 'opacity', '1');
        }
        if (swipeDistance < 0 && this.rightActionEl) {
          this.renderer.setStyle(this.rightActionEl, 'opacity', '1');
        }
      }
    });
    
    // Touch end
    const touchEndListener = this.renderer.listen(element, 'touchend', (e: TouchEvent) => {
      if (!this.swiping || !this.swipeEnabled) return;
      
      const swipeDistance = this.currentX - this.startX;
      
      // Check if threshold was reached
      if (Math.abs(swipeDistance) >= this.swipeThreshold) {
        if (swipeDistance > 0 && this.leftAction) {
          this.triggerAction(this.leftAction.action, 'left');
        } else if (swipeDistance < 0 && this.rightAction) {
          this.triggerAction(this.rightAction.action, 'right');
        }
      }
      
      // Reset position
      this.resetPosition();
      
      this.swiping = false;
      this.startX = 0;
      this.currentX = 0;
    });
    
    // Touch cancel
    const touchCancelListener = this.renderer.listen(element, 'touchcancel', () => {
      if (this.swiping) {
        this.resetPosition();
        this.swiping = false;
      }
    });
    
    this.unlisteners.push(
      touchStartListener, 
      touchMoveListener, 
      touchEndListener, 
      touchCancelListener
    );
  }
  
  private detachListeners(): void {
    this.unlisteners.forEach(unlisten => unlisten());
    this.unlisteners = [];
  }
  
  private triggerAction(action: string, direction: 'left' | 'right'): void {
    // Emit swipe event
    this.swipe.emit({ action, direction });
    
    // Add haptic feedback
    this.mobileService.vibrate(50);
    
    // Animate element out
    const element = this.el.nativeElement;
    const swipeOut = direction === 'left' ? '100%' : '-100%';
    this.renderer.setStyle(element, 'transition', 'transform 0.3s ease');
    this.renderer.setStyle(element, 'transform', `translateX(${swipeOut})`);
    this.renderer.setStyle(element, 'opacity', '0');
  }
  
  private resetPosition(): void {
    const element = this.el.nativeElement;
    this.renderer.setStyle(element, 'transition', 'transform 0.3s ease');
    this.renderer.setStyle(element, 'transform', 'translateX(0)');
    
    if (this.leftActionEl) {
      this.renderer.setStyle(this.leftActionEl, 'opacity', '0');
    }
    if (this.rightActionEl) {
      this.renderer.setStyle(this.rightActionEl, 'opacity', '0');
    }
  }
  
  /**
   * Public method to reset the swipe position
   */
  public reset(): void {
    this.resetPosition();
  }
}
