#pragma warning disable CA1852
using DeploymentGate;
using DeploymentGate.Configuration;
using DeploymentGate.GitHub;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Octokit.Webhooks;
using Octokit.Webhooks.AzureFunctions;

new HostBuilder()
    .ConfigureServices((context, collection) =>
    {
        var gitHubAppConfiguration = new GitHubAppConfiguration
        {
            AppId = context.Configuration["GitHubApp:AppId"],
            PrivateKey = context.Configuration["GitHubApp:PrivateKey"],
        };
        collection.AddSingleton(Options.Create(gitHubAppConfiguration));
        collection.AddSingleton<IGitHubClientFactory, GitHubClientFactory>();
        collection.AddSingleton<WebhookEventProcessor, GitHubWebhookEventProcessor>();
    })
    .ConfigureGitHubWebhooks()
    .ConfigureFunctionsWorkerDefaults()
    .Build()
    .Run();
