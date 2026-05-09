using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Postech.Payments.Api.Application.DTOs;
using Postech.Payments.Api.Application.Services;
using Postech.Shared.Contracts.Events;

namespace Postech.Payments.Api.Infrastructure.Messaging;

public class SqsOrderPlacedConsumer : BackgroundService
{
    private readonly IAmazonSQS _sqsClient;
    private readonly ILogger<SqsOrderPlacedConsumer> _logger;
    private readonly string _queueUrl;
    private readonly IServiceProvider _serviceProvider;

    public SqsOrderPlacedConsumer(
        IAmazonSQS sqsClient,
        ILogger<SqsOrderPlacedConsumer> logger,
        IConfiguration configuration,
        IServiceProvider serviceProvider)
    {
        _sqsClient = sqsClient;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _queueUrl = configuration["AWS:SqsQueueUrl"]
                    ?? throw new InvalidOperationException("AWS SQS Queue URL not configured");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SQS Order Placed Consumer started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var receiveRequest = new ReceiveMessageRequest
                {
                    QueueUrl = _queueUrl,
                    MaxNumberOfMessages = 10,
                    WaitTimeSeconds = 20,
                    MessageAttributeNames = new List<string> { "All" }
                };

                var response = await _sqsClient.ReceiveMessageAsync(receiveRequest, stoppingToken);

                if (response.Messages.Count == 0)
                {
                    continue;
                }

                foreach (var message in response.Messages)
                {
                    try
                    {
                        await ProcessMessageAsync(message, stoppingToken);

                        await _sqsClient.DeleteMessageAsync(_queueUrl, message.ReceiptHandle, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing SQS message {MessageId}", message.MessageId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error receiving messages from SQS");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        _logger.LogInformation("SQS Order Placed Consumer stopped");
    }

    private async Task ProcessMessageAsync(Message message, CancellationToken cancellationToken)
    {
        var eventType = message.MessageAttributes.ContainsKey("EventType")
            ? message.MessageAttributes["EventType"].StringValue
            : null;

        _logger.LogInformation("Processing SQS message {MessageId} of type {EventType}",
            message.MessageId, eventType);

        if (eventType == nameof(OrderPlacedEvent))
        {
            var orderEvent = JsonSerializer.Deserialize<OrderPlacedEvent>(message.Body);
            if (orderEvent != null)
            {
                using var scope = _serviceProvider.CreateAsyncScope();
                var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

                var dto = new OrderCreatedDto(
                    orderEvent.OrderId,
                    orderEvent.UserId,
                    orderEvent.GameId,
                    orderEvent.TotalAmount);

                await paymentService.ProccessOrderAsync(dto);
            }
        }
    }
}
