import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { NgChartsModule } from 'ng2-charts';
import { ChartConfiguration } from 'chart.js';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, NgChartsModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent {
  public barChartOptions: ChartConfiguration<'bar'>['options'] = {
    responsive: true,
    maintainAspectRatio: false, // ✅ important
    plugins: {
      legend: {
        display: true,
        position: 'top' as const
      }
    }
  }; // Fixed closing brace for barChartOptions

  public barChartLabels: string[] = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul'];

  public barChartData: ChartConfiguration<'bar'>['data'] = {
    labels: this.barChartLabels,
    datasets: [
      { data: [65, 59, 80, 81, 56, 55, 40], label: 'Approved', backgroundColor: '#1E88E5' },
      { data: [28, 48, 40, 19, 86, 27, 90], label: 'Engaged', backgroundColor: '#FDD835' }
    ]
  };

  public barChartType: 'bar' = 'bar';

  public pieChartLabels: string[] = [
    'Europe Explorer',
    'Himalayan Adventure',
    'Beach Retreat Bali',
    'Desert Safari Dubai',
    'Tokyo City Lights',
    'Amazon Jungle Trek'
  ];

  public pieChartData = {
    labels: this.pieChartLabels,
    datasets: [
      {
        data: [120, 95, 80, 70, 65, 50],
        backgroundColor: [
          '#1E88E5', // Dark Blue
          '#42A5F5', // Standard Blue
          '#90CAF9', // Light Blue
          '#64B5F6', // Medium Blue
          '#1565C0', // Deep Blue
          '#BBDEFB'  // Faded Blue
        ]
      }
    ]
  };

  public pieChartOptions: ChartConfiguration<'pie'>['options'] = {
    responsive: true,
    maintainAspectRatio: false, // ✅ important
    plugins: {
      legend: {
        position: 'top' as const
      },
      title: {
        display: true,
        text: 'Top 6 Most Approved Itineraries'
      }
    }
  };

  public pieChartType: 'pie' = 'pie';

  constructor(private router: Router) {} // Fixed closing brace for constructor

  navigateToAddItinerary() {
    this.router.navigate(['/add-itinerary']);
  }
}
