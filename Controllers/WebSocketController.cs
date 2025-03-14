using System.Collections.Concurrent;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;

namespace PdmoonblogApi.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class WebSocketController : ControllerBase
{
    private static readonly ConcurrentDictionary<WebSocket, bool> _sockets = new();

    [Route("/ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            _sockets.TryAdd(webSocket, true);
            await HandleWebSocketAsync(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private static async Task HandleWebSocketAsync(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
                
                // Check if the client wants to close the connection.
                if (receiveResult.MessageType == WebSocketMessageType.Close)
                    break;

                // Broadcast the message to all connected sockets
                await BroadcastMessageAsync(buffer, receiveResult.Count, receiveResult.MessageType, receiveResult.EndOfMessage);
            }
        }
        catch (Exception)
        {
            // Optionally log the exception here
        }
        finally
        {
            // Remove the socket and close the connection gracefully
            _sockets.TryRemove(webSocket, out _);
            if (webSocket.State != WebSocketState.Closed)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
            webSocket.Dispose();
        }
    }

    private static async Task BroadcastMessageAsync(byte[] buffer, int count, WebSocketMessageType messageType, bool endOfMessage)
    {
        var tasks = new List<Task>();
        foreach (var socket in _sockets.Keys)
        {
            if (socket.State == WebSocketState.Open)
            {
                tasks.Add(socket.SendAsync(new ArraySegment<byte>(buffer, 0, count),
                                            messageType,
                                            endOfMessage,
                                            CancellationToken.None));
            }
        }
        await Task.WhenAll(tasks);
    }
}
