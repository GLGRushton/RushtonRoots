# PersonFormComponent Usage Examples

## Overview

The PersonFormComponent is a comprehensive wizard-based form for creating and editing people in the RushtonRoots application. It features a 4-step process, automatic draft saving, photo upload, and extensive validation.

## Components

### 1. PersonFormComponent
Main form component with 4-step wizard using MatStepper.

### 2. DatePickerComponent
Reusable date picker component with Material Datepicker integration.

### 3. LocationAutocompleteComponent
Autocomplete component for location selection with city/state/country suggestions.

## Features

- **4-Step Wizard Process**:
  1. Basic Information (name, gender)
  2. Dates & Places (birth/death dates and locations)
  3. Additional Information (occupation, education, biography, notes)
  4. Photo Upload (optional profile photo)

- **Form Validation**:
  - Required fields (first name, last name)
  - Length constraints
  - Character counters for text areas
  - Conditional validation (death fields only when deceased)

- **Autosave Functionality**:
  - Saves draft to localStorage every 30 seconds
  - Restores draft on page load (if < 24 hours old)
  - Clears draft after successful submission

- **Photo Upload**:
  - File type validation (images only)
  - File size validation (max 5MB)
  - Image preview
  - Remove photo option

- **Notifications**:
  - Success/error messages via MatSnackBar
  - Draft save confirmations
  - Validation error alerts

## Usage in Razor Views

### Basic Usage (Create Mode)

```html
<app-person-form></app-person-form>
```

### Edit Mode with Initial Data

```html
<app-person-form 
  person-id="123"
  initial-data='{"firstName":"John","lastName":"Doe","gender":"Male"}'>
</app-person-form>
```

### With Event Handlers

```html
<app-person-form 
  id="personForm"
  person-id="456">
</app-person-form>

<script>
  const formElement = document.getElementById('personForm');
  
  // Handle form submission
  formElement.addEventListener('formSubmit', (event) => {
    const formData = event.detail;
    console.log('Form submitted:', formData);
    
    // Send to API
    fetch('/api/person', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(formData)
    })
    .then(response => response.json())
    .then(data => {
      console.log('Person saved:', data);
      window.location.href = `/person/details/${data.id}`;
    })
    .catch(error => console.error('Error:', error));
  });
  
  // Handle form cancellation
  formElement.addEventListener('formCancel', () => {
    console.log('Form cancelled');
    window.location.href = '/person/index';
  });
</script>
```

## Component Properties

### Inputs

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `personId` | `number` | `undefined` | ID of person being edited (optional) |
| `initialData` | `Partial<PersonFormData>` | `undefined` | Initial form data for editing |

### Outputs

| Event | Payload | Description |
|-------|---------|-------------|
| `formSubmit` | `PersonFormData` | Emitted when form is successfully submitted |
| `formCancel` | `void` | Emitted when user cancels the form |

## PersonFormData Interface

```typescript
interface PersonFormData {
  // Basic Information
  firstName: string;
  middleName?: string;
  lastName: string;
  suffix?: string;
  gender?: 'Male' | 'Female' | 'Other' | 'Unknown';
  
  // Dates & Places
  dateOfBirth?: Date | string;
  placeOfBirth?: string;
  dateOfDeath?: Date | string;
  placeOfDeath?: string;
  isDeceased: boolean;
  
  // Additional Information
  householdId?: number;
  biography?: string;
  occupation?: string;
  education?: string;
  notes?: string;
  
  // Photo Upload
  photoFile?: File;
  photoUrl?: string;
}
```

## DatePickerComponent Usage

### Standalone Usage

```html
<app-date-picker 
  label="Date of Birth"
  placeholder="MM/DD/YYYY"
  hint="Select the birth date"
  required="true">
</app-date-picker>
```

### With Min/Max Constraints

```html
<app-date-picker 
  label="Date of Death"
  min-date="1900-01-01"
  max-date="2025-12-31">
</app-date-picker>
```

## LocationAutocompleteComponent Usage

### Standalone Usage

```html
<app-location-autocomplete 
  label="Place of Birth"
  placeholder="City, State, Country"
  hint="Where was this person born?">
</app-location-autocomplete>
```

### With Event Handler

```html
<app-location-autocomplete 
  id="birthLocation"
  label="Place of Birth">
</app-location-autocomplete>

<script>
  document.getElementById('birthLocation')
    .addEventListener('locationSelected', (event) => {
      console.log('Location selected:', event.detail);
    });
</script>
```

## Form Validation Rules

### Required Fields
- First Name (min 1 char, max 100 chars)
- Last Name (min 1 char, max 100 chars)

### Optional Fields with Constraints
- Middle Name (max 100 chars)
- Suffix (max 20 chars)
- Biography (max 5000 chars)
- Occupation (max 200 chars)
- Education (max 500 chars)
- Notes (max 2000 chars)

