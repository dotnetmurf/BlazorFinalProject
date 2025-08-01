/*
    Event Model Class
    
    Purpose: Represents an event entity in the EventEase application with comprehensive data validation 
    and business rules for event management and scheduling.
    
    Properties:
    - Id: Unique identifier for the event (Guid)
    - Name: Event title with required validation and 100-character limit
    - Date: Event date with required validation and custom date constraint
    - Location: Event venue with required validation and 200-character limit
    - Notes: Optional additional information with 500-character limit
    
    Validation Rules:
    - Name: Required field with descriptive error message and length constraint
    - Date: Required, must be today or future date for new events (DateGreaterThanValidation)
    - Location: Required field with descriptive error message and length constraint
    - Notes: Optional field with maximum length constraint for data integrity
    
    Business Logic:
    - Default date set to today for new event creation
    - Custom validation prevents scheduling events in the past
    - String length limits ensure database compatibility and UI consistency
    - Empty string defaults prevent null reference issues
    
    Data Annotations:
    - Required: Enforces mandatory field validation
    - StringLength: Prevents data truncation and ensures consistent UI display
    - DataType.Date: Provides proper date picker rendering in forms
    - DateGreaterThanValidation: Custom attribute for business rule enforcement
    
    Usage: Core entity for event creation, editing, and display throughout the EventEase application.
    Used in forms, listings, details views, and data persistence operations.
*/

using System.ComponentModel.DataAnnotations;

namespace BlazorFinalProject.Models
{
    public class Event
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Please enter an event name.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [DataType(DataType.Date), Required, DateGreaterThanValidation]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Please enter an event location.")]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
    }
}