/*
    IMockDataService Interface
    
    Purpose: Defines the contract for generating and seeding sample data in the EventEase application
    for demonstration, testing, and development purposes.
    
    Core Operations:
    - SeedRecordsAsync: Generates and inserts sample event records into the data store
    - SeedAllAsync: Comprehensive data seeding including events, registrations, and attendee data
    
    Data Generation Features:
    - Creates realistic sample events with varied dates, locations, and details
    - Generates corresponding registrations with diverse attendee information
    - Populates attendance tracking data for demonstration of reporting features
    - Provides consistent, repeatable test data for development and demos
    
    Use Cases:
    - Application demonstration and showcase scenarios
    - Development environment setup with realistic data
    - Testing UI components with populated data sets
    - Training scenarios for new users learning the application
    - Quality assurance testing with known data sets
    
    Callback Support:
    - onSeedComplete: Optional callback function executed after seeding completion
    - Enables UI updates, notifications, or post-seeding operations
    - Supports asynchronous completion handling for better user experience
    
    Design Patterns:
    - Factory Pattern: Generates structured sample data objects
    - Async/Await: Non-blocking data generation and insertion operations
    - Callback Pattern: Flexible completion handling for different scenarios
    
    Usage: Implemented by mock data service and used in development builds
    to quickly populate the EventEase application with sample data for testing and demonstration.
*/

namespace BlazorFinalProject.Services.Interfaces
{
    public interface IMockDataService
    {
        Task SeedRecordsAsync();
        Task SeedAllAsync(Func<Task>? onSeedComplete = null);
    }
}