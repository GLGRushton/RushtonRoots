import { trigger, state, style, transition, animate } from '@angular/animations';

/**
 * Shared Angular animations for the application
 */

// Fade in/out animation
export const fadeInOut = trigger('fadeInOut', [
  transition(':enter', [
    style({ opacity: 0 }),
    animate('300ms ease-in', style({ opacity: 1 }))
  ]),
  transition(':leave', [
    animate('200ms ease-out', style({ opacity: 0 }))
  ])
]);

// Slide in/out animation
export const slideInOut = trigger('slideInOut', [
  transition(':enter', [
    style({ transform: 'translateY(100%)', opacity: 0 }),
    animate('300ms ease-out', style({ transform: 'translateY(0)', opacity: 1 }))
  ]),
  transition(':leave', [
    animate('200ms ease-in', style({ transform: 'translateY(100%)', opacity: 0 }))
  ])
]);

// Fade and slide in animation for back-to-top button
export const fadeSlideIn = trigger('fadeSlideIn', [
  transition(':enter', [
    style({ transform: 'translateX(-20px)', opacity: 0 }),
    animate('300ms ease-out', style({ transform: 'translateX(0)', opacity: 1 }))
  ]),
  transition(':leave', [
    animate('200ms ease-in', style({ transform: 'translateX(-20px)', opacity: 0 }))
  ])
]);

// Rotate animation for FAB icon
export const rotate = trigger('rotate', [
  state('default', style({ transform: 'rotate(0deg)' })),
  state('rotated', style({ transform: 'rotate(45deg)' })),
  transition('default <=> rotated', animate('300ms ease-in-out'))
]);

// Scale animation
export const scaleIn = trigger('scaleIn', [
  transition(':enter', [
    style({ transform: 'scale(0)', opacity: 0 }),
    animate('300ms cubic-bezier(0.175, 0.885, 0.32, 1.275)', 
      style({ transform: 'scale(1)', opacity: 1 }))
  ]),
  transition(':leave', [
    animate('200ms ease-in', style({ transform: 'scale(0)', opacity: 0 }))
  ])
]);

// Expand/collapse animation
export const expandCollapse = trigger('expandCollapse', [
  state('collapsed', style({ height: '0', overflow: 'hidden', opacity: 0 })),
  state('expanded', style({ height: '*', overflow: 'visible', opacity: 1 })),
  transition('collapsed <=> expanded', animate('300ms ease-in-out'))
]);
