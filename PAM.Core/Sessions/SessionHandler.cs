// ReSharper disable StringLiteralTypo

using PAM.Domain.Models;

namespace PAM.Core.Sessions;

internal class SessionHandler : IDisposable
{
    private readonly WsWrapper _socket;
    private readonly SshWrapper _ssh;

    public SessionHandler(WsWrapper socket, ResourceCredentials cred)
    {
        _socket = socket;
        _ssh = new SshWrapper(cred);
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        var client = RunClientHandler(cancellationToken);

        await _socket.Send("Подключаемся к ресурсу...", cancellationToken);

        _ssh.Connect();
        _ssh.OpenStream(m => _socket.Send(m, cancellationToken));

        if (_ssh.State != ConnectionState.Open)
        {
            await _socket.Close($"Не удалось подключиться. {_ssh.LastError}");
            return;
        }

        await _socket.Send("Подключено", cancellationToken);
        await client;

        await _socket.Close($"Отключено. {_ssh.LastError}");
        _ssh.Disconnect();
    }

    private async Task RunClientHandler(CancellationToken cancellationToken)
    {
        await Task.Yield();
        while (!cancellationToken.IsCancellationRequested)
        {
            var (message, isClosed) = await _socket.Read(cancellationToken);

            if (isClosed)
                break;

            if (_ssh.State != ConnectionState.Open)
            {
                await _socket.Send("Ресурс недоступен.", cancellationToken);
                continue;
            }

            _ssh.Execute(message);
        }
    }

    public void Dispose()
        => _ssh.Dispose();
}
