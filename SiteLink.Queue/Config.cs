namespace SiteLink.Queue;

public class Config
{
    public string[] ServersWithQueue { get; set; } = new[] { "default" };

    public Dictionary<string, string> AltConnectServers { get; set; } = new(StringComparer.OrdinalIgnoreCase)
    {
        ["default"] = "community"
    };

    public string QueueText { get; set; } =
        "<color=orange>{queue_server}</color> <color=white>is full</color>\n" +
        "<color=white>You are </color><color=orange>{queue_position_ordinal}</color><color=white> in the queue</color>\n\n" +
        "<color=orange>{alt_server}</color> <color=white>has </color><color=orange>{alt_online}/{alt_max}</color><color=white> players online</color>\n" +
        "<color=white>Press </color><color=orange>[Q]</color><color=white> to connect</color>";

    public float HintDuration { get; set; } = 1.2f;
}
