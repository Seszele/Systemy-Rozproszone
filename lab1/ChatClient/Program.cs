using System.Net;
using System.Net.Sockets;
using System.Text;

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

            // TCP client socket
            Socket tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpClient.Connect(new IPEndPoint(ipAddress, serverPort));
            Console.WriteLine($"Connected to server at tcp->{serverAddress}:{serverPort}");

            // UDP socket
            Socket udpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpClient.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpClient.Bind(new IPEndPoint(ipAddress, 0));
            Console.WriteLine($"Created UDP endpoint at {udpClient.LocalEndPoint.ToString()}");
            Console.Write($"Enter your nickname: ");
            string? input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                return;
            }
            input += "<|UDP|>" + udpClient.LocalEndPoint.ToString();
            byte[] nickMessageBytes = Encoding.ASCII.GetBytes(input);
            tcpClient.Send(nickMessageBytes);

            Thread thread = new Thread(() => ReceiveMessages(tcpClient, udpClient));
            thread.Start();

            while (true)
            {
                input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    continue;
                }

                // If the input starts with "/U ", send a UDP message
                if (input.StartsWith("/U "))
                {
                    string udpMessage = input.Substring(3);
                    byte[] messageBytes = Encoding.ASCII.GetBytes(udpMessage);
                    udpClient.SendTo(messageBytes, new IPEndPoint(ipAddress, serverPort));
                }
                else
                {
                    // Otherwise, send a TCP message
                    byte[] messageBytes = Encoding.ASCII.GetBytes(input);
                    tcpClient.Send(messageBytes);
                }
            }
        }

        static void ReceiveMessages(Socket tcpClient, Socket udpClient)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while (true)
                {
                    // Receive TCP message
                    if (tcpClient.Poll(1000, SelectMode.SelectRead))
                    {
                        bytesRead = tcpClient.Receive(buffer);
                        if (bytesRead > 0)
                        {
                            //if message contains <| SRV |>, it is a server message 
                            string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                            if (message.Contains("<|SRV|>"))
                            {
                                message = message.Replace("<|SRV|>", "Server:");
                            }
                            if (message.Contains("<|ACK|>"))
                            {
                                message = message.Replace("<|ACK|>", "You can start chatting now!");
                            }
                            Console.WriteLine(message);

                        }
                    }

                    // Receive UDP message
                    EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    if (udpClient.Poll(1000, SelectMode.SelectRead))
                    {
                        bytesRead = udpClient.ReceiveFrom(buffer, ref remoteEndPoint);
                        if (bytesRead > 0)
                        {
                            string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                            Console.WriteLine(message);
                        }
                    }

                    Thread.Sleep(100);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine($"Error receiving data from server: {e.ErrorCode}, Server closed");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error receiving data from server: {e.Message}");
                return;
            }

            Console.WriteLine("Disconnected from server");

            tcpClient.Shutdown(SocketShutdown.Both);
            tcpClient.Close();
            udpClient.Close();
            Environment.Exit(0);
        }
    }
}
