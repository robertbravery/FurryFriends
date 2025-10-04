# FurryFriends Booking System Documentation

## Overview
The FurryFriends Booking System is a comprehensive solution for managing pet walking appointments between pet owners and professional pet walkers. The system provides a complete workflow from PetWalker selection to booking confirmation.

## Architecture

### **Frontend Architecture**
- **Framework**: Blazor Hybrid (Server + WebAssembly)
- **Render Mode**: InteractiveAuto
- **Component Structure**: Modular, reusable components
- **Styling**: CSS with custom variables and responsive design
- **State Management**: Component-based with event callbacks

### **Backend Architecture**
- **API**: FastEndpoints with Clean Architecture
- **Database**: Entity Framework Core with SQL Server
- **Domain**: Domain-Driven Design with Aggregates
- **Services**: CQRS pattern with MediatR
- **Validation**: FluentValidation

## Components Overview

### **1. PetWalkerSelectionComponent**
**Purpose**: Display available pet walkers for selection
**Features**:
- Grid layout with walker cards
- Profile pictures, ratings, and badges
- Service area filtering
- Interactive selection with visual feedback

**Key Properties**:
- `ServiceArea`: Filter walkers by location
- `SelectedPetWalkerId`: Currently selected walker
- `OnPetWalkerSelected`: Callback when walker is selected

### **2. ScheduleDisplayComponent**
**Purpose**: Show pet walker's weekly schedule and available time slots
**Features**:
- Weekly calendar view
- Daily time slot details
- Real-time availability loading
- Interactive date and time selection

**Key Properties**:
- `PetWalkerId`: Walker to display schedule for
- `SelectedDate`: Currently selected date
- `OnTimeSlotSelected`: Callback when time slot is selected

### **3. DateTimeSelectionComponent**
**Purpose**: Advanced date and time selection with validation
**Features**:
- Date picker with business rules
- Available time slots display
- Custom time range option
- Real-time availability validation
- Booking summary display

**Key Properties**:
- `PetWalkerId`: Walker for availability checking
- `AllowCustomTime`: Enable custom time selection
- `OnTimeSelectionChanged`: Callback when selection changes

### **4. BookingFormComponent**
**Purpose**: Main booking form orchestrating the entire process
**Features**:
- Multi-step wizard interface
- Progress indicator
- Form validation
- Pet selection
- Special instructions input
- Price calculation

**Key Properties**:
- `ClientId`: Client creating the booking
- `ServiceArea`: Location filter
- `OnBookingCompleted`: Callback when booking is created

### **5. BookingConfirmationComponent**
**Purpose**: Final confirmation and booking summary
**Features**:
- Comprehensive booking review
- Pet walker and pet information
- Price breakdown
- Important terms and conditions
- Confirmation and edit options

**Key Properties**:
- `BookingRequest`: Complete booking details
- `SelectedPetWalker`: Walker information
- `SelectedPet`: Pet information
- `OnConfirm`: Final confirmation callback

### **6. BookingManagement Page**
**Purpose**: Main entry point for booking management
**Features**:
- Client selection interface
- Booking form integration
- Success confirmation modal
- Navigation and routing
- Error handling

**Routes**:
- `/bookings` - Main booking management
- `/bookings/new` - New booking creation
- `/bookings/new/{ClientId}` - New booking for specific client

## API Endpoints

### **Booking Endpoints**
- `POST /Bookings` - Create new booking
- `GET /petwalker/{id}/available-slots?date={date}` - Get available time slots
- `GET /petwalker/{id}/can-book?startTime={start}&endTime={end}` - Check availability
- `GET /bookings/client/{id}?startDate={start}&endDate={end}` - Get client bookings
- `GET /bookings/petwalker/{id}?startDate={start}&endDate={end}` - Get walker bookings
- `GET /petwalkers/available?serviceArea={area}` - Get available walkers

### **Supporting Endpoints**
- `GET /petwalker/{id}/schedule` - Get walker schedule
- `GET /clients/{id}/pets` - Get client's pets
- `GET /clients` - Get clients list

## Data Models

