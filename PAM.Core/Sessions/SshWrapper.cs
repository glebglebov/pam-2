using System.Text;
using PAM.Domain.Models;
using Renci.SshNet;

namespace PAM.Core.Sessions;

internal class SshWrapper : IDisposable
{
    private readonly SshClient _client;
    private ShellStream? _shell;

    public ConnectionState State { get; private set; } = ConnectionState.Wait;
    public string? LastError { get; private set; }

    public SshWrapper(ResourceCredentials credentials)
    {
        var info = new PasswordConnectionInfo(credentials.Address, 22, credentials.Login, credentials.Password)
        {
            Encoding = Encoding.UTF8
        };
        _client = new SshClient(info);
    }

    public void Connect()
    {
        try
        {
            _client.Connect();
            State = ConnectionState.Open;
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            State = ConnectionState.Failed;
            Disconnect();
        }
    }

    public void OpenStream(Func<string, Task> onMessage)
    {
        try
        {
            _shell = _client.CreateShellStream("", 80, 40, 80, 40, 1024);
            _shell.DataReceived += async (_, args) =>
            {
                var message = Encoding.UTF8.GetString(args.Data);
                if (!string.IsNullOrWhiteSpace(message))
                    await onMessage(message);
            };
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            Disconnect();
        }
    }

    public void Execute(string command)
    {
        try
        {
            _shell?.WriteLine(command);
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            Disconnect();
        }
    }

    public void Disconnect()
    {
        _shell?.Close();
        if (_client.IsConnected)
            _client.Disconnect();
    }

    public void Dispose()
    {
        _client.Dispose();
        _shell?.Dispose();
    }
}

internal enum ConnectionState
{
    Wait = 0,
    Open,
    Failed
}
