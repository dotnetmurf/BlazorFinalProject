/*
    MockDataService Implementation
    
    Purpose: Provides mock data generation and seeding functionality for the EventEase application,
    managing sample data persistence through browser local storage for development and demonstration.
    
    Core Operations:
    - SeedRecordsAsync: Clears existing data and populates fresh sample events and registrations
    - SeedAllAsync: Comprehensive seeding with optional completion callback support
    
    Data Management:
    - Uses browser localStorage for client-side data persistence
    - Manages events and registrations as separate storage keys
    - Clears existing data before seeding to ensure clean state
    - Integrates with MockDataFactory for realistic sample data generation
    
    Storage Strategy:
    - EventsKey: "events" - stores all sample event records
    - RegistrationsKey: "registrations" - stores all sample registration records
    - JSON serialization through ILocalStorageService for browser compatibility
    - Asynchronous operations for non-blocking UI interactions
    
    Callback Support:
    - onSeedComplete: Optional callback executed after seeding completion
    - Enables UI updates, notifications, or post-seeding operations
    - Supports component refresh and state synchronization after data loading
    
    Dependencies:
    - ILocalStorageService: Browser storage abstraction for data persistence
    - MockDataFactory: Static factory for generating sample data objects
    - Implements IMockDataService interface for dependency injection
    
    Usage: Injected into components and pages to populate the EventEase application
    with realistic sample data for development, testing, and demonstration scenarios.
    Particularly useful for showcasing application features without requiring backend services.
*/

namespace BlazorFinalProject.Services.Mock;

public class MockDataService : IMockDataService
{
    private readonly ILocalStorageService _localStorage;
    private const string EventsKey = "events";
    private const string RegistrationsKey = "registrations";

    public MockDataService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task SeedRecordsAsync()
    {
        await _localStorage.RemoveItemAsync(EventsKey);
        await _localStorage.RemoveItemAsync(RegistrationsKey);

        var (events, registrations) = MockDataFactory.CreateSeedData();

        await _localStorage.SetItemAsync(EventsKey, events);
        await _localStorage.SetItemAsync(RegistrationsKey, registrations);
    }

    public async Task SeedAllAsync(Func<Task>? onSeedComplete = null)
    {
        await SeedRecordsAsync();

        if (onSeedComplete is not null)
        {
            await onSeedComplete();
        }
    }
}