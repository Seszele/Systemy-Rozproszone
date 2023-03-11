using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            StartServer();
        }

        static void StartServer()
        {
            int port = 8888;
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(IPAddress.Any, port));
            listener.Listen(100);
            Console.WriteLine($"Server started on port {port}");

            while (true)
            {
                Socket client = listener.Accept();
                client.Send(Encoding.ASCII.GetBytes("<|SRV|>enter your nickname: "));
                byte[] buffer = new byte[1024];
                int bytesRead = client.Receive(buffer);
                string nickname = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Client {nickname} connected from {((IPEndPoint?)client.RemoteEndPoint)?.Address}");

                // add the client to the list of connected clients
                lock (connectedClientsLock)
                {
                    connectedClients.Add(client, nickname);
                }
                Thread thread = new Thread(() => HandleClient(client));
                thread.Start();
            }
        }

        static void HandleClient(Socket client)
        {
            // send an ACK message to the client
            client.Send(Encoding.ASCII.GetBytes("<|ACK|>"));
            // send a message to all other connected clients
            string? nickname;
            lock (connectedClientsLock)
            {
                connectedClients.TryGetValue(client, out nickname);
            }
            SendToAllClients($"{nickname} has joined the chat", client);
            // send nicks of all connected clients to the new client
            if (connectedClients.Count > 1)
                client.Send(Encoding.ASCII.GetBytes($"<|SRV|>Connected clients: {string.Join(", ", connectedClients.Values)}\n"));


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
                    SendToAllClients(message, client);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                DisconnectClient(client);
            }

            Console.WriteLine($"Client disconnected from {((IPEndPoint?)client.RemoteEndPoint)?.Address}");

            // remove the client from the list of connected clients
            connectedClients.Remove(client);

            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        private static void DisconnectClient(Socket client)
        {
            // check if the client is still connected
            if (!client.Connected)
            {
                return;
            }
            // disconnect the client from the server
            string? nickname;
            lock (connectedClientsLock)
            {
                connectedClients.TryGetValue(client, out nickname);
                connectedClients.Remove(client);
            }
            client.Shutdown(SocketShutdown.Both);
            client.Close();
            connectedClients.Remove(client);
            Console.WriteLine($"Client {nickname} disconnected from {((IPEndPoint?)client.RemoteEndPoint)?.Address}");
        }

        static void SendToAllClients(string message, Socket? excludeClient = null)
        {
            lock (connectedClientsLock)
            {
                foreach (var client in connectedClients)
                {
                    if (client.Key != excludeClient)
                    {
                        client.Key.Send(Encoding.ASCII.GetBytes(message));
                    }
                }
            }
        }

        static readonly object connectedClientsLock = new object();
        static readonly Dictionary<Socket, string> connectedClients = new Dictionary<Socket, string>();
    }
}
