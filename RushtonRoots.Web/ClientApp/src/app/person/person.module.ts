import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

// Material Modules
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatChipsModule } from '@angular/material/chips';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTabsModule } from '@angular/material/tabs';
import { MatMenuModule } from '@angular/material/menu';

// Components
import { PersonIndexComponent } from './components/person-index/person-index.component';
import { PersonTableComponent } from './components/person-table/person-table.component';
import { PersonSearchComponent } from './components/person-search/person-search.component';
import { PersonDetailsComponent } from './components/person-details/person-details.component';
import { PersonTimelineComponent } from './components/person-timeline/person-timeline.component';
import { RelationshipVisualizerComponent } from './components/relationship-visualizer/relationship-visualizer.component';
import { PhotoGalleryComponent } from './components/photo-gallery/photo-gallery.component';

@NgModule({
  declarations: [
    PersonIndexComponent,
    PersonTableComponent,
    PersonSearchComponent,
    PersonDetailsComponent,
    PersonTimelineComponent,
    RelationshipVisualizerComponent,
    PhotoGalleryComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatChipsModule,
    MatCheckboxModule,
    MatTooltipModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatDividerModule,
    MatProgressSpinnerModule,
    MatTabsModule,
    MatMenuModule
  ],
  exports: [
    PersonIndexComponent,
    PersonTableComponent,
    PersonSearchComponent,
    PersonDetailsComponent,
    PersonTimelineComponent,
    RelationshipVisualizerComponent,
    PhotoGalleryComponent
  ]
})
export class PersonModule { }
