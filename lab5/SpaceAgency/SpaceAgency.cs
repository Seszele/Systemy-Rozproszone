using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
public class SpaceAgency
{
    private readonly RabbitMQService _rabbitMQService;

    public SpaceAgency(RabbitMQService rabbitMQService)
    {
        _rabbitMQService = rabbitMQService;
    }

    public void SendOrder(Order order)
    {
        var message = JsonConvert.SerializeObject(order);
        _rabbitMQService.PublishMessage("orders", order.ServiceType, message);
    }
}


