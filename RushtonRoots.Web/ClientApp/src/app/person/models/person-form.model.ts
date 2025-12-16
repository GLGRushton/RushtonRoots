/**
 * Models and interfaces for Person Form components
 */

export interface PersonFormData {
  // Basic Information (Step 1)
  firstName: string;
  middleName?: string;
  lastName: string;
  suffix?: string;
  gender?: 'Male' | 'Female' | 'Other' | 'Unknown';
  
  // Dates & Places (Step 2)
  dateOfBirth?: Date | string;
  placeOfBirth?: string;
  dateOfDeath?: Date | string;
  placeOfDeath?: string;
  isDeceased: boolean;
  
  // Additional Information (Step 3)
  householdId?: number;
  biography?: string;
  occupation?: string;
  education?: string;
  notes?: string;
  
  // Photo Upload (Step 4)
  photoFile?: File;
  photoUrl?: string;
}

export interface PersonFormStep {
  label: string;
  icon: string;
  completed: boolean;
  optional: boolean;
}

export interface LocationSuggestion {
  id: string;
  name: string;
  description?: string;
  city?: string;
  state?: string;
  country?: string;
  fullAddress?: string;
}

export interface FormDraft {
  formData: Partial<PersonFormData>;
  lastSaved: Date;
  step: number;
}

export interface ValidationError {
  field: string;
  message: string;
}
