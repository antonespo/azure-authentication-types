using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace _59_ManagedIdentity;

public static class StorageAccountFunction
{
    public static string accountName = "storaccountantonespo";
    public static string containerName = "data";

    [FunctionName("CredentialsStorageAccountFunction")]
    public static IActionResult RunCredentialsStorageAccountFunction(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        var accountKey = "<account-key>";

        var accountCredentials = new StorageSharedKeyCredential(accountName, accountKey);
        var blobServiceClient = new BlobServiceClient(new Uri($"https://{accountName}.blob.core.windows.net"), accountCredentials);

        var blobItem = GetFirstBlobItem(blobServiceClient);

        return new OkObjectResult(blobItem.Name);
    }

    [FunctionName("ServicePrincipalStorageAccountFunction")]
    public static IActionResult RunServicePrincipalStorageAccountFunction(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        string clientId = "<client-id>";
        string tenantId = "<tenant-id>";
        string clientSecret = "<client-secret>";

        var secretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        var blobServiceClient = new BlobServiceClient(new Uri($"https://{accountName}.blob.core.windows.net"), secretCredential);

        var blobItem = GetFirstBlobItem(blobServiceClient);

        return new OkObjectResult(blobItem.Name);
    }

    [FunctionName("ManagedIdentityStorageAccountFunction")]
    public static IActionResult RunManagedIdentityStorageAccountFunction(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        var blobServiceClient = new BlobServiceClient(
            new Uri($"https://{accountName}.blob.core.windows.net"),
            new DefaultAzureCredential());

        var blobItem = GetFirstBlobItem(blobServiceClient);

        return new OkObjectResult(blobItem.Name);
    }

    private static BlobItem GetFirstBlobItem(BlobServiceClient blobServiceClient)
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
        var blobItem = blobContainerClient.GetBlobs().ToList().FirstOrDefault();
        return blobItem;
    }
}
