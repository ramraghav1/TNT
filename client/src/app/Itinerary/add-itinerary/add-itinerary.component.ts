import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-add-itinerary',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './add-itinerary.component.html',
  styleUrls: ['./add-itinerary.component.scss']
})
export class AddItineraryComponent {
  itineraryForm: FormGroup;
  dailyPlanForm: FormGroup;
  showPopup = false;
  editMode = false;
  editIndex: number | null = null;

  constructor(private fb: FormBuilder) {
    this.itineraryForm = this.fb.group({
      title: ['', Validators.required],
      description: [''],
      destination: ['', Validators.required],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
      dailyPlans: this.fb.array([]),
      transport: this.fb.group({
        transportMode: [''],
        departureTime: [''],
        arrivalTime: [''],
        bookingReference: ['']
      }),
      accommodation: this.fb.group({
        hotelName: [''],
        checkInDate: [''],
        checkOutDate: [''],
        address: ['']
      })
    });

    this.dailyPlanForm = this.fb.group({
      date: ['', Validators.required],
      activityTitle: ['', Validators.required],
      location: [''],
      startTime: [''],
      endTime: [''],
      notes: ['']
    });
  }

  get dailyPlans(): FormArray {
    return this.itineraryForm.get('dailyPlans') as FormArray;
  }

  openDailyPlanPopup() {
    this.dailyPlanForm.reset();
    this.showPopup = true;
    this.editMode = false;
    this.editIndex = null;
  }

  editDailyPlan(index: number) {
    const plan = this.dailyPlans.at(index).value;
    this.dailyPlanForm.patchValue(plan);
    this.showPopup = true;
    this.editMode = true;
    this.editIndex = index;
  }

  saveDailyPlan() {
    if (this.dailyPlanForm.valid) {
      if (this.editMode && this.editIndex !== null) {
        this.dailyPlans.at(this.editIndex).patchValue(this.dailyPlanForm.value);
      } else {
        this.dailyPlans.push(this.fb.group(this.dailyPlanForm.value));
      }
      this.closePopup();
    }
  }

  removeDailyPlan(index: number) {
    this.dailyPlans.removeAt(index);
  }

  closePopup() {
    this.showPopup = false;
  }

  onSubmit() {
    if (this.itineraryForm.valid) {
      console.log('Itinerary submitted:', this.itineraryForm.value);
    } else {
      this.itineraryForm.markAllAsTouched();
    }
  }
}