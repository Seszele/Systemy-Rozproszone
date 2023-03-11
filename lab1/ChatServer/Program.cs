using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatServer
{
    class Program
    {
        public class ClientInfo
        {
            public ClientInfo(Socket tcpClient, EndPoint udpEndpoint)
            {
                TcpClient = tcpClient;
                UdpEndpoint = udpEndpoint;
            }
            public Socket TcpClient { get; set; }
            public EndPoint UdpEndpoint { get; set; }

            public override int GetHashCode()
            {
                return TcpClient.GetHashCode() ^ UdpEndpoint.GetHashCode();
            }

            public override bool Equals(object? obj)
            {
                if (obj == null)
                {
                    return false;
                }
                if (!(obj is ClientInfo))
                {
                    return false;
                }
                ClientInfo clientInfo = (ClientInfo)obj;
                return clientInfo.TcpClient == TcpClient && clientInfo.UdpEndpoint == UdpEndpoint;
            }
        }
        static void Main(string[] args)
        {
            StartServer();
        }

        static void StartServer()
        {
            int port = 8888;
            // TCP listener socket
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(IPAddress.Any, port));
            listener.Listen(100);
            Console.WriteLine($"Server started on tcp port {port}");

            // UDP socket
            Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            Console.WriteLine($"Server started on udp port {port}");

            while (true)
            {
                Socket tcpClient = listener.Accept();
                byte[] buffer = new byte[1024];
                int bytesRead = tcpClient.Receive(buffer);
                string initialMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                // get nick and udp endpoint from the initial message
                string nickname = initialMessage.Substring(0, initialMessage.IndexOf("<|UDP|>"));
                string udpEndPoint = initialMessage.Substring(initialMessage.IndexOf("<|UDP|>") + 7);
                EndPoint udpEndPointParsed = new IPEndPoint(IPAddress.Parse(udpEndPoint.Split(':')[0]), int.Parse(udpEndPoint.Split(':')[1]));
                Console.WriteLine($"Client {nickname} connected");

                // add the client to the list of connected clients
                ClientInfo clientInfo = new ClientInfo(tcpClient, udpEndPointParsed);

                lock (connectedClientsLock)
                {
                    connectedClients.Add(clientInfo, nickname);
                }
                // start a new thread to handle the tcp client
                Thread tcpThread = new Thread(() => HandleClient(clientInfo));
                tcpThread.Start();

                // start a new thread to handle the udp client
                Thread udpThread = new Thread(() => HandleUdpClient(clientInfo, udpSocket));
                udpThread.Start();
            }
        }

        static void HandleClient(ClientInfo clientInfo)
        {
            Socket client = clientInfo.TcpClient;
            string nickname = connectedClients[clientInfo];
            // send a message to all other connected clients
            SendToAllClients($"{nickname} has joined the chat", clientInfo);
            // send nicks of all connected clients to the new client

            if (connectedClients.Count > 1)
            {
                Console.WriteLine($"connectedClients.Values: {string.Join(", ", connectedClients.Values)}");
                client.Send(Encoding.ASCII.GetBytes($"<|SRV|>Connected clients: {string.Join(", ", connectedClients.Values)}\n"));

            }
            client.Send(Encoding.ASCII.GetBytes("<|ACK|>"));


            byte[] buffer = new byte[1024];
            int bytesRead;

            try
            {
                while ((bytesRead = client.Receive(buffer)) > 0)
                {
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received message: {message}");
                    // append nick to the message and send to all clients
                    message = $"{nickname}: {message}";
                    SendToAllClients(message, clientInfo);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            DisconnectClient(clientInfo);
        }
        static void HandleUdpClient(ClientInfo clientInfo, Socket udpSocket)
        {
            EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] buffer = new byte[1024];

            while (true)
            {
                int bytesRead = udpSocket.ReceiveFrom(buffer, ref clientEndPoint);
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received UDP message: {message} from port {((IPEndPoint)clientEndPoint).Port}");
                // find the client that sent the message (where key.UdpEndpoint == clientEndPoint)
                ClientInfo? senderInfo = connectedClients.Keys.FirstOrDefault(c => c.UdpEndpoint.Equals(clientEndPoint));

                if (senderInfo == null)
                    continue;
                message = $"{connectedClients[senderInfo]}: {message}";
                // Send the UDP message to all other connected clients
                SendToAllClients(message, senderInfo, udpSocket);
            }
        }


        private static void DisconnectClient(ClientInfo clientInfo)
        {
            Console.WriteLine($"Client {connectedClients[clientInfo]} disconnected from the server");
            connectedClients.Remove(clientInfo);

            // check if the client is still connected
            if (!clientInfo.TcpClient.Connected)
                return;
            Console.WriteLine($"here");

            clientInfo.TcpClient.Shutdown(SocketShutdown.Both);
            clientInfo.TcpClient.Close();
        }

        static void SendToAllClients(string message, ClientInfo? excludedClient = null, Socket? udpSocket = null)
        {
            lock (connectedClientsLock)
            {
                foreach (var clientInfo in connectedClients.Keys)
                {
                    if (clientInfo.Equals(excludedClient))
                        continue;
                    //if udp socket is not null, send via udp, else send via tcp
                    if (udpSocket != null)
                    {
                        udpSocket.SendTo(Encoding.ASCII.GetBytes(message), clientInfo.UdpEndpoint);
                    }
                    else
                    {
                        clientInfo.TcpClient.Send(Encoding.ASCII.GetBytes(message));
                    }
                }
            }
        }

        static readonly object connectedClientsLock = new object();
        static readonly Dictionary<ClientInfo, string> connectedClients = new Dictionary<ClientInfo, string>();
    }
}
