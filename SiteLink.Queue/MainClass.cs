using Microsoft.Extensions.DependencyInjection;
using Mirror;
using SiteLink.API;
using SiteLink.API.Events;
using SiteLink.API.Events.Args;
using SiteLink.API.Misc;
using SiteLink.API.Networking;
using SiteLink.API.Plugins;
using SiteLink.API.Structs;
using SiteLink.Queue.Services;

namespace SiteLink.Queue;

public class MainClass : Plugin<Config>
{
    public static MainClass Instance { get; private set; }

    public override string Name { get; } = "Queue";

    public override string Description { get; } = "Adds queue system for servers.";

    public override string Author { get; } = "Killers0992";

    public override Version Version { get; } = new Version(1, 0, 1);

    public override Version ApiVersion { get; } = new Version(SiteLinkAPI.ApiVersionText);

    public override void OnLoad(IServiceCollection collection)
    {
        Instance = this;

        collection.AddHostedService<QueueService>();

        EventManager.Client.ConnectionResponse += OnConnectionResponse;
        EventManager.Client.JoinedServer += OnJoinedServer;
        EventManager.Listener.ListenerRegistered += OnListenerRegistered;

        foreach (Listener listener in Listener.List)
            RegisterVoiceHandler(listener);
    }

    public override void OnUnload()
    {
        EventManager.Client.ConnectionResponse -= OnConnectionResponse;
        EventManager.Client.JoinedServer -= OnJoinedServer;
        EventManager.Listener.ListenerRegistered -= OnListenerRegistered;

        Instance = null;
    }

    private void OnListenerRegistered(ListenerRegisteredEvent ev) => RegisterVoiceHandler(ev.Listener);

    private static void RegisterVoiceHandler(Listener listener) =>
        listener.ClientToServer.Register(NetworkMessages.VoiceMessage, OnVoiceMessage);

    private static InterceptResult OnVoiceMessage(ushort id, NetworkReader reader, ArraySegment<byte> original, Session session)
    {
        if (session.World is not QueueWorld world)
            return InterceptResult.Pass();

        world.TryConnectToAltServer(session);
        return InterceptResult.Drop();
    }

    private void OnJoinedServer(SessionJoinedServerEvent ev)
    {
        if (!Config.ServersWithQueue.Contains(ev.Server.Name))
            return;

        if (!QueueService.ServerQueues.TryGetValue(ev.Server, out List<string> queues))
            return;

        if (!queues.Contains(ev.Session.UserId))
            return;

        queues.Remove(ev.Session.UserId);
    }

    private void OnConnectionResponse(ClientConnectionResponseEvent ev)
    {
        if (!Config.ServersWithQueue.Contains(ev.Server.Name))
            return;

        switch (ev.Response)
        {
            case ServerIsFullResponse _:
                if (ev.Connection.Session.World is QueueWorld world)
                {
                    world.ConnectingTo = ev.Server;
                }
                else
                {
                    ev.Connection.Session.World = new QueueWorld(ev.Server);
                    SiteLinkLogger.Info($"{ev.Connection.Tag} Added to queue, position '(f=green){QueueService.GetPositionInQueue(ev.Connection.Session, ev.Server)}(f=white)' to '(f=green){ev.Server.Name}(f=white)'.");
                }
                break;
        }
    }
}
