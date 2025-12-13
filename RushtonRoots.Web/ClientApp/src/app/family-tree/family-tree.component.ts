import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

interface Person {
  id: number;
  firstName: string;
  lastName: string;
  fullName: string;
  dateOfBirth?: Date;
  dateOfDeath?: Date;
  isDeceased: boolean;
  photoUrl?: string;
}

interface TreeNode {
  person: Person;
  partner?: Person;
  generation: number;
  parents?: TreeNode[];
  children?: TreeNode[];
}

interface FamilyDataResponse {
  people: Person[];
  parentChildRelationships: any[];
  partnerships: any[];
}

type ViewMode = 'descendant' | 'pedigree' | 'fan';

@Component({
  selector: 'app-family-tree',
  standalone: false,
  templateUrl: './family-tree.component.html',
  styleUrl: './family-tree.component.css'
})
export class FamilyTreeComponent implements OnInit {
  viewMode: ViewMode = 'descendant';
  treeData: TreeNode | null = null;
  allPeople: Person[] = [];
  loading = false;
  error: string | null = null;
  selectedPersonId = 1; // Default to first person
  zoom = 1.0;
  panX = 0;
  panY = 0;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.loadFamilyData();
  }

  async loadFamilyData() {
    this.loading = true;
    this.error = null;
    try {
      const data = await firstValueFrom(this.http.get<FamilyDataResponse>('/api/familytree/all'));
      this.allPeople = data.people || [];
      
      // Default to first person if available
      if (this.allPeople.length > 0 && !this.selectedPersonId) {
        this.selectedPersonId = this.allPeople[0].id;
      }
      
      await this.loadTreeView();
    } catch (err: unknown) {
      console.error('Error loading family data:', err);
      this.error = 'Failed to load family data';
      this.loadSampleData(); // Fallback to sample data
    } finally {
      this.loading = false;
    }
  }

  async loadTreeView() {
    if (!this.selectedPersonId) return;

    this.loading = true;
    this.error = null;
    try {
      const endpoint = this.viewMode === 'pedigree' 
        ? `/api/familytree/pedigree/${this.selectedPersonId}`
        : `/api/familytree/descendants/${this.selectedPersonId}`;
      
      const data = await firstValueFrom(this.http.get<TreeNode>(endpoint));
      this.treeData = data || null;
    } catch (err: unknown) {
      console.error('Error loading tree view:', err);
      this.error = 'Failed to load tree view';
      this.loadSampleData(); // Fallback to sample data
    } finally {
      this.loading = false;
    }
  }

  setViewMode(mode: ViewMode) {
    this.viewMode = mode;
    this.loadTreeView();
  }

  setSelectedPerson(personId: number) {
    this.selectedPersonId = personId;
    this.loadTreeView();
  }

  zoomIn() {
    this.zoom = Math.min(this.zoom + 0.1, 2.0);
  }

  zoomOut() {
    this.zoom = Math.max(this.zoom - 0.1, 0.5);
  }

  resetZoom() {
    this.zoom = 1.0;
    this.panX = 0;
    this.panY = 0;
  }

  print() {
    window.print();
  }

  // Fallback sample data for when API is not available
  loadSampleData() {
    this.allPeople = [
      { id: 1, firstName: 'John', lastName: 'Rushton', fullName: 'John Rushton', isDeceased: false },
      { id: 2, firstName: 'Mary', lastName: 'Rushton', fullName: 'Mary Rushton', isDeceased: false }
    ];
    this.selectedPersonId = 1;
    this.treeData = {
      person: this.allPeople[0],
      partner: this.allPeople[1],
      generation: 0,
      children: []
    };
  }

  getPersonDisplayName(person: Person): string {
    return person.fullName || `${person.firstName} ${person.lastName}`;
  }

  getPersonYears(person: Person): string {
    if (!person.dateOfBirth && !person.dateOfDeath) return '';
    const birth = person.dateOfBirth ? new Date(person.dateOfBirth).getFullYear() : '?';
    const death = person.dateOfDeath ? new Date(person.dateOfDeath).getFullYear() : '';
    return death ? `(${birth} - ${death})` : `(b. ${birth})`;
  }
}
