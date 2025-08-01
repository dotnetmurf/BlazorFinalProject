/*
    IHybridRegistrationStateService Interface
    
    Purpose: Defines the contract for managing registration form state and pagination across 
    components in the EventEase application, providing centralized state management for registration operations.
    
    State Management:
    - CurrentRegistrationForm: Tracks the registration being created or edited in forms
    - IsEditing: Indicates whether the form is in edit mode (vs. create mode)
    - IsFormDirty: Monitors unsaved changes to prevent data loss
    - CurrentRegistrationPageSizes: Maintains pagination state per event for registration lists
    
    Form Operations:
    - InitializeForNewRegistrationAsync: Sets up form state for creating new registrations
    - InitializeForEditRegistrationAsync: Prepares form state with existing registration data
    - SaveRegistrationFormAsync: Persists form changes and handles data validation
    - ResetRegistrationFormAsync: Clears form state and returns to clean state
    
    State Tracking:
    - MarkFormClean: Indicates form has no unsaved changes
    - MarkFormDirty: Flags form as having unsaved modifications
    - HasUnsavedChanges: Checks for any pending changes to prevent data loss
    - Prevents accidental data loss during navigation or form closure
    
    Pagination Management:
    - Tracks registration list page sizes per individual event
    - Preserves user preferences for pagination across navigation
    - Enables different page sizes for different events simultaneously
    
    Design Patterns:
    - Singleton Pattern: Shared state across application components
    - State Machine: Manages form lifecycle states (new, edit, clean, dirty)
    - Repository Pattern: Abstracts registration data operations
    
    Usage: Implemented by hybrid registration state service and injected into registration-related 
    components to coordinate form state, pagination, and cross-component communication in the EventEase application.
*/

namespace BlazorFinalProject.Services;

public interface IHybridRegistrationStateService
{
    Registration? CurrentRegistrationForm { get; }
    public Dictionary<Guid, int> CurrentRegistrationPageSizes { get; set; }
    bool IsEditing { get; }
    bool IsFormDirty { get; }
    Task InitializeForNewRegistrationAsync(Guid eventId);
    Task InitializeForEditRegistrationAsync(Registration registration);
    void MarkFormClean();
    void MarkFormDirty();
    Task SaveRegistrationFormAsync();
    Task ResetRegistrationFormAsync();
    bool HasUnsavedChanges();
}