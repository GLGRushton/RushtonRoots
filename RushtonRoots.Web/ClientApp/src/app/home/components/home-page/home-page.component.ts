import { Component, OnInit, Input } from '@angular/core';
import {
  HomePageData,
  FamilyStatistics,
  RecentAddition,
  UpcomingEvent,
  ActivityFeedItem,
  QuickLink
} from '../../models/home-page.model';

/**
 * HomePageComponent - Main landing page dashboard
 * Phase 6.1: Home Landing Page
 * 
 * Features:
 * - Hero section with welcome message and search
 * - Family overview with statistics and recent updates
 * - Family tree preview
 * - Recent activity feed
 * - Quick links to major features
 * - Statistics dashboard
 */
@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.scss'],
  standalone: false
})
export class HomePageComponent implements OnInit {
  @Input() data?: string; // JSON string from Razor view
  @Input() userName?: string;
  @Input() canEdit: boolean = false;
  @Input() canCreate: boolean = false;

  // Parsed data
  homePageData: HomePageData | null = null;
  
  // Section states
  isLoading: boolean = true;
  searchQuery: string = '';
  
  // Default quick links
  defaultQuickLinks: QuickLink[] = [
    {
      title: 'Browse People',
      icon: 'people',
      url: '/Person',
      description: 'View all family members',
      color: 'primary'
    },
    {
      title: 'View Households',
      icon: 'home',
      url: '/Household',
      description: 'Explore family households',
      color: 'accent'
    },
    {
      title: 'Photo Gallery',
      icon: 'photo_library',
      url: '/MediaGallery',
      description: 'Browse family photos',
      color: 'warn'
    },
    {
      title: 'Family Wiki',
      icon: 'menu_book',
      url: '/Wiki',
      description: 'Read family articles',
      color: 'primary'
    },
    {
      title: 'Calendar',
      icon: 'event',
      url: '/Calendar',
      description: 'View events and birthdays',
      color: 'accent'
    },
    {
      title: 'Recipes',
      icon: 'restaurant',
      url: '/Recipe',
      description: 'Family recipes collection',
      color: 'warn'
    }
  ];

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;
    
    if (this.data) {
      try {
        this.homePageData = JSON.parse(this.data);
      } catch (error) {
        console.error('Error parsing home page data:', error);
        this.homePageData = this.getDefaultData();
      }
    } else {
      this.homePageData = this.getDefaultData();
    }
    
    // Ensure quick links are set
    if (this.homePageData && (!this.homePageData.quickLinks || this.homePageData.quickLinks.length === 0)) {
      this.homePageData.quickLinks = this.defaultQuickLinks;
    }
    
    this.isLoading = false;
  }

  getDefaultData(): HomePageData {
    return {
      statistics: {
        totalMembers: 0,
        oldestAncestor: null,
        newestMember: null,
        totalPhotos: 0,
        totalStories: 0,
        activeHouseholds: 0
      },
      recentAdditions: [],
      upcomingBirthdays: [],
      upcomingAnniversaries: [],
      recentEvents: [],
      activityFeed: [],
      quickLinks: this.defaultQuickLinks,
      familyTagline: 'Discover, Preserve, and Celebrate Your Family Heritage'
    };
  }

  onSearch(): void {
    if (this.searchQuery.trim()) {
      window.location.href = `/Person?search=${encodeURIComponent(this.searchQuery)}`;
    }
  }

  onViewTree(): void {
    window.location.href = '/FamilyTree';
  }

  onAddPerson(): void {
    window.location.href = '/Person/Create';
  }

  onBrowsePhotos(): void {
    window.location.href = '/MediaGallery';
  }

  onQuickLinkClick(link: QuickLink): void {
    window.location.href = link.url;
  }

  onActivityItemClick(item: ActivityFeedItem): void {
    if (item.relatedUrl) {
      window.location.href = item.relatedUrl;
    }
  }

  onPersonClick(personId: number): void {
    window.location.href = `/Person/Details/${personId}`;
  }

  getEventIcon(eventType: string): string {
    switch (eventType) {
      case 'birthday':
        return 'cake';
      case 'anniversary':
        return 'favorite';
      default:
        return 'event';
    }
  }

  getEventColor(eventType: string): string {
    switch (eventType) {
      case 'birthday':
        return 'accent';
      case 'anniversary':
        return 'warn';
      default:
        return 'primary';
    }
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
  }

  formatYear(dateString: string): string {
    const date = new Date(dateString);
    return date.getFullYear().toString();
  }

  formatTimestamp(dateString: string): string {
    const date = new Date(dateString);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMins / 60);
    const diffDays = Math.floor(diffHours / 24);

    if (diffMins < 1) return 'Just now';
    if (diffMins < 60) return `${diffMins}m ago`;
    if (diffHours < 24) return `${diffHours}h ago`;
    if (diffDays < 7) return `${diffDays}d ago`;
    return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
  }

  getDaysUntilText(days: number): string {
    if (days === 0) return 'Today';
    if (days === 1) return 'Tomorrow';
    return `In ${days} days`;
  }
}
