using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace order_processor_function;

public class ProcessOrderFunction
{
    private readonly ILogger<ProcessOrderFunction> _logger;

    public ProcessOrderFunction(ILogger<ProcessOrderFunction> logger)
    {
        _logger = logger;
    }

    [Function(nameof(ProcessOrderFunction))]
    public async Task Run(
        [ServiceBusTrigger("notification-queue", Connection = "ServiceBusConnection")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

        // Message Serialization
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var orderInfo = JsonSerializer.Deserialize<OrderModel>(message.Body.ToString(), options);

        var orderDetailInfo = $"Order Details: \n" +
            $"Customer Name: {orderInfo.CustomerName}\n" +
            $"Email: {orderInfo.Email}\n" +
            $"Order Date: {orderInfo.OrderDate}\n" +
            $"Order Amount: {orderInfo.OrderAmount}\n" +
            $"Items: \n";

        foreach (var item in orderInfo.Items)
        {
            orderDetailInfo += $"Product ID: {item.ProductId}, Quantity: {item.Quantity}\n";
        }

        // Upload to Blob
        var blobContainerName = Environment.GetEnvironmentVariable("BlobContainerName");
        var connString = Environment.GetEnvironmentVariable("ReceiptStorageConnection");

        var blobServiceClient = new BlobServiceClient(connString);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
        await blobContainerClient.CreateIfNotExistsAsync();

        var blobName = $"order-{orderInfo.CustomerName}-{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}.txt";
        var blobClient = blobContainerClient.GetBlobClient(blobName);

        using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(orderDetailInfo)))
        {
            await blobClient.UploadAsync(stream, true);
        }

        

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }
}