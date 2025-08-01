/*
    DateGreaterThanValidation Custom Validation Attribute
    
    Purpose: Provides custom validation logic to ensure event dates are not set in the past,
    enforcing business rules for event scheduling in the EventEase application.
    
    Validation Rules:
    - New events: Date must be today or in the future
    - Existing events: Date validation is bypassed (allows editing of past events)
    - Empty/null dates: Validation passes (handled by Required attribute separately)
    - Invalid date formats: Returns descriptive error message
    
    Business Logic:
    - Prevents creation of events with past dates
    - Allows editing of existing events regardless of date (preserves historical data)
    - Uses DateTime.Today as the minimum acceptable date for new events
    - Provides user-friendly error messages for validation failures
    
    Implementation Details:
    - Inherits from ValidationAttribute for integration with DataAnnotations
    - Uses ValidationContext to access the Event object being validated
    - Checks Event.Id to distinguish between new (Guid.Empty) and existing events
    - Handles string-to-DateTime conversion with error handling
    
    Usage: Applied as a data annotation to Event.Date property to enforce date constraints
    during form validation in event creation and editing scenarios.
*/

using System;
using System.ComponentModel.DataAnnotations;
using BlazorFinalProject.Models;

public class DateGreaterThanValidation : ValidationAttribute
{
    public const string MINIMUM_EVENT_DATE = "The event date must not be earlier than today";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var valueString = value != null ? value.ToString() : null;

        if (string.IsNullOrWhiteSpace(valueString))
        {
            return ValidationResult.Success;
        }

        if (!DateTime.TryParse(valueString, out DateTime eventDate))
        {
            return new ValidationResult("Unable to convert the event date to a valid date");
        }

        var eventObj = validationContext.ObjectInstance as Event;

        if (eventObj != null && eventObj.Id != Guid.Empty)
        {
            return ValidationResult.Success;
        }

        DateTime minEventDate = DateTime.Today;

        if (eventDate < minEventDate)
        {
            return new ValidationResult(MINIMUM_EVENT_DATE);
        }

        return ValidationResult.Success;
    }
}