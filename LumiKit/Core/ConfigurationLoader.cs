using Lavalink4NET;
using Lavalink4NET.Cluster.Nodes;
using Newtonsoft.Json;

namespace LumiKit.Core;

public sealed class ConfigurationLoader
{
    public DiscordConfig Discord { get; init; } = new DiscordConfig();
    public LavalinkConfig[] Lavalink { get; init; } = [];
    public int WebpanelPort { get; init; } = 5000;
    public string WebpanelUsername { get; init; } = "admin";
    public string WebpanelPassword { get; init; } = "admin";

    public static ConfigurationLoader LoadFile(string path = "config.json")
    {
        var jsonReader = new JsonTextReader(File.OpenText(path));
        return new JsonSerializer().Deserialize<ConfigurationLoader>(jsonReader)!;
    }
    
    public sealed class DiscordConfig
    {
        [JsonRequired] public string? Token { get; init; }
        public int ShardId { get; init; } = 0;
        public int ShardCount { get; init; } = 1;
    }

    public sealed class LavalinkConfig
    {
        [JsonRequired] public string? Label { get; init; }
        [JsonRequired] public string? Uri { get; init; }
        [JsonRequired] public string? Password { get; init; }

        public LavalinkClusterNodeOptions ToLavalinkClusterNodeOptions()
        {
            return new LavalinkClusterNodeOptions {
                Label = Label,
                BaseAddress = new Uri(Uri ?? ""),
                Passphrase = Password ?? "",
                ResumptionOptions = new LavalinkSessionResumptionOptions(TimeSpan.FromSeconds(10))
            };
        }
    }
}