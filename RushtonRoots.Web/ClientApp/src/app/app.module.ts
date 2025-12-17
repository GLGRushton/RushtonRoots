import { NgModule, Injector } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { createCustomElement } from '@angular/elements';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { WelcomeComponent } from './welcome/welcome.component';
import { FamilyTreeComponent } from './family-tree/family-tree.component';
import { StyleGuideComponent } from './style-guide/style-guide.component';
import { SharedModule } from './shared/shared.module';
import { PersonModule } from './person/person.module';
import { HouseholdModule } from './household/household.module';
import { PartnershipModule } from './partnership/partnership.module';
import { ParentChildModule } from './parent-child/parent-child.module';
import { AuthModule } from './auth/auth.module';
import { WikiModule } from './wiki/wiki.module';
import { ContentModule } from './content/content.module';
import { MediaGalleryModule } from './media-gallery/media-gallery.module';
import { CalendarModule } from './calendar/calendar.module';
import { MessagingModule } from './messaging/messaging.module';
import { PwaModule } from './pwa/pwa.module';
import { AccessibilityModule } from './accessibility/accessibility.module';
import { HomeModule } from './home/home.module';

// Import Phase 6.1 Home Page component for Angular Elements registration
import { HomePageComponent } from './home/components/home-page/home-page.component';

// Import core reusable components for Angular Elements registration
import { PersonCardComponent } from './shared/components/person-card/person-card.component';
import { PersonListComponent } from './shared/components/person-list/person-list.component';
import { SearchBarComponent } from './shared/components/search-bar/search-bar.component';
import { PageHeaderComponent } from './shared/components/page-header/page-header.component';
import { EmptyStateComponent } from './shared/components/empty-state/empty-state.component';
import { LoadingSpinnerComponent } from './shared/components/loading-spinner/loading-spinner.component';
import { BreadcrumbComponent } from './shared/components/breadcrumb/breadcrumb.component';
import { HeaderComponent } from './shared/components/header/header.component';
import { NavigationComponent } from './shared/components/navigation/navigation.component';
import { UserMenuComponent } from './shared/components/user-menu/user-menu.component';
import { FooterComponent } from './shared/components/footer/footer.component';
import { PageLayoutComponent } from './shared/components/page-layout/page-layout.component';
import { LayoutWrapperComponent } from './shared/components/layout-wrapper/layout-wrapper.component';

// Import Phase 3.1 Person Index & Search components for Angular Elements registration
import { PersonIndexComponent } from './person/components/person-index/person-index.component';
import { PersonTableComponent } from './person/components/person-table/person-table.component';
import { PersonSearchComponent } from './person/components/person-search/person-search.component';

// Import Phase 3.2 Person Details & Timeline components for Angular Elements registration
import { PersonDetailsComponent } from './person/components/person-details/person-details.component';
import { PersonTimelineComponent } from './person/components/person-timeline/person-timeline.component';
import { RelationshipVisualizerComponent } from './person/components/relationship-visualizer/relationship-visualizer.component';
import { PhotoGalleryComponent } from './person/components/photo-gallery/photo-gallery.component';

// Import Phase 3.3 Person Create & Edit Forms components for Angular Elements registration
import { PersonFormComponent } from './person/components/person-form/person-form.component';
import { DatePickerComponent } from './person/components/date-picker/date-picker.component';
import { LocationAutocompleteComponent } from './person/components/location-autocomplete/location-autocomplete.component';

// Import Phase 2.4 Person Delete Dialog component for Angular Elements registration
import { PersonDeleteDialogComponent } from './person/components/person-delete-dialog/person-delete-dialog.component';

// Import Phase 4.1 Household Index & Cards components for Angular Elements registration
import { HouseholdIndexComponent } from './household/components/household-index/household-index.component';
import { HouseholdCardComponent } from './household/components/household-card/household-card.component';

// Import Phase 4.2 Household Details & Members components for Angular Elements registration
import { HouseholdDetailsComponent } from './household/components/household-details/household-details.component';
import { HouseholdMembersComponent } from './household/components/household-members/household-members.component';
import { MemberInviteDialogComponent } from './household/components/member-invite-dialog/member-invite-dialog.component';
import { HouseholdSettingsComponent } from './household/components/household-settings/household-settings.component';
import { HouseholdActivityTimelineComponent } from './household/components/household-activity-timeline/household-activity-timeline.component';

