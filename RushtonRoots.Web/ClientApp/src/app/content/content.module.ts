import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

// Material Design Modules
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatMenuModule } from '@angular/material/menu';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTooltipModule } from '@angular/material/tooltip';

// Components
import { RecipeCardComponent } from './components/recipe-card/recipe-card.component';
import { RecipeDetailsComponent } from './components/recipe-details/recipe-details.component';
import { RecipeIndexComponent } from './components/recipe-index/recipe-index.component';
import { StoryCardComponent } from './components/story-card/story-card.component';
import { StoryDetailsComponent } from './components/story-details/story-details.component';
import { StoryIndexComponent } from './components/story-index/story-index.component';
import { TraditionCardComponent } from './components/tradition-card/tradition-card.component';
import { ContentGridComponent } from './components/content-grid/content-grid.component';

// Shared Module for BreadcrumbComponent
import { SharedModule } from '../shared/shared.module';

/**
 * ContentModule
 * Module for recipes, stories, and traditions functionality
 * Phase 7.2 of UI Design Plan
 */
@NgModule({
  declarations: [
    RecipeCardComponent,
    RecipeDetailsComponent,
    RecipeIndexComponent,
    StoryCardComponent,
    StoryDetailsComponent,
    StoryIndexComponent,
    TraditionCardComponent,
    ContentGridComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatMenuModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    MatDividerModule,
    MatProgressSpinnerModule,
    MatTabsModule,
    MatTooltipModule
  ],
  exports: [
    RecipeCardComponent,
    RecipeDetailsComponent,
    RecipeIndexComponent,
    StoryCardComponent,
    StoryDetailsComponent,
    StoryIndexComponent,
    TraditionCardComponent,
    ContentGridComponent
  ]
})
export class ContentModule { }
