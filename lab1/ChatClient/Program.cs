using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatClient
{
    class Program
    {
        static void Main(string[] args)
        {
            StartClient();
        }

        static void StartClient()
        {
            string serverAddress = "localhost";
            IPAddress ipAddress = Dns.GetHostEntry("127.0.0.1").AddressList[0];
            int serverPort = 8888;
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(new IPEndPoint(ipAddress, serverPort));
            Console.WriteLine($"Connected to server at {serverAddress}:{serverPort}");

            Thread thread = new Thread(() => ReceiveMessages(client));
            thread.Start();

            while (true)
            {
                string message = Console.ReadLine();
                byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                if (client.Connected)
                {
                    client.Send(messageBytes);
                }
                else
                {
                    Console.WriteLine("Unable to send message, you are not connected to the server");
                    break;
                }
            }
        }

        static void ReceiveMessages(Socket client)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = client.Receive(buffer)) > 0)
                {
                    // if message contains <|SRV|>, it is a server message
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    if (message.Contains("<|SRV|>"))
                    {
                        message = message.Replace("<|SRV|>", "");
                        Console.Write("Server: " + message);
                    }
                    else if (message.Contains("<|ACK|>"))
                    {
                        message = message.Replace("<|ACK|>", "");
                        Console.WriteLine("You can start chatting now!");
                    }
                    else
                    {
                        Console.WriteLine(message);
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("Error receiving data from server: {0}, Server closed", e.ErrorCode);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error receiving data from server: {0}", e.Message);
                return;
            }

            Console.WriteLine("Disconnected from server");

            client.Shutdown(SocketShutdown.Both);
            client.Close();
            Environment.Exit(0);
        }
    }
}