### Conditional Fields
- Date of Death (enabled only when isDeceased = true)
- Place of Death (enabled only when isDeceased = true)

### Photo Validation
- Accepted types: image/*
- Max file size: 5MB
- Preview generated using FileReader API

## Autosave Behavior

1. **Autosave Interval**: Every 30 seconds
2. **Storage**: localStorage with key `person-form-draft`
3. **Draft Structure**:
   ```json
   {
     "formData": { /* PersonFormData */ },
     "lastSaved": "2025-12-16T00:00:00.000Z",
     "step": 0
   }
   ```
4. **Draft Restoration**:
   - Only if draft is < 24 hours old
   - User is prompted to restore or discard
   - Draft cleared after restore or discard
5. **Draft Cleared**:
   - After successful form submission
   - When user manually discards draft

## Navigation Flow

### Stepper Navigation
- **Linear Mode**: User must complete each step before proceeding
- **Non-linear Mode**: User can jump between steps (configurable)

### Step Transitions
- **Next**: Validates current step before proceeding
- **Back**: No validation required
- **Submit**: Validates all steps

### Cancel Behavior
- If form is dirty: Confirmation prompt
- If form is pristine: Immediate cancellation
- Emits `formCancel` event

## Styling and Theming

The form uses Material Design components and follows the RushtonRoots color scheme:
- Primary color: #2e7d32 (Forest Green)
- Accent color: #4caf50 (Light Green)
- Form max-width: 900px
- Responsive breakpoint: 768px

## Best Practices

1. **Always handle the `formSubmit` event** to save data to the backend
2. **Handle the `formCancel` event** to navigate back to the appropriate page
3. **Provide initial data when editing** an existing person
4. **Validate on the server** - client-side validation is not sufficient
5. **Handle photo uploads separately** if needed (multipart/form-data)
6. **Test on mobile devices** to ensure responsive behavior

## Example: Full Integration

```html
<!-- Create New Person Page -->
<!DOCTYPE html>
<html>
<head>
  <title>Create Person - RushtonRoots</title>
  <!-- Include Angular bundles -->
  <script src="/dist/runtime.js"></script>
  <script src="/dist/polyfills.js"></script>
  <script src="/dist/main.js"></script>
</head>
<body>
  <div class="container">
    <h1>Create New Person</h1>
    <app-person-form id="createPersonForm"></app-person-form>
  </div>

  <script>
    const form = document.getElementById('createPersonForm');
    
    form.addEventListener('formSubmit', async (event) => {
      const formData = event.detail;
      
      try {
        // If there's a photo, handle multipart upload
        if (formData.photoFile) {
          const uploadData = new FormData();
          uploadData.append('photo', formData.photoFile);
          Object.keys(formData).forEach(key => {
            if (key !== 'photoFile') {
              uploadData.append(key, formData[key]);
            }
          });
          
          const response = await fetch('/api/person', {
            method: 'POST',
            body: uploadData
          });
          
          if (response.ok) {
            const person = await response.json();
            window.location.href = `/person/details/${person.id}`;
          }
        } else {
          // JSON submission without photo
          const response = await fetch('/api/person', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(formData)
          });
          
          if (response.ok) {
            const person = await response.json();
            window.location.href = `/person/details/${person.id}`;
          }
        }
      } catch (error) {
        console.error('Failed to create person:', error);
        alert('Failed to create person. Please try again.');
      }
    });
    
    form.addEventListener('formCancel', () => {
      window.location.href = '/person/index';
    });
  </script>
</body>
</html>
```

## Troubleshooting

### Form not submitting
- Check that all required fields are filled
- Verify the `formSubmit` event handler is attached
- Check browser console for validation errors

### Autosave not working
- Ensure localStorage is enabled in browser
- Check browser console for localStorage errors
- Verify the form has been modified (dirty state)

### Photo preview not showing
- Verify file is an image type
- Check file size is under 5MB
- Ensure FileReader API is supported

### Date picker not working
- Verify MatDatepicker module is imported
- Check that date values are valid
- Ensure min/max constraints are correct

## Related Components

- **PersonDetailsComponent**: Display person information
- **PersonTableComponent**: List people in table format
- **PersonSearchComponent**: Search and filter people
- **PersonIndexComponent**: Main people listing page

## API Integration

The form is designed to work with a RESTful API:

### Create Person
```
POST /api/person
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  ...
}
```

### Update Person
```
PUT /api/person/{id}
Content-Type: application/json

{
  "id": 123,
  "firstName": "John",
  "lastName": "Doe",
  ...
}
```

### Upload Photo
```
POST /api/person/{id}/photo
Content-Type: multipart/form-data

photo: (binary)
```
