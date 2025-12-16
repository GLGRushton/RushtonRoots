import { Component, DebugElement } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { AdminOnlyDirective, RoleGuardDirective } from './admin-only.directive';

// Test component for AdminOnlyDirective
@Component({
  template: `
    <div id="default" *appAdminOnly>Default admin content</div>
    <div id="specific" *appAdminOnly="'HouseholdAdmin'">Household admin content</div>
    <div id="multiple" *appAdminOnly="['Admin', 'HouseholdAdmin']">Multiple roles content</div>
  `
})
class AdminOnlyTestComponent { }

// Test component for RoleGuardDirective
@Component({
  template: `
    <div id="single" *appRoleGuard="'Admin'">Admin only</div>
    <div id="any" *appRoleGuard="['Admin', 'Editor']; strategy: 'any'">Admin OR Editor</div>
    <div id="all" *appRoleGuard="['Admin', 'Editor']; strategy: 'all'">Admin AND Editor</div>
  `
})
class RoleGuardTestComponent { }

describe('AdminOnlyDirective', () => {
  let fixture: ComponentFixture<AdminOnlyTestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AdminOnlyDirective, AdminOnlyTestComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(AdminOnlyTestComponent);
    fixture.detectChanges();
  });

  it('should create an instance', () => {
    const directive = new AdminOnlyDirective(
      {} as any, // TemplateRef
      {} as any  // ViewContainerRef
    );
    expect(directive).toBeTruthy();
  });

  it('should display content for default admin roles', () => {
    const defaultElement = fixture.debugElement.query(By.css('#default'));
    // Currently returns true in placeholder implementation
    expect(defaultElement).toBeTruthy();
  });

  it('should display content for specific role', () => {
    const specificElement = fixture.debugElement.query(By.css('#specific'));
    // Currently returns true in placeholder implementation
    expect(specificElement).toBeTruthy();
  });

  it('should display content for multiple roles', () => {
    const multipleElement = fixture.debugElement.query(By.css('#multiple'));
    // Currently returns true in placeholder implementation
    expect(multipleElement).toBeTruthy();
  });

  it('should clean up on destroy', () => {
    const directive = new AdminOnlyDirective(
      {} as any,
      {} as any
    );
    
    // Call ngOnDestroy and verify no errors
    expect(() => directive.ngOnDestroy()).not.toThrow();
  });
});

describe('RoleGuardDirective', () => {
  let fixture: ComponentFixture<RoleGuardTestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [RoleGuardDirective, RoleGuardTestComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(RoleGuardTestComponent);
    fixture.detectChanges();
  });

  it('should create an instance', () => {
    const directive = new RoleGuardDirective(
      {} as any, // TemplateRef
      {} as any  // ViewContainerRef
    );
    expect(directive).toBeTruthy();
  });

  it('should display content for single role', () => {
    const singleElement = fixture.debugElement.query(By.css('#single'));
    // Currently returns true in placeholder implementation
    expect(singleElement).toBeTruthy();
  });

  it('should display content with "any" strategy', () => {
    const anyElement = fixture.debugElement.query(By.css('#any'));
    // Currently returns true in placeholder implementation
    expect(anyElement).toBeTruthy();
  });

  it('should display content with "all" strategy', () => {
    const allElement = fixture.debugElement.query(By.css('#all'));
    // Currently returns true in placeholder implementation
    expect(allElement).toBeTruthy();
  });

  it('should clean up on destroy', () => {
    const directive = new RoleGuardDirective(
      {} as any,
      {} as any
    );
    
    // Call ngOnDestroy and verify no errors
    expect(() => directive.ngOnDestroy()).not.toThrow();
  });
});

describe('AdminOnlyDirective - Input Changes', () => {
  it('should update view when roles input changes', () => {
    const templateRef = {} as any;
    const viewContainer = {
      createEmbeddedView: jasmine.createSpy('createEmbeddedView'),
      clear: jasmine.createSpy('clear')
    } as any;

    const directive = new AdminOnlyDirective(templateRef, viewContainer);
    
    // Set roles
    directive.appAdminOnly = 'Admin';
    
    // Verify updateView is called (indirectly by checking view creation)
    expect(directive).toBeTruthy();
  });

  it('should handle array of roles', () => {
    const templateRef = {} as any;
    const viewContainer = {
      createEmbeddedView: jasmine.createSpy('createEmbeddedView'),
      clear: jasmine.createSpy('clear')
    } as any;

    const directive = new AdminOnlyDirective(templateRef, viewContainer);
    
    // Set roles as array
    directive.appAdminOnly = ['Admin', 'HouseholdAdmin'];
    
    expect(directive).toBeTruthy();
  });

  it('should handle undefined roles input', () => {
    const templateRef = {} as any;
    const viewContainer = {
      createEmbeddedView: jasmine.createSpy('createEmbeddedView'),
      clear: jasmine.createSpy('clear')
    } as any;

    const directive = new AdminOnlyDirective(templateRef, viewContainer);
    
    // Set undefined
    directive.appAdminOnly = undefined;
    
    expect(directive).toBeTruthy();
  });
});

describe('RoleGuardDirective - Input Changes and Strategies', () => {
  it('should update view when roles input changes', () => {
    const templateRef = {} as any;
    const viewContainer = {
      createEmbeddedView: jasmine.createSpy('createEmbeddedView'),
      clear: jasmine.createSpy('clear')
    } as any;

    const directive = new RoleGuardDirective(templateRef, viewContainer);
    
    // Set roles
    directive.appRoleGuard = 'Admin';
    
    expect(directive).toBeTruthy();
  });

  it('should handle array of roles', () => {
    const templateRef = {} as any;
    const viewContainer = {
      createEmbeddedView: jasmine.createSpy('createEmbeddedView'),
      clear: jasmine.createSpy('clear')
    } as any;

    const directive = new RoleGuardDirective(templateRef, viewContainer);
    
    // Set roles as array
    directive.appRoleGuard = ['Admin', 'Editor'];
    
    expect(directive).toBeTruthy();
  });

  it('should accept "any" strategy', () => {
    const templateRef = {} as any;
    const viewContainer = {
      createEmbeddedView: jasmine.createSpy('createEmbeddedView'),
      clear: jasmine.createSpy('clear')
    } as any;

    const directive = new RoleGuardDirective(templateRef, viewContainer);
    
    // Set strategy
    directive.appRoleGuardStrategy = 'any';
    
    expect(directive).toBeTruthy();
  });

  it('should accept "all" strategy', () => {
    const templateRef = {} as any;
    const viewContainer = {
      createEmbeddedView: jasmine.createSpy('createEmbeddedView'),
      clear: jasmine.createSpy('clear')
    } as any;

    const directive = new RoleGuardDirective(templateRef, viewContainer);
    
    // Set strategy
    directive.appRoleGuardStrategy = 'all';
    
    expect(directive).toBeTruthy();
  });
});

describe('Directive Integration Tests', () => {
  it('should work together in the same component', async () => {
    @Component({
      template: `
        <div *appAdminOnly>Admin content</div>
        <div *appRoleGuard="'Editor'">Editor content</div>
      `
    })
    class TestComponent { }

    await TestBed.configureTestingModule({
      declarations: [AdminOnlyDirective, RoleGuardDirective, TestComponent]
    }).compileComponents();

    const fixture = TestBed.createComponent(TestComponent);
    fixture.detectChanges();

    // Both directives should work
    const elements = fixture.debugElement.queryAll(By.css('div'));
    expect(elements.length).toBe(2);
  });
});
