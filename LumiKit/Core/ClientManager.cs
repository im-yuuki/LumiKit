using Discord;
using Discord.WebSocket;

namespace LumiKit.Core;

public sealed class ClientManager : IHostedService
{
    private readonly ConfigurationLoader _config;
    private readonly DiscordSocketClient _client;
    private readonly ILogger<DiscordSocketClient> _logger;
    
    public ClientManager(ConfigurationLoader config, DiscordSocketClient client, ILogger<DiscordSocketClient> logger)
    {
        _config = config;
        _client = client;
        _logger = logger;
        _client.Log += LogProxy;
    }
    
    private Task LogProxy(LogMessage message)
    {
        var logLevel = message.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Debug,
            LogSeverity.Debug => LogLevel.Trace,
            _ => LogLevel.None
        };

        _logger.Log(logLevel, message.Exception, "[{Source}] {Message}", message.Source, message.Message);
        return Task.CompletedTask;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _client.LoginAsync(TokenType.Bot, _config.Discord.Token).ConfigureAwait(false);
        await _client.StartAsync().ConfigureAwait(false);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.StopAsync().ConfigureAwait(false);
    }
}