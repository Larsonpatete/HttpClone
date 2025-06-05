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

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Client connected.");

            // Handle each client in a new thread
            _ = HandleClient(client); // Fire and forget for simplicity
        }
        listener.Stop();
    }

    private static async Task HandleClient(TcpClient client)
    {
        try
        {
            // Read from client
            NetworkStream stream = client.GetStream();

            Console.WriteLine("Client connected.");

            byte[] buffer = new byte[1024];

            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead == 0) client.Close(); // client closed

            string clientMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Console.WriteLine("From client: " + clientMessage);

            // Split the client message into lines
            string[] parts = clientMessage.Split('\n');
            if (parts.Length > 0)
            {
                // Extract the HTTP request line: e.g., "GET / HTTP/1.1"
                string[] httpHeaderParts = parts[0].Split(' ');

                if (httpHeaderParts.Length >= 2)
                {
                    string httpMethod = httpHeaderParts[0];
                    string route = httpHeaderParts[1];

                    string body;
                    string type;
                    int statusCode;

                    if (httpMethod == "GET")
                    {
                        if (route == "/")
                        {
                            body = "<h1>Hello from TCP Server!</h1>";
                            type = "html";
                            statusCode = 200;
                        }
                        else if (route == "/api/hello")
                        {
                            body = "{\"message\": \"Hello, world!\"}";
                            type = "json";
                            statusCode = 200;
                        }
                        else
                        {
                            body = "404 Not Found";
                            type = "plain";
                            statusCode = 404;
                        }
                    }
                    else
                    {
                        body = "400 Bad Request - Only GET is supported";
                        type = "plain";
                        statusCode = 400;
                    }

                    await SendHttpResponse(stream, statusCode, body, type);
                }
                else
                {
                    await SendHttpResponse(stream, 400, "400 Bad Request - Malformed request");
                }
            }
            else
            {
                await SendHttpResponse(stream, 400, "400 Bad Request - Empty request");
            }

            client.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }
    }

    private static async Task SendHttpResponse(NetworkStream stream, int statusCode, string body, string type = "plain")
    {
        string statusText = statusCode switch
        {
            200 => "OK",
            400 => "Bad Request",
            404 => "Not Found",
            500 => "Internal Server Error",
            _ => "OK"
        };

        string contentType = type switch
        {
            "html" => "text/html",
            "json" => "application/json",
            _ => "text/plain"
        };

        string response = $"HTTP/1.1 {statusCode} {statusText}\r\n" +
                          $"Content-Type: {contentType}; charset=UTF-8\r\n" +
                          $"Content-Length: {Encoding.UTF8.GetByteCount(body)}\r\n" +
                          $"Connection: close\r\n" +
                          $"\r\n" +
                          $"{body}";

        byte[] responseBytes = Encoding.UTF8.GetBytes(response);
        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
    }


}
