import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { RelationshipSuggestion } from '../../models/parent-child.model';

/**
 * RelationshipSuggestionsComponent - Displays AI-powered relationship suggestions
 * Phase 5.2: Parent-Child Relationships
 */
@Component({
  selector: 'app-relationship-suggestions',
  standalone: false,
  templateUrl: './relationship-suggestions.component.html',
  styleUrls: ['./relationship-suggestions.component.scss']
})
export class RelationshipSuggestionsComponent implements OnInit {
  /**
   * Event emitted when a suggestion is accepted
   */
  @Output() suggestionAccepted = new EventEmitter<RelationshipSuggestion>();

  /**
   * Event emitted when a suggestion is rejected
   */
  @Output() suggestionRejected = new EventEmitter<string>();

  /**
   * Relationship suggestions
   */
  suggestions: RelationshipSuggestion[] = [];

  /**
   * Loading state
   */
  isLoading = false;

  /**
   * Show help text
   */
  showHelp = false;

  ngOnInit(): void {
    this.loadSuggestions();
  }

  /**
   * Load AI-powered suggestions
   */
  private loadSuggestions(): void {
    this.isLoading = true;

    // Sample data - replace with API call to AI service
    setTimeout(() => {
      this.suggestions = this.getSampleSuggestions();
      this.isLoading = false;
    }, 1500);
  }

  /**
   * Get sample suggestions for demonstration
   */
  private getSampleSuggestions(): RelationshipSuggestion[] {
    return [
      {
        id: 'sugg-1',
        parentPersonId: 101,
        childPersonId: 201,
        parentName: 'Robert Smith Sr.',
        childName: 'John Smith',
        parentPhotoUrl: undefined,
        childPhotoUrl: undefined,
        confidence: 95,
        reasoning: 'Based on matching surnames and birth year proximity (25-year gap)',
        suggestedType: 'biological',
        sources: ['Census records 1960', 'Birth certificate']
      },
      {
        id: 'sugg-2',
        parentPersonId: 102,
        childPersonId: 202,
        parentName: 'Mary Johnson',
        childName: 'Emily Johnson',
        parentPhotoUrl: undefined,
        childPhotoUrl: undefined,
        confidence: 88,
        reasoning: 'Matching household records and 30-year age difference',
        suggestedType: 'biological',
        sources: ['Household census 1990', 'School records']
      },
      {
        id: 'sugg-3',
        parentPersonId: 103,
        childPersonId: 203,
        parentName: 'David Brown',
        childName: 'Sophie Martinez',
        parentPhotoUrl: undefined,
        childPhotoUrl: undefined,
        confidence: 72,
        reasoning: 'Found in same household, different surnames suggest adoption',
        suggestedType: 'adopted',
        sources: ['Adoption records', 'Family photos']
      }
    ];
  }

  /**
   * Accept a suggestion
   */
  onAccept(suggestion: RelationshipSuggestion): void {
    if (confirm(`Accept this relationship suggestion?\n\n${suggestion.parentName} → ${suggestion.childName}\nConfidence: ${suggestion.confidence}%`)) {
      this.suggestionAccepted.emit(suggestion);
      this.suggestions = this.suggestions.filter(s => s.id !== suggestion.id);
    }
  }

  /**
   * Reject a suggestion
   */
  onReject(suggestion: RelationshipSuggestion): void {
    if (confirm('Reject this suggestion? This will help improve future recommendations.')) {
      this.suggestionRejected.emit(suggestion.id);
      this.suggestions = this.suggestions.filter(s => s.id !== suggestion.id);
    }
  }

  /**
   * Get confidence badge color
   */
  getConfidenceBadgeColor(confidence: number): string {
    if (confidence >= 80) return 'primary';
    if (confidence >= 60) return 'accent';
    return 'warn';
  }

  /**
   * Get confidence level text
   */
  getConfidenceLevel(confidence: number): string {
    if (confidence >= 90) return 'Very High';
    if (confidence >= 80) return 'High';
    if (confidence >= 70) return 'Medium';
    if (confidence >= 60) return 'Moderate';
    return 'Low';
  }

  /**
   * Get person initials
   */
  getInitials(name: string): string {
    const names = name.split(' ');
    return names.length > 1
      ? `${names[0][0]}${names[names.length - 1][0]}`.toUpperCase()
      : name.substring(0, 2).toUpperCase();
  }

  /**
   * Refresh suggestions
   */
  onRefresh(): void {
    this.loadSuggestions();
  }
}
