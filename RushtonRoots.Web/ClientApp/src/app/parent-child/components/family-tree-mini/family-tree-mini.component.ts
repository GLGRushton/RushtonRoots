import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
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

  ngOnInit(): void {
    this.loadTree();
  }

  /**
   * Load family tree data
   */
  private loadTree(): void {
    this.isLoading = true;

    // Sample data - replace with API call
    setTimeout(() => {
      this.rootNode = this.getSampleTreeData();
      this.isLoading = false;
    }, 500);
  }

  /**
   * Get sample tree data for demonstration
   */
  private getSampleTreeData(): FamilyTreeNode {
    const grandparent1: FamilyTreeNode = {
      id: 1,
      name: 'Robert Smith Sr.',
      photoUrl: undefined,
      birthDate: new Date('1930-05-10'),
      deathDate: new Date('2010-03-15'),
      generation: -2,
      children: []
    };

    const grandparent2: FamilyTreeNode = {
      id: 2,
      name: 'Mary Smith',
      photoUrl: undefined,
      birthDate: new Date('1932-08-20'),
      deathDate: undefined,
      generation: -2,
      children: []
    };

    const parent1: FamilyTreeNode = {
      id: 3,
      name: 'John Smith',
      photoUrl: undefined,
      birthDate: new Date('1955-12-25'),
      deathDate: undefined,
      generation: -1,
      parents: [grandparent1, grandparent2],
      children: [],
      spouses: []
    };

    const parent2: FamilyTreeNode = {
      id: 4,
      name: 'Jane Doe',
      photoUrl: undefined,
      birthDate: new Date('1957-03-14'),
      deathDate: undefined,
      generation: -1,
      children: [],
      spouses: [parent1]
    };

    parent1.spouses = [parent2];

    const focusPerson: FamilyTreeNode = {
      id: this.focusPersonId,
      name: 'Robert Smith Jr.',
      photoUrl: undefined,
      birthDate: new Date('1980-06-15'),
      deathDate: undefined,
      generation: 0,
      parents: [parent1, parent2],
      children: [],
      spouses: []
    };

    const child1: FamilyTreeNode = {
      id: 101,
      name: 'Emily Smith',
      photoUrl: undefined,
      birthDate: new Date('2005-06-15'),
      deathDate: undefined,
      generation: 1,
      parents: [focusPerson],
      children: []
    };

    const child2: FamilyTreeNode = {
      id: 102,
      name: 'Michael Smith',
      photoUrl: undefined,
      birthDate: new Date('2008-09-20'),
      deathDate: undefined,
      generation: 1,
      parents: [focusPerson],
      children: []
    };

    focusPerson.children = [child1, child2];
    parent1.children = [focusPerson];
    parent2.children = [focusPerson];
    grandparent1.children = [parent1];
    grandparent2.children = [parent1];

    return focusPerson;
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
