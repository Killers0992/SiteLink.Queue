![GitHub Downloads (all assets, all releases)](https://img.shields.io/github/downloads/Killers0992/SiteLink.Queue/total?label=Downloads&labelColor=2e343e&color=00FFFF&style=for-the-badge)
[![Discord](https://img.shields.io/discord/1434213646510325762?label=Discord&labelColor=2e343e&color=00FFFF&style=for-the-badge)](https://discord.gg/Sva8TaCR7Q)

# SiteLink.Queue

**SiteLink.Queue** is a plugin for [SiteLink](https://github.com/Killers0992/SiteLink) that adds a fully automated **queue system** to your SCP: Secret Laboratory proxy network.  
When a target server is full, players are placed into a managed queue and automatically redirected as soon as a free slot becomes available.

---

## 🧩 Requirements

To use **SiteLink.Queue**, you need:

| Dependency | Version |
|-----------|---------|
| [SiteLink](https://github.com/Killers0992/SiteLink) | **0.1.0** or newer |

Ensure SiteLink is installed and functioning before adding this plugin.

---

## ✨ Features

- **Automatic Queue Handling** – When a server reaches capacity, players are seamlessly added to a queue.  
- **Live Position Updates** – Players receive updates about their current queue position.  
- **Auto-Redirect on Free Slot** – As soon as the server reports an open slot, the next queued player is instantly moved.  
- **Configurable Settings** – Adjust supported servers.  
- **Strict FCFS Ordering** – Queue operates on first-come-first-served logic.  
- **Timeout & Disconnect Cleanup** – Automatically removes disconnected or timed-out players.

---

## 🚀 How It Works

1. A player attempts to join a SiteLink target server.  
2. If the server is full, the player is redirected to the **Queue server**.  
3. The player is added to the proper queue and receives status updates.  
4. SiteLink.Queue continuously monitors the target server.  
5. When a slot opens, the next player in line is automatically redirected to the destination server.

---

## 📦 Installation

1. Place the compiled `SiteLink.Queue.dll` inside your SiteLink `Plugins` directory.  
2. Start SiteLink once — the plugin will generate a default `config.yml`.  
3. Open `Plugins/SiteLink.Queue/config.yml` and configure queue behavior.

### Enabling Queue for Specific Servers

To add servers that should use the queue system, edit:
1. Plugins/SiteLink.Queue/config.yml
2. Then add the server names under:
```yml
servers_with_queue:
  - default
```

You may add multiple servers:
```yml
servers_with_queue:
  - default
  - survival
  - eventserver
```
