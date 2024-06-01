// See https://aka.ms/new-console-template for more information

using Azure.Storage.Queues;
using System.Text;
await SendMessagesAsync();
async Task SendMessagesAsync()
{
    string connectionString = "[Azure Storage Queue Connection String]";
    string queueName = "[Queue Name]";
    QueueClient queueClient = new QueueClient(connectionString, queueName);
    await queueClient.CreateIfNotExistsAsync();
    var tasks= new List<Task>();
    for (int i = 0; i < 21; i++)
    {
        byte[] buffer = Encoding.UTF8.GetBytes($"Hello, World! + {i}");
        string base64Message = Convert.ToBase64String(buffer);
        tasks.Add(queueClient.SendMessageAsync(base64Message));    
    }
    if (tasks.Count > 0)
    {
           await Task.WhenAll(tasks);
    }
}