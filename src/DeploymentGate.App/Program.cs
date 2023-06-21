using DeploymentGate.App;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Octokit.Webhooks;
using Octokit.Webhooks.AzureFunctions;

var host = new HostBuilder()
    .ConfigureServices(collection =>
    {
        collection.AddSingleton<WebhookEventProcessor, GitHubWebhookEventProcessor>();
    })
    .ConfigureGitHubWebhooks()
    .ConfigureFunctionsWorkerDefaults()
    .Build();

host.Run();