import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

// Material Modules
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatBadgeModule } from '@angular/material/badge';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTabsModule } from '@angular/material/tabs';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatRadioModule } from '@angular/material/radio';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSnackBarModule } from '@angular/material/snack-bar';

// Components - Phase 4.1
import { HouseholdCardComponent } from './components/household-card/household-card.component';
import { HouseholdIndexComponent } from './components/household-index/household-index.component';

// Components - Phase 4.2
import { HouseholdDetailsComponent } from './components/household-details/household-details.component';
import { HouseholdMembersComponent } from './components/household-members/household-members.component';
import { MemberInviteDialogComponent } from './components/member-invite-dialog/member-invite-dialog.component';
import { HouseholdSettingsComponent } from './components/household-settings/household-settings.component';
import { HouseholdActivityTimelineComponent } from './components/household-activity-timeline/household-activity-timeline.component';

// Components - Phase 3.3 (UpdateDesigns.md)
import { HouseholdFormComponent } from './components/household-form/household-form.component';

@NgModule({
  declarations: [
    // Phase 4.1
    HouseholdCardComponent,
    HouseholdIndexComponent,
    // Phase 4.2
    HouseholdDetailsComponent,
    HouseholdMembersComponent,
    MemberInviteDialogComponent,
    HouseholdSettingsComponent,
    HouseholdActivityTimelineComponent,
    // Phase 3.3 (UpdateDesigns.md)
    HouseholdFormComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatBadgeModule,
    MatTooltipModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDividerModule,
    MatProgressSpinnerModule,
    MatTabsModule,
    MatChipsModule,
    MatDialogModule,
    MatSlideToggleModule,
    MatRadioModule,
    MatAutocompleteModule,
    MatCheckboxModule,
    MatSnackBarModule
  ],
  exports: [
    // Phase 4.1
    HouseholdCardComponent,
    HouseholdIndexComponent,
    // Phase 4.2
    HouseholdDetailsComponent,
    HouseholdMembersComponent,
    MemberInviteDialogComponent,
    HouseholdSettingsComponent,
    HouseholdActivityTimelineComponent,
    // Phase 3.3 (UpdateDesigns.md)
    HouseholdFormComponent
  ]
})
export class HouseholdModule { }