// Import Phase 3.3 (UpdateDesigns.md) Household Form component for Angular Elements registration
import { HouseholdFormComponent } from './household/components/household-form/household-form.component';

// Import Phase 3.4 (UpdateDesigns.md) Household Delete Dialog component for Angular Elements registration
import { HouseholdDeleteDialogComponent } from './household/components/household-delete-dialog/household-delete-dialog.component';

// Import Phase 5.1 Partnership Management components for Angular Elements registration
import { PartnershipIndexComponent } from './partnership/components/partnership-index/partnership-index.component';
import { PartnershipCardComponent } from './partnership/components/partnership-card/partnership-card.component';
import { PartnershipFormComponent } from './partnership/components/partnership-form/partnership-form.component';
import { PartnershipTimelineComponent } from './partnership/components/partnership-timeline/partnership-timeline.component';

// Import Phase 4.2 Partnership Details component for Angular Elements registration
import { PartnershipDetailsComponent } from './partnership/components/partnership-details/partnership-details.component';

// Import Phase 4.4 Partnership Delete Dialog component for Angular Elements registration
import { PartnershipDeleteDialogComponent } from './partnership/components/partnership-delete-dialog/partnership-delete-dialog.component';

// Import Phase 5.2 Parent-Child Relationships components for Angular Elements registration
import { ParentChildIndexComponent } from './parent-child/components/parent-child-index/parent-child-index.component';
import { ParentChildCardComponent } from './parent-child/components/parent-child-card/parent-child-card.component';
import { ParentChildFormComponent } from './parent-child/components/parent-child-form/parent-child-form.component';
import { ParentChildDetailsComponent } from './parent-child/components/parent-child-details/parent-child-details.component';
import { FamilyTreeMiniComponent } from './parent-child/components/family-tree-mini/family-tree-mini.component';
import { RelationshipValidationComponent } from './parent-child/components/relationship-validation/relationship-validation.component';
import { RelationshipSuggestionsComponent } from './parent-child/components/relationship-suggestions/relationship-suggestions.component';

// Import Phase 5.4 Parent-Child Delete Dialog component
import { ParentChildDeleteDialogComponent } from './parent-child/components/parent-child-delete-dialog/parent-child-delete-dialog.component';
import { BulkRelationshipImportComponent } from './parent-child/components/bulk-relationship-import/bulk-relationship-import.component';

// Import Phase 6.1 Login & Registration components for Angular Elements registration
import { LoginComponent } from './auth/components/login/login.component';
import { ForgotPasswordComponent } from './auth/components/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './auth/components/reset-password/reset-password.component';

// Import Phase 1.2 Password Confirmation and Email Verification components for Angular Elements registration
import { ForgotPasswordConfirmationComponent } from './auth/components/forgot-password-confirmation/forgot-password-confirmation.component';
import { ResetPasswordConfirmationComponent } from './auth/components/reset-password-confirmation/reset-password-confirmation.component';
import { ConfirmEmailComponent } from './auth/components/confirm-email/confirm-email.component';

// Import Phase 1.3 User Management components for Angular Elements registration
import { CreateUserComponent } from './auth/components/create-user/create-user.component';

// Import Phase 1.4 Access Control components for Angular Elements registration
import { AccessDeniedComponent } from './auth/components/access-denied/access-denied.component';

// Import Phase 6.2 User Profile & Settings components for Angular Elements registration
import { UserProfileComponent } from './auth/components/user-profile/user-profile.component';
import { NotificationPreferencesComponent } from './auth/components/notification-preferences/notification-preferences.component';
import { PrivacySettingsComponent } from './auth/components/privacy-settings/privacy-settings.component';
import { ConnectedAccountsComponent } from './auth/components/connected-accounts/connected-accounts.component';
import { AccountDeletionComponent } from './auth/components/account-deletion/account-deletion.component';

// Import Phase 7.1 Wiki & Knowledge Base components for Angular Elements registration
import { WikiIndexComponent } from './wiki/components/wiki-index/wiki-index.component';
import { WikiArticleComponent } from './wiki/components/wiki-article/wiki-article.component';
import { MarkdownEditorComponent } from './wiki/components/markdown-editor/markdown-editor.component';