### **Core DTOs**
```csharp
// Booking Request
public class BookingRequestDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid PetId { get; set; }
    public Guid PetWalkerId { get; set; }
    public Guid PetOwnerId { get; set; }
    public decimal Price { get; set; }
    public string? Notes { get; set; }
}

// Booking Response
public class BookingResponseDto
{
    public Guid BookingId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Message { get; set; }
    public bool Success { get; set; }
}

// Available Slot
public class AvailableSlotDto
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsAvailable { get; set; }
    public decimal Price { get; set; }
    public string? Notes { get; set; }
}

// PetWalker Summary
public class PetWalkerSummaryDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public decimal HourlyRate { get; set; }
    public bool IsVerified { get; set; }
    public int YearsOfExperience { get; set; }
    public PhotoDto? BioPicture { get; set; }
    public List<string> ServiceAreas { get; set; }
}
```

## Styling System

### **CSS Architecture**
- **booking-components.css**: Core component styles with CSS variables
- **booking-responsive.css**: Mobile-first responsive design
- **booking-themes.css**: Multiple theme variations

### **Theme Support**
- **Default**: Standard blue theme
- **Dark**: Dark mode with adjusted colors
- **High Contrast**: Accessibility-focused high contrast
- **Colorful**: Gradient-based colorful theme
- **Minimal**: Clean, minimal black and white theme

### **Responsive Breakpoints**
- **Mobile**: < 576px
- **Small**: 576px - 767px
- **Medium**: 768px - 991px
- **Large**: 992px - 1199px
- **Extra Large**: ≥ 1200px

## User Flow

### **Complete Booking Process**
1. **Entry**: User navigates to `/bookings`
2. **Client Selection**: Choose client for booking (if not specified)
3. **PetWalker Selection**: Browse and select available pet walkers
4. **Schedule Review**: View walker's schedule and availability
5. **Date/Time Selection**: Choose specific date and time slot
6. **Booking Details**: Select pet and add special instructions
7. **Confirmation**: Review all details and confirm booking
8. **Success**: Receive confirmation and booking ID

### **Validation Rules**
- **Date**: Must be in the future, within 3 months
- **Time**: Must be during walker's available hours
- **Pet**: Must belong to the selected client
- **Walker**: Must be active and verified
- **Availability**: Real-time checking against existing bookings

## Error Handling

### **Client-Side Validation**
- Form field validation with immediate feedback
- Business rule validation (dates, times, availability)
- User-friendly error messages
- Retry mechanisms for failed operations

### **Server-Side Integration**
- API response handling with proper error mapping
- Network error recovery
- Loading states and progress indicators
- Graceful degradation for offline scenarios

## Testing

### **Component Testing**
- Individual component functionality
- Props and event handling
- State management
- Responsive behavior

### **Integration Testing**
- Complete booking workflow
- API integration
- Error scenarios
- Cross-browser compatibility

### **Accessibility Testing**
- Keyboard navigation
- Screen reader compatibility
- High contrast mode
- Focus management

## Performance Considerations

### **Optimization Strategies**
- Lazy loading of components
- Efficient API calls with caching
- Optimized CSS with minimal bundle size
- Image optimization for profile pictures
- Debounced search and filtering

### **Loading States**
- Skeleton screens for data loading
- Progressive enhancement
- Smooth transitions between states
- Error boundaries for fault tolerance

## Security

### **Client-Side Security**
- Input validation and sanitization
- XSS prevention
- CSRF protection
- Secure API communication

### **Data Protection**
- Personal information handling
- Secure transmission of booking data
- Client-side data minimization
- Privacy-compliant logging

## Deployment

### **Build Process**
- CSS bundling and minification
- Component compilation
- Asset optimization
- Environment-specific configuration

### **Browser Support**
- Modern browsers (Chrome, Firefox, Safari, Edge)
- Mobile browsers (iOS Safari, Chrome Mobile)
- Progressive enhancement for older browsers
- Polyfills for missing features

## Future Enhancements

### **Planned Features**
- Real-time notifications
- Calendar integration
- Payment processing
- Review and rating system
- Advanced scheduling options
- Multi-language support

### **Technical Improvements**
- Performance monitoring
- Advanced caching strategies
- Offline functionality
- Push notifications
- Analytics integration
