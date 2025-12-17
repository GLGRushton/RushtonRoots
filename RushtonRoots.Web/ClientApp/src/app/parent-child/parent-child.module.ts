import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

// Material Modules
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatBadgeModule } from '@angular/material/badge';
import { MatTableModule } from '@angular/material/table';
import { MatDialogModule } from '@angular/material/dialog';
import { MatStepperModule } from '@angular/material/stepper';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatTabsModule } from '@angular/material/tabs';
import { MatListModule } from '@angular/material/list';
import { MatRadioModule } from '@angular/material/radio';

// Components - Phase 5.2
import { ParentChildIndexComponent } from './components/parent-child-index/parent-child-index.component';
import { ParentChildCardComponent } from './components/parent-child-card/parent-child-card.component';
import { ParentChildFormComponent } from './components/parent-child-form/parent-child-form.component';
import { ParentChildDetailsComponent } from './components/parent-child-details/parent-child-details.component';
import { FamilyTreeMiniComponent } from './components/family-tree-mini/family-tree-mini.component';
import { RelationshipValidationComponent } from './components/relationship-validation/relationship-validation.component';
import { RelationshipSuggestionsComponent } from './components/relationship-suggestions/relationship-suggestions.component';
import { BulkRelationshipImportComponent } from './components/bulk-relationship-import/bulk-relationship-import.component';

// Components - Phase 5.4
import { ParentChildDeleteDialogComponent } from './components/parent-child-delete-dialog/parent-child-delete-dialog.component';

@NgModule({
  declarations: [
    // Phase 5.2
    ParentChildIndexComponent,
    ParentChildCardComponent,
    ParentChildFormComponent,
    ParentChildDetailsComponent,
    FamilyTreeMiniComponent,
    RelationshipValidationComponent,
    RelationshipSuggestionsComponent,
    BulkRelationshipImportComponent,
    // Phase 5.4
    ParentChildDeleteDialogComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatChipsModule,
    MatTooltipModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDividerModule,
    MatProgressSpinnerModule,
    MatAutocompleteModule,
    MatCheckboxModule,
    MatExpansionModule,
    MatBadgeModule,
    MatTableModule,
    MatDialogModule,
    MatStepperModule,
    MatSlideToggleModule,
    MatButtonToggleModule,
    MatTabsModule,
    MatListModule,
    MatRadioModule
  ],
  exports: [
    // Phase 5.2
    ParentChildIndexComponent,
    ParentChildCardComponent,
    ParentChildFormComponent,
    ParentChildDetailsComponent,
    FamilyTreeMiniComponent,
    RelationshipValidationComponent,
    RelationshipSuggestionsComponent,
    BulkRelationshipImportComponent,
    // Phase 5.4
    ParentChildDeleteDialogComponent
  ]
})
export class ParentChildModule { }
