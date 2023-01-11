using Shared.Configurations;

namespace Basket.API.Service;

public class BackgroundJobHttpService
{
    public HttpClient Client { get; }

    public string ScheduledJobUrl { get; }

    public BackgroundJobHttpService(
        HttpClient client,
        BackgroundJobSettings settings)
    {
        if (settings == null ||
            string.IsNullOrEmpty(settings.HangfireUrl) ||
            string.IsNullOrEmpty(settings.ScheduledJobUrl))
            throw new ArgumentNullException($"{nameof(BackgroundJobSettings)} is not configured properly");

        client.BaseAddress = new Uri(settings.HangfireUrl);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        Client = client;

        ScheduledJobUrl = settings.ScheduledJobUrl;
    }
}