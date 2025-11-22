using Microsoft.Extensions.DependencyInjection;
using SiteLink.API;
using SiteLink.API.Events;
using SiteLink.API.Events.Args;
using SiteLink.API.Misc;
using SiteLink.API.Plugins;
using SiteLink.API.Structs;
using SiteLink.Queue.Services;

namespace SiteLink.Queue;

public class MainClass : Plugin<Config>
{
    public override string Name { get; } = "SiteLink.Queue";

    public override string Description { get; } = "Adds queue system for servers.";

    public override string Author { get; } = "Killers0992";

    public override Version Version { get; } = new Version(1, 0, 0);

    public override Version ApiVersion { get; } = new Version(SiteLinkAPI.ApiVersionText);

    public override void OnLoad(IServiceCollection collection)
    {
        collection.AddHostedService<QueueService>();

        EventManager.Client.ConnectionResponse += OnConnectionResponse;
        EventManager.Client.JoinedServer += OnJoinedServer;
    }

    private void OnJoinedServer(ClientJoinedServerEvent ev)
    {
        if (!Config.ServersWithQueue.Contains(ev.Server.Name))
            return;

        if (!QueueService.ServerQueues.TryGetValue(ev.Server, out List<string> queues))
            return;

        if (!queues.Contains(ev.Client.PreAuth.UserId))
            return;

        queues.Remove(ev.Client.PreAuth.UserId);
    }

    private void OnConnectionResponse(ClientConnectionResponseEvent ev)
    {
        if (!Config.ServersWithQueue.Contains(ev.Server.Name))
            return;

        switch (ev.Response)
        {
            case ServerIsFullResponse _:
                if (ev.Client.World is QueueWorld world)
                {
                    world.ConnectingTo = ev.Server;
                }
                else
                {
                    ev.Client.World = new QueueWorld(ev.Server);
                    SiteLinkLogger.Info($"{ev.Client.Tag} Added to queue, position '(f=green){QueueService.GetPositionInQueue(ev.Client, ev.Server)}(f=white)' to '(f=green){ev.Server.Name}(f=white)'.");
                }
                break;
        }
    }
}
