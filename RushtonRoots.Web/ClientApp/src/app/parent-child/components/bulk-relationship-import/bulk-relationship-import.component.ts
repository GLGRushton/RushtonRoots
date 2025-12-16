import { Component, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { BulkImportData, BulkImportResult } from '../../models/parent-child.model';

/**
 * BulkRelationshipImportComponent - Import multiple relationships at once
 * Phase 5.2: Parent-Child Relationships
 */
@Component({
  selector: 'app-bulk-relationship-import',
  standalone: false,
  templateUrl: './bulk-relationship-import.component.html',
  styleUrls: ['./bulk-relationship-import.component.scss']
})
export class BulkRelationshipImportComponent {
  /**
   * Event emitted when import is completed
   */
  @Output() importCompleted = new EventEmitter<BulkImportResult>();

  /**
   * Import form
   */
  importForm!: FormGroup;

  /**
   * CSV file input
   */
  csvFile?: File;

  /**
   * Import method (manual or csv)
   */
  importMethod: 'manual' | 'csv' = 'manual';

  /**
   * Import state
   */
  isImporting = false;

  /**
   * Import result
   */
  importResult?: BulkImportResult;

  /**
   * Show help
   */
  showHelp = false;

  constructor(private fb: FormBuilder) {
    this.initializeForm();
  }

  /**
   * Initialize form
   */
  private initializeForm(): void {
    this.importForm = this.fb.group({
      relationships: this.fb.array([
        this.createRelationshipFormGroup()
      ])
    });
  }

  /**
   * Create relationship form group
   */
  private createRelationshipFormGroup(): FormGroup {
    return this.fb.group({
      parentName: ['', Validators.required],
      childName: ['', Validators.required],
      relationshipType: ['biological', Validators.required],
      notes: ['']
    });
  }

  /**
   * Get relationships form array
   */
  get relationships(): FormArray {
    return this.importForm.get('relationships') as FormArray;
  }

  /**
   * Add relationship row
   */
  addRelationship(): void {
    this.relationships.push(this.createRelationshipFormGroup());
  }

  /**
   * Remove relationship row
   */
  removeRelationship(index: number): void {
    if (this.relationships.length > 1) {
      this.relationships.removeAt(index);
    }
  }

  /**
   * Handle CSV file selection
   */
  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.csvFile = input.files[0];
      this.parseCSVFile();
    }
  }

  /**
   * Parse CSV file
   */
  private parseCSVFile(): void {
    if (!this.csvFile) return;

    const reader = new FileReader();
    reader.onload = (e) => {
      const text = e.target?.result as string;
      this.parseCsvText(text);
    };
    reader.readAsText(this.csvFile);
  }

  /**
   * Parse CSV text
   */
  private parseCsvText(text: string): void {
    const lines = text.split('\n');
    
    // Clear existing relationships
    while (this.relationships.length > 0) {
      this.relationships.removeAt(0);
    }

    // Skip header row
    for (let i = 1; i < lines.length; i++) {
      const line = lines[i].trim();
      if (!line) continue;

      const [parentName, childName, relationshipType, notes] = line.split(',').map(s => s.trim());
      
      if (parentName && childName) {
        const group = this.createRelationshipFormGroup();
        group.patchValue({
          parentName,
          childName,
          relationshipType: relationshipType || 'biological',
          notes: notes || ''
        });
        this.relationships.push(group);
      }
    }

    if (this.relationships.length === 0) {
      this.addRelationship();
    }
  }

  /**
   * Download CSV template
   */
  downloadTemplate(): void {
    const csvContent = 'Parent Name,Child Name,Relationship Type,Notes\n' +
                      'John Smith,Emily Smith,biological,\n' +
                      'Jane Doe,Michael Doe,adopted,';
    
    const blob = new Blob([csvContent], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = 'parent-child-import-template.csv';
    link.click();
    window.URL.revokeObjectURL(url);
  }

  /**
   * Import relationships
   */
  onImport(): void {
    if (this.importForm.valid && !this.isImporting) {
      this.isImporting = true;

      // Simulate import process
      setTimeout(() => {
        const total = this.relationships.length;
        const successful = Math.floor(total * 0.9); // 90% success rate
        const failed = total - successful;

        this.importResult = {
          total,
          successful,
          failed,
          errors: failed > 0 ? [
            {
              row: 2,
              parentName: 'Unknown Parent',
              childName: 'Unknown Child',
              error: 'Parent not found in database'
            }
          ] : []
        };

        this.isImporting = false;
        this.importCompleted.emit(this.importResult);
      }, 2000);
    }
  }

  /**
   * Reset form
   */
  onReset(): void {
    this.importResult = undefined;
    this.csvFile = undefined;
    this.initializeForm();
  }

  /**
   * Switch import method
   */
  onMethodChange(method: 'manual' | 'csv'): void {
    this.importMethod = method;
    if (method === 'manual') {
      this.csvFile = undefined;
    }
  }
}
