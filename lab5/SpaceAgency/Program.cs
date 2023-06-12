using RabbitMQ.Client;
using System;
using System.Threading;
public enum ServiceType
{
    PeopleTransport,
    CargoTransport,
    SatelliteLaunch
}
public class Order
{
    public string AgencyName { get; set; }
    public int OrderNumber { get; set; }
    public string ServiceType { get; set; }
}
class Program
{
    static void Main(string[] args)
    {
        var rabbitMQService = new RabbitMQService("localhost", "guest", "guest");

        // Deklaracja kolejek dla każdego typu usługi
        var services = new List<string> { "przewóz osób", "przewóz ładunku", "umieszczenie satelity na orbicie" };
        foreach (var service in services)
        {
            rabbitMQService.AddQueue(service, "orders");
        }

        var agency1 = new SpaceAgency(rabbitMQService);
        var agency2 = new SpaceAgency(rabbitMQService);

        var carrier1 = new SpaceCarrier(rabbitMQService, "Carrier1", new List<string> { "przewóz osób", "przewóz ładunku" });
        var carrier2 = new SpaceCarrier(rabbitMQService, "Carrier2", new List<string> { "przewóz ładunku", "umieszczenie satelity na orbicie" });

        // Symulacja zleceń
        agency1.SendOrder(new Order { AgencyName = "Agency1", OrderNumber = 1, ServiceType = "przewóz osób" });
        agency2.SendOrder(new Order { AgencyName = "Agency2", OrderNumber = 2, ServiceType = "przewóz ładunku" });
        agency2.SendOrder(new Order { AgencyName = "Agency2", OrderNumber = 2, ServiceType = "przewóz ładunku" });
        agency1.SendOrder(new Order { AgencyName = "Agency1", OrderNumber = 3, ServiceType = "przewóz ładunku" });
        agency2.SendOrder(new Order { AgencyName = "Agency2", OrderNumber = 4, ServiceType = "przewóz ładunku" });
        agency1.SendOrder(new Order { AgencyName = "Agency1", OrderNumber = 5, ServiceType = "umieszczenie satelity na orbicie" });

        // Symulacja odbioru potwierdzeń
        rabbitMQService.AddQueue("Agency1", "confirmations");
        rabbitMQService.HandleQueueMessage("Agency1", message => Console.WriteLine($"Agencja1 otrzymała potwierdzenie: {message}"));

        rabbitMQService.AddQueue("Agency2", "confirmations");
        rabbitMQService.HandleQueueMessage("Agency2", message => Console.WriteLine($"Agencja2 otrzymała potwierdzenie: {message}"));

        Console.ReadLine();
        rabbitMQService.Close();
    }
}