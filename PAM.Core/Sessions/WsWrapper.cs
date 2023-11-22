using System.Net.WebSockets;
using System.Text;

namespace PAM.Core.Sessions;

internal class WsWrapper
{
    private readonly WebSocket _socket;

    public WsWrapper(WebSocket socket)
        => _socket = socket;

    public async Task<(string, bool)> Read(CancellationToken cancellationToken)
    {
        try
        {
            var buffer = new byte[1024];
            var segment = new ArraySegment<byte>(buffer);
            var result = await _socket.ReceiveAsync(segment, cancellationToken);

            var message = Encoding.UTF8.GetString(buffer);
            var isClosed = result.CloseStatus.HasValue;

            return (message, isClosed);
        }
        catch (Exception)
        {
            return (string.Empty, true);
        }
    }

    public async Task Send(string message, CancellationToken cancellationToken)
    {
        var bytes = Encoding.UTF8.GetBytes(message);
        var segment = new ArraySegment<byte>(bytes, 0, bytes.Length);
        await _socket.SendAsync(segment, WebSocketMessageType.Text, true, cancellationToken);
    }

    public async Task Close(string description)
    {
        if (_socket.State == WebSocketState.Open)
            await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, description, CancellationToken.None);
    }
}
