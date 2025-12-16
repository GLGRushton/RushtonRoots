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
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatMenuModule } from '@angular/material/menu';
import { MatBadgeModule } from '@angular/material/badge';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatListModule } from '@angular/material/list';

// Components
import { WikiIndexComponent } from './components/wiki-index/wiki-index.component';
import { WikiArticleComponent } from './components/wiki-article/wiki-article.component';
import { MarkdownEditorComponent } from './components/markdown-editor/markdown-editor.component';

/**
 * WikiModule - Knowledge base and wiki functionality
 * 
 * This module contains all wiki-related components for Phase 7.1:
 * - WikiIndexComponent: Main wiki navigation and article listing
 * - WikiArticleComponent: Article display with table of contents
 * - MarkdownEditorComponent: Full-featured markdown editor with preview
 * 
 * All components are built with Angular Material and follow the design system.
 */
@NgModule({
  declarations: [
    WikiIndexComponent,
    WikiArticleComponent,
    MarkdownEditorComponent
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
    MatChipsModule,
    MatTooltipModule,
    MatDividerModule,
    MatProgressSpinnerModule,
    MatMenuModule,
    MatBadgeModule,
    MatExpansionModule,
    MatListModule
  ],
  exports: [
    WikiIndexComponent,
    WikiArticleComponent,
    MarkdownEditorComponent
  ]
})
export class WikiModule { }
