using Microsoft.Extensions.Hosting;
using SiteLink.API.Core;
using SiteLink.API.Misc;
using SiteLink.API.Networking;
using System.Collections.Concurrent;

namespace SiteLink.Queue.Services;

public class QueueService : BackgroundService
{
    public static ConcurrentDictionary<Server, List<string>> ServerQueues = new ConcurrentDictionary<Server, List<string>>();

    public static int GetPositionInQueue(Client client, Server server)
    {
        if (!ServerQueues.TryGetValue(server, out List<string> queues))
            return -1;

        return queues.IndexOf(client.PreAuth.UserId);
    }

    public static int GetQueueLength(Server server)
    {
        if (!ServerQueues.TryGetValue(server, out List<string> queues))
            return 0;

        return queues.Count;
    }

    public static void AddToQueue(Client client, Server server)
    {
        if (!ServerQueues.TryGetValue(server, out List<string> queues))
        {
            queues = new List<string>();
            ServerQueues.TryAdd(server, queues);
        }

        if (queues.Contains(client.PreAuth.UserId))
            return;

        queues.Add(client.PreAuth.UserId);
    }

    public static void RemoveFromQueue(Client client, Server server)
    {
        if (!ServerQueues.TryGetValue(server, out List<string> queues))
            return;

        queues.Remove(client.PreAuth.UserId);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                foreach(var queue in ServerQueues)
                {
                    // If server is still full dont do anything and skip.
                    if (queue.Key.ClientsCount >= queue.Key.MaxClientsCount)
                        continue;

                    // If theres no one in queue then skip.
                    if (queue.Value.Count == 0)
                        continue;

                    string nextPlayer = queue.Value[0];

                    // If returns false then it means client is not connected to proxy anymore.
                    if (!Client.TryGet(nextPlayer, out Client client))
                        continue;

                    client.Connect(queue.Key, true);
                }

                await Task.Delay(500);
            }
            catch(Exception ex)
            {
                SiteLinkLogger.Error(ex, "SiteLink.Queue");
            }
        }
    }
}