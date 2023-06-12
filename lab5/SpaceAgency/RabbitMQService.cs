using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
public class RabbitMQService
{
    private readonly IConnectionFactory _connectionFactory;
    private IConnection _connection;
    private IModel _channel;

    public RabbitMQService(string hostName, string userName, string password)
    {
        _connectionFactory = new ConnectionFactory() { HostName = hostName, UserName = userName, Password = password };
        _connection = _connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare("orders", ExchangeType.Direct); // Deklaracja wymiany "orders"
        _channel.ExchangeDeclare("confirmations", ExchangeType.Direct); // Deklaracja wymiany "confirmations"
        // _channel.QueueDeclare("Agency1", false, false, false, null); // Deklaracja kolejki "Agency1"
        // _channel.QueueDeclare("Agency2", false, false, false, null); // Deklaracja kolejki "Agency2"
    }

    public void PublishMessage(string exchange, string routingKey, string message)
    {
        _channel.ExchangeDeclare(exchange, ExchangeType.Direct);
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(exchange, routingKey, null, body);
    }

    public void HandleQueueMessage(string queue, Action<string> onReceived)
    {
        _channel.QueueDeclare(queue, false, false, false, null);
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body.ToArray());
            onReceived(message);
        };
        _channel.BasicConsume(queue, true, consumer);
    }

    public void BindQueueToExchange(string queue, string exchange, string routingKey)
    {
        _channel.QueueBind(queue, exchange, routingKey);
    }

    public void Close()
    {
        _channel.Close();
        _connection.Close();
    }

    internal void AddQueue(string service, string exchange)
    {
        _channel.QueueDeclare(service, false, false, false, null);
        BindQueueToExchange(service, "orders", service);
    }
}
