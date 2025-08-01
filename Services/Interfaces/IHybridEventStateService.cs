/*
    IHybridEventStateService Interface
    
    Purpose: Defines the contract for managing event form state and statistics caching across 
    components in the EventEase application, providing centralized state management for event operations.
    
    State Management:
    - CurrentEventForm: Tracks the event being created or edited in forms
    - IsFormDirty: Monitors unsaved changes to prevent data loss
    - CurrentEventPageSize: Maintains pagination state across navigation
    
    Statistics Caching:
    - RegistrationCounts: Cached registration counts per event for performance
    - AttendeeCounts: Cached attendance counts per event for real-time display
    - Reduces redundant database queries and improves UI responsiveness
    
    Form Operations:
    - InitializeForNewEventAsync: Sets up form state for creating new events
    - InitializeForEditAsync: Prepares form state with existing event data
    - SaveFormAsync: Persists form changes and updates cached statistics
    - ResetEventFormAsync: Clears form state and returns to clean state
    
    State Tracking:
    - MarkFormClean: Indicates form has no unsaved changes
    - MarkFormDirty: Flags form as having unsaved modifications
    - Prevents accidental data loss during navigation
    
    Design Patterns:
    - Singleton Pattern: Shared state across application components
    - Observer Pattern: Components can react to state changes
    - Disposable Pattern: Proper cleanup of resources and event handlers
    
    Usage: Implemented by hybrid state service and injected into event-related components
    to coordinate form state, caching, and cross-component communication in the EventEase application.
*/

using BlazorFinalProject.Models;

namespace BlazorFinalProject.Services.Interfaces
{
    public interface IHybridEventStateService : IDisposable
    {
        Event CurrentEventForm { get; }
        Dictionary<Guid, int> RegistrationCounts { get; }
        Dictionary<Guid, int> AttendeeCounts { get; }
        bool IsFormDirty { get; }
        public int? CurrentEventPageSize { get; set; }
        Task InitializeForNewEventAsync();
        Task InitializeForEditAsync(Event eventToEdit);
        void MarkFormClean();
        void MarkFormDirty();
        Task SaveFormAsync();
        Task ResetEventFormAsync();
    }
}