using System.Net;
using System.Net.Sockets;
using System.Text;

class TcpServer
{
    const int PORT = 80;

    static void Main()
    {
        TcpListener listener = new TcpListener(IPAddress.Any, PORT);
        listener.Start();
        Console.WriteLine("Server listening on port " + PORT);

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
            Console.WriteLine("From client: " + clientMessage);

            // Split the client message into lines
            string[] parts = clientMessage.Split('\n');
            if (parts.Length > 0)
            {
                // Extract the HTTP header line
                string[] httpHeaderParts = parts[0].Split(' ');
                string method = "plain";
                string body = "Unsupported method or malformed request";
                if (httpHeaderParts[0] == "GET")
                {
                    if (httpHeaderParts[1] == "/")
                    {
                        body = "<h1>Hello from TCP Server!</h1>";
                        method = "html";

                    } else if(httpHeaderParts[1] == "/api/hello")
                    {
                        body = "{\"message\": \"Hello, world!\"}";
                    }
                }
                SendHttpResponse(stream, httpHeaderParts[0], body, method);
            }
        }
        client.Close();
        listener.Stop();
    }

    private static void SendHttpResponse(NetworkStream stream, string type, string body, string method = "")
    {
        string response = $"HTTP/1.1 200 OK\r\n" +
                  $"Content-Type: text{(method ?? "/plain")}\r\n" +
                  $"Content-Length: {body.Length}\r\n" +
                  $"\r\n" +
                  $"{body}";
        byte[] responseBytes = Encoding.ASCII.GetBytes(response);
        stream.Write(responseBytes, 0, responseBytes.Length);
    }
}
