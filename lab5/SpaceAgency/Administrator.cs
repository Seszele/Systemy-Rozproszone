using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
public class Administrator
{
    private readonly IModel _channel;

    public Administrator(IModel channel)
    {
        _channel = channel;
        _channel.ExchangeDeclare("admin", ExchangeType.Fanout);
    }

    public void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish("admin", "", null, body);
    }

    public void ListenForMessages()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("Received message: " + message);
        };
        _channel.BasicConsume("admin", true, consumer);
    }
}