/**
 * Models and interfaces for Person Details components
 */

export interface PersonDetails {
  id: number;
  firstName: string;
  middleName?: string;
  lastName: string;
  suffix?: string;
  fullName: string;
  dateOfBirth?: Date | string;
  placeOfBirth?: string;
  dateOfDeath?: Date | string;
  placeOfDeath?: string;
  isDeceased: boolean;
  gender?: 'Male' | 'Female' | 'Other' | 'Unknown';
  householdId?: number;
  householdName?: string;
  photoUrl?: string;
  biography?: string;
  occupation?: string;
  education?: string;
  notes?: string;
}

export interface TimelineEvent {
  id: number;
  personId: number;
  title: string;
  date: Date | string;
  description?: string;
  eventType: 'birth' | 'death' | 'marriage' | 'education' | 'career' | 'milestone' | 'other';
  icon?: string;
  location?: string;
}

export interface PersonRelationship {
  relationshipType: 'parent' | 'child' | 'spouse' | 'partner' | 'sibling';
  relatedPerson: RelatedPersonInfo;
  relationshipDetails?: string;
  startDate?: Date | string;
  endDate?: Date | string;
}

export interface RelatedPersonInfo {
  id: number;
  firstName: string;
  lastName: string;
  fullName: string;
  dateOfBirth?: Date | string;
  dateOfDeath?: Date | string;
  isDeceased: boolean;
  photoUrl?: string;
}

export interface PersonPhoto {
  id: number;
  personId: number;
  photoUrl: string;
  thumbnailUrl?: string;
  title?: string;
  description?: string;
  uploadDate: Date | string;
  isPrimary: boolean;
  tags?: string[];
}

export interface PersonDetailsTab {
  label: string;
  icon?: string;
  content: 'overview' | 'timeline' | 'relationships' | 'photos';
}
