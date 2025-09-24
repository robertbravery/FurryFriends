# Booking Components Testing Instructions

## Overview
We've successfully implemented the first 5 components of the booking system. Here's how to test them:

## What We've Built

### ✅ Completed Components:
1. **BookingService** - Full HTTP client service for booking operations
2. **API Endpoints** - All necessary endpoints for the UI
3. **PetWalkerSelectionComponent** - Rich PetWalker selection interface
4. **ScheduleDisplayComponent** - Calendar-like schedule view
5. **DateTimeSelectionComponent** - Advanced date/time picker with validation

### 🧪 Test Page Created:
- **URL**: `/booking-test`
- **Navigation**: Added "Booking Test" under "Bookings" section in the sidebar

## How to Test

### 1. Start the Application
```bash
# Navigate to the solution directory
cd src/FurryFriends.BlazorUI

# Run the application
dotnet run
```

### 2. Access the Test Page
1. Open your browser and navigate to the application
2. Look for "Bookings" section in the sidebar navigation
3. Click on "🧪 Booking Test"

### 3. Test Individual Components

#### **PetWalker Selection Component**
- Click "PetWalker Selection" button
- Should display available PetWalkers in a grid layout
- Each card shows: profile picture, name, hourly rate, experience, badges
- Click on a PetWalker card to select it
- Selected PetWalker should be highlighted and show in the "Current Selection" area

#### **Schedule Display Component**
- First select a PetWalker (required)
- Click "Schedule Display" button
- Should show a weekly calendar view with the PetWalker's availability
- Click on different days to see available time slots
- Time slots should load dynamically for each selected date

#### **Date/Time Selection Component**
- First select a PetWalker (required)
- Click "Date/Time Selection" button
- Use the date picker to select a future date
- Available time slots should load automatically
- Click on time slots to select them
- Try the "custom time range" option if enabled

#### **All Components Together**
- Click "All Components" to see the full workflow
- Test the interaction between components:
  1. Select a PetWalker
  2. Choose a date
  3. Pick a time slot
- Watch how selections flow between components

### 4. Debug Information
The test page includes a debug section showing:
- Currently selected component
- PetWalker ID
- Selected date and time
- All current state values

## Expected Behavior

### **Success Scenarios:**
- PetWalkers load and display properly
- Clicking PetWalkers updates the selection
- Schedules load when a PetWalker is selected
- Date selection triggers time slot loading
- All selections are reflected in the debug section

### **Error Scenarios to Test:**
- No PetWalkers available (should show appropriate message)
- No schedule available for a PetWalker
- No time slots available for a selected date
- Network errors (if API is not running)

## Troubleshooting

### **If PetWalkers Don't Load:**
- Check that the Web API is running
- Verify the `ApiBaseUrl` configuration
- Check browser console for HTTP errors
- Ensure PetWalker data exists in the database

### **If Schedules Don't Load:**
- Ensure the selected PetWalker has schedule data
- Check the GetSchedule API endpoint
- Verify the PetWalker ID is being passed correctly

### **If Time Slots Don't Load:**
- Check the GetAvailableSlots API endpoint
- Ensure the date is being passed correctly
- Verify the PetWalker has availability on the selected date

## API Dependencies

The components depend on these API endpoints:
- `GET /petwalkers/available` - Get available PetWalkers
- `GET /petwalker/{id}/schedule` - Get PetWalker schedule
- `GET /petwalker/{id}/available-slots?date={date}` - Get available time slots
- `GET /petwalker/{id}/can-book?startTime={start}&endTime={end}` - Check availability

## Next Steps

After testing these components, we can proceed with:
1. **BookingFormComponent** - Combine all components into a booking form
2. **BookingConfirmationComponent** - Show booking summary and confirmation
3. **BookingManagementPage** - Main page orchestrating the booking flow
4. **CSS Styling** - Polish the visual design
5. **Integration Testing** - End-to-end booking flow testing

## Logging

All components include comprehensive logging. Check the browser console and server logs for detailed information about component behavior and any errors.
