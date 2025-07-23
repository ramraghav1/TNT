import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
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
    this.dropdownOpen = false;
  }

  viewProfile() {
    console.log('View Profile clicked');
    // Add navigation logic here if needed
  }

  logout() {
    console.log('Logout clicked');
    // Redirect to login page
    this.router.navigate(['/login']);
  }
}
