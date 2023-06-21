using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.Deployment;
using Octokit.Webhooks.Events.GithubAppAuthorization;
using Octokit.Webhooks.Events.Installation;

namespace DeploymentGate.App;

public class GitHubWebhookEventProcessor : WebhookEventProcessor
{
    private readonly ILogger<GitHubWebhookEventProcessor> _logger;

    public GitHubWebhookEventProcessor(ILogger<GitHubWebhookEventProcessor> logger)
    {
        _logger = logger;
    }

    public override Task ProcessWebhookAsync(IDictionary<string, StringValues> headers, string body)
    {
        _logger.LogInformation("Webhook received: {Headers} {Body}", headers, body);
        return base.ProcessWebhookAsync(headers, body);
    }

    public override Task ProcessWebhookAsync(WebhookHeaders headers, WebhookEvent webhookEvent)
    {
        _logger.LogInformation("Webhook event received: {WebhookEvent}", webhookEvent);
        return base.ProcessWebhookAsync(headers, webhookEvent);
    }

    protected override Task ProcessGithubAppAuthorizationWebhookAsync(WebhookHeaders headers, GithubAppAuthorizationEvent githubAppAuthorizationEvent, GithubAppAuthorizationAction action)
    {
        _logger.LogInformation("GithubAppAuthorization event received: {GithubAppAuthorizationEvent} {Action}", githubAppAuthorizationEvent, action);
        return base.ProcessGithubAppAuthorizationWebhookAsync(headers, githubAppAuthorizationEvent, action);
    }

    protected override Task ProcessInstallationWebhookAsync(WebhookHeaders headers, InstallationEvent installationEvent,
        InstallationAction action)
    {
        _logger.LogInformation("Installation event received: {InstallationEvent} {Action}", installationEvent, action);
        return base.ProcessInstallationWebhookAsync(headers, installationEvent, action);
    }
    
    protected override Task ProcessDeploymentWebhookAsync(WebhookHeaders headers, DeploymentEvent deploymentEvent, DeploymentAction action)
    {
        _logger.LogInformation("Deployment event received: {DeploymentEvent} {Action}", deploymentEvent, action);
        return base.ProcessDeploymentWebhookAsync(headers, deploymentEvent, action);
    }
}