import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

// Component imports
import { PersonCardComponent } from './components/person-card/person-card.component';
import { PersonListComponent } from './components/person-list/person-list.component';
import { SearchBarComponent } from './components/search-bar/search-bar.component';
import { PageHeaderComponent } from './components/page-header/page-header.component';
import { EmptyStateComponent } from './components/empty-state/empty-state.component';
import { ConfirmDialogComponent } from './components/confirm-dialog/confirm-dialog.component';
import { LoadingSpinnerComponent } from './components/loading-spinner/loading-spinner.component';
import { BreadcrumbComponent } from './components/breadcrumb/breadcrumb.component';

// Angular Material Modules
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatTabsModule } from '@angular/material/tabs';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatChipsModule } from '@angular/material/chips';
import { MatBadgeModule } from '@angular/material/badge';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatMenuModule } from '@angular/material/menu';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatRadioModule } from '@angular/material/radio';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatBottomSheetModule } from '@angular/material/bottom-sheet';
import { MatStepperModule } from '@angular/material/stepper';
import { MatSliderModule } from '@angular/material/slider';

const materialModules = [
  MatButtonModule,
  MatIconModule,
  MatCardModule,
  MatFormFieldModule,
  MatInputModule,
  MatSelectModule,
  MatTableModule,
  MatPaginatorModule,
  MatSortModule,
  MatDialogModule,
  MatSnackBarModule,
  MatToolbarModule,
  MatSidenavModule,
  MatListModule,
  MatTabsModule,
  MatExpansionModule,
  MatDatepickerModule,
  MatNativeDateModule,
  MatChipsModule,
  MatBadgeModule,
  MatProgressSpinnerModule,
  MatProgressBarModule,
  MatTooltipModule,
  MatMenuModule,
  MatCheckboxModule,
  MatRadioModule,
  MatSlideToggleModule,
  MatAutocompleteModule,
  MatBottomSheetModule,
  MatStepperModule,
  MatSliderModule,
];

/**
 * SharedModule - Provides common Angular Material components and reusable custom components
 * Import this module in any feature module that needs Material components or shared components
 */
@NgModule({
  declarations: [
    PersonCardComponent,
    PersonListComponent,
    SearchBarComponent,
    PageHeaderComponent,
    EmptyStateComponent,
    ConfirmDialogComponent,
    LoadingSpinnerComponent,
    BreadcrumbComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ...materialModules
  ],
  exports: [
    CommonModule,
    FormsModule,
    ...materialModules,
    PersonCardComponent,
    PersonListComponent,
    SearchBarComponent,
    PageHeaderComponent,
    EmptyStateComponent,
    ConfirmDialogComponent,
    LoadingSpinnerComponent,
    BreadcrumbComponent
  ]
})
export class SharedModule { }
