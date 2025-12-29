import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FamilyTreeNode } from '../../models/parent-child.model';

/**
 * FamilyTreeMiniComponent - Compact family tree visualization
 * Phase 5.2: Parent-Child Relationships
 */
@Component({
  selector: 'app-family-tree-mini',
  standalone: false,
  templateUrl: './family-tree-mini.component.html',
  styleUrls: ['./family-tree-mini.component.scss']
})
export class FamilyTreeMiniComponent implements OnInit {
  /**
   * Person ID to center the tree on
   */
  @Input() focusPersonId!: number;

  /**
   * Number of generations to show (above and below)
   */
  @Input() generations: number = 2;

  /**
   * Show spouse relationships
   */
  @Input() showSpouses: boolean = true;

  /**
   * Compact mode (smaller cards)
   */
  @Input() compact: boolean = false;

  /**
   * Event emitted when a person is clicked
   */
  @Output() personClicked = new EventEmitter<number>();

  /**
   * Root node of the tree
   */
  rootNode?: FamilyTreeNode;

  /**
   * Loading state
   */
  isLoading = false;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadTree();
  }

  /**
   * Load family tree data
   */
  private loadTree(): void {
    this.isLoading = true;

    // Determine which endpoint to call
    const apiUrl = this.focusPersonId 
      ? `/api/familytree/mini/${this.focusPersonId}?generations=${this.generations}`
      : `/api/familytree/mini/current?generations=${this.generations}`;

    this.http.get<FamilyTreeNode>(apiUrl).subscribe({
      next: (data) => {
        this.rootNode = this.mapApiResponseToTreeNode(data);
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading family tree:', error);
        this.isLoading = false;
        // Set rootNode to undefined to show empty state
        this.rootNode = undefined;
      }
    });
  }

  /**
   * Map API response to FamilyTreeNode (handles date strings)
   */
  private mapApiResponseToTreeNode(node: FamilyTreeNode): FamilyTreeNode {
    return {
      id: node.id,
      name: node.name,
      photoUrl: node.photoUrl,
      birthDate: node.birthDate ? new Date(node.birthDate) : undefined,
      deathDate: node.deathDate ? new Date(node.deathDate) : undefined,
      generation: node.generation,
      parents: node.parents?.map((p: FamilyTreeNode) => this.mapApiResponseToTreeNode(p)),
      children: node.children?.map((c: FamilyTreeNode) => this.mapApiResponseToTreeNode(c)),
      spouses: node.spouses?.map((s: FamilyTreeNode) => this.mapApiResponseToTreeNode(s))
    };
  }

  /**
   * Handle person click
   */
  onPersonClick(personId: number): void {
    this.personClicked.emit(personId);
  }

  /**
   * Get person initials for fallback avatar
   */
  getInitials(name: string): string {
    const names = name.split(' ');
    return names.length > 1 
      ? `${names[0][0]}${names[names.length - 1][0]}`.toUpperCase()
      : name.substring(0, 2).toUpperCase();
  }

  /**
   * Get person life span
   */
  getLifeSpan(node: FamilyTreeNode): string {
    const birthYear = node.birthDate ? node.birthDate.getFullYear() : '?';
    const deathYear = node.deathDate ? node.deathDate.getFullYear() : '';
    return deathYear ? `${birthYear} - ${deathYear}` : `${birthYear} - Present`;
  }

  /**
   * Calculate age or years lived
   */
  getAge(node: FamilyTreeNode): number | null {
    if (!node.birthDate) return null;
    const endDate = node.deathDate || new Date();
    const age = endDate.getFullYear() - node.birthDate.getFullYear();
    return age;
  }
}
