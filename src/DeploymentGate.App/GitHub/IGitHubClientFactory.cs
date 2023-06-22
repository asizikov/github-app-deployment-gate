using Octokit;

namespace DeploymentGate.GitHub;

public interface IGitHubClientFactory
{
    IGitHubClient GetClient();
}