// Import Phase 7.2 Recipes, Stories, & Traditions components for Angular Elements registration
import { RecipeCardComponent } from './content/components/recipe-card/recipe-card.component';
import { RecipeDetailsComponent } from './content/components/recipe-details/recipe-details.component';
import { RecipeIndexComponent } from './content/components/recipe-index/recipe-index.component';
import { StoryCardComponent } from './content/components/story-card/story-card.component';
import { StoryDetailsComponent } from './content/components/story-details/story-details.component';
import { StoryIndexComponent } from './content/components/story-index/story-index.component';
import { TraditionCardComponent } from './content/components/tradition-card/tradition-card.component';
import { TraditionDetailsComponent } from './content/components/tradition-details/tradition-details.component';
import { TraditionIndexComponent } from './content/components/tradition-index/tradition-index.component';
import { ContentGridComponent } from './content/components/content-grid/content-grid.component';

// Import Phase 8.1 Media Gallery Enhancements components for Angular Elements registration
import { MediaGalleryComponent } from './media-gallery/components/media-gallery/media-gallery.component';
import { PhotoLightboxComponent } from './media-gallery/components/photo-lightbox/photo-lightbox.component';
import { PhotoTaggingComponent } from './media-gallery/components/photo-tagging/photo-tagging.component';
import { AlbumManagerComponent } from './media-gallery/components/album-manager/album-manager.component';
import { PhotoUploadComponent } from './media-gallery/components/photo-upload/photo-upload.component';
import { PhotoEditorComponent } from './media-gallery/components/photo-editor/photo-editor.component';
import { VideoPlayerComponent } from './media-gallery/components/video-player/video-player.component';

// Import Phase 8.2 Calendar & Events components for Angular Elements registration
import { CalendarComponent } from './calendar/components/calendar/calendar.component';
import { EventCardComponent } from './calendar/components/event-card/event-card.component';
import { EventFormDialogComponent } from './calendar/components/event-form-dialog/event-form-dialog.component';
import { EventRsvpDialogComponent } from './calendar/components/event-rsvp-dialog/event-rsvp-dialog.component';
import { EventDetailsDialogComponent } from './calendar/components/event-details-dialog/event-details-dialog.component';

// Import Phase 8.3 Messaging & Notifications components for Angular Elements registration
import { MessageThreadComponent } from './messaging/components/message-thread/message-thread.component';
import { ChatInterfaceComponent } from './messaging/components/chat-interface/chat-interface.component';
import { NotificationPanelComponent } from './messaging/components/notification-panel/notification-panel.component';
import { MessageCompositionDialogComponent } from './messaging/components/message-composition-dialog/message-composition-dialog.component';

// Import Phase 10.1 Accessibility components for Angular Elements registration
import { SkipNavigationComponent } from './accessibility/components/skip-navigation/skip-navigation.component';
import { KeyboardShortcutsDialogComponent } from './accessibility/components/keyboard-shortcuts-dialog/keyboard-shortcuts-dialog.component';
import { AccessibilityStatementComponent } from './accessibility/components/accessibility-statement/accessibility-statement.component';

