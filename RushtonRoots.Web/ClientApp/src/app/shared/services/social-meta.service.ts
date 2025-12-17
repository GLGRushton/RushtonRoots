import { Injectable } from '@angular/core';
import { Meta, Title } from '@angular/platform-browser';

/**
 * SocialMetaService
 * 
 * Purpose: Dynamically updates HTML meta tags for social media sharing
 * 
 * Features:
 * - Open Graph (Facebook, LinkedIn) meta tags
 * - Twitter Card meta tags
 * - Dynamic updates based on current page/content
 * - Support for person profiles, stories, recipes, traditions, photos
 * - SEO-friendly meta descriptions
 * 
 * Usage:
 * ```typescript
 * constructor(private socialMetaService: SocialMetaService) {}
 * 
 * ngOnInit() {
 *   this.socialMetaService.updatePersonMeta(person);
 *   // or
 *   this.socialMetaService.updateStoryMeta(story);
 * }
 * ```
 */
@Injectable({
  providedIn: 'root'
})
export class SocialMetaService {

  private readonly defaultImage = '/assets/images/rushton-roots-og-image.jpg';
  private readonly defaultDescription = 'RushtonRoots - Preserve your family history and connect with your heritage';
  private readonly siteName = 'RushtonRoots';

  constructor(
    private meta: Meta,
    private titleService: Title
  ) { }

  /**
   * Update meta tags for person profile page
   */
  updatePersonMeta(person: {
    id: number;
    firstName: string;
    lastName: string;
    dateOfBirth?: string;
    dateOfDeath?: string;
    photoUrl?: string;
    occupation?: string;
    placeOfBirth?: string;
  }): void {
    const fullName = `${person.firstName} ${person.lastName}`;
    const title = `${fullName} - Family Profile`;
    const description = this.buildPersonDescription(person);
    const url = `${window.location.origin}/Person/Details/${person.id}`;
    const image = person.photoUrl || this.defaultImage;

    this.updateTags(title, description, url, image, 'profile');
  }

  /**
   * Update meta tags for story page
   */
  updateStoryMeta(story: {
    id: number;
    title: string;
    content: string;
    authorName?: string;
    imageUrl?: string;
    createdDate?: string;
  }): void {
    const title = `${story.title} - Family Story`;
    const description = this.truncateText(story.content, 160);
    const url = `${window.location.origin}/StoryView?id=${story.id}`;
    const image = story.imageUrl || this.defaultImage;

    this.updateTags(title, description, url, image, 'article');

    // Add article-specific tags
    if (story.authorName) {
      this.meta.updateTag({ property: 'article:author', content: story.authorName });
    }
    if (story.createdDate) {
      this.meta.updateTag({ property: 'article:published_time', content: story.createdDate });
    }
  }

  /**
   * Update meta tags for recipe page
   */
  updateRecipeMeta(recipe: {
    id: number;
    title: string;
    description: string;
    authorName?: string;
    imageUrl?: string;
    prepTime?: number;
    cookTime?: number;
  }): void {
    const title = `${recipe.title} - Family Recipe`;
    const description = recipe.description || 'A treasured family recipe passed down through generations';
    const url = `${window.location.origin}/Recipe?id=${recipe.id}`;
    const image = recipe.imageUrl || this.defaultImage;

    this.updateTags(title, description, url, image, 'article');

    // Add recipe-specific structured data hints
    if (recipe.prepTime || recipe.cookTime) {
      const totalTime = (recipe.prepTime || 0) + (recipe.cookTime || 0);
      this.meta.updateTag({ name: 'recipe:duration', content: `${totalTime} minutes` });
    }
  }

  /**
   * Update meta tags for tradition page
   */
  updateTraditionMeta(tradition: {
    id: number;
    title: string;
    description: string;
    imageUrl?: string;
    season?: string;
  }): void {
    const title = `${tradition.title} - Family Tradition`;
    const description = tradition.description || 'A cherished family tradition celebrated across generations';
    const url = `${window.location.origin}/Tradition?id=${tradition.id}`;
    const image = tradition.imageUrl || this.defaultImage;

    this.updateTags(title, description, url, image, 'article');
  }

  /**
   * Update meta tags for photo/media page
   */
  updatePhotoMeta(photo: {
    id: number;
    title: string;
    description?: string;
    url: string;
    captureDate?: string;
    location?: string;
  }): void {
    const title = `${photo.title} - Family Photo`;
    const description = photo.description || 
      `Family photo${photo.captureDate ? ` from ${photo.captureDate}` : ''}${photo.location ? ` at ${photo.location}` : ''}`;
    const pageUrl = `${window.location.origin}/Media/Photo/${photo.id}`;
    const image = photo.url;

    this.updateTags(title, description, pageUrl, image, 'photo');
  }

