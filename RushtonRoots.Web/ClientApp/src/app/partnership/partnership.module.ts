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
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatAutocompleteModule } from '@angular/material/autocomplete';

// Components - Phase 5.1
import { PartnershipIndexComponent } from './components/partnership-index/partnership-index.component';
import { PartnershipCardComponent } from './components/partnership-card/partnership-card.component';
import { PartnershipFormComponent } from './components/partnership-form/partnership-form.component';
import { PartnershipTimelineComponent } from './components/partnership-timeline/partnership-timeline.component';

@NgModule({
  declarations: [
    // Phase 5.1
    PartnershipIndexComponent,
    PartnershipCardComponent,
    PartnershipFormComponent,
    PartnershipTimelineComponent
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
    MatDatepickerModule,
    MatNativeDateModule,
    MatAutocompleteModule
  ],
  exports: [
    // Phase 5.1
    PartnershipIndexComponent,
    PartnershipCardComponent,
    PartnershipFormComponent,
    PartnershipTimelineComponent
  ]
})
export class PartnershipModule { }
