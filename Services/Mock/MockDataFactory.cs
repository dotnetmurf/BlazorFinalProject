/*
    MockDataFactory Static Class
    
    Purpose: Provides factory methods for generating realistic sample data for the EventEase application,
    including events, registrations, and attendee information for development, testing, and demonstration.
    
    Core Functionality:
    - CreateSeedData: Main factory method returning complete dataset of events and registrations
    - CreateEventIds: Generates consistent GUIDs for predefined event categories
    - CreateEvents: Creates diverse event objects with varied dates, locations, and descriptions
    - CreateRegistrations: Populates realistic attendee data linked to specific events
    
    Sample Data Features:
    - 10 distinct technology-focused events (conferences, workshops, meetups)
    - Mixed past and future event dates for realistic demonstration scenarios
    - Diverse geographic locations across major US cities
    - Comprehensive registration data with varied attendee profiles
    - Realistic contact information with proper formatting
    - Attendance tracking with both registered and attended participants
    - Special accommodation notes and preferences for accessibility testing
    
    Event Categories:
    - Developer conferences and summits
    - Technical workshops and bootcamps
    - Industry forums and networking events
    - Startup and innovation gatherings
    - Specialized technology topics (AI, UX, Cloud, Security)
    
    Registration Data:
    - Authentic-looking names and contact information
    - Properly formatted phone numbers following validation patterns
    - Diverse special notes (dietary restrictions, accessibility needs, VIP status)
    - Mixed attendance status for realistic reporting scenarios
    - Speaker and panelist designations for role-based testing
    
    Design Patterns:
    - Factory Pattern: Centralized creation of related object hierarchies
    - Record Types: Immutable data structures for internal data modeling
    - Dictionary Mapping: Consistent relationship management between events and registrations
    - Static Methods: No state management required for data generation
    
    Usage: Called by MockDataService to populate the EventEase application with
    comprehensive sample data for development, testing, and demonstration purposes.
*/

namespace BlazorFinalProject.Services.Mock;

public static class MockDataFactory
{
    public static (List<Event> Events, List<Registration> Registrations) CreateSeedData()
    {
        var eventIds = CreateEventIds();
        var events = CreateEvents(eventIds);
        var registrations = CreateRegistrations(eventIds);

        return (events, registrations);
    }

    private static Dictionary<string, Guid> CreateEventIds()
    {
        return new Dictionary<string, Guid>
        {
            ["devSummit"] = Guid.NewGuid(),
            ["uxWorkshop"] = Guid.NewGuid(),
            ["cloudExpo"] = Guid.NewGuid(),
            ["aiBootcamp"] = Guid.NewGuid(),
            ["cybersecurityForum"] = Guid.NewGuid(),
            ["agileDays"] = Guid.NewGuid(),
            ["mobileDevCon"] = Guid.NewGuid(),
            ["dataScienceSummit"] = Guid.NewGuid(),
            ["techLeadersMeetup"] = Guid.NewGuid(),
            ["startupPitchNight"] = Guid.NewGuid()
        };
    }

    private static List<Event> CreateEvents(Dictionary<string, Guid> eventIds)
    {
        return new List<Event>
        {
            new Event { Id = eventIds["devSummit"], Name = "Developer Summit", Date = DateTime.Today.AddDays(-5), Location = "Chicago", Notes = "Tech insights from top minds." },
            new Event { Id = eventIds["uxWorkshop"], Name = "UX Workshop", Date = DateTime.Today.AddDays(12), Location = "Remote", Notes = "Hands-on UI prototyping." },
            new Event { Id = eventIds["cloudExpo"], Name = "Cloud Expo", Date = DateTime.Today.AddDays(-2), Location = "San Francisco", Notes = "Latest in cloud technologies." },
            new Event { Id = eventIds["aiBootcamp"], Name = "AI Bootcamp", Date = DateTime.Today.AddDays(30), Location = "New York", Notes = "Deep dive into AI and ML." },
            new Event { Id = eventIds["cybersecurityForum"], Name = "Cybersecurity Forum", Date = DateTime.Today.AddDays(15), Location = "Austin", Notes = "Protecting digital assets." },
            new Event { Id = eventIds["agileDays"], Name = "Agile Days", Date = DateTime.Today.AddDays(25), Location = "Seattle", Notes = "Agile best practices." },
            new Event { Id = eventIds["mobileDevCon"], Name = "Mobile DevCon", Date = DateTime.Today.AddDays(18), Location = "Boston", Notes = "Mobile app development trends." },
            new Event { Id = eventIds["dataScienceSummit"], Name = "Data Science Summit", Date = DateTime.Today.AddDays(22), Location = "Denver", Notes = "Big data and analytics." },
            new Event { Id = eventIds["techLeadersMeetup"], Name = "Tech Leaders Meetup", Date = DateTime.Today.AddDays(10), Location = "Atlanta", Notes = "Networking for tech leaders." },
            new Event { Id = eventIds["startupPitchNight"], Name = "Startup Pitch Night", Date = DateTime.Today.AddDays(28), Location = "Los Angeles", Notes = "Pitch your startup ideas." }
        };
    }

