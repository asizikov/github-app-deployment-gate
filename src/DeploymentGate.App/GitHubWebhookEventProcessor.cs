using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using DeploymentGate.GitHub;
using Microsoft.Extensions.Logging;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.DeploymentProtectionRule;

namespace DeploymentGate;

public class GitHubWebhookEventProcessor : WebhookEventProcessor
{
    private readonly ILogger<GitHubWebhookEventProcessor> _logger;
    private readonly IGitHubClientFactory _gitHubClientFactory;

    public GitHubWebhookEventProcessor(ILogger<GitHubWebhookEventProcessor> logger, IGitHubClientFactory gitHubClientFactory)
    {
        _logger = logger;
        _gitHubClientFactory = gitHubClientFactory;
    }
    
    protected override async Task ProcessDeployProtectionRuleWebhookAsync(WebhookHeaders headers, DeploymentProtectionRuleEvent deploymentProtectionRuleEvent, DeploymentProtectionRuleAction action)
    {
        _logger.LogInformation("deployment protection rule event received: {Deployment}", deploymentProtectionRuleEvent.Installation.Id);
        var gitHubClient = _gitHubClientFactory.GetClient();
        var installationToken = await gitHubClient.GitHubApps.CreateInstallationToken(deploymentProtectionRuleEvent.Installation.Id);

        var deploymentProtectionRuleRequestedEvent = deploymentProtectionRuleEvent as DeploymentProtectionRuleRequestedEvent;
        var deploymentCallbackUrl = deploymentProtectionRuleRequestedEvent.DeploymentCallbackUrl;
        
        using var httpClient = new HttpClient();
        
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", installationToken.Token);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
        httpClient.DefaultRequestHeaders.Add("User-Agent", "changelog-custom-deployment-gate");

        var building = @"
## Deployment protection rule is being evaluated. 👨‍🚒
Please wait for the deployment protection rule to be evaluated before approving the deployment...
";
        await Task.Delay(TimeSpan.FromSeconds(10));
        await SendPayloadWithStatusAsync(null, building);
        await Task.Delay(TimeSpan.FromSeconds(10));

        var approved = @"## Deployment protection rule approved
Shipt it! 🚀
";
        await SendPayloadWithStatusAsync("approved", approved);
        
        async Task SendPayloadWithStatusAsync(string state, string comment)
        {
            var payload = new
            {
                environment_name = deploymentProtectionRuleRequestedEvent.Environment,
                state = state,
                comment = comment
            };
            _logger.LogInformation("Sending deployment protection rule event {Payload} to {Url}", payload, deploymentCallbackUrl);

            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                IgnoreNullValues = true
            };

            var content = JsonContent.Create(payload, options: serializerOptions);
            _logger.LogInformation("Content {Content}", await content.ReadAsStringAsync());
            
            var response = await httpClient.PostAsync(deploymentCallbackUrl, content);
        
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("deployment protection rule event {Payload} response {Response}", payload, response);    
            }
            else 
            {
                _logger.LogError("deployment protection rule event {Payload} failed with response {Response}", payload, response);
            }
        } 
    }
}
