using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
public class SpaceCarrier
{
    private readonly RabbitMQService _rabbitMQService;
    private readonly string _name;
    private readonly List<string> _services;
    public SpaceCarrier(RabbitMQService rabbitMQService, String Name, List<string> Services)
    {
        _rabbitMQService = rabbitMQService;
        _services = Services;
        _name = Name;
        foreach (var service in _services)
        {
            _rabbitMQService.HandleQueueMessage(service, ProcessOrder);
        }
    }

    private void ProcessOrder(string message)
    {
        var order = JsonConvert.DeserializeObject<Order>(message);
        if (order == null)
        {
            Console.WriteLine($"Przewoźnik {_name} nie może przetworzyć zamówienia - order deserialization issue.\n {message}");
            return;
        }
        Console.WriteLine($"Przewoźnik {_name} przetwarza zamówienie {order.OrderNumber} od {order.AgencyName}.");

        // Symulacja przetwarzania zamówienia
        Thread.Sleep(1000);

        var confirmation = $"Zamówienie {order.OrderNumber} od {order.AgencyName} zostało przetworzone przez {_name}.";
        _rabbitMQService.PublishMessage("confirmations", order.AgencyName, confirmation);
    }
}