    private static List<Registration> CreateRegistrations(Dictionary<string, Guid> eventIds)
    {
        var registrationData = GetRegistrationData();
        var registrations = new List<Registration>();

        foreach (var (eventKey, userDataList) in registrationData)
        {
            var eventId = eventIds[eventKey];
            foreach (var userData in userDataList)
            {
                registrations.Add(new Registration
                {
                    Id = Guid.NewGuid(),
                    EventId = eventId,
                    AttendeeName = userData.Name,
                    Telephone = userData.Phone,
                    EmailAddress = userData.Email,
                    Notes = userData.Notes,
                    AttendedEvent = userData.Attended
                });
            }
        }

        return registrations;
    }

    private record UserRegistrationData(string Name, string Phone, string Email, string Notes, bool Attended);

    private static Dictionary<string, List<UserRegistrationData>> GetRegistrationData()
    {
        return new Dictionary<string, List<UserRegistrationData>>
        {
            ["devSummit"] = new List<UserRegistrationData>
            {
                new("Alice Smith", "312-985-7612", "alice.smith@example.com", "Requested front row seating", true),
                new("James Wright", "402-555-2345", "james.wright@example.com", "Needs wheelchair access", true),
                new("Alexander Baker", "402-555-0123", "alexander.baker@example.com", "Needs parking", true),
                new("Joseph Stewart", "402-555-1123", "joseph.stewart@example.com", "Panelist", true),
                new("Elijah Cooper", "402-555-2123", "elijah.cooper@example.com", "Needs invoice", true),
                new("Levi Watson", "402-555-3123", "levi.watson@example.com", "Prefers email updates", true),
                new("Parker Mitchell", "402-555-4123", "parker.mitchell@example.com", "Needs invoice", true),
                new("Luke Powell", "402-555-5123", "luke.powell@example.com", "Needs parking", true),
                new("Zoey Foster", "402-555-6123", "zoey.foster@example.com", "VIP guest", true)
            },
            ["uxWorkshop"] = new List<UserRegistrationData>
            {
                new("John Jones", "312-985-8592", "john.jones@example.com", "Will arrive late", false),
                new("Mia Davis", "402-555-7890", "mia.davis@example.com", "Bringing guest", false),
                new("Ella Carter", "402-555-0234", "ella.carter@example.com", "Vegetarian meal", false),
                new("Penelope Sanchez", "402-555-1234", "penelope.sanchez@example.com", "Speaker", false),
                new("Sofia Richardson", "402-555-2234", "sofia.richardson@example.com", "Returning attendee", false),
                new("Aurora Brooks", "402-555-3234", "aurora.brooks@example.com", "Allergic to nuts", false),
                new("Paisley Simmons", "402-555-4234", "paisley.simmons@example.com", "Bringing guest", false),
                new("Ellie Long", "402-555-5234", "ellie.long@example.com", "First time attendee", false),
                new("Nathan Simmons", "402-555-6234", "nathan.simmons@example.com", "Vegetarian meal", false)
            },
            ["cloudExpo"] = new List<UserRegistrationData>
            {
                new("Mike Parry", "708-274-8726", "mike.parry@example.com", "Needs projector access", true),
                new("Benjamin Hall", "402-555-1212", "benjamin.hall@example.com", "Prefers email updates", true),
                new("Sebastian Perez", "402-555-0345", "sebastian.perez@example.com", "VIP guest", true),
                new("Samuel Morris", "402-555-1345", "samuel.morris@example.com", "VIP guest", true),
                new("Aiden Cox", "402-555-2345", "aiden.cox@example.com", "Panelist", true),
                new("Hudson Kelly", "402-555-3345", "hudson.kelly@example.com", "Needs wheelchair access", true),
                new("Grayson Foster", "402-555-4345", "grayson.foster@example.com", "Prefers email updates", true),
                new("Jackson Patterson", "402-555-5345", "jackson.patterson@example.com", "Needs invoice", true),
                new("Savannah Bryant", "402-555-6345", "savannah.bryant@example.com", "Needs parking", true)
            },
            ["aiBootcamp"] = new List<UserRegistrationData>
            {
                new("Lisa Wilson", "224-845-0087", "lisa.wilson@example.com", "Prefers digital materials", false),
                new("Charlotte Young", "402-555-3412", "charlotte.young@example.com", "Allergic to nuts", false),
                new("Grace Turner", "402-555-0456", "grace.turner@example.com", "First time attendee", false),
                new("Lily Rogers", "402-555-1456", "lily.rogers@example.com", "Vegetarian meal", false),
                new("Camila Howard", "402-555-2456", "camila.howard@example.com", "Speaker", false),
                new("Savannah Sanders", "402-555-3456", "savannah.sanders@example.com", "Returning attendee", false),
                new("Aubrey Bryant", "402-555-4456", "aubrey.bryant@example.com", "Allergic to nuts", false),
                new("Layla Hughes", "402-555-5456", "layla.hughes@example.com", "Bringing guest", false),
                new("Leah Russell", "402-555-6456", "leah.russell@example.com", "First time attendee", false)
            },
            ["cybersecurityForum"] = new List<UserRegistrationData>
            {
                new("Ethan Brown", "402-555-1234", "ethan.brown@example.com", "VIP guest", false),
                new("Lucas Hernandez", "402-555-4567", "lucas.hernandez@example.com", "Needs invoice", false),
                new("Daniel Phillips", "402-555-0567", "daniel.phillips@example.com", "Needs invoice", false),
                new("Owen Reed", "402-555-1567", "owen.reed@example.com", "Needs parking", false),
                new("Carter Ward", "402-555-2567", "carter.ward@example.com", "VIP guest", false),
                new("Gabriel Price", "402-555-3567", "gabriel.price@example.com", "Panelist", false),
                new("Madison Russell", "402-555-4567", "madison.russell@example.com", "Needs wheelchair access", false),
                new("Avery Butler", "402-555-5567", "avery.butler@example.com", "Prefers email updates", false),
                new("Wyatt Griffin", "402-555-6567", "wyatt.griffin@example.com", "Needs invoice", false)
            },
            ["agileDays"] = new List<UserRegistrationData>
            {
                new("Sophia Lee", "402-555-4321", "sophia.lee@example.com", "Vegetarian meal", false),
                new("Amelia King", "402-555-5678", "amelia.king@example.com", "Returning attendee", false),
                new("Chloe Campbell", "402-555-0678", "chloe.campbell@example.com", "Bringing guest", false),
                new("Zoe Cook", "402-555-1678", "zoe.cook@example.com", "First time attendee", false),
                new("Riley Torres", "402-555-2678", "riley.torres@example.com", "Vegetarian meal", false),
                new("Violet Bennett", "402-555-3678", "violet.bennett@example.com", "Speaker", false),
                new("Easton Griffin", "402-555-4678", "easton.griffin@example.com", "Returning attendee", false),
                new("Harper Barnes", "402-555-5678", "harper.barnes@example.com", "Allergic to nuts", false),
                new("Hazel Hayes", "402-555-6678", "hazel.hayes@example.com", "Bringing guest", false)
            },
            ["mobileDevCon"] = new List<UserRegistrationData>
            {
                new("Noah Kim", "402-555-6789", "noah.kim@example.com", "Needs parking", false),
                new("Henry Scott", "402-555-6712", "henry.scott@example.com", "Prefers phone contact", false),
                new("Matthew Parker", "402-555-0789", "matthew.parker@example.com", "Prefers email updates", false),
                new("Mason Bell", "402-555-1789", "mason.bell@example.com", "Prefers phone contact", false),
                new("Wyatt Peterson", "402-555-2789", "wyatt.peterson@example.com", "Needs parking", false),
                new("Lincoln Wood", "402-555-3789", "lincoln.wood@example.com", "VIP guest", false),
                new("Penelope Hayes", "402-555-4789", "penelope.hayes@example.com", "Panelist", false),
                new("Ella Brooks", "402-555-5789", "ella.brooks@example.com", "Needs wheelchair access", false),
                new("Julian Jenkins", "402-555-6789", "julian.jenkins@example.com", "Prefers email updates", false)
            },
            ["dataScienceSummit"] = new List<UserRegistrationData>
            {
                new("Olivia Chen", "402-555-9876", "olivia.chen@example.com", "Speaker", false),
                new("Emily Green", "402-555-7823", "emily.green@example.com", "Needs gluten-free meal", false),
                new("Scarlett Evans", "402-555-0890", "scarlett.evans@example.com", "Allergic to nuts", false),
                new("Layla Murphy", "402-555-1890", "layla.murphy@example.com", "Needs gluten-free meal", false),
                new("Nora Gray", "402-555-2890", "nora.gray@example.com", "First time attendee", false),
                new("Brooklyn Barnes", "402-555-3890", "brooklyn.barnes@example.com", "Vegetarian meal", false),
                new("Hudson Morris", "402-555-4890", "hudson.morris@example.com", "Speaker", false),
                new("Carter Reed", "402-555-5890", "carter.reed@example.com", "Returning attendee", false),
                new("Aurora Perry", "402-555-6890", "aurora.perry@example.com", "Allergic to nuts", false)
            },
            ["techLeadersMeetup"] = new List<UserRegistrationData>
            {
                new("William Clark", "402-555-7654", "william.clark@example.com", "First time attendee", false),
                new("Jack Adams", "402-555-8934", "jack.adams@example.com", "Speaker", false),
                new("David Edwards", "402-555-0901", "david.edwards@example.com", "Needs wheelchair access", false),
                new("Logan Bailey", "402-555-1901", "logan.bailey@example.com", "Speaker", false),
                new("Julian Ramirez", "402-555-2901", "julian.ramirez@example.com", "Needs invoice", false),
                new("Jayden Ross", "402-555-3901", "jayden.ross@example.com", "Needs parking", false),
                new("Lillian Jenkins", "402-555-4901", "lillian.jenkins@example.com", "VIP guest", false),
                new("Scarlett Bennett", "402-555-5901", "scarlett.bennett@example.com", "Panelist", false),
                new("Mason Powell", "402-555-6901", "mason.powell@example.com", "Needs wheelchair access", false)
            },
            ["startupPitchNight"] = new List<UserRegistrationData>
            {
                new("Ava Patel", "402-555-3456", "ava.patel@example.com", "Panelist", false),
                new("Harper Nelson", "402-555-9045", "harper.nelson@example.com", "Panelist", false),
                new("Victoria Collins", "402-555-1012", "victoria.collins@example.com", "Returning attendee", false),
                new("Aria Rivera", "402-555-2012", "aria.rivera@example.com", "Panelist", false),
                new("Hazel James", "402-555-3012", "hazel.james@example.com", "Bringing guest", false),
                new("Stella Henderson", "402-555-4012", "stella.henderson@example.com", "First time attendee", false),
                new("Mila Perry", "402-555-5012", "mila.perry@example.com", "Vegetarian meal", false),
                new("Henry Wood", "402-555-6012", "henry.wood@example.com", "Speaker", false),
                new("Layla Long", "402-555-7012", "layla.long@example.com", "Returning attendee", false)
            }
        };
    }
}