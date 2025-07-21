import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule], // Import CommonModule for directives like *ngIf
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent {
  dropdownOpen = false;

  constructor(private router: Router) {}

  toggleDropdown() {
    this.dropdownOpen = !this.dropdownOpen;
  }

  closeDropdown() {
    setTimeout(() => (this.dropdownOpen = false), 200); // allow click events
  }

  viewProfile() {
    this.router.navigate(['/profile']);
  }

  logout() {
    // Handle logout logic here
    console.log('Logging out...');
    this.router.navigate(['/login']);
  }
}
