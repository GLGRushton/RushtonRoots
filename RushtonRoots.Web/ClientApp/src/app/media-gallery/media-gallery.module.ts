import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

// Angular Material Modules
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatMenuModule } from '@angular/material/menu';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDividerModule } from '@angular/material/divider';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatSliderModule } from '@angular/material/slider';
import { MatBadgeModule } from '@angular/material/badge';

// Components
import { MediaGalleryComponent } from './components/media-gallery/media-gallery.component';
import { PhotoLightboxComponent } from './components/photo-lightbox/photo-lightbox.component';
import { PhotoTaggingComponent } from './components/photo-tagging/photo-tagging.component';
import { AlbumManagerComponent } from './components/album-manager/album-manager.component';
import { PhotoUploadComponent } from './components/photo-upload/photo-upload.component';
import { PhotoEditorComponent } from './components/photo-editor/photo-editor.component';
import { VideoPlayerComponent } from './components/video-player/video-player.component';

@NgModule({
  declarations: [
    MediaGalleryComponent,
    PhotoLightboxComponent,
    PhotoTaggingComponent,
    AlbumManagerComponent,
    PhotoUploadComponent,
    PhotoEditorComponent,
    VideoPlayerComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    // Material Modules
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    MatMenuModule,
    MatChipsModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatDividerModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatAutocompleteModule,
    MatSliderModule,
    MatBadgeModule
  ],
  exports: [
    MediaGalleryComponent,
    PhotoLightboxComponent,
    PhotoTaggingComponent,
    AlbumManagerComponent,
    PhotoUploadComponent,
    PhotoEditorComponent,
    VideoPlayerComponent
  ]
})
export class MediaGalleryModule { }
