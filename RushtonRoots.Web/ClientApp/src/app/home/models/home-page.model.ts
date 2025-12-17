/**
 * Home Page Data Models
 * Phase 6.1: Home Landing Page
 */

export interface FamilyStatistics {
  totalMembers: number;
  oldestAncestor: PersonSummary | null;
  newestMember: PersonSummary | null;
  totalPhotos: number;
  totalStories: number;
  activeHouseholds: number;
}

export interface PersonSummary {
  id: number;
  fullName: string;
  photoUrl?: string;
  birthDate?: string;
  deathDate?: string;
  age?: number;
}

export interface RecentAddition {
  person: PersonSummary;
  addedDate: string;
  addedBy: string;
}

export interface UpcomingEvent {
  id: number;
  personId: number;
  personName: string;
  photoUrl?: string;
  eventType: 'birthday' | 'anniversary' | 'other';
  eventDate: string;
  daysUntil: number;
  description?: string;
}

export interface ActivityFeedItem {
  id: string;
  type: 'member_added' | 'photo_uploaded' | 'story_published' | 'comment_posted';
  icon: string;
  color: string;
  title: string;
  description: string;
  timestamp: string;
  userName: string;
  userPhotoUrl?: string;
  relatedUrl?: string;
}

export interface QuickLink {
  title: string;
  icon: string;
  url: string;
  description: string;
  color: string;
}

export interface HomePageData {
  statistics: FamilyStatistics;
  recentAdditions: RecentAddition[];
  upcomingBirthdays: UpcomingEvent[];
  upcomingAnniversaries: UpcomingEvent[];
  recentEvents: UpcomingEvent[];
  activityFeed: ActivityFeedItem[];
  quickLinks: QuickLink[];
  familyTagline?: string;
}
