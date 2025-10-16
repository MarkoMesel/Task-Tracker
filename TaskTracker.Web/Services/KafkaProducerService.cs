using Confluent.Kafka;
using System.Text.Json;

public class KafkaProducerService
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topic = "task-events";

    public KafkaProducerService()
    {
        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092"
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task SendEventAsync(string eventType, object data)
    {
        var message = new
        {
            EventType = eventType,
            Data = data,
            Timestamp = DateTime.UtcNow
        };

        string json = JsonSerializer.Serialize(message);

        await _producer.ProduceAsync(_topic, new Message<string, string>
        {
            Key = eventType,
            Value = json
        });

        Console.WriteLine($"Kafka event sent: {json}");
    }
}
