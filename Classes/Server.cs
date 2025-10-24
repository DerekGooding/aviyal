/*
	MIT License
    Copyright (c) 2025 Ajaykrishnan R	
*/

using System.Text;
using System.Net;
using System.Net.Sockets;

namespace aviyal.Classes;
public class Server : IDisposable
{
	Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
	int port;
	public delegate string RequestEventHandler(string request);
	public event RequestEventHandler REQUEST_RECEIVED = (request) => "";

	List<Socket> clients = [];
	public Server(Config config)
	{
		port = config.serverPort;
		socket.Bind(new IPEndPoint(IPAddress.Any, port));
		socket.Listen(128);
		Console.WriteLine($"server: listening on {IPAddress.Any}:{port}");
		Task.Run(() =>
		{
			while (true)
			{
				var client = socket.Accept();
				clients.Add(client);
				Console.WriteLine("server: socket connected");
				Task.Run(() =>
				{
					while (client.Connected)
					{
						var buffer = new byte[1024];
						var bytesRead = client.Receive(buffer);
						var request = Encoding.UTF8.GetString([.. buffer.Take(bytesRead)]);
						var response = REQUEST_RECEIVED(request);
						var bytes = Encoding.UTF8.GetBytes(response);
						client.Send(bytes);
						Console.WriteLine($"server: request recieved: {request}, response: {response}");
					}
					client.Close();
					clients.Remove(client);
					Console.WriteLine("server: connection closed");
				});
			}
		});
	}

	public void Broadcast(string message)
	{
		//Console.WriteLine($"[[[BROADCASTING TO {clients.Count}]]]");
		clients?.ForEach(client =>
		{
			var bytes = Encoding.UTF8.GetBytes(message);
			if (client.Connected) client?.Send(bytes);
		});
	}

	// necessary for hot reloading (restarting)
	public void Dispose()
	{
		clients?.ForEach(client =>
		{
			client?.Close();
			client?.Dispose();
		});
		socket?.Close();
		socket?.Dispose();
	}
}
