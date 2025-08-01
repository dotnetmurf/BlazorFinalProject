namespace BlazorFinalProject
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<IRegistrationService, RegistrationService>();
            builder.Services.AddScoped<IMockDataService, MockDataService>();
            builder.Services.AddScoped<IHybridEventStateService, HybridEventStateService>();
            builder.Services.AddScoped<IHybridRegistrationStateService, HybridRegistrationStateService>();

            await builder.Build().RunAsync();
        }
    }
}