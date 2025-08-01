/*
    EventStatistics Model Class
    
    Purpose: Provides statistical data and calculations for event attendance and registration metrics
    in the EventEase application.
    
    Properties:
    - RegistrationCount: Total number of people registered for an event
    - AttendeeCount: Number of registered people who actually attended the event
    - AttendanceRate: Calculated percentage of registrants who attended (read-only)
    
    Calculations:
    - AttendanceRate: Automatically calculated as (AttendeeCount / RegistrationCount) * 100
    - Division by zero protection: Returns 0% when no registrations exist
    - Percentage format: Returns decimal value that can be formatted as percentage in UI
    
    Business Logic:
    - Tracks event participation effectiveness through attendance rates
    - Provides insights for event planning and capacity management
    - Supports reporting and analytics features
    - Helps identify popular events and attendance patterns
    
    Usage: Used in event cards, details views, and reporting components to display
    registration and attendance metrics. Calculated values update automatically
    when RegistrationCount or AttendeeCount properties change.
*/

public class EventStatistics
{
    public int RegistrationCount { get; set; }
    public int AttendeeCount { get; set; }
    public double AttendanceRate => RegistrationCount > 0 ? (double)AttendeeCount / RegistrationCount * 100 : 0;
}