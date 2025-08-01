/*
    HybridRegistrationStateService Implementation
    
    Purpose: Provides centralized state management for registration forms with automatic persistence to browser 
    localStorage, ensuring data integrity and preventing data loss during registration creation and editing operations.
    
    Core Features:
    - Registration form state management with thread-safe access
    - Automatic draft saving with configurable intervals (1 second)
    - Separate storage for new registrations vs. editing existing registrations
    - Cross-component state sharing for registration operations
    - Pagination state preservation per event across navigation
    - Edit mode detection for form behavior customization
    
    State Management:
    - CurrentRegistrationForm: Thread-safe access to current form data
    - IsFormDirty: Tracks unsaved changes to prevent data loss
    - IsEditing: Distinguishes between create and edit modes
    - CurrentRegistrationPageSizes: Maintains pagination preferences per event
    
    Auto-Save Features:
    - Timer-based automatic saving every 1000ms when form is dirty
    - Separate storage keys for new vs. edit scenarios
    - Draft recovery on form initialization
    - Graceful error handling for storage failures
    
    Storage Strategy:
    - NewRegistrationDraftKey: "registrationForm_newDraft" for new registration drafts
    - EditRegistrationDraftPrefix: "registrationForm_edit_" + RegistrationId for edit drafts
    - JSON serialization for browser localStorage compatibility
    - Automatic cleanup of drafts when forms are reset
    
    Thread Safety:
    - Lock-based synchronization for concurrent access protection
    - Safe property access across multiple components
    - Thread-safe timer operations for auto-save functionality
    
    Lifecycle Management:
    - InitializeForNewRegistrationAsync: Sets up state for creating new registrations
    - InitializeForEditRegistrationAsync: Prepares state for editing existing registrations
    - SaveRegistrationFormAsync: Manual save trigger with clean state marking
    - ResetRegistrationFormAsync: Cleanup and state reset with draft removal
    - Dispose: Proper timer cleanup and resource management
    
    Dependencies:
    - IJSRuntime: Browser localStorage access for persistence
    - ILogger<HybridRegistrationStateService>: Structured logging for monitoring
    - System.Timers.Timer: Auto-save functionality
    - System.Text.Json: Serialization for storage operations
    - Implements IHybridRegistrationStateService and IDisposable interfaces
    
    Usage: Injected into registration-related components to provide consistent
    state management, draft persistence, and pagination across the EventEase application.
*/

using Microsoft.JSInterop;
using System.Text.Json;
using System.Timers;
using BlazorFinalProject.Models;

namespace BlazorFinalProject.Services;

