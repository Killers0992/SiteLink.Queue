using System.ComponentModel;

namespace SiteLink.Queue;

public sealed class Translations
{
    [Description("Placeholders: {tag}, {queue_server}, {queue_server_name}, {queue_position}, {queue_position_ordinal}, {queue_length}, {alt_server}, {alt_server_name}, {alt_online}, {alt_max}")]
    public string QueueText { get; set; } =
        "<color=orange>{queue_server}</color> <color=white>is full</color>\n" +
        "<color=white>You are </color><color=orange>{queue_position_ordinal}</color><color=white> in the queue</color>\n\n" +
        "<color=orange>{alt_server}</color> <color=white>has </color><color=orange>{alt_online}/{alt_max}</color><color=white> players online</color>\n" +
        "<color=white>Press </color><color=orange>[ALT]</color><color=white> to connect</color>";

    [Description("Returned by {queue_count} when the server queue is empty.")]
    public string QueueCountEmpty { get; set; } = "";

    [Description("Placeholders: {queue_length}. Returned by {queue_count} when players are waiting.")]
    public string QueueCount { get; set; } = "+{queue_length} in queue";

    [Description("Placeholders: {server}, {server_name}, {queue_position}")]
    public string AddedToQueueLog { get; set; } =
        "Added (f=cyan){user_id}(f=white) to the (f=yellow){server_name}(f=white) queue at position (f=green){queue_position}(f=white).";

    [Description("No placeholders.")]
    public string NullSessionLog { get; set; } =
        "(f=red)Failed to remove a player from the queue because the session was null.(f=white)";
}
