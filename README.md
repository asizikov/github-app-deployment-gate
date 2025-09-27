# GitHub App Deployment Gate

A custom GitHub deployment protection rule implementation using Azure Functions and C#. This application acts as a deployment gate that evaluates deployment requests and provides automated approval/rejection based on your custom logic.

## Overview

This GitHub App integrates with GitHub's deployment protection rules to create a custom approval workflow for deployments. When a deployment is triggered in a GitHub repository with this protection rule enabled, the app:

1. Receives a webhook notification
2. Evaluates the deployment request (with customizable logic)
3. Provides feedback to GitHub about the deployment approval status
4. Allows or blocks the deployment accordingly

## Features

- **Custom Deployment Logic**: Implement your own deployment approval criteria
- **Real-time Feedback**: Provides status updates during the evaluation process
- **GitHub Integration**: Seamless integration with GitHub's native deployment protection rules
- **Azure Functions**: Serverless deployment for cost-effective scaling
- **Secure Authentication**: Uses GitHub App authentication with JWT tokens

## Architecture

The application is built using:
- **.NET 7.0** - Core runtime
- **Azure Functions v4** - Serverless hosting platform
- **Octokit** - GitHub API client library
- **Octokit.Webhooks** - GitHub webhook handling

## Prerequisites

Before setting up this deployment gate, you'll need:

1. **Azure Account** with permission to create Function Apps
2. **GitHub Account** with admin access to repositories where you want to use deployment protection
3. **GitHub App** registration (instructions below)

## Setup Instructions

### 1. Create a GitHub App

1. Navigate to your GitHub organization settings → Developer settings → GitHub Apps
2. Click "New GitHub App"
3. Fill in the required information:
   - **App name**: Choose a unique name (e.g., "My Deployment Gate")
   - **Homepage URL**: Your organization or app URL
   - **Webhook URL**: Your Azure Function URL (you'll get this after deployment)
   - **Webhook secret**: Generate a secure random string
4. Set permissions:
   - **Actions**: Read (to read deployment information)
   - **Deployments**: Write (to approve/reject deployments)
5. Subscribe to events:
   - **Deployment protection rule**
6. Save and note the **App ID**
7. Generate and download a **private key**

### 2. Deploy to Azure

#### Option A: Using Azure Portal

1. Create a new Function App in Azure Portal
2. Choose:
   - **Runtime stack**: .NET
   - **Version**: 7 (Isolated)
   - **Operating System**: Windows or Linux
3. Configure application settings (see Configuration section below)
4. Deploy the code using your preferred method (VS Code, Visual Studio, or CI/CD)

#### Option B: Using the provided GitHub Action

The repository includes a CI/CD workflow that automatically deploys to Azure Functions:

1. Fork this repository
2. Set up the following secrets in your repository:
   - `AZURE_FUNCTIONAPP_PUBLISH_PROFILE`: Download from your Azure Function App
3. Update the `AZURE_FUNCTIONAPP_NAME` in `.github/workflows/ci.yml`
4. Push changes to trigger deployment

### 3. Configuration

Set the following application settings in your Azure Function App:

| Setting | Description | Example |
|---------|-------------|---------|
| `GitHubApp:AppId` | Your GitHub App ID | `123456` |
| `GitHubApp:PrivateKey` | Your GitHub App private key (full PEM content) | `-----BEGIN RSA PRIVATE KEY-----...` |
| `GitHubApp:WebhookSecret` | Webhook secret you set in GitHub App | `your-webhook-secret` |

### 4. Install the GitHub App

1. Go to your GitHub App settings
2. Click "Install App"
3. Choose the repositories where you want to use deployment protection
4. Complete the installation

### 5. Enable Deployment Protection Rules

1. Go to your repository settings → Environments
2. Create or edit an environment
3. Under "Deployment protection rules", click "Add rule"
4. Select your GitHub App from the list
5. Save the environment settings

## Usage

Once configured, the deployment gate will automatically:

1. **Intercept Deployments**: When a deployment targets a protected environment
2. **Show Status**: Display "Deployment protection rule is being evaluated 👨‍🚒"
3. **Process Request**: Run your custom evaluation logic (currently includes a 10-second delay)
4. **Provide Decision**: Approve with "Ship it! 🚀" or reject based on your criteria

## Customization

### Modifying Evaluation Logic

The main evaluation logic is in `GitHubWebhookEventProcessor.cs` in the `ProcessDeployProtectionRuleWebhookAsync` method. You can customize:

- **Evaluation criteria**: Add your own business logic
- **Approval conditions**: Define when deployments should be approved/rejected  
- **Timing**: Adjust delays or timeouts
- **Messages**: Customize the feedback messages shown to users

Example customization:
```csharp
// Add custom evaluation logic
var shouldApprove = await EvaluateDeploymentAsync(deploymentProtectionRuleRequestedEvent);

if (shouldApprove)
{
    await SendPayloadWithStatusAsync("approved", "✅ Deployment approved!");
}
else
{
    await SendPayloadWithStatusAsync("rejected", "❌ Deployment rejected - criteria not met");
}
```

### Environment Variables

You can add additional configuration through environment variables:
- Custom API endpoints
- Feature flags
- Evaluation timeout settings
- Integration credentials

## Monitoring and Debugging

### Application Insights

The application is configured to use Azure Application Insights for monitoring:
- Track webhook events
- Monitor response times
- Debug approval/rejection decisions
- Set up alerts for failures

### Logging

The application logs important events:
- Incoming webhook events
- Evaluation decisions
- API responses to GitHub
- Error conditions

Check the Azure Function logs in the Azure portal under "Monitor" → "Logs".

### Testing

To test your deployment gate:

1. Create a test deployment in your repository
2. Monitor the Azure Function logs
3. Check the deployment status in GitHub Actions
4. Verify the approval/rejection behavior

## Troubleshooting

### Common Issues

**Webhook not received**
- Verify the webhook URL in your GitHub App settings
- Check if the Azure Function is running
- Ensure the webhook secret matches your configuration

**Authentication errors**
- Verify the GitHub App ID and private key are correct
- Check that the private key includes the full PEM content
- Ensure the app is installed on the target repository

**Deployment stuck in "waiting"**
- Check Azure Function logs for errors
- Verify the callback URL is reachable
- Ensure proper permissions are set on the GitHub App

### Getting Help

1. Check Azure Function logs in the portal
2. Review GitHub App webhook deliveries
3. Verify application settings are correct
4. Test webhook delivery using GitHub's webhook testing tools

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## Security Considerations

- **Store secrets securely**: Use Azure Key Vault for sensitive configuration
- **Validate webhooks**: Always verify webhook signatures
- **Limit permissions**: Grant minimal required permissions to the GitHub App
- **Monitor access**: Regularly review app installations and access logs
- **Update dependencies**: Keep NuGet packages up to date

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

For issues and questions:
- Create an issue in this repository
- Check existing issues for solutions
- Review the troubleshooting section above

---

**Note**: This is a sample implementation. Customize the evaluation logic to match your specific deployment approval requirements.