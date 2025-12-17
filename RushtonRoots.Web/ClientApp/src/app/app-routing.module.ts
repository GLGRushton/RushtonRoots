import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NotFoundComponent } from './shared/components/not-found/not-found.component';

/**
 * Main application routing configuration
 * 
 * NOTE: This application currently uses Angular Elements embedded in Razor views,
 * not a full SPA. This routing module is configured to support a future migration
 * to a full SPA architecture while maintaining backward compatibility.
 * 
 * For now, these routes will be available but may not be the primary navigation method.
 * The application still relies on ASP.NET Core MVC routing for most navigation.
 */
const routes: Routes = [
  // Home routes
  {
    path: '',
    redirectTo: '/home',
    pathMatch: 'full'
  },
  {
    path: 'home',
    loadChildren: () => import('./home/home.module').then(m => m.HomeModule)
  },

  // Account routes
  {
    path: 'account',
    loadChildren: () => import('./auth/auth.module').then(m => m.AuthModule)
  },

  // Person routes (lazy loaded)
  {
    path: 'people',
    loadChildren: () => import('./person/person.module').then(m => m.PersonModule)
  },

  // Household routes (lazy loaded)
  {
    path: 'households',
    loadChildren: () => import('./household/household.module').then(m => m.HouseholdModule)
  },

  // Partnership routes (lazy loaded)
  {
    path: 'partnerships',
    loadChildren: () => import('./partnership/partnership.module').then(m => m.PartnershipModule)
  },

  // Parent-Child relationship routes (lazy loaded)
  {
    path: 'relationships',
    loadChildren: () => import('./parent-child/parent-child.module').then(m => m.ParentChildModule)
  },

  // Wiki routes (lazy loaded)
  {
    path: 'wiki',
    loadChildren: () => import('./wiki/wiki.module').then(m => m.WikiModule)
  },

  // Recipe routes (lazy loaded)
  // NOTE: ContentModule handles recipes, stories, and traditions
  // Child routes within ContentModule differentiate between content types
  {
    path: 'recipes',
    loadChildren: () => import('./content/content.module').then(m => m.ContentModule)
  },

  // Story routes (lazy loaded)
  // NOTE: ContentModule handles recipes, stories, and traditions
  // Child routes within ContentModule differentiate between content types
  {
    path: 'stories',
    loadChildren: () => import('./content/content.module').then(m => m.ContentModule)
  },

  // Tradition routes (lazy loaded)
  // NOTE: ContentModule handles recipes, stories, and traditions
  // Child routes within ContentModule differentiate between content types
  {
    path: 'traditions',
    loadChildren: () => import('./content/content.module').then(m => m.ContentModule)
  },

  // Calendar routes (lazy loaded)
  {
    path: 'calendar',
    loadChildren: () => import('./calendar/calendar.module').then(m => m.CalendarModule)
  },
  {
    path: 'events',
    loadChildren: () => import('./calendar/calendar.module').then(m => m.CalendarModule)
  },

  // Media gallery routes (lazy loaded)
  {
    path: 'gallery',
    loadChildren: () => import('./media-gallery/media-gallery.module').then(m => m.MediaGalleryModule)
  },
  {
    path: 'media',
    loadChildren: () => import('./media-gallery/media-gallery.module').then(m => m.MediaGalleryModule)
  },

  // 404 Not Found page
  {
    path: 'not-found',
    component: NotFoundComponent
  },

  // Fallback - redirect to 404 Not Found
  {
    path: '**',
    redirectTo: '/not-found'
  }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, {
      // Enable router tracing in development
      enableTracing: false,
      // Use hash-based routing for compatibility with ASP.NET Core MVC
      useHash: true,
      // Scroll to top on navigation
      scrollPositionRestoration: 'top',
      // Anchor scrolling for hash fragments
      anchorScrolling: 'enabled'
    })
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
