import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

/**
 * NotFoundComponent - 404 Error Page
 * 
 * Displays a user-friendly 404 error message when a route is not found.
 * Provides helpful navigation options to get back to the application.
 */
@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule],
  templateUrl: './not-found.component.html',
  styleUrls: ['./not-found.component.scss']
})
export class NotFoundComponent {
  /**
   * Navigate to home page
   */
  goHome(): void {
    window.location.href = '/';
  }

  /**
   * Navigate back to previous page
   */
  goBack(): void {
    window.history.back();
  }

  /**
   * Navigate to people page
   */
  goToPeople(): void {
    window.location.href = '/Person';
  }

  /**
   * Navigate to search
   */
  goToSearch(): void {
    const searchBox = document.querySelector('input[type="search"]') as HTMLInputElement;
    if (searchBox) {
      searchBox.focus();
    }
  }
}
