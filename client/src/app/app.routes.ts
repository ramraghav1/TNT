import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { AddItineraryComponent } from './Itinerary/add-itinerary/add-itinerary.component';
export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'dashboard',
    component: DashboardComponent
  },
  {
    path: 'add-itinerary',
    component: AddItineraryComponent // Ensure this route is correctly defined
  },
  {
    path: '**',
    redirectTo: 'login'
  }
];