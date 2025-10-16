using Confluent.Kafka;
using System.Text.Json;

namespace TaskTracker.KafkaConsumer
{
    public class Worker : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "task-logger",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe("task-events");

            while (!stoppingToken.IsCancellationRequested)
            {
                var cr = consumer.Consume(stoppingToken);
                var message = JsonSerializer.Deserialize<JsonElement>(cr.Message.Value);

                Console.WriteLine($"[Kafka Consumer] Event: {message}");
            }

            consumer.Close();
        }
    }
}
