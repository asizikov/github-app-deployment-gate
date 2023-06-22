namespace DeploymentGate.Configuration;

public class GitHubAppConfiguration
{
    public string AppId { get; set; } = null!;
    public string PrivateKey { get; set; } = null!;
    public string WebhookSecret { get; set; } = null!;
}