  /**
   * Update meta tags for household page
   */
  updateHouseholdMeta(household: {
    id: number;
    householdName: string;
    description?: string;
    memberCount: number;
    anchorPersonName?: string;
  }): void {
    const title = `${household.householdName} - Household`;
    const description = household.description || 
      `${household.householdName} household with ${household.memberCount} members${household.anchorPersonName ? `, led by ${household.anchorPersonName}` : ''}`;
    const url = `${window.location.origin}/Household/Details/${household.id}`;

    this.updateTags(title, description, url, this.defaultImage, 'profile');
  }

  /**
   * Update meta tags for wiki article page
   */
  updateWikiMeta(article: {
    id: number;
    title: string;
    content: string;
    categoryName?: string;
  }): void {
    const title = `${article.title} - Family Wiki`;
    const description = this.truncateText(article.content, 160);
    const url = `${window.location.origin}/Wiki/Article/${article.id}`;

    this.updateTags(title, description, url, this.defaultImage, 'article');

    if (article.categoryName) {
      this.meta.updateTag({ property: 'article:section', content: article.categoryName });
    }
  }

  /**
   * Reset to default meta tags (for home page, index pages, etc.)
   */
  resetToDefault(): void {
    const title = 'RushtonRoots - Family Tree and Genealogy';
    const description = this.defaultDescription;
    const url = window.location.origin;
    const image = this.defaultImage;

    this.updateTags(title, description, url, image, 'website');
  }

  /**
   * Core method to update all meta tags
   */
  private updateTags(
    title: string,
    description: string,
    url: string,
    image: string,
    type: 'website' | 'article' | 'profile' | 'photo' = 'website'
  ): void {
    // Update page title
    this.titleService.setTitle(title);

    // Standard meta tags
    this.meta.updateTag({ name: 'description', content: description });

    // Open Graph (Facebook, LinkedIn)
    this.meta.updateTag({ property: 'og:title', content: title });
    this.meta.updateTag({ property: 'og:description', content: description });
    this.meta.updateTag({ property: 'og:url', content: url });
    this.meta.updateTag({ property: 'og:image', content: this.getAbsoluteUrl(image) });
    this.meta.updateTag({ property: 'og:type', content: type });
    this.meta.updateTag({ property: 'og:site_name', content: this.siteName });

    // Twitter Cards
    this.meta.updateTag({ name: 'twitter:card', content: 'summary_large_image' });
    this.meta.updateTag({ name: 'twitter:title', content: title });
    this.meta.updateTag({ name: 'twitter:description', content: description });
    this.meta.updateTag({ name: 'twitter:image', content: this.getAbsoluteUrl(image) });
    this.meta.updateTag({ name: 'twitter:url', content: url });
  }

  /**
   * Build description for person profile
   */
  private buildPersonDescription(person: {
    firstName: string;
    lastName: string;
    dateOfBirth?: string;
    dateOfDeath?: string;
    occupation?: string;
    placeOfBirth?: string;
  }): string {
    const parts: string[] = [`${person.firstName} ${person.lastName}`];

    if (person.occupation) {
      parts.push(person.occupation);
    }

    if (person.dateOfBirth && person.dateOfDeath) {
      parts.push(`(${person.dateOfBirth} - ${person.dateOfDeath})`);
    } else if (person.dateOfBirth) {
      parts.push(`(b. ${person.dateOfBirth})`);
    }

    if (person.placeOfBirth) {
      parts.push(`Born in ${person.placeOfBirth}`);
    }

    const description = parts.join(' ');
    return this.truncateText(description, 160);
  }

  /**
   * Truncate text to specified length with ellipsis
   */
  private truncateText(text: string, maxLength: number): string {
    if (!text) return this.defaultDescription;
    if (text.length <= maxLength) return text;
    return text.substring(0, maxLength - 3).trim() + '...';
  }

  /**
   * Convert relative URL to absolute URL
   */
  private getAbsoluteUrl(url: string): string {
    if (!url) return `${window.location.origin}${this.defaultImage}`;
    if (url.startsWith('http://') || url.startsWith('https://')) {
      return url;
    }
    return `${window.location.origin}${url.startsWith('/') ? url : '/' + url}`;
  }
}
