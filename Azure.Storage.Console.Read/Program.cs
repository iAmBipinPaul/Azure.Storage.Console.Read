using System.Text.Json;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Discord.Webhook;

var connectionString = Environment.GetEnvironmentVariable("StorageQueueConnectionString")!;
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new ArgumentNullException(nameof(connectionString));
}

var queueName = Environment.GetEnvironmentVariable("QueueName");
if (string.IsNullOrWhiteSpace(queueName))
{
    throw new ArgumentNullException(nameof(queueName));
}

var discordWebhook = Environment.GetEnvironmentVariable("DiscordWebhook");
if (string.IsNullOrWhiteSpace(discordWebhook))
{
    throw new ArgumentNullException(nameof(discordWebhook));
}

var discordWebhookClient = new DiscordWebhookClient(discordWebhook);

QueueClient queueClient = new QueueClient(connectionString, queueName);

while (true)
{
    QueueMessage[] retrievedMessages = queueClient.ReceiveMessages();

    if (retrievedMessages.Length == 0)
        break;
    foreach (QueueMessage message in retrievedMessages)
    {
        Console.WriteLine($"Processed message: {message.Body.ToString()}");


        var jsonObject = new
        {
            Message = message,
        };
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        string jsonString = JsonSerializer.Serialize(jsonObject, options);
        string codeBlock = $"```json\n{jsonString}\n```";
        await discordWebhookClient.SendMessageAsync(text: codeBlock);
        queueClient.DeleteMessage(message.MessageId, message.PopReceipt);
    }
}