@NgModule({
  declarations: [
    AppComponent,
    WelcomeComponent,
    FamilyTreeComponent,
    StyleGuideComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    AppRoutingModule,
    SharedModule,
    PersonModule,
    HouseholdModule,
    PartnershipModule,
    ParentChildModule,
    AuthModule,
    WikiModule,
    ContentModule,
    MediaGalleryModule,
    CalendarModule,
    MessagingModule,
    PwaModule,
    AccessibilityModule,
    HomeModule
  ],
  providers: []
})
export class AppModule {
  constructor(private injector: Injector) {
    // Helper function to safely define custom elements
    const safeDefine = (name: string, component: any) => {
      if (!customElements.get(name)) {
        const element = createCustomElement(component, { injector: this.injector });
        customElements.define(name, element);
      }
    };

    // Register Angular Elements for use in Razor views
    safeDefine('app-welcome', WelcomeComponent);
    safeDefine('app-family-tree', FamilyTreeComponent);
    safeDefine('app-style-guide', StyleGuideComponent);

    // Register Phase 6.1 Home Page component as Angular Element
    safeDefine('app-home-page', HomePageComponent);

    // Register Phase 1.2 core reusable components as Angular Elements
    safeDefine('app-person-card', PersonCardComponent);
    safeDefine('app-person-list', PersonListComponent);
    safeDefine('app-search-bar', SearchBarComponent);
    safeDefine('app-page-header', PageHeaderComponent);
    safeDefine('app-empty-state', EmptyStateComponent);
    safeDefine('app-loading-spinner', LoadingSpinnerComponent);
    safeDefine('app-breadcrumb', BreadcrumbComponent);

    // Register Phase 2.1 Header & Navigation components as Angular Elements
    safeDefine('app-header', HeaderComponent);
    safeDefine('app-navigation', NavigationComponent);
    safeDefine('app-user-menu', UserMenuComponent);

    // Register Phase 2.2 Footer & Page Layout components as Angular Elements
    safeDefine('app-footer', FooterComponent);
    safeDefine('app-page-layout', PageLayoutComponent);

    // Register Phase 11.1 Layout Wrapper component as Angular Element
    safeDefine('app-layout-wrapper', LayoutWrapperComponent);

    // Register Phase 3.1 Person Index & Search components as Angular Elements
    safeDefine('app-person-index', PersonIndexComponent);
    safeDefine('app-person-table', PersonTableComponent);
    safeDefine('app-person-search', PersonSearchComponent);

    // Register Phase 3.2 Person Details & Timeline components as Angular Elements
    safeDefine('app-person-details', PersonDetailsComponent);
    safeDefine('app-person-timeline', PersonTimelineComponent);
    safeDefine('app-relationship-visualizer', RelationshipVisualizerComponent);
    safeDefine('app-photo-gallery', PhotoGalleryComponent);

    // Register Phase 3.3 Person Create & Edit Forms components as Angular Elements
    safeDefine('app-person-form', PersonFormComponent);
    safeDefine('app-date-picker', DatePickerComponent);
    safeDefine('app-location-autocomplete', LocationAutocompleteComponent);

    // Register Phase 2.4 Person Delete Dialog component as Angular Element
    safeDefine('app-person-delete-dialog', PersonDeleteDialogComponent);

    // Register Phase 4.1 Household Index & Cards components as Angular Elements
    safeDefine('app-household-index', HouseholdIndexComponent);
    safeDefine('app-household-card', HouseholdCardComponent);

    // Register Phase 4.2 Household Details & Members components as Angular Elements
    safeDefine('app-household-details', HouseholdDetailsComponent);
    safeDefine('app-household-members', HouseholdMembersComponent);
    safeDefine('app-member-invite-dialog', MemberInviteDialogComponent);
    safeDefine('app-household-settings', HouseholdSettingsComponent);
    safeDefine('app-household-activity-timeline', HouseholdActivityTimelineComponent);

    // Register Phase 3.3 (UpdateDesigns.md) Household Form component as Angular Element
    safeDefine('app-household-form', HouseholdFormComponent);

    // Register Phase 3.4 (UpdateDesigns.md) Household Delete Dialog component as Angular Element
    safeDefine('app-household-delete-dialog', HouseholdDeleteDialogComponent);

    // Register Phase 5.1 Partnership Management components as Angular Elements
    safeDefine('app-partnership-index', PartnershipIndexComponent);
    safeDefine('app-partnership-card', PartnershipCardComponent);
    safeDefine('app-partnership-form', PartnershipFormComponent);
    safeDefine('app-partnership-timeline', PartnershipTimelineComponent);

    // Register Phase 4.2 Partnership Details component as Angular Element
    safeDefine('app-partnership-details', PartnershipDetailsComponent);

    // Register Phase 4.4 Partnership Delete Dialog component as Angular Element
    safeDefine('app-partnership-delete-dialog', PartnershipDeleteDialogComponent);

    // Register Phase 5.2 Parent-Child Relationships components as Angular Elements
    safeDefine('app-parent-child-index', ParentChildIndexComponent);
    safeDefine('app-parent-child-card', ParentChildCardComponent);
    safeDefine('app-parent-child-form', ParentChildFormComponent);
    safeDefine('app-parent-child-details', ParentChildDetailsComponent);
    safeDefine('app-family-tree-mini', FamilyTreeMiniComponent);
    safeDefine('app-relationship-validation', RelationshipValidationComponent);
    safeDefine('app-relationship-suggestions', RelationshipSuggestionsComponent);
    
    // Register Phase 5.4 Parent-Child Delete Dialog component as Angular Element
    safeDefine('app-parent-child-delete-dialog', ParentChildDeleteDialogComponent);
    safeDefine('app-bulk-relationship-import', BulkRelationshipImportComponent);

    // Register Phase 6.1 Login & Registration components as Angular Elements
    safeDefine('app-login', LoginComponent);
    safeDefine('app-forgot-password', ForgotPasswordComponent);
    safeDefine('app-reset-password', ResetPasswordComponent);

    // Register Phase 1.2 Password Confirmation and Email Verification components as Angular Elements
    safeDefine('app-forgot-password-confirmation', ForgotPasswordConfirmationComponent);
    safeDefine('app-reset-password-confirmation', ResetPasswordConfirmationComponent);
    safeDefine('app-confirm-email', ConfirmEmailComponent);

    // Register Phase 1.3 User Management components as Angular Elements
    safeDefine('app-create-user', CreateUserComponent);

    // Register Phase 1.4 Access Control components as Angular Elements
    safeDefine('app-access-denied', AccessDeniedComponent);

    // Register Phase 6.2 User Profile & Settings components as Angular Elements
    safeDefine('app-user-profile', UserProfileComponent);
    safeDefine('app-notification-preferences', NotificationPreferencesComponent);
    safeDefine('app-privacy-settings', PrivacySettingsComponent);
    safeDefine('app-connected-accounts', ConnectedAccountsComponent);
    safeDefine('app-account-deletion', AccountDeletionComponent);

    // Register Phase 7.1 Wiki & Knowledge Base components as Angular Elements
    safeDefine('app-wiki-index', WikiIndexComponent);
    safeDefine('app-wiki-article', WikiArticleComponent);
    safeDefine('app-markdown-editor', MarkdownEditorComponent);

    // Register Phase 7.2 Recipes, Stories, & Traditions components as Angular Elements
    safeDefine('app-recipe-card', RecipeCardComponent);
    safeDefine('app-recipe-details', RecipeDetailsComponent);
    safeDefine('app-recipe-index', RecipeIndexComponent);
    safeDefine('app-story-card', StoryCardComponent);
    safeDefine('app-story-details', StoryDetailsComponent);
    safeDefine('app-story-index', StoryIndexComponent);
    safeDefine('app-tradition-card', TraditionCardComponent);
    safeDefine('app-tradition-details', TraditionDetailsComponent);
    safeDefine('app-tradition-index', TraditionIndexComponent);
    safeDefine('app-content-grid', ContentGridComponent);

    // Register Phase 8.1 Media Gallery Enhancements components as Angular Elements
    safeDefine('app-media-gallery', MediaGalleryComponent);
    safeDefine('app-photo-lightbox', PhotoLightboxComponent);
    safeDefine('app-photo-tagging', PhotoTaggingComponent);
    safeDefine('app-album-manager', AlbumManagerComponent);
    safeDefine('app-photo-upload', PhotoUploadComponent);
    safeDefine('app-photo-editor', PhotoEditorComponent);
    safeDefine('app-video-player', VideoPlayerComponent);

    // Register Phase 8.2 Calendar & Events components as Angular Elements
    safeDefine('app-calendar', CalendarComponent);
    safeDefine('app-event-card', EventCardComponent);
    safeDefine('app-event-form-dialog', EventFormDialogComponent);
    safeDefine('app-event-rsvp-dialog', EventRsvpDialogComponent);
    safeDefine('app-event-details-dialog', EventDetailsDialogComponent);

    // Register Phase 8.3 Messaging & Notifications components as Angular Elements
    safeDefine('app-message-thread', MessageThreadComponent);
    safeDefine('app-chat-interface', ChatInterfaceComponent);
    safeDefine('app-notification-panel', NotificationPanelComponent);
    safeDefine('app-message-composition-dialog', MessageCompositionDialogComponent);

    // Register Phase 10.1 Accessibility components as Angular Elements
    safeDefine('app-skip-navigation', SkipNavigationComponent);
    safeDefine('app-keyboard-shortcuts-dialog', KeyboardShortcutsDialogComponent);
    safeDefine('app-accessibility-statement', AccessibilityStatementComponent);
  }

  ngDoBootstrap() {
    // Bootstrap AppComponent if present in the page
    const appRoot = document.querySelector('app-root');
    if (appRoot) {
      import('@angular/platform-browser-dynamic').then(m => {
        m.platformBrowserDynamic().bootstrapModule(AppModule);
      });
    }
  }
}
