using PlayerRoles;
using SiteLink.API.Core;
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

        foreach(var client in GetClientsSnapshot())
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"<color=orange>{ConnectingTo.DisplayName} is full</color>");
            sb.AppendLine("");
            sb.AppendLine($"<color=orange>Position in queue <color=white>{QueueService.GetPositionInQueue(client, ConnectingTo)}</color>/<color=white>{QueueService.GetQueueLength(ConnectingTo)}</color></color>");

            client.SendHint(sb.ToString(), 1.2f);
        }

        _delay = DateTime.Now.AddSeconds(1);
    }

    public override void OnLoad(Client client)
    {
        QueueService.AddToQueue(client, ConnectingTo);
        client.SpawnPlayer();
    }

    public override void OnUnload(Client client)
    {
        QueueService.RemoveFromQueue(null, ConnectingTo);
    }

    public override void OnObjectsSpawned(Client client)
    {
        client.SetRole(RoleTypeId.Tutorial);
        client.SetHealth(100f);
        client.SetSeed(350);
    }
}
