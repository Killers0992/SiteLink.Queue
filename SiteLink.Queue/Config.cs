namespace SiteLink.Queue;

public class Config
{
    public string[] ServersWithQueue { get; set; } = new[] { "default" };

    public Dictionary<string, string> AltConnectServers { get; set; } = new(StringComparer.OrdinalIgnoreCase)
    {
        ["default"] = "community"
    };

    public float HintDuration { get; set; } = 1.2f;
}
