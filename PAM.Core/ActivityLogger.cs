using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using PAM.Domain.Contracts;
using PAM.Domain.Models;

// ReSharper disable StringLiteralTypo

namespace PAM.Core;

public class ActivityLogger
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };
    
    private readonly ILogRepository _repository;

    public ActivityLogger(ILogRepository repository)
        => _repository = repository;

    public async Task LogInfo(string message, Dictionary<string, string> data)
        => await Log(message, data, LogLevel.Info);

    public async Task LogWarning(string message, Dictionary<string, string> data)
        => await Log(message, data, LogLevel.Warning);

    public async Task LogCritical(string message, Dictionary<string, string> data)
        => await Log(message, data, LogLevel.Critical);

    private async Task Log(string title, Dictionary<string, string> data, LogLevel level)
    {
        var obj = data.Select(x => new { x.Key, x.Value });
        var serialized = JsonSerializer.SerializeToNode(obj);
        
        if (serialized == null)
            throw new Exception();
        
        var json = serialized.ToJsonString(JsonOptions);
        var guid = Guid.NewGuid();
        var timestamp = DateTime.Now;

        await _repository.AddLog(guid, title, json, timestamp, (int)level);
    }
}
