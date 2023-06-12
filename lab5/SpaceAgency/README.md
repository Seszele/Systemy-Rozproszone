# Broker wiadomości dla agencji kosmicznych

Ten projekt to system brokera wiadomości dla agencji kosmicznych i dostawców transportu kosmicznego, zaimplementowany przy użyciu .NET i RabbitMQ.

## Przegląd systemu

System umożliwia agencjom kosmicznym składanie zamówień na trzy rodzaje usług: transport pasażerski, transport ładunków i rozmieszczanie satelitów. Zamówienia są przetwarzane przez dostawców usług transportu kosmicznego, którzy oferują dwa z trzech rodzajów usług.

Poniżej znajduje się diagram ilustrujący system:

![Schemat systemu](https://showme.redstarplugin.com/d/cn2bmbeT)

## Wymagania wstępne

- .NET 5.0 lub nowszy
- Docker

## Pierwsze kroki

1. Pobierz obraz RabbitMQ Docker:

```bash
docker pull rabbitmq:3-management
```

2. Uruchom kontener Docker RabbitMQ:
```bash
docker run -d --hostname my-rabbit --name some-rabbit -p 8080:15672 -p 5672:5672 rabbitmq:3-management
```
`To polecenie uruchamia nowy kontener i udostępnia interfejs zarządzania RabbitMQ na porcie 8080 oraz serwer RabbitMQ na porcie 5672.`

3. Sklonuj repozytorium i przejdź do katalogu projektu:
```bash
git clone https://github.com/yourusername/space-agency-message-broker.git
cd space-agency-message-broker
```
4. Uruchom projekt:
```bash
dotnet run
```