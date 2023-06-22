using DeploymentGate.Configuration;
using GitHubJwt;
using Microsoft.Extensions.Options;
using Octokit;

namespace DeploymentGate.GitHub;

public class GitHubClientFactory : IGitHubClientFactory
{
    private readonly IOptions<GitHubAppConfiguration> _options;

    public GitHubClientFactory(IOptions<GitHubAppConfiguration> options)
    {
        _options = options;
    }
    public IGitHubClient GetClient()
    {
        var appId = _options.Value.AppId;
        var privateKey = _options.Value.PrivateKey;
        
        var generator = new GitHubJwtFactory(
            new StringPrivateKeySource(privateKey),
            new GitHubJwtFactoryOptions
            {
                AppIntegrationId = int.Parse(appId),
                ExpirationSeconds = 600
            });

        var jwtToken = generator.CreateEncodedJwtToken();

        var client = new GitHubClient(new ProductHeaderValue("changelog-custom-deployment-gate"))
        {
            Credentials = new Credentials(jwtToken, AuthenticationType.Bearer)
        };

        return client;
    }
}