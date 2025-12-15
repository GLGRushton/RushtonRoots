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
    SharedModule
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
