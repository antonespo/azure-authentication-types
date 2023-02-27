using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace _59_ManagedIdentity;

public static class KeyVaultFunction
{
    public static string keyVaultName = "kvantonespo";
    public static string secretName = "test";

    [FunctionName("ServicePrincipalKeyVaultFunction")]
    public static async Task<IActionResult> RunServicePrincipalKeyVaultFunction(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    ILogger log)
    {
        string clientId = "<client-id>";
        string tenantId = "<tenant-id>";
        string clientSecret = "<client-secret>";

        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);

        var client = new SecretClient(new Uri($"https://{keyVaultName}.vault.azure.net"), credential);

        var secret = await client.GetSecretAsync(secretName);

        return new OkObjectResult(secret.Value.Value);
    }

    [FunctionName("ManagedIdentityKeyVaultFunction")]
    public static async Task<IActionResult> RunManagedIdentityKeyVaultFunction(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        var client = new SecretClient(new Uri($"https://{keyVaultName}.vault.azure.net/"), new DefaultAzureCredential());

        var secret = await client.GetSecretAsync(secretName);

        return new OkObjectResult(secret.Value.Value);
    }

}