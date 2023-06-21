namespace AzureFunctions;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.Installation;

public class GitHubWebhookEventProcessor : WebhookEventProcessor
{
    private readonly ILogger<GitHubWebhookEventProcessor> logger;

    public GitHubWebhookEventProcessor(ILogger<GitHubWebhookEventProcessor> logger)
    {
        this.logger = logger;
    }

    protected override async Task ProcessInstallationWebhookAsync(
        WebhookHeaders headers,
        InstallationEvent installationEvent,
        InstallationAction action)
    {q
        switch (action)
        {
            case InstallationActionValue.Created:
                this.logger.LogInformation("installation created {InstEvent}", installationEvent);
                await Task.Delay(1000);
                break;
            default:
                break;
        }

        this.logger.LogInformation("installation event {Action}", action);
        await Task.Delay(1000);
    }
}
