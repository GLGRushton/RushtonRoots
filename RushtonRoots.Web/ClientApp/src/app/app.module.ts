import { NgModule, Injector } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { createCustomElement } from '@angular/elements';

import { AppComponent } from './app.component';
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

// Import Phase 4.1 Household Index & Cards components for Angular Elements registration
import { HouseholdIndexComponent } from './household/components/household-index/household-index.component';
import { HouseholdCardComponent } from './household/components/household-card/household-card.component';

// Import Phase 4.2 Household Details & Members components for Angular Elements registration
import { HouseholdDetailsComponent } from './household/components/household-details/household-details.component';
import { HouseholdMembersComponent } from './household/components/household-members/household-members.component';
import { MemberInviteDialogComponent } from './household/components/member-invite-dialog/member-invite-dialog.component';
import { HouseholdSettingsComponent } from './household/components/household-settings/household-settings.component';
import { HouseholdActivityTimelineComponent } from './household/components/household-activity-timeline/household-activity-timeline.component';

// Import Phase 5.1 Partnership Management components for Angular Elements registration
import { PartnershipIndexComponent } from './partnership/components/partnership-index/partnership-index.component';
import { PartnershipCardComponent } from './partnership/components/partnership-card/partnership-card.component';
import { PartnershipFormComponent } from './partnership/components/partnership-form/partnership-form.component';
import { PartnershipTimelineComponent } from './partnership/components/partnership-timeline/partnership-timeline.component';

// Import Phase 5.2 Parent-Child Relationships components for Angular Elements registration
import { ParentChildIndexComponent } from './parent-child/components/parent-child-index/parent-child-index.component';
import { ParentChildCardComponent } from './parent-child/components/parent-child-card/parent-child-card.component';
import { ParentChildFormComponent } from './parent-child/components/parent-child-form/parent-child-form.component';
import { FamilyTreeMiniComponent } from './parent-child/components/family-tree-mini/family-tree-mini.component';
import { RelationshipValidationComponent } from './parent-child/components/relationship-validation/relationship-validation.component';
import { RelationshipSuggestionsComponent } from './parent-child/components/relationship-suggestions/relationship-suggestions.component';
import { BulkRelationshipImportComponent } from './parent-child/components/bulk-relationship-import/bulk-relationship-import.component';

