# Gniazda TCP/UDP
## Chat

- Klienci łączą się z serwerem przez protokół TCP.
- Serwer przyjmuje wiadomości od każdego klienta i rozsyła je do pozostałych, wraz z id/nickiem klienta.
- Serwer jest wielowątkowy - każde połączenie od klienta ma swój wątek.
- Dodano dodatkowy kanał UDP - Serwer oraz każdy klient otwierają dodatkowy kanał UDP (ten sam numer portu jak przy TCP).
- Po wpisaniu komendy ‘U’ u klienta przesyłana jest wiadomość przez UDP na serwer, który rozsyła ją do pozostałych klientów.
- Wiadomość symuluje dane multimedialne (można np. wysłać ASCII Art).
- Dodano implementację powyższego punktu w wersji multicast.
- Multicast przesyła bezpośrednio do wszystkich przez adres grupowy (serwer może, ale nie musi odbierać).
