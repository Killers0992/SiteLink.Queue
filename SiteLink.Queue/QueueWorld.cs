using PlayerRoles;
using SiteLink.API.Core;
using SiteLink.API.Misc;
using SiteLink.API.Networking;
using SiteLink.API.Networking.Objects;
using SiteLink.Queue.Services;
using System.Text;
using UnityEngine;

namespace SiteLink.Queue;

public class QueueWorld : World
{
    public WaypointToyObject Waypoint;
    public TextToyObject TextToy;

    public Server ConnectingTo;

    public DateTime Delay;

    public QueueWorld(Server server) : base("Queue")
    {
        DestroyOnEmpty = true;

        ConnectingTo = server;

        AddWaypoint(new Vector3(0f, -300f, 0f));
    }

    DateTime _delay;

    public override void Update()
    {
        if (_delay > DateTime.Now)
            return;

        foreach (var client in GetClientsSnapshot())
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"<color=orange>{ConnectingTo.DisplayName}</color> <color=white>is full</color>");
            sb.AppendLine("");
            sb.AppendLine($"<color=orange>Position in queue <color=white>{QueueService.GetPositionInQueue(client, ConnectingTo)}</color>/<color=white>{QueueService.GetQueueLength(ConnectingTo)}</color></color>");

            client.Connection?.AsServer.Hint(sb.ToString(), 1.2f);
        }

        _delay = DateTime.Now.AddSeconds(1);
    }

    public override void OnLoad(Session session)
    {
        QueueService.AddToQueue(session, ConnectingTo);
        session.SpawnPlayer(new Vector3(0f, -300f, 0f));
    }

    public override void OnUnload(Session session)
    {
        QueueService.RemoveFromQueue(session, ConnectingTo);
    }

    public override void OnObjectsSpawned(Session session)
    {
        session.Connection.AsServer.Role(session.NetworkId, RoleTypeId.Tutorial);
        session.Connection.AsServer.Health(session.NetworkId, 100f);
        session.Connection.AsServer.Seed(350);
    }
}
