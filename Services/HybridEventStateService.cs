/*
    HybridEventStateService Implementation
    
    Purpose: Provides centralized state management for event forms with automatic persistence to browser 
    localStorage, ensuring data integrity and preventing data loss during event creation and editing operations.
    
    Core Features:
    - Event form state management with thread-safe access
    - Automatic draft saving with configurable intervals (1 second)
    - Separate storage for new events vs. editing existing events
    - Cross-component state sharing for event operations
    - Statistics caching for registration and attendee counts
    - Pagination state preservation across navigation
    
    State Management:
    - CurrentEventForm: Thread-safe access to current form data
    - IsFormDirty: Tracks unsaved changes to prevent data loss
    - CurrentEventPageSize: Maintains pagination preferences
    - RegistrationCounts: Cached registration statistics per event
    - AttendeeCounts: Cached attendance statistics per event
    
    Auto-Save Features:
    - Timer-based automatic saving every 1000ms when form is dirty
    - Separate storage keys for new vs. edit scenarios
    - Draft recovery on form initialization
    - Graceful error handling for storage failures
    
    Storage Strategy:
    - NewEventDraftKey: "eventForm_newDraft" for new event drafts
    - EditEventDraftPrefix: "eventForm_edit_" + EventId for edit drafts
    - JSON serialization for browser localStorage compatibility
    - Automatic cleanup of drafts when forms are reset
    
    Thread Safety:
    - Lock-based synchronization for concurrent access protection
    - Safe property access across multiple components
    - Thread-safe timer operations for auto-save functionality
    
    Lifecycle Management:
    - InitializeForNewEventAsync: Sets up state for creating new events
    - InitializeForEditAsync: Prepares state for editing existing events
    - SaveFormAsync: Manual save trigger with clean state marking
    - ResetEventFormAsync: Cleanup and state reset with draft removal
    - Dispose: Proper timer cleanup and resource management
    
    Dependencies:
    - IJSRuntime: Browser localStorage access for persistence
    - System.Timers.Timer: Auto-save functionality
    - System.Text.Json: Serialization for storage operations
    - Implements IHybridEventStateService and IDisposable interfaces
    
    Usage: Injected as singleton into event-related components to provide consistent
    state management, draft persistence, and statistics caching throughout the EventEase application.
*/

using Microsoft.JSInterop;
using System.Text.Json;
using System.Timers;

namespace BlazorFinalProject.Services;

public class HybridEventStateService : IHybridEventStateService, IDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private System.Timers.Timer? _autoSaveTimer;
    private bool _isDirty = false;
    private readonly object _lockObject = new object();
    public int? CurrentEventPageSize { get; set; } = 4;

    private Event _currentEventForm = new Event();
    private Guid? _currentEditingEventId = null;

    public Event CurrentEventForm
    {
        get
        {
            lock (_lockObject)
            {
                return _currentEventForm;
            }
        }
        private set
        {
            lock (_lockObject)
            {
                _currentEventForm = value;
            }
        }
    }

    public bool IsFormDirty
    {
        get
        {
            lock (_lockObject)
            {
                return _isDirty;
            }
        }
    }

    public Dictionary<Guid, int> RegistrationCounts { get; set; } = new();
    public Dictionary<Guid, int> AttendeeCounts { get; set; } = new();

    private const int AutoSaveIntervalMs = 1000;
    private const string NewEventDraftKey = "eventForm_newDraft";
    private const string EditEventDraftPrefix = "eventForm_edit_";

    public HybridEventStateService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        InitializeAutoSave();
    }

    private void InitializeAutoSave()
    {
        _autoSaveTimer = new System.Timers.Timer(AutoSaveIntervalMs);
        _autoSaveTimer.Elapsed += OnAutoSaveTimerElapsed;
        _autoSaveTimer.AutoReset = true;
        _autoSaveTimer.Start();
    }

    public async Task InitializeForNewEventAsync()
    {
        _currentEditingEventId = null;

        try
        {
            var savedJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", NewEventDraftKey);
            if (!string.IsNullOrEmpty(savedJson))
            {
                var savedForm = JsonSerializer.Deserialize<Event>(savedJson);
                if (savedForm != null)
                {
                    CurrentEventForm = savedForm;
                    MarkClean();
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR loading new event draft: {ex.Message}");
        }

        CurrentEventForm = new Event();
        MarkClean();
    }

    public async Task InitializeForEditAsync(Event eventToEdit)
    {
        _currentEditingEventId = eventToEdit.Id;

        var editDraftKey = $"{EditEventDraftPrefix}{eventToEdit.Id}";

        try
        {
            var savedJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", editDraftKey);
            if (!string.IsNullOrEmpty(savedJson))
            {
                var savedForm = JsonSerializer.Deserialize<Event>(savedJson);
                if (savedForm != null && savedForm.Id == eventToEdit.Id)
                {
                    CurrentEventForm = savedForm;
                    MarkClean();
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR loading edit draft: {ex.Message}");
        }

        CurrentEventForm = new Event
        {
            Id = eventToEdit.Id,
            Name = eventToEdit.Name,
            Date = eventToEdit.Date,
            Location = eventToEdit.Location,
            Notes = eventToEdit.Notes
        };
        MarkClean();
    }

    public void MarkFormClean()
    {
        MarkClean();
    }

    public void MarkFormDirty()
    {
        MarkDirty();
    }

    public async Task SaveFormAsync()
    {
        await SaveToStorageAsync();
        MarkClean();
    }

    public async Task ResetEventFormAsync()
    {

        if (_currentEditingEventId.HasValue)
        {
            var editDraftKey = $"{EditEventDraftPrefix}{_currentEditingEventId.Value}";
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", editDraftKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR removing edit draft: {ex.Message}");
            }
        }
        else
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", NewEventDraftKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR removing new event draft: {ex.Message}");
            }
        }

        CurrentEventForm = new Event();
        _currentEditingEventId = null;
        MarkClean();
    }
    private void MarkDirty()
    {
        _isDirty = true;
    }

    private void MarkClean()
    {
        _isDirty = false;
    }

    private async void OnAutoSaveTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (_isDirty)
        {
            await SaveToStorageAsync();
            MarkClean();
        }
    }

    private async Task SaveToStorageAsync()
    {
        try
        {
            Event formToSave;
            lock (_lockObject)
            {
                formToSave = new Event
                {
                    Id = _currentEventForm.Id,
                    Name = _currentEventForm.Name,
                    Date = _currentEventForm.Date,
                    Location = _currentEventForm.Location,
                    Notes = _currentEventForm.Notes
                };
            }

            var json = JsonSerializer.Serialize(formToSave);
            string storageKey;

            if (_currentEditingEventId.HasValue)
            {
                storageKey = $"{EditEventDraftPrefix}{_currentEditingEventId.Value}";
            }
            else
            {
                storageKey = NewEventDraftKey;
            }

            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", storageKey, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR saving form to storage: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _autoSaveTimer?.Stop();
        _autoSaveTimer?.Dispose();
    }
}
