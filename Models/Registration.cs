/*
    Registration Model Class
    
    Purpose: Represents an event registration entity in the EventEase application with comprehensive 
    data validation for attendee information and event participation tracking.
    
    Properties:
    - Id: Unique identifier for the registration (Guid)
    - EventId: Foreign key linking registration to specific event (required)
    - AttendeeName: Full name of the person registering (required, 100-character limit)
    - Telephone: Contact phone number with specific format validation (required)
    - EmailAddress: Valid email address for communication (required, 254-character limit)
    - Notes: Optional additional information (500-character limit)
    - AttendedEvent: Boolean flag tracking actual attendance (defaults to false)
    
    Validation Rules:
    - AttendeeName: Required field with descriptive error message and length constraint
    - Telephone: Required with regex pattern validation (format: ###-###-####)
    - EmailAddress: Required with built-in email format validation
    - Notes: Optional field with maximum length constraint
    - EventId: Required to ensure registration is linked to an event
    
    Business Logic:
    - AttendedEvent defaults to false (attendance marked separately from registration)
    - Phone number format enforces North American numbering plan structure
    - Email length follows RFC 5321 specification (254 characters maximum)
    - String length limits ensure database compatibility and UI consistency
    
    Data Annotations:
    - Required: Enforces mandatory field validation with custom error messages
    - StringLength: Prevents data truncation and ensures consistent display
    - RegularExpression: Enforces telephone number format validation
    - EmailAddress: Provides built-in email format validation
    
    Usage: Core entity for event registration management, used in registration forms,
    attendee lists, attendance tracking, and reporting throughout the EventEase application.
*/

using System.ComponentModel.DataAnnotations;

namespace BlazorFinalProject.Models
{
    public class Registration
    {
        public Guid Id { get; set; }

        [Required]
        public Guid EventId { get; set; }

        [Required(ErrorMessage = "Please enter a name.")]
        [StringLength(100)]
        public string AttendeeName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a telephone number.")]
        [RegularExpression(@"^[1-9][0-9]{2}-[1-9]{3}-[0-9]{4}$", ErrorMessage = "Invalid telephone number.")]
        [StringLength(12)]
        public string Telephone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a valid email address.")]
        [EmailAddress]
        [StringLength(254)]
        public string EmailAddress { get; set; } = string.Empty;

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;

        public bool AttendedEvent { get; set; } = false;
    }
}