// Import Phase 6.1 Login & Registration components for Angular Elements registration
import { LoginComponent } from './auth/components/login/login.component';
import { ForgotPasswordComponent } from './auth/components/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './auth/components/reset-password/reset-password.component';

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
import { StoryCardComponent } from './content/components/story-card/story-card.component';
import { TraditionCardComponent } from './content/components/tradition-card/tradition-card.component';
import { ContentGridComponent } from './content/components/content-grid/content-grid.component';

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
    SharedModule,
    PersonModule,
    HouseholdModule,
    PartnershipModule,
    ParentChildModule,
    AuthModule,
    WikiModule,
    ContentModule
  ],
  providers: []
})
export class AppModule {
  constructor(private injector: Injector) {
    // Register Angular Elements for use in Razor views
    const welcomeElement = createCustomElement(WelcomeComponent, { injector: this.injector });
    customElements.define('app-welcome', welcomeElement);

    const familyTreeElement = createCustomElement(FamilyTreeComponent, { injector: this.injector });
    customElements.define('app-family-tree', familyTreeElement);

    const styleGuideElement = createCustomElement(StyleGuideComponent, { injector: this.injector });
    customElements.define('app-style-guide', styleGuideElement);

    // Register Phase 1.2 core reusable components as Angular Elements
    const personCardElement = createCustomElement(PersonCardComponent, { injector: this.injector });
    customElements.define('app-person-card', personCardElement);

    const personListElement = createCustomElement(PersonListComponent, { injector: this.injector });
    customElements.define('app-person-list', personListElement);

    const searchBarElement = createCustomElement(SearchBarComponent, { injector: this.injector });
    customElements.define('app-search-bar', searchBarElement);

    const pageHeaderElement = createCustomElement(PageHeaderComponent, { injector: this.injector });
    customElements.define('app-page-header', pageHeaderElement);

    const emptyStateElement = createCustomElement(EmptyStateComponent, { injector: this.injector });
    customElements.define('app-empty-state', emptyStateElement);

    const loadingSpinnerElement = createCustomElement(LoadingSpinnerComponent, { injector: this.injector });
    customElements.define('app-loading-spinner', loadingSpinnerElement);

    const breadcrumbElement = createCustomElement(BreadcrumbComponent, { injector: this.injector });
    customElements.define('app-breadcrumb', breadcrumbElement);

    // Register Phase 2.1 Header & Navigation components as Angular Elements
    const headerElement = createCustomElement(HeaderComponent, { injector: this.injector });
    customElements.define('app-header', headerElement);

    const navigationElement = createCustomElement(NavigationComponent, { injector: this.injector });
    customElements.define('app-navigation', navigationElement);

    const userMenuElement = createCustomElement(UserMenuComponent, { injector: this.injector });
    customElements.define('app-user-menu', userMenuElement);

    // Register Phase 2.2 Footer & Page Layout components as Angular Elements
    const footerElement = createCustomElement(FooterComponent, { injector: this.injector });
    customElements.define('app-footer', footerElement);

    const pageLayoutElement = createCustomElement(PageLayoutComponent, { injector: this.injector });
    customElements.define('app-page-layout', pageLayoutElement);

    // Register Phase 3.1 Person Index & Search components as Angular Elements
    const personIndexElement = createCustomElement(PersonIndexComponent, { injector: this.injector });
    customElements.define('app-person-index', personIndexElement);

    const personTableElement = createCustomElement(PersonTableComponent, { injector: this.injector });
    customElements.define('app-person-table', personTableElement);

    const personSearchElement = createCustomElement(PersonSearchComponent, { injector: this.injector });
    customElements.define('app-person-search', personSearchElement);

    // Register Phase 3.2 Person Details & Timeline components as Angular Elements
    const personDetailsElement = createCustomElement(PersonDetailsComponent, { injector: this.injector });
    customElements.define('app-person-details', personDetailsElement);

    const personTimelineElement = createCustomElement(PersonTimelineComponent, { injector: this.injector });
    customElements.define('app-person-timeline', personTimelineElement);

    const relationshipVisualizerElement = createCustomElement(RelationshipVisualizerComponent, { injector: this.injector });
    customElements.define('app-relationship-visualizer', relationshipVisualizerElement);

    const photoGalleryElement = createCustomElement(PhotoGalleryComponent, { injector: this.injector });
    customElements.define('app-photo-gallery', photoGalleryElement);

    // Register Phase 3.3 Person Create & Edit Forms components as Angular Elements
    const personFormElement = createCustomElement(PersonFormComponent, { injector: this.injector });
    customElements.define('app-person-form', personFormElement);

    const datePickerElement = createCustomElement(DatePickerComponent, { injector: this.injector });
    customElements.define('app-date-picker', datePickerElement);

    const locationAutocompleteElement = createCustomElement(LocationAutocompleteComponent, { injector: this.injector });
    customElements.define('app-location-autocomplete', locationAutocompleteElement);

    // Register Phase 4.1 Household Index & Cards components as Angular Elements
    const householdIndexElement = createCustomElement(HouseholdIndexComponent, { injector: this.injector });
    customElements.define('app-household-index', householdIndexElement);

    const householdCardElement = createCustomElement(HouseholdCardComponent, { injector: this.injector });
    customElements.define('app-household-card', householdCardElement);

    // Register Phase 4.2 Household Details & Members components as Angular Elements
    const householdDetailsElement = createCustomElement(HouseholdDetailsComponent, { injector: this.injector });
    customElements.define('app-household-details', householdDetailsElement);

    const householdMembersElement = createCustomElement(HouseholdMembersComponent, { injector: this.injector });
    customElements.define('app-household-members', householdMembersElement);

    const memberInviteDialogElement = createCustomElement(MemberInviteDialogComponent, { injector: this.injector });
    customElements.define('app-member-invite-dialog', memberInviteDialogElement);

    const householdSettingsElement = createCustomElement(HouseholdSettingsComponent, { injector: this.injector });
    customElements.define('app-household-settings', householdSettingsElement);

    const householdActivityTimelineElement = createCustomElement(HouseholdActivityTimelineComponent, { injector: this.injector });
    customElements.define('app-household-activity-timeline', householdActivityTimelineElement);

    // Register Phase 5.1 Partnership Management components as Angular Elements
    const partnershipIndexElement = createCustomElement(PartnershipIndexComponent, { injector: this.injector });
    customElements.define('app-partnership-index', partnershipIndexElement);

    const partnershipCardElement = createCustomElement(PartnershipCardComponent, { injector: this.injector });
    customElements.define('app-partnership-card', partnershipCardElement);

    const partnershipFormElement = createCustomElement(PartnershipFormComponent, { injector: this.injector });
    customElements.define('app-partnership-form', partnershipFormElement);

    const partnershipTimelineElement = createCustomElement(PartnershipTimelineComponent, { injector: this.injector });
    customElements.define('app-partnership-timeline', partnershipTimelineElement);

    // Register Phase 5.2 Parent-Child Relationships components as Angular Elements
    const parentChildIndexElement = createCustomElement(ParentChildIndexComponent, { injector: this.injector });
    customElements.define('app-parent-child-index', parentChildIndexElement);

    const parentChildCardElement = createCustomElement(ParentChildCardComponent, { injector: this.injector });
    customElements.define('app-parent-child-card', parentChildCardElement);

    const parentChildFormElement = createCustomElement(ParentChildFormComponent, { injector: this.injector });
    customElements.define('app-parent-child-form', parentChildFormElement);

    const familyTreeMiniElement = createCustomElement(FamilyTreeMiniComponent, { injector: this.injector });
    customElements.define('app-family-tree-mini', familyTreeMiniElement);

    const relationshipValidationElement = createCustomElement(RelationshipValidationComponent, { injector: this.injector });
    customElements.define('app-relationship-validation', relationshipValidationElement);

    const relationshipSuggestionsElement = createCustomElement(RelationshipSuggestionsComponent, { injector: this.injector });
    customElements.define('app-relationship-suggestions', relationshipSuggestionsElement);

    const bulkRelationshipImportElement = createCustomElement(BulkRelationshipImportComponent, { injector: this.injector });
    customElements.define('app-bulk-relationship-import', bulkRelationshipImportElement);

    // Register Phase 6.1 Login & Registration components as Angular Elements
    const loginElement = createCustomElement(LoginComponent, { injector: this.injector });
    customElements.define('app-login', loginElement);

    const forgotPasswordElement = createCustomElement(ForgotPasswordComponent, { injector: this.injector });
    customElements.define('app-forgot-password', forgotPasswordElement);

    const resetPasswordElement = createCustomElement(ResetPasswordComponent, { injector: this.injector });
    customElements.define('app-reset-password', resetPasswordElement);

    // Register Phase 6.2 User Profile & Settings components as Angular Elements
    const userProfileElement = createCustomElement(UserProfileComponent, { injector: this.injector });
    customElements.define('app-user-profile', userProfileElement);

    const notificationPreferencesElement = createCustomElement(NotificationPreferencesComponent, { injector: this.injector });
    customElements.define('app-notification-preferences', notificationPreferencesElement);

    const privacySettingsElement = createCustomElement(PrivacySettingsComponent, { injector: this.injector });
    customElements.define('app-privacy-settings', privacySettingsElement);

    const connectedAccountsElement = createCustomElement(ConnectedAccountsComponent, { injector: this.injector });
    customElements.define('app-connected-accounts', connectedAccountsElement);

    const accountDeletionElement = createCustomElement(AccountDeletionComponent, { injector: this.injector });
    customElements.define('app-account-deletion', accountDeletionElement);

    // Register Phase 7.1 Wiki & Knowledge Base components as Angular Elements
    const wikiIndexElement = createCustomElement(WikiIndexComponent, { injector: this.injector });
    customElements.define('app-wiki-index', wikiIndexElement);

    const wikiArticleElement = createCustomElement(WikiArticleComponent, { injector: this.injector });
    customElements.define('app-wiki-article', wikiArticleElement);

    const markdownEditorElement = createCustomElement(MarkdownEditorComponent, { injector: this.injector });
    customElements.define('app-markdown-editor', markdownEditorElement);

    // Register Phase 7.2 Recipes, Stories, & Traditions components as Angular Elements
    const recipeCardElement = createCustomElement(RecipeCardComponent, { injector: this.injector });
    customElements.define('app-recipe-card', recipeCardElement);

    const recipeDetailsElement = createCustomElement(RecipeDetailsComponent, { injector: this.injector });
    customElements.define('app-recipe-details', recipeDetailsElement);

    const storyCardElement = createCustomElement(StoryCardComponent, { injector: this.injector });
    customElements.define('app-story-card', storyCardElement);

    const traditionCardElement = createCustomElement(TraditionCardComponent, { injector: this.injector });
    customElements.define('app-tradition-card', traditionCardElement);

    const contentGridElement = createCustomElement(ContentGridComponent, { injector: this.injector });
    customElements.define('app-content-grid', contentGridElement);
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