public class HybridRegistrationStateService : IHybridRegistrationStateService, IDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<HybridRegistrationStateService> _logger;
    private System.Timers.Timer? _autoSaveTimer;
    private bool _isDirty = false;
    private readonly object _lockObject = new object();
    public Dictionary<Guid, int> CurrentRegistrationPageSizes { get; set; } = new();

    private Registration _currentRegistrationForm = new Registration();
    private Guid? _currentEditingRegistrationId = null;

    public Registration? CurrentRegistrationForm
    {
        get
        {
            lock (_lockObject)
            {
                return _currentRegistrationForm;
            }
        }
        private set
        {
            lock (_lockObject)
            {
                _currentRegistrationForm = value ?? new Registration();
            }
        }
    }

    public bool IsEditing => _currentEditingRegistrationId.HasValue && _currentEditingRegistrationId != Guid.Empty;

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

    private const int AutoSaveIntervalMs = 1000;
    private const string NewRegistrationDraftKey = "registrationForm_newDraft";
    private const string EditRegistrationDraftPrefix = "registrationForm_edit_";

    public HybridRegistrationStateService(IJSRuntime jsRuntime, ILogger<HybridRegistrationStateService> logger)
    {
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        InitializeAutoSave();
    }

    private void InitializeAutoSave()
    {
        _autoSaveTimer = new System.Timers.Timer(AutoSaveIntervalMs);
        _autoSaveTimer.Elapsed += OnAutoSaveTimerElapsed;
        _autoSaveTimer.AutoReset = true;
        _autoSaveTimer.Start();
    }

    public async Task InitializeForNewRegistrationAsync(Guid eventId)
    {
        try
        {
            if (eventId == Guid.Empty)
                throw new ArgumentException("Event ID cannot be empty", nameof(eventId));

            _currentEditingRegistrationId = null;

            try
            {
                var savedJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", NewRegistrationDraftKey);
                if (!string.IsNullOrEmpty(savedJson))
                {
                    var savedForm = JsonSerializer.Deserialize<Registration>(savedJson);
                    if (savedForm != null && savedForm.EventId == eventId)
                    {
                        CurrentRegistrationForm = savedForm;
                        MarkClean();
                        _logger.LogInformation("Loaded existing draft for new registration (Event: {EventId})", eventId);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error loading new registration draft");
            }

            CurrentRegistrationForm = new Registration
            {
                Id = Guid.Empty,
                EventId = eventId,
                AttendeeName = string.Empty,
                Telephone = string.Empty,
                EmailAddress = string.Empty,
                Notes = string.Empty,
                AttendedEvent = false
            };

            MarkClean();
            _logger.LogInformation("Initialized new registration form for event {EventId}", eventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing new registration form for event {EventId}", eventId);
            throw;
        }
    }

    public async Task InitializeForEditRegistrationAsync(Registration registration)
    {
        try
        {
            if (registration == null)
                throw new ArgumentNullException(nameof(registration));

            _currentEditingRegistrationId = registration.Id;
            var editDraftKey = $"{EditRegistrationDraftPrefix}{registration.Id}";

            try
            {
                var savedJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", editDraftKey);
                if (!string.IsNullOrEmpty(savedJson))
                {
                    var savedForm = JsonSerializer.Deserialize<Registration>(savedJson);
                    if (savedForm != null && savedForm.Id == registration.Id)
                    {
                        CurrentRegistrationForm = savedForm;
                        MarkClean();
                        _logger.LogInformation("Loaded existing edit draft for registration {RegistrationId}", registration.Id);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error loading edit draft for registration {RegistrationId}", registration.Id);
            }

            CurrentRegistrationForm = new Registration
            {
                Id = registration.Id,
                EventId = registration.EventId,
                AttendeeName = registration.AttendeeName,
                Telephone = registration.Telephone,
                EmailAddress = registration.EmailAddress,
                Notes = registration.Notes,
                AttendedEvent = registration.AttendedEvent
            };

            MarkClean();
            _logger.LogInformation("Initialized edit registration form for registration {RegistrationId}", registration.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing edit registration form for registration {RegistrationId}", registration?.Id);
            throw;
        }
    }

    public void MarkFormClean()
    {
        MarkClean();
    }

    public void MarkFormDirty()
    {
        MarkDirty();
    }

    public async Task SaveRegistrationFormAsync()
    {
        await SaveToStorageAsync();
        MarkClean();
    }

    public async Task ResetRegistrationFormAsync()
    {
        try
        {
            if (_currentEditingRegistrationId.HasValue)
            {
                var editDraftKey = $"{EditRegistrationDraftPrefix}{_currentEditingRegistrationId.Value}";
                try
                {
                    await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", editDraftKey);
                    _logger.LogInformation("Cleared edit draft for registration {RegistrationId}", _currentEditingRegistrationId.Value);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error removing edit draft for registration {RegistrationId}", _currentEditingRegistrationId.Value);
                }
            }
            else
            {
                try
                {
                    await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", NewRegistrationDraftKey);
                    _logger.LogInformation("Cleared new registration draft");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error removing new registration draft");
                }
            }

            CurrentRegistrationForm = new Registration();
            _currentEditingRegistrationId = null;
            MarkClean();

            _logger.LogInformation("Registration form reset");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting registration form");
            throw;
        }
    }

    public bool HasUnsavedChanges()
    {
        return IsFormDirty;
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
            Registration formToSave;
            lock (_lockObject)
            {
                formToSave = new Registration
                {
                    Id = _currentRegistrationForm.Id,
                    EventId = _currentRegistrationForm.EventId,
                    AttendeeName = _currentRegistrationForm.AttendeeName,
                    Telephone = _currentRegistrationForm.Telephone,
                    EmailAddress = _currentRegistrationForm.EmailAddress,
                    Notes = _currentRegistrationForm.Notes,
                    AttendedEvent = _currentRegistrationForm.AttendedEvent
                };
            }

            var json = JsonSerializer.Serialize(formToSave);
            string storageKey;

            if (_currentEditingRegistrationId.HasValue)
            {
                storageKey = $"{EditRegistrationDraftPrefix}{_currentEditingRegistrationId.Value}";
            }
            else
            {
                storageKey = NewRegistrationDraftKey;
            }

            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", storageKey, json);
            _logger.LogDebug("Registration form saved to storage with key: {StorageKey}", storageKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving registration form to storage");
        }
    }

    public void Dispose()
    {
        _autoSaveTimer?.Stop();
        _autoSaveTimer?.Dispose();
        _autoSaveTimer = null;
    }
}