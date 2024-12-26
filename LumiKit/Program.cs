using Discord;
using Discord.WebSocket;
using Lavalink4NET.Cluster.Extensions;
using Lavalink4NET.DiscordNet;
using LumiKit.Core;

var config = ConfigurationLoader.LoadFile();
var client = new DiscordSocketClient(new DiscordSocketConfig {
    GatewayIntents = GatewayIntents.GuildVoiceStates,
    ShardId = config.Discord.ShardId,
    TotalShards = config.Discord.ShardCount
});

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(config);
builder.Services.AddSingleton(client);
builder.Services.AddHostedService<ClientManager>();
builder.Services.AddLavalinkCluster<DiscordClientWrapper>();

builder.Services.ConfigureLavalinkCluster(options => {
    options.Nodes = [..config.Lavalink.Select(item => item.ToLavalinkClusterNodeOptions()).ToArray()];
    options.ReadyTimeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddConsole().SetMinimumLevel(LogLevel.Information));

builder.WebHost.ConfigureKestrel(options => { options.ListenAnyIP(config.WebpanelPort); });

var app = builder.Build();
await app.RunAsync();