import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddItineraryComponent } from './add-itinerary.component';

describe('AddItineraryComponent', () => {
  let component: AddItineraryComponent;
  let fixture: ComponentFixture<AddItineraryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddItineraryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddItineraryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
