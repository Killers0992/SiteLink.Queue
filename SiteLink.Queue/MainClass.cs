using Microsoft.Extensions.DependencyInjection;
using Mirror;
using SiteLink.API;
using SiteLink.API.Events;
using SiteLink.API.Events.Args;
using SiteLink.API.Misc;
using SiteLink.API.Networking;
using SiteLink.API.Plugins;
using SiteLink.API.Structs;
using SiteLink.API.Translations;
using SiteLink.Queue.Services;

namespace SiteLink.Queue;

public class MainClass : Plugin<Config, Translations>
{
    public static MainClass Instance { get; private set; }

    public override string Name { get; } = "Queue";

    public override string Description { get; } = "Adds queue system for servers.";

    public override string Author { get; } = "Killers0992";

    public override Version Version { get; } = new Version(1, 0, 1);

    public override Version ApiVersion { get; } = new Version(SiteLinkAPI.ApiVersionText);
    public override string Repository => "Killers0992/SiteLink.Queue";

    public override void OnLoad(IServiceCollection collection)
    {
        Instance = this;

        PlaceholderRegistry.Register("queue_count", context =>
        {
            if (context.Server == null)
                return string.Empty;

            int count = QueueService.GetQueueLength(context.Server);
            Translations translations = Instance.GetTranslation(context.Session);
            string template = count == 0
                ? translations.QueueCountEmpty
                : translations.QueueCount;

            return TranslationManager.Format(template, context)
                .Add("queue_length", count)
                .Format();
        });

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
        PlaceholderRegistry.Unregister("queue_count");

        Instance = null;
        base.OnUnload();
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
                    int position = QueueService.GetPositionInQueue(ev.Connection.Session, ev.Server);
                    SiteLinkLogger.Info(Translate(
                        ev.Connection.Session,
                        translations => translations.AddedToQueueLog,
                        TranslationContext.For(ev.Connection.Session, ev.Server)
                            .With("queue_position", position)));
                }
                break;
        }
    }
}
