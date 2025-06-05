# Minimal HTTP Server in C# over TCP

This minimalist HTTP server, written in C#, runs over raw TCP sockets. It demonstrates how HTTP works under the hood by manually handling TCP connections, parsing requests, and sending HTTP responses — all without using a web framework.
## 🚀 Features
- Handles basic GET requests
- Supports routes:
  - `/` → Returns simple HTML
  - `/api/hello` → Returns JSON
- Custom HTTP response builder
- Basic error handling (400, 404)
- Runs inside a Docker container
## 🐳 Running with Docker
1. Build the Docker image:
```docker
docker build -t mini-http-server .
```

2. Run the container

```docker
docker run -p 8080:80 mini-http-server
```

This maps port 80 in the container to 8080 on your machine.

3. Test the server

HTML Response
```bash
curl http://localhost:8080/
```
Response:
```html
<h1>Hello from TCP Server!</h1>
```
JSON Response
```bash
curl http://localhost:8080/api/hello
```
Response:
```json
{"message": "Hello, world!"}
```
Unknown Route
```bash
curl http://localhost:8080/unknown
```
Response:
```
404 Not Found
