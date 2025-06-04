using System.Net;
using System.Net.Sockets;
using System.Text;

class TcpServer
{
    const int Port = 8080;

    static void Main()
    {
        TcpListener listener = new TcpListener(IPAddress.Any, Port);
        listener.Start();
        Console.WriteLine("Server listening on port " + Port);

        TcpClient client = listener.AcceptTcpClient();
        Console.WriteLine("Client connected.");

        NetworkStream stream = client.GetStream();

        byte[] buffer = new byte[1024];
        while (true)
        {
            // Read from client
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            if (bytesRead == 0) break; // client closed

            string clientMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            if (clientMessage.StartsWith("exit", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Server Exit...");
                break;
            }
            Console.WriteLine("From client: " + clientMessage);

            byte[] responseBytes = Encoding.ASCII.GetBytes(clientMessage + "\n");
            stream.Write(responseBytes, 0, responseBytes.Length);
        }

        client.Close();
        listener.Stop();
    }
}