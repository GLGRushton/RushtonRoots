import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule } from '@angular/material/dialog';

// Components
import { SkipNavigationComponent } from './components/skip-navigation/skip-navigation.component';
import { KeyboardShortcutsDialogComponent } from './components/keyboard-shortcuts-dialog/keyboard-shortcuts-dialog.component';
import { AccessibilityStatementComponent } from './components/accessibility-statement/accessibility-statement.component';

// Services
import { AccessibilityTestingService } from './services/accessibility-testing.service';
import { FocusManagementService } from './services/focus-management.service';
import { KeyboardNavigationService } from './services/keyboard-navigation.service';

/**
 * Accessibility module providing comprehensive accessibility features
 */
@NgModule({
  imports: [
    CommonModule,
    MatDialogModule,
    SkipNavigationComponent,
    KeyboardShortcutsDialogComponent,
    AccessibilityStatementComponent
  ],
  exports: [
    SkipNavigationComponent,
    KeyboardShortcutsDialogComponent,
    AccessibilityStatementComponent
  ],
  providers: [
    AccessibilityTestingService,
    FocusManagementService,
    KeyboardNavigationService
  ]
})
export class AccessibilityModule {}
